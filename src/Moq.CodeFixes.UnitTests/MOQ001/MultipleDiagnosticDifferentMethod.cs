using System;

namespace Moq.CodeFixes.UnitTests.MOQ001
{
    public class MultipleDiagnosticDifferentMethod
    {
        public void Test1()
        {
            var mock = Mock.Of<IServiceProvider>();

            using (mock.Setup())
            {
                mock.GetService(typeof(IFormatProvider)).Returns(default(IFormatProvider));
            }

            using (mock.Setup())
            {
                mock.GetService(typeof(IFormattable)).Returns(default(IFormattable));
            }
        }

        public void Test2()
        {
            var mock = Mock.Of<IServiceProvider>();

            using (mock.Setup())
            {
                mock.GetService(typeof(IFormatProvider)).Returns(default(IFormatProvider));
            }

            using (mock.Setup())
            {
                mock.GetService(typeof(IFormattable)).Returns(default(IFormattable));
            }
        }
    }
}
