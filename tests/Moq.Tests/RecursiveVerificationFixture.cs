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
	}
}
