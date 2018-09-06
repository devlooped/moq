// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	/// <summary>
	/// Tracks the current mock and interception context.
	/// </summary>
	internal class FluentMockContext : IDisposable
	{
		[ThreadStatic]
		private static FluentMockContext current;

		private List<MockInvocation> invocations = new List<MockInvocation>();

		public static FluentMockContext Current
		{
			get { return current; }
		}

		/// <summary>
		/// Having an active fluent mock context means that the invocation 
		/// is being performed in "trial" mode, just to gather the 
		/// target method and arguments that need to be matched later 
		/// when the actual invocation is made.
		/// </summary>
		public static bool IsActive
		{
			get { return current != null; }
		}

		public FluentMockContext()
		{
			current = this;
		}

		public void Add(Mock mock, Invocation invocation)
		{
			invocations.Add(new MockInvocation(mock, invocation, LastMatch));
		}

		public MockInvocation LastInvocation
		{
			get { return invocations.LastOrDefault(); }
		}

		public Match LastMatch { get; set; }

		public void Dispose()
		{
			invocations.Reverse();
			foreach (var invocation in invocations)
			{
				invocation.Dispose();
			}

			current = null;
		}

		internal class MockInvocation : IDisposable
		{
			private DefaultValueProvider defaultValueProvider;

			public MockInvocation(Mock mock, Invocation invocation, Match matcher)
			{
				this.Mock = mock;
				this.Invocation = invocation;
				this.Match = matcher;
				this.defaultValueProvider = mock.DefaultValueProvider;

				// Temporarily set mock default value to Mock so that recursion works.
				mock.DefaultValue = DefaultValue.Mock;
			}

			public Mock Mock { get; private set; }

			public Invocation Invocation { get; private set; }

			public Match Match { get; private set; }

			public void Dispose()
			{
				this.Mock.DefaultValueProvider = this.defaultValueProvider;
			}
		}
	}
}
