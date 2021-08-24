// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Setup used by <see cref="Mock.SetupAllProperties(Mock)"/> for property getters.
	/// </summary>
	internal sealed class StubbedPropertyGetterSetup : MethodSetup
	{
		private static Expression[] noArguments = new Expression[0];

		private Func<object> getter;

		public StubbedPropertyGetterSetup(Mock mock, LambdaExpression originalExpression, MethodInfo method, Func<object> getter)
			: base(originalExpression: null, mock, new MethodExpectation(originalExpression, method, noArguments))
		{
			this.getter = getter;

			this.MarkAsVerifiable();
		}

		public override IEnumerable<Mock> InnerMocks
		{
			get
			{
				var innerMock = TryGetInnerMockFrom(this.getter.Invoke());
				if (innerMock != null)
				{
					yield return innerMock;
				}
			}
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			invocation.ReturnValue = this.getter.Invoke();
		}

		protected override void VerifySelf()
		{
		}
	}
}
