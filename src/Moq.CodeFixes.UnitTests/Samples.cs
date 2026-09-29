using System.Linq;

namespace Moq.CodeFixes.UnitTests
{
    static class Samples
    {
        /// <summary>
        /// Mocked types, with members that get typed setups, members that are hidden by 
        /// members of the mock, and members that are skipped by the setup generator.
        /// </summary>
        public const string Types =
            """
            public interface ICalculator
            {
                int Add(int x, int y);
                int Mode { get; set; }
                int this[string name] { get; }
                bool TryParse(string input, out int value);
                bool TryParse(string input, int radix, out int value);
                IParser Parser { get; }
                bool CallBase { get; }
                void As();
                ref int GetRef();
            }

            public interface IParser
            {
                bool TryParse(string input, out int value);
                bool TryParse(string input, int radix, out int value);
            }

            public sealed class Sealed { }

            public abstract class Calculator { }
            """;

        /// <summary>
        /// Creates a test class whose <c>Run</c> method has the given body, with the 
        /// given usings and the <see cref="Types"/> appended.
        /// </summary>
        public static string Test(string body, string usings = "using Moq;\nusing static Moq.Syntax;") =>
            (usings + "\n\n" +
            """
            public class Tests
            {
                public void Run()
                {
                    var mock = new Mock<ICalculator>();

            """ +
            string.Join("\n", body.Split('\n').Select(line => "        " + line.TrimEnd('\r'))) + "\n" +
            """
                }
            }

            """ +
            Types).Replace("\r\n", "\n");
    }
}
