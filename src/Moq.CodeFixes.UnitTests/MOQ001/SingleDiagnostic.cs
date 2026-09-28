using System;

namespace Moq.CodeFixes.UnitTests.MOQ001
{
    public class SingleDiagnostic
    {
        public void Test()
        {
            var mock = Mock.Of<IServiceProvider>();

            using (mock.Setup())
            {
                mock.GetService(typeof(IFormatProvider)).Returns(default(IFormatProvider));
            }
        }
    }
}
