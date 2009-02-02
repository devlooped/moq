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

		[Fact]
		public void ShouldExposeMatcherType()
		{
			AdvancedMatcherAttribute attr = new AdvancedMatcherAttribute(typeof(MockMatcher));

			Assert.Equal(typeof(MockMatcher), attr.MatcherType);
		}

		[Fact]
		public void ShouldThrowRealException()
		{
			AdvancedMatcherAttribute attr = new AdvancedMatcherAttribute(typeof(ThrowingMatcher));
			Assert.Throws<ArgumentException>(() => attr.CreateMatcher());
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

		class ThrowingMatcher : IMatcher
		{
			public ThrowingMatcher()
			{
				throw new ArgumentException();
			}

			public void Initialize(Expression matcherExpression)
			{
			}

			public bool Matches(object value)
			{
				return false;
			}
		}
	}

}
