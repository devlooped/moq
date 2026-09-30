using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Moq.CodeFixes
{
    /// <summary>
    /// Reports when a mock generator signature uses a ref struct or pointer while
    /// compile-time stunts or unsafe blocks are off.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class UnsafeSignatureAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(MockDiagnostics.UnsafeSignature);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(start =>
            {
                if (start.Compilation.GetTypeByMetadataName(typeof(MockGeneratorAttribute).FullName!) is not { } generator)
                    return;

                start.RegisterOperationAction(context =>
                {
                    var invocation = (IInvocationOperation)context.Operation;
                    if (IsGenerator(invocation.TargetMethod, generator))
                        AnalyzeTypes(context, invocation.TargetMethod.TypeArguments);
                }, OperationKind.Invocation);

                start.RegisterOperationAction(context =>
                {
                    var creation = (IObjectCreationOperation)context.Operation;
                    if (creation.Constructor is { } constructor && IsGenerator(constructor, generator))
                        AnalyzeTypes(context, constructor.ContainingType.TypeArguments);
                }, OperationKind.ObjectCreation);
            });
        }

        static bool IsGenerator(IMethodSymbol method, INamedTypeSymbol generator)
            => HasGenerator(method.OriginalDefinition, generator) ||
                (method.MethodKind == MethodKind.Constructor && HasGenerator(method.ContainingType.OriginalDefinition, generator));

        static bool HasGenerator(ISymbol symbol, INamedTypeSymbol generator)
            => symbol.GetAttributes().Any(attribute =>
                SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, generator));

        static void AnalyzeTypes(OperationAnalysisContext context, ImmutableArray<ITypeSymbol> types)
        {
            if (CompileTimeStuntsAndUnsafe(context.Options.AnalyzerConfigOptionsProvider.GlobalOptions))
                return;

            foreach (var type in types)
            {
                if (type is not INamedTypeSymbol named || named.TypeKind == TypeKind.Error)
                    continue;

                if (UnsafeMember(named, context.Compilation.Assembly) is not { } member)
                    continue;

                context.ReportDiagnostic(Diagnostic.Create(
                    MockDiagnostics.UnsafeSignature,
                    context.Operation.Syntax.GetLocation(),
                    member.ContainingType.Name,
                    member.Name));
                return;
            }
        }

        static bool CompileTimeStuntsAndUnsafe(AnalyzerConfigOptions options)
            => IsTrue(options, "build_property.EnableCompileTimeStunts") &&
               IsTrue(options, "build_property.AllowUnsafeBlocks");

        static bool IsTrue(AnalyzerConfigOptions options, string name)
            => options.TryGetValue(name, out var value) &&
               string.Equals(value, "true", System.StringComparison.OrdinalIgnoreCase);

        static ISymbol? UnsafeMember(INamedTypeSymbol type, IAssemblySymbol assembly)
        {
            if (type.TypeKind == TypeKind.Interface)
            {
                foreach (var current in new[] { type }.Concat(type.AllInterfaces))
                {
                    foreach (var member in current.GetMembers())
                    {
                        if (IsInterfaceMember(member) && UsesUnsafe(member))
                            return member;
                    }
                }

                return null;
            }

            var overridden = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
            for (var current = type; current != null && current.SpecialType != SpecialType.System_Object; current = current.BaseType)
            {
                foreach (var member in current.GetMembers())
                {
                    if (Overridden(member) is ISymbol previous)
                        overridden.Add(previous);
                    if (overridden.Contains(member))
                        continue;
                    if (IsOverridable(member, assembly) && UsesUnsafe(member))
                        return member;
                }
            }

            foreach (var iface in type.AllInterfaces)
            {
                foreach (var member in iface.GetMembers())
                {
                    if (!IsInterfaceMember(member) || !UsesUnsafe(member))
                        continue;

                    var implementation = type.FindImplementationForInterfaceMember(member);
                    if (implementation == null || implementation.ContainingType.TypeKind == TypeKind.Interface)
                        return member;
                }
            }

            return null;
        }

        static bool IsOverridable(ISymbol member, IAssemblySymbol assembly)
            => IsCallable(member) &&
               !member.IsSealed &&
               (member.IsAbstract || member.IsVirtual || member.IsOverride) &&
               Accessible(member, assembly);

        static bool IsInterfaceMember(ISymbol member)
            => IsCallable(member) && !IsExplicit(member);

        static bool IsCallable(ISymbol member)
        {
            if (member.IsStatic || member.IsImplicitlyDeclared)
                return false;

            if (member is IMethodSymbol method)
                return method.MethodKind == MethodKind.Ordinary && method.CanBeReferencedByName;

            return member is IPropertySymbol or IEventSymbol;
        }

        static bool IsExplicit(ISymbol member) => member switch
        {
            IMethodSymbol method => method.ExplicitInterfaceImplementations.Length > 0,
            IPropertySymbol property => property.ExplicitInterfaceImplementations.Length > 0,
            IEventSymbol symbol => symbol.ExplicitInterfaceImplementations.Length > 0,
            _ => false,
        };

        static bool Accessible(ISymbol member, IAssemblySymbol assembly)
        {
            switch (member.DeclaredAccessibility)
            {
                case Accessibility.Public:
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                    return true;
                case Accessibility.Internal:
                    return member.ContainingAssembly.GivesAccessTo(assembly);
                case Accessibility.ProtectedAndInternal:
                    return SymbolEqualityComparer.Default.Equals(member.ContainingAssembly, assembly);
                default:
                    return false;
            }
        }

        static ISymbol? Overridden(ISymbol member) => member switch
        {
            IMethodSymbol method => method.OverriddenMethod,
            IPropertySymbol property => property.OverriddenProperty,
            IEventSymbol symbol => symbol.OverriddenEvent,
            _ => null,
        };

        static bool UsesUnsafe(ISymbol member) => member switch
        {
            IMethodSymbol method => IsUnsafe(method.ReturnType) || method.Parameters.Any(parameter => IsUnsafe(parameter.Type)),
            IPropertySymbol property => IsUnsafe(property.Type) || property.Parameters.Any(parameter => IsUnsafe(parameter.Type)),
            IEventSymbol symbol => IsUnsafe(symbol.Type),
            _ => false,
        };

        static bool IsUnsafe(ITypeSymbol type)
        {
            if (type.IsRefLikeType || type.TypeKind is TypeKind.Pointer or TypeKind.FunctionPointer)
                return true;

            if (type is IArrayTypeSymbol array)
                return IsUnsafe(array.ElementType);

            if (type is INamedTypeSymbol named)
            {
                foreach (var argument in named.TypeArguments)
                {
                    if (IsUnsafe(argument))
                        return true;
                }
            }

            return false;
        }
    }
}
