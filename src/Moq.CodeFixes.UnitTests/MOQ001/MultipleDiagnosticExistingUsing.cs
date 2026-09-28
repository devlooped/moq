using System;
using static Moq.Syntax;

namespace Moq.CodeFixes.UnitTests.MOQ001
{
    public class MultipleDiagnosticExistingUsing
    {
        public void Test()
        {
            var mock = Mock.Of<IServiceProvider>();

            using (Setup())
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
