using System;
using static Moq.Syntax;

namespace Moq.CodeFixes.UnitTests.MOQ001.Fixed
{
    public class MultipleDiagnosticDifferentMethod
    {
        public void Test1()
        {
            var mock = Mock.Of<IServiceProvider>();

            using (Setup())
            {
                mock.GetService(typeof(IFormatProvider)).Returns(default(IFormatProvider));
            }

            using (Setup())
            {
                mock.GetService(typeof(IFormattable)).Returns(default(IFormattable));
            }
        }

        public void Test2()
        {
            var mock = Mock.Of<IServiceProvider>();

            using (Setup())
            {
                mock.GetService(typeof(IFormatProvider)).Returns(default(IFormatProvider));
            }

            using (Setup())
            {
                mock.GetService(typeof(IFormattable)).Returns(default(IFormattable));
            }
        }
    }
}
