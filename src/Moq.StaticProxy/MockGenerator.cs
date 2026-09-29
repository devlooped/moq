using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq.Processors;
using Moq.Sdk;
using Stunts;
using Stunts.CodeAnalysis;
using Stunts.Processors;

namespace Moq
{
    /// <summary>
    /// Generates mocks by inspecting the current compilation for 
    /// invocations to methods and object creations with constructors 
    /// annotated with [MockGenerator].
    /// </summary>
    [Generator]
    public class MockGenerator : ISourceGenerator
    {
        readonly StuntGenerator generator;

        public MockGenerator()
        {
            generator = new StuntGenerator()
                .WithNamingConvention(new MockNamingConvention())
                .WithGeneratorAttribute(typeof(MockGeneratorAttribute))
                .WithProcessor(new DefaultImports(typeof(IMocked).Namespace, typeof(LazyInitializer).Namespace))
                .WithProcessor(new CSharpMocked())
                .WithSyntaxReceiver(() => new MockCreationCandidatesReceiver(typeof(MockGeneratorAttribute)))
                .WithSyntaxReceiver(() => new RecursiveMockCandidatesReceiver(typeof(MockGeneratorAttribute)));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(GeneratorExecutionContext context)
        {
            context.AnalyzerConfigOptions.CheckDebugger(nameof(MockGenerator));

            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MoqAnalyzerDir", out var analyerDir))
                DependencyResolver.AddSearchPath(analyerDir);

            generator.Execute(context);
        }

        public void Initialize(GeneratorInitializationContext context) => generator.Initialize(context);

        /// <summary>
        /// Collects object creations, like <c>new Mock&lt;T, T1&gt;()</c>, whose constructor 
        /// or created type is annotated with [MockGenerator], and mocks the type arguments of the created type.
        /// </summary>
        class MockCreationCandidatesReceiver : IStuntCandidatesReceiver
        {
            readonly Type generatorAttribute;
            readonly List<BaseObjectCreationExpressionSyntax> creations = new();

            public MockCreationCandidatesReceiver(Type generatorAttribute) => this.generatorAttribute = generatorAttribute;

            public IEnumerable<(SyntaxNode source, INamedTypeSymbol[] candidate)> GetCandidates(ProcessorContext context)
            {
                var generatorAttr = context.Compilation.GetTypeByMetadataName(generatorAttribute.FullName);
                if (generatorAttr == null)
                    yield break;

                foreach (var creation in creations)
                {
                    var semantic = context.Compilation.GetSemanticModel(creation.SyntaxTree);
                    if (semantic.GetSymbolInfo(creation, context.CancellationToken).Symbol is not IMethodSymbol { MethodKind: MethodKind.Constructor } ctor ||
                        ctor.ContainingType.TypeArguments.IsEmpty ||
                        !ctor.OriginalDefinition.GetAttributes().Concat(ctor.ContainingType.OriginalDefinition.GetAttributes())
                            .Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, generatorAttr)))
                        continue;

                    var types = ctor.ContainingType.TypeArguments.OfType<INamedTypeSymbol>().ToArray();
                    // Open generic or unmockable types are flagged by the corresponding analyzer.
                    if (types.Length != ctor.ContainingType.TypeArguments.Length ||
                        types.Any(type => type.TypeKind == TypeKind.Error || type.GetMembers().OfType<IMethodSymbol>()
                            .SelectMany(method => method.Parameters).Any(parameter => parameter.Type.Kind == SymbolKind.PointerType)))
                        continue;

                    yield return (creation, types);
                }
            }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is BaseObjectCreationExpressionSyntax creation)
                    creations.Add(creation);
            }
        }

        class RecursiveMockCandidatesReceiver : IStuntCandidatesReceiver
        {
            readonly Type generatorAttribute;
            readonly List<SyntaxNode> nodes = new();

            public RecursiveMockCandidatesReceiver(Type generatorAttribute) => this.generatorAttribute = generatorAttribute;

            public IEnumerable<(SyntaxNode source, INamedTypeSymbol[] candidate)> GetCandidates(ProcessorContext context)
            {
                var generatorAttr = context.Compilation.GetTypeByMetadataName(generatorAttribute.FullName);
                if (generatorAttr == null)
                    yield break;

                var moqmodule = context.Compilation.GetTypeByMetadataName("Moq.IMock")!.ContainingModule;
                var sdkmodule = context.Compilation.GetTypeByMetadataName(typeof(IMockRuntime).FullName!)!.ContainingModule;

                foreach (var node in nodes)
                {
                    var semantic = context.Compilation.GetSemanticModel(node.SyntaxTree);
                    if (semantic == null)
                        break;

                    var flow = semantic.AnalyzeDataFlow(node);

                    // There are two possible flows:
                    // mock.Prop.Method().Returns(...): this is recursive "normal" flow In
                    // mock.Setup(x => x.Prop.Method()).Returns(...): 
                    //      this is a recursive "read outside" flow: the flow into the recursive expression
                    //      is actually the lambda parameter, not useful. But the "read outside" is the actual 
                    //      mock variable where the Setup is being performed, which is what we need.
                    // Roslyn 4.x does not report a data flow for a method-group receiver
                    // (mock.GetBar()), so also walk to the root identifier and to an enclosing lambda.
                    bool IsGeneratedMock(ISymbol symbol) =>
                        symbol.DeclaringSyntaxReferences.Length == 1 &&
                        symbol.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken) is VariableDeclaratorSyntax variable &&
                        variable.Initializer?.Value is ExpressionSyntax initializer &&
                        IsGeneratorCall(initializer);

                    // Also covers wrapped generator calls, such as Mock.Get(Mock.Of<T>()).
                    bool IsGeneratorCall(ExpressionSyntax expression) =>
                        expression is InvocationExpressionSyntax or BaseObjectCreationExpressionSyntax &&
                        semantic.GetSymbolInfo(expression, context.CancellationToken).Symbol is IMethodSymbol method &&
                        (method.GetAttributes().Concat(method.MethodKind == MethodKind.Constructor ? method.ContainingType.OriginalDefinition.GetAttributes() : ImmutableArray<AttributeData>.Empty)
                            .Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, generatorAttr)) ||
                         expression is InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 } invocation && IsGeneratorCall(invocation.ArgumentList.Arguments[0].Expression));

                    bool IsMockFlow(ImmutableArray<ISymbol> data) =>
                        data.Length == 1 && IsGeneratedMock(data[0]);

                    bool IsMockAccess()
                    {
                        if (IsMockFlow(flow.DataFlowsIn) || IsMockFlow(flow.ReadOutside))
                            return true;

                        var current = node;
                        while (current is InvocationExpressionSyntax or MemberAccessExpressionSyntax or ParenthesizedExpressionSyntax)
                        {
                            var next = current switch
                            {
                                InvocationExpressionSyntax invocation => invocation.Expression,
                                MemberAccessExpressionSyntax access => access.Expression,
                                ParenthesizedExpressionSyntax parenthesized => parenthesized.Expression,
                                _ => null
                            };
                            if (next == null || next == current)
                                break;

                            current = next;
                        }

                        if (semantic.GetSymbolInfo(current, context.CancellationToken).Symbol is ISymbol root &&
                            IsGeneratedMock(root))
                            return true;

                        foreach (var lambda in node.Ancestors().OfType<AnonymousFunctionExpressionSyntax>())
                        {
                            var lambdaFlow = semantic.AnalyzeDataFlow(lambda);
                            if (IsMockFlow(lambdaFlow.DataFlowsIn) || IsMockFlow(lambdaFlow.ReadOutside))
                                return true;
                        }

                        return false;
                    }

                    // Detect if the variable being accessed was initialized from a generator method call
                    if (!IsMockAccess())
                        continue;

                    var symbol = semantic.GetSymbolInfo(node);
                    // TODO: see if we need to consider symbol.CandidateSymbols too
                    if (symbol.Symbol == null ||
                        SymbolEqualityComparer.Default.Equals(symbol.Symbol.ContainingModule, moqmodule) ||
                        SymbolEqualityComparer.Default.Equals(symbol.Symbol.ContainingModule, sdkmodule))
                        continue;

                    // We only process recursive property and method accesses
                    if (symbol.Symbol.Kind != SymbolKind.Property &&
                        symbol.Symbol.Kind != SymbolKind.Method)
                        continue;

                    // Only intercepted members can return recursive mocks
                    if (symbol.Symbol.IsStatic || symbol.Symbol.IsSealed ||
                        !(symbol.Symbol.IsAbstract || symbol.Symbol.IsVirtual || symbol.Symbol.IsOverride))
                        continue;

                    var methodSymbol = symbol.Symbol as IMethodSymbol;
                    var propertySymbol = symbol.Symbol as IPropertySymbol;

                    // Extension methods are not considered for mocking
                    if (methodSymbol?.IsExtensionMethod == true ||
                        // void methods can't result in a recursive mock either
                        methodSymbol?.ReturnsVoid == true)
                        continue;

                    var type = (methodSymbol?.ReturnType ?? propertySymbol?.Type) as INamedTypeSymbol;
                    if (type != null && type.CanBeIntercepted() == false)
                        continue;

                    yield return (node, new[] { type! });
                }
            }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleMemberAccessExpression) ||
                    syntaxNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression))
                    nodes.Add(syntaxNode);
            }
        }
    }
}