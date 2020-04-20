// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Diagnostics;
using System.Linq;

namespace Moq.Behaviors
{
	/// <todo/>
	public sealed class ReturnBaseOrDefaultValue : Behavior
	{
		internal static readonly ReturnBaseOrDefaultValue Instance = new ReturnBaseOrDefaultValue();

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			Debug.Assert(invocation.Method != null);
			Debug.Assert(invocation.Method.ReturnType != null);

			var method = invocation.Method;
			var mock = context.Mock;

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
								return BehaviorExecution.ReturnBase();
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
						return BehaviorExecution.ReturnBase();
					}
				}
			}

			if (method.ReturnType == typeof(void))
			{
				return BehaviorExecution.Return();
			}
			else
			{
				var returnValue = mock.GetDefaultValue(method, out var innerMock);
				if (innerMock != null)
				{
					mock.AddInnerMockSetup(invocation, returnValue);
				}
				return BehaviorExecution.ReturnValue(returnValue);
			}
		}
	}
}
