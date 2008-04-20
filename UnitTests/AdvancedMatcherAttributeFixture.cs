using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;

namespace Moq.Tests
{
	public class AdvancedMatcherAttributeFixture
	{
		[Fact]
		public void ShouldThrowIfNullMatcherType()
		{
			Assert.Throws<ArgumentNullException>(() => new AdvancedMatcherAttribute(null));
		}

		[Fact]
		public void ShouldThrowIfMatcherNotIExpressionMatcher()
		{
			Assert.Throws<ArgumentException>(() => new AdvancedMatcherAttribute(typeof(object)));
		}

		[Fact]
		public void ShouldCreateMatcher()
		{
			AdvancedMatcherAttribute attr = new AdvancedMatcherAttribute(typeof(MockMatcher));
			IMatcher matcher = attr.CreateMatcher();

			Assert.NotNull(matcher);
		}

		class MockMatcher : IMatcher
		{
			#region IMatcher Members

			public void Initialize(Expression matcherExpression)
			{
			}

			public bool Matches(object value)
			{
				return false;
			}

			#endregion
		}

	}

}
