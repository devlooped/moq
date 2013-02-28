using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class ExtensionsFixture
	{
		[Fact]
		public void IsMockeableReturnsFalseForValueType()
		{
			Assert.False(typeof(int).IsMockeable());
		}

        public void OnceDoesNotThrowOnSecondCallIfCountWasResetBefore()
        {
            var mock = new Mock<IFooReset>();
            mock.Setup(foo => foo.Execute("ping"))
                .Returns("ack")
                .AtMostOnce();

            Assert.Equal("ack", mock.Object.Execute("ping"));
            mock.ResetAllCalls();
            Assert.DoesNotThrow(() => mock.Object.Execute("ping"));
        }
	}

    public interface IFooReset
    {
        object Execute(string ping);
    }
}
