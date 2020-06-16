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
			if (IsObjectMethod(invocation.Method) && !mock.MutableSetups.Any(c => IsObjectMethod(c.Method, "Equals")))
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
			if (IsObjectMethod(invocation.Method) && !mock.MutableSetups.Any(c => IsObjectMethod(c.Method, "GetHashCode")))
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
			if (IsObjectMethod(invocation.Method) && !mock.MutableSetups.Any(c => IsObjectMethod(c.Method, "ToString")))
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
			var matchingSetup = mock.MutableSetups.FindMatchFor(invocation);
			if (matchingSetup != null)
			{
				matchingSetup.Execute(invocation);
				return true;
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
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			var methodName = invocation.Method.Name;

			// Special case for event accessors. The following, seemingly random character checks are guards against
			// more expensive checks (for the common case where the invoked method is *not* an event accessor).
			if (methodName.Length > 4)
			{
				if (methodName[0] == 'a' && methodName[3] == '_' && invocation.Method.IsEventAddAccessor())
				{
					var implementingMethod = invocation.Method.GetImplementingMethod(invocation.ProxyType);
					var @event = implementingMethod.DeclaringType.GetEvents(bindingFlags).SingleOrDefault(e => e.GetAddMethod(true) == implementingMethod);
					if (@event != null)
					{
						bool doesntHaveEventSetup = !mock.MutableSetups.HasEventSetup;

						if (mock.CallBase && !invocation.Method.IsAbstract)
						{
							if (doesntHaveEventSetup)
							{
								invocation.ReturnBase();
							}
						}
						else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
						{
							mock.EventHandlers.Add(@event, delegateInstance);

							if (doesntHaveEventSetup)
							{
								invocation.Return();
							}
						}

						return doesntHaveEventSetup;
					}
				}
				else if (methodName[0] == 'r' && methodName.Length > 7 && methodName[6] == '_' && invocation.Method.IsEventRemoveAccessor())
				{
					var implementingMethod = invocation.Method.GetImplementingMethod(invocation.ProxyType);
					var @event = implementingMethod.DeclaringType.GetEvents(bindingFlags).SingleOrDefault(e => e.GetRemoveMethod(true) == implementingMethod);
					if (@event != null)
					{
						bool doesntHaveEventSetup = !mock.MutableSetups.HasEventSetup;

						if (mock.CallBase && !invocation.Method.IsAbstract)
						{
							if (doesntHaveEventSetup)
							{
								invocation.ReturnBase();
							}
						}
						else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
						{
							mock.EventHandlers.Remove(@event, delegateInstance);

							if (doesntHaveEventSetup)
							{
								invocation.Return();
							}
						}

						return doesntHaveEventSetup;
					}
				}
			}

			return false;
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
					if (mock.MockedType.IsInterface)
					{
						// Case 1: Interface method of an interface proxy.
						// There is no base method to call, so fall through.
					}
					else
					{
						Debug.Assert(mock.MockedType.IsClass);
						Debug.Assert(mock.ImplementsInterface(declaringType));

						// Case 2: Explicitly implemented interface method of a class proxy.

						if (mock.InheritedInterfaces.Contains(declaringType))
						{
							// Case 2a: Re-implemented interface.
							// The base class has its own implementation. Only call base method if it isn't an event accessor.
							if (!method.IsEventAddAccessor() && !method.IsEventRemoveAccessor())
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
				if (innerMock != null && invocation.MatchingSetup == null)
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

		public static bool Handle(Invocation invocation, Mock mock)
		{
			if (mock.AutoSetupPropertiesDefaultValueProvider == null)
			{
				return false;
			}
			
			MethodInfo invocationMethod = invocation.Method;
			if (invocationMethod.IsPropertyAccessor())
			{
				string propertyNameToSearch = invocationMethod.Name.Substring(AccessorPrefixLength);
				PropertyInfo property = invocationMethod.DeclaringType.GetProperty(propertyNameToSearch);

				if (property == null)
				{
					return false;
				}

				var expression = GetPropertyExpression(invocationMethod.DeclaringType, property);

				object propertyValue;

				Setup getterSetup = null;
				if (property.CanRead(out var getter))
				{
					if (ProxyFactory.Instance.IsMethodVisible(getter, out _))
					{
						propertyValue = CreateInitialPropertyValue(mock, getter);
						getterSetup = new AutoImplementedPropertyGetterSetup(mock, expression, getter, () => propertyValue);
						mock.MutableSetups.Add(getterSetup);
					}

					// If we wanted to optimise for speed, we'd probably be forgiven
					// for removing the above `IsMethodVisible` guard, as it's rather
					// unlikely to encounter non-public getters such as the following
					// in real-world code:
					//
					//     public T Property { internal get; set; }
					//
					// Usually, it's only the setters that are made non-public. For
					// now however, we prefer correctness.
				}

				Setup setterSetup = null;
				if (property.CanWrite(out var setter))
				{
					if (ProxyFactory.Instance.IsMethodVisible(setter, out _))
					{
						setterSetup = new AutoImplementedPropertySetterSetup(mock, expression, setter, (newValue) =>
						{
							propertyValue = newValue;
						});
						mock.MutableSetups.Add(setterSetup);
					}
				}

				Setup setupToExecute = invocationMethod.IsGetAccessor() ? getterSetup : setterSetup;
				setupToExecute.Execute(invocation);

				return true;
			}
			else
			{
				return false;
			}
		}

		private static object CreateInitialPropertyValue(Mock mock, MethodInfo getter)
		{
			object initialValue = mock.GetDefaultValue(getter, out Mock innerMock, 
				useAlternateProvider: mock.AutoSetupPropertiesDefaultValueProvider);

			if (innerMock != null)
			{
				Mock.SetupAllProperties(innerMock, mock.AutoSetupPropertiesDefaultValueProvider);
			}

			return initialValue;
		}

		private static LambdaExpression GetPropertyExpression(Type mockType, PropertyInfo property)
		{
			var param = Expression.Parameter(mockType, "m");
			return Expression.Lambda(Expression.MakeMemberAccess(param, property), param);
		}
	}

	internal static class FailForStrictMock
	{
		public static void Handle(Invocation invocation, Mock mock)
		{
			if (mock.Behavior == MockBehavior.Strict)
			{
				throw MockException.NoSetup(invocation);
			}
		}
	}
}
