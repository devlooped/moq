using System;
using System.Linq;
using Castle.Core.Interceptor;
using System.Collections.Generic;
using System.Linq.Expressions;

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

		/// <summary>
		/// Having an active fluent mock context means that the invocation 
		/// is being performed in "trial" mode, just to gather the 
		/// target method and arguments that need to be matched later 
		/// when the actual invocation is made.
		/// </summary>
		public static bool IsActive { get { return current != null; } }

		public FluentMockContext()
		{
			current = this;
		}

		public void Add(Mock mock, IInvocation invocation)
		{
			invocations.Add(new MockInvocation(mock, invocation, LastMatcherInvocation));
		}

		public MockInvocation LastInvocation { get { return invocations.LastOrDefault(); } }
		public Expression LastMatcherInvocation { get; set; }

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

			public MockInvocation(Mock mock, IInvocation invocation, Expression matcher)
			{
				this.Mock = mock;
				this.Invocation = invocation;
				this.Matcher = matcher;
				defaultValue = mock.DefaultValue;
				// Temporarily set mock default value to Mock so that recursion works.
				mock.DefaultValue = DefaultValue.Mock;
			}

			public Mock Mock { get; private set; }
			public IInvocation Invocation { get; private set; }
			public Expression Matcher { get; private set; }

			public void Dispose()
			{
				Mock.DefaultValue = defaultValue;
			}
		}
	}
}
