using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
    public class ExtensionsFixture
    {
        #region Public Methods

        [Fact]
        public void IsMockeableReturnsFalseForValueType()
        {
            Assert.False(typeof(int).IsMockeable());
        }

        [Fact]
        public void OnceDoesNotThrowOnSecondCallIfCountWasResetBefore()
        {
            var mock = new Mock<IFooReset>();
            mock.Setup(foo => foo.Execute("ping")).Returns("ack");

            mock.Object.Execute("ping");
            mock.ResetCalls();
            mock.Object.Execute("ping");
            mock.Verify(o => o.Execute("ping"), Times.Once());
        }

        #endregion
    }

    public interface IFooReset
    {
        #region Public Methods

        object Execute(string ping);

        #endregion
    }
}
