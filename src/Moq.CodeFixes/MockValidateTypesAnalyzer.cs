using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Stunts.CodeAnalysis;

namespace Moq
{
    /// <summary>
    /// Analyzer that validates the used types for a mock generator method.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class MockValidateTypesAnalyzer : ValidateTypesAnalyzer
    {
        /// <summary>
        /// Initializes the validator passing <see cref="MockGeneratorAttribute"/> as the 
        /// generator attribute to look for to signal generator methods.
        /// </summary>
        public MockValidateTypesAnalyzer()
            : base(typeof(MockGeneratorAttribute)) { }
    }
}
