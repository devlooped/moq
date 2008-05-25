using System;
using System.Linq.Expressions;
using Xunit;

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
