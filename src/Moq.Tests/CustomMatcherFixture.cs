// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class CustomMatcherFixture
	{
		[Fact]
		public void UsesCustomMatcher()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Do(Any<string>())).Returns(true);

			Assert.True(mock.Object.Do("foo"));
		}

		[Fact]
		public void UsesCustomMatcherWithArgument()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Do(Between(1, 5, Range.Inclusive))).Returns(true);

			Assert.False(mock.Object.Do(6));
			Assert.True(mock.Object.Do(1));
			Assert.True(mock.Object.Do(5));
		}

		public TValue Any<TValue>()
		{
			return Match.Create<TValue>(v => true);
		}

		public TValue Between<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			return Match.Create<TValue>(value =>
			{
				if (value == null)
				{
					return false;
				}

				if (rangeKind == Range.Exclusive)
				{
					return value.CompareTo(from) > 0 &&
						value.CompareTo(to) < 0;
				}
				else
				{
					return value.CompareTo(from) >= 0 &&
						value.CompareTo(to) <= 0;
				}
			});
		}

		public interface IFoo
		{
			bool Do(string value);
			bool Do(int value);
		}

		[Fact]
		public void Custom_matcher_property_appears_by_name_in_verification_error_message()
		{
			var child = new Mock<IChild>();
			child.Setup(c => c.PlayWith(Toy.IsRed)).Verifiable();

			var ex = Assert.Throws<MockException>(() => child.Verify());

			Assert.Contains(".PlayWith(CustomMatcherFixture.Toy.IsRed)", ex.Message);
		}

		[Fact]
		public void Custom_matcher_method_appears_by_name_in_verification_error_message()
		{
			var child = new Mock<IChild>();
			child.Setup(c => c.PlayWith(Toy.IsGreen())).Verifiable();

			var ex = Assert.Throws<MockException>(() => child.Verify());

			Assert.Contains(".PlayWith(CustomMatcherFixture.Toy.IsGreen())", ex.Message);
		}

		public class Toy
		{
			public static Toy IsRed => Match.Create((Toy toy) => toy.Color == "red");

			public static Toy IsGreen() => Match.Create((Toy toy) => toy.Color == "green");

			public string Color { get; set; }
		}

		public interface IChild
		{
			void PlayWith(Toy toy);
		}
	}
}
