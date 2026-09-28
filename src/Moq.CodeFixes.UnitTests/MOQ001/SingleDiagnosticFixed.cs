using System;
using static Moq.Syntax;

namespace Moq.CodeFixes.UnitTests.MOQ001.Fixed
{
    public class SingleDiagnostic
    {
        public void Test()
        {
            var mock = Mock.Of<IServiceProvider>();

            using (Setup())
            {
                mock.GetService(typeof(IFormatProvider)).Returns(default(IFormatProvider));
            }
        }
    }
}
