// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Moq.Behaviors;

namespace Moq
{
	internal sealed class BehaviorSetup : SetupWithOutParameterSupport
	{
		private readonly Behavior[] behaviors;
		private string failMessage;

		public BehaviorSetup(Expression originalExpression, Mock mock, InvocationShape expectation, IReadOnlyList<Behavior> behaviors)
			: base(originalExpression, mock, expectation)
		{
			this.behaviors = behaviors.ToArray();
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			var context = new BehaviorExecutionContext(this.Mock);

			for (int i = 0, n = this.behaviors.Length; i < n; ++i)
			{
				var result = this.behaviors[i].Execute(invocation, in context);

				invocation.Apply(result);

				if (result.Kind != BehaviorExecutionKind.Continue)
				{
					return;
				}
			}

			if (this.Mock.Behavior == MockBehavior.Strict && invocation.Method.ReturnType != typeof(void))
			{
				throw MockException.ReturnValueRequired(invocation);
			}
			else
			{
				invocation.Apply(ReturnBaseOrDefaultValue.Instance.Execute(invocation, in context));
			}
		}

		public void SetFailMessage(string failMessage)
		{
			this.failMessage = failMessage;
		}

		public override string ToString()
		{
			var message = new StringBuilder();

			if (this.failMessage != null)
			{
				message.Append(this.failMessage).Append(": ");
			}

			message.Append(base.ToString());

			return message.ToString().Trim();
		}

		public override bool TryGetReturnValue(out object returnValue)
		{
			var behavior = this.behaviors.OfType<ReturnValue>().FirstOrDefault();
			returnValue = behavior?.Value;
			return behavior != null;
		}
	}
}
