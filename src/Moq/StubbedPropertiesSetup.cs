// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

using E = System.Linq.Expressions.Expression;

namespace Moq
{
	internal sealed class StubbedPropertiesSetup : Setup
	{
		private readonly ConcurrentDictionary<string, object> values;
		private readonly DefaultValueProvider defaultValueProvider;

		public StubbedPropertiesSetup(Mock mock, DefaultValueProvider defaultValueProvider = null)
			: base(originalExpression: null, mock, new PropertyAccessorExpectation(mock))
		{
			this.values = new ConcurrentDictionary<string, object>();
			this.defaultValueProvider = defaultValueProvider ?? mock.DefaultValueProvider;

			this.MarkAsVerifiable();
		}

		public DefaultValueProvider DefaultValueProvider => this.defaultValueProvider;

		public override IEnumerable<Mock> InnerMocks
		{
			get
			{
				foreach (var value in this.values.Values)
				{
					var innerMock = TryGetInnerMockFrom(value);
					if (innerMock != null)
					{
						yield return innerMock;
					}
				}
			}
		}

		public void SetProperty(string propertyName, object value)
		{
			this.values[propertyName] = value;
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			if (invocation.Method.ReturnType == typeof(void))
			{
				Debug.Assert(invocation.Method.IsSetAccessor());
				Debug.Assert(invocation.Arguments.Length == 1);

				var propertyName = invocation.Method.Name.Substring(4);
				this.values[propertyName] = invocation.Arguments[0];
			}
			else
			{
				Debug.Assert(invocation.Method.IsGetAccessor());

				var propertyName = invocation.Method.Name.Substring(4);
				var value = this.values.GetOrAdd(propertyName, pn => this.Mock.GetDefaultValue(invocation.Method, out _, this.defaultValueProvider));
				invocation.ReturnValue = value;
			}
		}

		protected override void VerifySelf()
		{
		}

		private sealed class PropertyAccessorExpectation : Expectation
		{
			private readonly LambdaExpression expression;

			public PropertyAccessorExpectation(Mock mock)
			{
				Debug.Assert(mock != null);

				var mockType = mock.GetType();
				var setupAllPropertiesMethod = mockType.GetMethod(nameof(Mock<object>.SetupAllProperties));
				var mockedType = setupAllPropertiesMethod.ReturnType.GetGenericArguments()[0];
				var mockGetMethod = Mock.GetMethod.MakeGenericMethod(mockedType);
				var mockParam = E.Parameter(mockedType, "m");
				this.expression = E.Lambda(E.Call(E.Call(mockGetMethod, mockParam), setupAllPropertiesMethod), mockParam);
			}

			public override LambdaExpression Expression => this.expression;

			public override bool Equals(Expectation other)
			{
				return other is PropertyAccessorExpectation pae && ExpressionComparer.Default.Equals(this.expression, pae.expression);
			}

			public override int GetHashCode()
			{
				return typeof(PropertyAccessorExpectation).GetHashCode();
			}

			public override bool IsMatch(Invocation invocation)
			{
				return invocation.Method.IsPropertyAccessor();
			}
		}
	}
}
