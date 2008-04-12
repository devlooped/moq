using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Linq.Expressions;

namespace Moq.Tests
{
	[TestFixture]
	public class AdvancedMatcherAttributeFixture
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullMatcherType()
		{
			new AdvancedMatcherAttribute(null);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfMatcherNotIExpressionMatcher()
		{
			AdvancedMatcherAttribute attr = new AdvancedMatcherAttribute(typeof(object));
		}

		[Test]
		public void ShouldCreateMatcher()
		{
			AdvancedMatcherAttribute attr = new AdvancedMatcherAttribute(typeof(MockMatcher));
			IMatcher matcher = attr.CreateMatcher();

			Assert.IsNotNull(matcher);
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
