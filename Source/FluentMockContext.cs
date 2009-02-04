using System;
using System.Linq;
using Castle.Core.Interceptor;
using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	/// Tracks the current mock and interception context.
	/// </summary>
	internal class FluentMockContext : IDisposable
	{
		[ThreadStatic]
		static FluentMockContext current;

		List<MockInvocation> invocations = new List<MockInvocation>();

		public static FluentMockContext Current { get { return current; } }

		public FluentMockContext()
		{
			current = this;
		}

		public void Add(Mock mock, IInvocation invocation)
		{
			invocations.Add(new MockInvocation(mock, invocation));
		}

		public MockInvocation LastInvocation { get { return invocations[invocations.Count - 1]; } }

		public void Dispose()
		{
			foreach (var invocation in invocations)
			{
				invocation.Dispose();
			}

			current = null;
		}

		internal class MockInvocation : IDisposable
		{
			DefaultValue defaultValue;

			public MockInvocation(Mock mock, IInvocation invocation)
			{
				this.Mock = mock;
				this.Invocation = invocation;
				defaultValue = mock.DefaultValue;
				// Temporarily set mock default value to Mock so that recursion works.
				mock.DefaultValue = DefaultValue.Mock;
			}

			public Mock Mock { get; private set; }
			public IInvocation Invocation { get; private set; }

			public void Dispose()
			{
				Mock.DefaultValue = defaultValue;
			}
		}
	}
}
