// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	/// <summary>
	///   This fixture contains tests about recursive verification.
	/// </summary>
	public class RecursiveVerificationFixture
	{
		public interface IX
		{
			IY Y { get; set; }
		}

		public interface IY
		{
			void Method();
		}

		public class Reachability
		{
			[Fact]
			public void Inner_mock_that_becomes_unreachable_is_no_longer_included_in_verification()
			{
				var xMock = new Mock<IX>();
				xMock.Setup(m => m.Y.Method()).Verifiable("1");
				xMock.Setup(m => m.Y).Verifiable("2");
				_ = xMock.Object.Y;

				xMock.Verify();
			}

			[Fact]
			public void Initial_value_of_stubbed_property_is_no_longer_included_in_verification_when_property_set()
			{
				var xMock = new Mock<IX> { DefaultValue = DefaultValue.Mock };
				xMock.SetupAllProperties();
				Mock.Get(xMock.Object.Y).Setup(m => m.Method()).Verifiable();
				xMock.Object.Y = null;

				xMock.Verify();
			}
		}

		public class SetReturnsDefault
		{
			[Fact]
			public void If_property_is_not_queried_SetReturnsDefault_mock_is_not_included_in_verification()
			{
				// Set up `yMock` such that verifying it would fail:
				var yMock = new Mock<IY>();
				yMock.Setup(y => y.Method()).Verifiable();

				// Set up `xMock` such that a path to `yMock` *could* be established:
				var xMock = new Mock<IX>();
				xMock.SetReturnsDefault<IY>(yMock.Object);

				// Since there is no path from `xMock` to `yMock`, this should succeed:
				xMock.Verify();
			}

			[Fact]
			public void If_property_is_queried_SetReturnsDefault_mock_is_included_in_verification()
			{
				// As in the previous test:
				var yMock = new Mock<IY>();
				yMock.Setup(y => y.Method()).Verifiable();

				var xMock = new Mock<IX>();
				xMock.SetReturnsDefault<IY>(yMock.Object);

				// But this time, also establish a connection from `xMock` to `yMock`:
				_ = xMock.Object.Y;

				// This should now throw:
				Assert.Throws<MockException>(() => xMock.Verify());
			}
		}

		public class StubbedProperties
		{
			[Fact]
			public void Stubbed_property_can_retain_its_value_even_in_presence_of_a_inner_mock_registration()
			{
				var xMock = new Mock<IX> { DefaultValue = DefaultValue.Mock };
				xMock.SetupAllProperties();

				// The first query to a stubbed property should cause generation of a default value.
				// Because we are using `DefaultValue.Mock`, this should trigger registration of an inner mock:
				var y = xMock.Object.Y;
				Assert.NotNull(y);

				// We should (obviously) still be able to override that initial value:
				xMock.Object.Y = null;
				y = xMock.Object.Y;
				Assert.Null(y);
			}
		}
	}
}
