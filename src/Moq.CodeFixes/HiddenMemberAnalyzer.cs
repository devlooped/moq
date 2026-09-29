using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Moq.CodeFixes
{
    /// <summary>
    /// Reports accesses like <c>mock.As()</c> that bind to a member of the mock itself, 
    /// rather than to the typed setup of the mocked type member with the same name.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HiddenMemberAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(MockDiagnostics.HiddenMember);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleMemberAccessExpression);
        }

        static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var access = (MemberAccessExpressionSyntax)context.Node;
            var name = access.Name.Identifier.ValueText;

            // Assigning mock members (i.e. mock.CallBase = true) configures the mock itself.
            if (name == "Object" ||
                access.Parent is AssignmentExpressionSyntax assignment && assignment.Left == access)
                return;

            var symbol = context.SemanticModel.GetSymbolInfo(access, context.CancellationToken).Symbol;
            if (symbol == null || MockSyntax.IsGeneratedSetup(symbol) ||
                MockSyntax.GetMockedType(context.SemanticModel.GetTypeInfo(access.Expression, context.CancellationToken).Type) is not { } mocked ||
                MockSyntax.FindMember(mocked, name) is not { } member)
                return;

            context.ReportDiagnostic(Diagnostic.Create(
                MockDiagnostics.HiddenMember,
                access.Name.GetLocation(),
                name,
                member.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
        }
    }
}
