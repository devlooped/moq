// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Diagnostics;
using System.Linq;

namespace Moq.Behaviors
{
	internal sealed class ReturnBaseOrDefaultValue : Behavior
	{
		private readonly Mock mock;

		public ReturnBaseOrDefaultValue(Mock mock)
		{
			Debug.Assert(mock != null);

			this.mock = mock;
		}

		public override void Execute(Invocation invocation)
		{
			Debug.Assert(invocation.Method != null);
			Debug.Assert(invocation.Method.ReturnType != null);

			var method = invocation.Method;

			if (this.mock.CallBase)
			{

#if FEATURE_DEFAULT_INTERFACE_IMPLEMENTATIONS
				var tryCallDefaultInterfaceImplementation = false;
#endif

				var declaringType = method.DeclaringType;
				if (declaringType.IsInterface)
				{
					if (this.mock.MockedType.IsInterface)
					{
						// Case 1: Interface method of an interface proxy.

#if FEATURE_DEFAULT_INTERFACE_IMPLEMENTATIONS
						// Fall through to invoke default implementation (if one exists).
						tryCallDefaultInterfaceImplementation = true;
#else
						// There is no base method to call, so fall through.
#endif
					}
					else
					{
						Debug.Assert(mock.MockedType.IsClass);
						Debug.Assert(mock.ImplementsInterface(declaringType));

						// Case 2: Explicitly implemented interface method of a class proxy.

						if (this.mock.InheritedInterfaces.Contains(declaringType))
						{
							// Case 2a: Re-implemented interface.
							// The base class has its own implementation. Only call base method if it isn't an event accessor.
							if (!method.IsEventAddAccessor() && !method.IsEventRemoveAccessor())
							{
								invocation.ReturnValue = invocation.CallBase();
								return;
							}
						}
						else
						{
							Debug.Assert(this.mock.AdditionalInterfaces.Contains(declaringType));

							// Case 2b: Additional interface.

#if FEATURE_DEFAULT_INTERFACE_IMPLEMENTATIONS
							// Fall through to invoke default implementation (if one exists).
							tryCallDefaultInterfaceImplementation = true;
#else
							// There is no base method to call, so fall through.
#endif
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
						invocation.ReturnValue = invocation.CallBase();
						return;
					}
				}

#if FEATURE_DEFAULT_INTERFACE_IMPLEMENTATIONS
				if (tryCallDefaultInterfaceImplementation && !method.IsAbstract)
				{
					// Invoke default implementation.
					invocation.ReturnValue = invocation.CallBase();
					return;
				}
#endif

			}

			if (method.ReturnType != typeof(void))
			{
				var returnValue = this.mock.GetDefaultValue(method, out var innerMock);
				if (innerMock != null && invocation.MatchingSetup == null)
				{
					var setup = new InnerMockSetup(originalExpression: null, this.mock, expectation: MethodExpectation.CreateFrom(invocation), returnValue);
					this.mock.MutableSetups.Add(setup);
					setup.Execute(invocation);
				}
				else
				{
					invocation.ReturnValue = returnValue;
				}
			}
		}
	}
}
