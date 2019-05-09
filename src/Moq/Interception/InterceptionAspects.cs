// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	internal static class HandleWellKnownMethods
	{
		private static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
		{
			["Equals"] = HandleEquals,
			["Finalize"] = HandleFinalize,
			["GetHashCode"] = HandleGetHashCode,
			["get_" + nameof(IMocked.Mock)] = HandleMockGetter,
			["ToString"] = HandleToString,
		};

		public static bool Handle(Invocation invocation, Mock mock)
		{
			return specialMethods.TryGetValue(invocation.Method.Name, out var handler)
				&& handler.Invoke(invocation, mock);
		}

		private static bool HandleEquals(Invocation invocation, Mock mock)
		{
			if (IsObjectMethod(invocation.Method) && !mock.Setups.Any(c => IsObjectMethod(c.Method, "Equals")))
			{
				invocation.Return(ReferenceEquals(invocation.Arguments.First(), mock.Object));
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool HandleFinalize(Invocation invocation, Mock mock)
		{
			return IsFinalizer(invocation.Method);
		}

		private static bool HandleGetHashCode(Invocation invocation, Mock mock)
		{
			// Only if there is no corresponding setup for `GetHashCode()`
			if (IsObjectMethod(invocation.Method) && !mock.Setups.Any(c => IsObjectMethod(c.Method, "GetHashCode")))
			{
				invocation.Return(mock.GetHashCode());
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool HandleToString(Invocation invocation, Mock mock)
		{
			// Only if there is no corresponding setup for `ToString()`
			if (IsObjectMethod(invocation.Method) && !mock.Setups.Any(c => IsObjectMethod(c.Method, "ToString")))
			{
				invocation.Return(mock.ToString() + ".Object");
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool HandleMockGetter(Invocation invocation, Mock mock)
		{
			if (typeof(IMocked).IsAssignableFrom(invocation.Method.DeclaringType))
			{
				invocation.Return(mock);
				return true;
			}
			else
			{
				return false;
			}
		}

		private static bool IsFinalizer(MethodInfo method)
		{
			return method.GetBaseDefinition() == typeof(object).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		private static bool IsObjectMethod(MethodInfo method) => method.DeclaringType == typeof(object);

		private static bool IsObjectMethod(MethodInfo method, string name) => IsObjectMethod(method) && method.Name == name;
	}

	internal static class FindAndExecuteMatchingSetup
	{
		public static bool Handle(Invocation invocation, Mock mock)
		{
			var matchedSetup = mock.Setups.FindMatchFor(invocation);
			if (matchedSetup != null)
			{
				matchedSetup.Condition?.EvaluatedSuccessfully();

				if (matchedSetup.IsVerifiable)
				{
					invocation.MarkAsMatchedByVerifiableSetup();
				}
				else
				{
					invocation.MarkAsMatchedBySetup();
				}

				matchedSetup.SetOutParameters(invocation);

				// We first execute, as there may be a Throws 
				// and therefore we might never get to the 
				// next line.
				matchedSetup.Execute(invocation);
				return true;
			}
			else if (mock.Behavior == MockBehavior.Strict)
			{
				throw MockException.NoSetup(invocation);
			}
			else
			{
				return false;
			}
		}
	}

	internal static class HandleEventSubscription
	{
		public static bool Handle(Invocation invocation, Mock mock)
		{
			var methodName = invocation.Method.Name;

			// Special case for event accessors. The following, seemingly random character checks are guards against
			// more expensive checks (for the common case where the invoked method is *not* an event accessor).
			if (methodName.Length > 4)
			{
				if (methodName[0] == 'a' && methodName[3] == '_' && invocation.Method.LooksLikeEventAttach())
				{
					var eventInfo = GetEventFromName(invocation.Method.Name.Substring("add_".Length), mock);
					if (eventInfo != null)
					{
						// TODO: We could compare `invocation.Method` and `eventInfo.GetAddMethod()` here.
						// If they are equal, then `invocation.Method` is definitely an event `add` accessor.
						// Not sure whether this would work with F# and COM; see commit 44070a9.

						if (mock.CallBase && !invocation.Method.IsAbstract)
						{
							invocation.ReturnBase();
							return true;
						}
						else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
						{
							mock.EventHandlers.Add(eventInfo.Name, delegateInstance);
							invocation.Return();
							return true;
						}
					}
				}
				else if (methodName[0] == 'r' && methodName.Length > 7 && methodName[6] == '_' && invocation.Method.LooksLikeEventDetach())
				{
					var eventInfo = GetEventFromName(invocation.Method.Name.Substring("remove_".Length), mock);
					if (eventInfo != null)
					{
						// TODO: We could compare `invocation.Method` and `eventInfo.GetRemoveMethod()` here.
						// If they are equal, then `invocation.Method` is definitely an event `remove` accessor.
						// Not sure whether this would work with F# and COM; see commit 44070a9.

						if (mock.CallBase && !invocation.Method.IsAbstract)
						{
							invocation.ReturnBase();
							return true;
						}
						else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
						{
							mock.EventHandlers.Remove(eventInfo.Name, delegateInstance);
							invocation.Return();
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		/// <param name="mock"/>
		private static EventInfo GetEventFromName(string eventName, Mock mock)
		{
			return GetEventFromName(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, mock)
				?? GetEventFromName(eventName, BindingFlags.Instance | BindingFlags.NonPublic, mock);
		}

		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// Searches events using the specified binding constraints.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		/// <param name="bindingAttr">Specifies how the search for events is conducted</param>
		/// <param name="mock"/>
		private static EventInfo GetEventFromName(string eventName, BindingFlags bindingAttr, Mock mock)
		{
			// Ignore inherited interfaces and internally defined IMocked<T>
			var depthFirstProgress = new Queue<Type>(mock.AdditionalInterfaces);
			depthFirstProgress.Enqueue(mock.TargetType);
			while (depthFirstProgress.Count > 0)
			{
				var currentType = depthFirstProgress.Dequeue();
				var eventInfo = currentType.GetEvent(eventName, bindingAttr);
				if (eventInfo != null)
				{
					return eventInfo;
				}

				foreach (var implementedType in GetAncestorTypes(currentType))
				{
					depthFirstProgress.Enqueue(implementedType);
				}
			}

			return null;
		}

		/// <summary>
		/// Given a type return all of its ancestors, both types and interfaces.
		/// </summary>
		/// <param name="initialType">The type to find immediate ancestors of</param>
		private static IEnumerable<Type> GetAncestorTypes(Type initialType)
		{
			var baseType = initialType.BaseType;
			if (baseType != null)
			{
				return new[] { baseType };
			}

			return initialType.GetInterfaces();
		}
	}

	internal static class RecordInvocation
	{
		public static void Handle(Invocation invocation, Mock mock)
		{
			// Save to support Verify[expression] pattern.
			mock.MutableInvocations.Add(invocation);
		}
	}

	internal static class Return
	{
		public static void Handle(Invocation invocation, Mock mock)
		{
			Debug.Assert(invocation.Method != null);
			Debug.Assert(invocation.Method.ReturnType != null);

			var method = invocation.Method;

			if (mock.CallBase)
			{
				var declaringType = method.DeclaringType;
				if (declaringType.IsInterface)
				{
					if (mock.TargetType.IsInterface)
					{
						// Case 1: Interface method of an interface proxy.
						// There is no base method to call, so fall through.
					}
					else
					{
						Debug.Assert(mock.TargetType.IsClass);
						Debug.Assert(mock.ImplementsInterface(declaringType));

						// Case 2: Explicitly implemented interface method of a class proxy.

						if (mock.InheritedInterfaces.Contains(declaringType))
						{
							// Case 2a: Re-implemented interface.
							// The base class has its own implementation. Only call base method if it isn't an event accessor.
							if (!method.LooksLikeEventAttach() && !method.LooksLikeEventDetach())
							{
								invocation.ReturnBase();
								return;
							}
						}
						else
						{
							Debug.Assert(mock.AdditionalInterfaces.Contains(declaringType));

							// Case 2b: Additional interface.
							// There is no base method to call, so fall through.
						}
					}
				}
				else
				{
					Debug.Assert(declaringType.IsClass);

					// Case 3: Non-interface method of a class proxy.
					// Only call base method if it isn't abstract.
					if (!method.IsAbstract)
					{
						invocation.ReturnBase();
						return;
					}
				}
			}

			if (method.ReturnType == typeof(void))
			{
				invocation.Return();
			}
			else
			{
				var returnValue = mock.GetDefaultValue(method, out var innerMock);
				if (innerMock != null)
				{
					mock.AddInnerMockSetup(invocation, returnValue);
				}
				invocation.Return(returnValue);
			}
		}
	}

	internal static class HandleAutoSetupProperties
	{
		private static readonly int AccessorPrefixLength = "?et_".Length; // get_ or set_

		public static void Handle(Invocation invocation, Mock mock)
		{
			MethodInfo invocationMethod = invocation.Method;
			// Original implementation with eager setup skipped indexers too.
			if (invocationMethod.IsPropertyAccessor() && !invocationMethod.IsPropertyIndexerAccessor())
			{
				if (mock.Setups.FindMatchFor(invocation) == null)
				{
					PropertyInfo property = GetPropertyFromAccessorMethod(mock, invocationMethod, out Type propertyHolderType);

					// Should ignore write-only properties, as they will be handled by Return aspect.
					if (invocationMethod.IsPropertySetter() && !property.CanRead)
					{
						return;
					}
					
					var getter = property.GetGetMethod(true);
					var expression = GetPropertyExpression(propertyHolderType, property);

					object value = null;
					bool valueNotSet = true;

					mock.Setups.Add(new AutoImplementedPropertyGetterSetup(expression, getter, () =>
					{
						if (valueNotSet)
						{
							object initialValue;
							Mock innerMock;
							try
							{
								initialValue = mock.GetDefaultValue(getter, out innerMock,
									useAlternateProvider: mock.AutoSetupPropertiesDefaultValueProvider);
							}
							catch
							{
								// Since this method performs a batch operation, a single failure of the default value
								// provider should not tear down the whole operation. The empty default value provider
								// is a safe fallback because it does not throw.
								initialValue = mock.GetDefaultValue(getter, out innerMock,
									useAlternateProvider: DefaultValueProvider.Empty);
							}

							if (innerMock != null)
							{
								Mock.SetupAllProperties(innerMock, mock.AutoSetupPropertiesDefaultValueProvider);
							}

							value = initialValue;
							valueNotSet = false;
						}

						return value;
					}));

					if (property.CanWrite)
					{
						mock.Setups.Add(new AutoImplementedPropertySetterSetup(expression, property.GetSetMethod(true),
							(newValue) =>
							{
								value = newValue;
								valueNotSet = false;
							}));
					}
				}
			}
		}

		private static PropertyInfo GetPropertyFromAccessorMethod(Mock mock, MethodInfo accessorMethod, out Type propertyHolderType)
		{
			string propertyNameToSearch = accessorMethod.Name.Substring(AccessorPrefixLength);
			Type mockedType = mock.MockedType;
			
			// Firstly try to search directly in mocked type.
			PropertyInfo result = mockedType.GetProperty(propertyNameToSearch);
			propertyHolderType = mockedType;

			if (result == null)
			{
				// Interfaces do not simply return properties from base interfaces, so need to search there directly.
				if (mockedType.IsInterface)
					result = SearchPropertyInInterfaces(mock.InheritedInterfaces, propertyNameToSearch, out _);
				// If there are additional interfaces applied via As<IInterface>, we also need to look there.
				// At this point we need to capture interface type in which property was found. It is required to build
				// property accessor expression later in GetPropertyExpression (mocked type can not be used in this case,
				// since the property was found not in mocked type, but in additional interface.
				if (result == null)
					result = SearchPropertyInInterfaces(mock.AdditionalInterfaces, propertyNameToSearch, out propertyHolderType);
			}
			
			return result;
		}

		private static PropertyInfo SearchPropertyInInterfaces(IEnumerable<Type> interfaces, string propertyNameToSearch, out Type propertyHolderType)
		{
			foreach (Type @interface in interfaces)
			{
				PropertyInfo property = @interface.GetProperty(propertyNameToSearch);
				if (property != null)
				{
					propertyHolderType = @interface;
					return property;
				}
					
				Type[] parentInterfaces = @interface.GetInterfaces();
				if (parentInterfaces.Any())
				{
					property = SearchPropertyInInterfaces(parentInterfaces, propertyNameToSearch, out propertyHolderType);
					if (property != null)
						return property;
				}
			}

			propertyHolderType = null;
			return null;
		}
		
		private static LambdaExpression GetPropertyExpression(Type mockType, PropertyInfo property)
		{
			var param = Expression.Parameter(mockType, "m");
			return Expression.Lambda(Expression.MakeMemberAccess(param, property), param);
		}
	}
}
