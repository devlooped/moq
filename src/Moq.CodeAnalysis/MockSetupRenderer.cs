using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Moq.CodeAnalysis
{
    /// <summary>
    /// The generated source for the setup extensions of a single mocked type.
    /// </summary>
    record MockSetup(string HintName, string Source);

    /// <summary>
    /// Identifies a mocked type by its documentation comment ID and the assembly 
    /// that defines it, since the ID alone may match types in other assemblies.
    /// </summary>
    record MockedType(string Id, string Assembly);

    static class MockSetupRenderer
    {
        const string ClassName = "MockSetupExtensions";
        const string Factory = "global::Moq.Sdk.SetupFactory";
        const string Setup = "global::Moq.ISetup";
        const string PropertySetup = "global::Moq.IPropertySetup";

        /// <summary>
        /// Maximum number of parameters supported by <see cref="Func{TResult}"/> and <see cref="Action"/>.
        /// </summary>
        const int MaxDelegateParameters = 16;

        static readonly SymbolDisplayFormat typeFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        static readonly SymbolDisplayFormat keyFormat = SymbolDisplayFormat.FullyQualifiedFormat;

        static readonly SymbolDisplayFormat docFormat = SymbolDisplayFormat.CSharpShortErrorMessageFormat;

        public static MockSetup? Render(Compilation compilation, MockedType mocked, int languageVersion, CancellationToken cancellation)
        {
            if (compilation.GetTypeByMetadataName("Moq.IMock`1") is not { } mockType ||
                compilation.GetTypeByMetadataName("Moq.Sdk.SetupFactory") is null ||
                DocumentationCommentId.GetSymbolsForReferenceId(mocked.Id, compilation)
                    .OfType<INamedTypeSymbol>()
                    .FirstOrDefault(x => x.ContainingAssembly.Identity.GetDisplayName() == mocked.Assembly) is not { } type ||
                !compilation.IsSymbolAccessibleWithin(type, compilation.Assembly))
                return null;

            var name = GetSafeName(mocked.Id);
            var source = new TypeRenderer(compilation, mockType, type, name, languageVersion > MockSetupGenerator.ExtensionMembersVersion, cancellation).Render();

            return source is null ? null : new MockSetup($"{ClassName}.{name}.g.cs", source);
        }

        enum MemberKind { Method, Property }

        sealed class TypeRenderer(Compilation compilation, INamedTypeSymbol mockType, INamedTypeSymbol type, string safeName, bool indexers, CancellationToken cancellation)
        {
            readonly string typeName = type.ToDisplayString(typeFormat);
            readonly string delegatesClass = safeName.Replace('.', '_').Replace('-', '_');
            readonly HashSet<string> reserved = GetReserved(compilation, mockType);
            readonly HashSet<string> signatures = new();
            readonly Dictionary<string, MemberKind> names = new();
            readonly HashSet<string> delegateNames = new();
            readonly StringBuilder members = new();
            readonly StringBuilder delegates = new();
            string self = "mock";

            public string? Render()
            {
                var candidates = GetMembers(type)
                    .Where(member => !member.IsStatic && (!member.IsImplicitlyDeclared || member is IMethodSymbol { MethodKind: MethodKind.DelegateInvoke }) &&
                        !IsObsoleteError(member) && IsAccessible(member))
                    .ToArray();

                // The receiver parameter is in scope for all extension members, so it can't clash with any of their parameters.
                self = Escape(Unique("mock", new HashSet<string>(candidates.SelectMany(GetParameterNames))));

                foreach (var member in candidates)
                {
                    cancellation.ThrowIfCancellationRequested();

                    // Members inherited from other types are invoked through a cast to avoid ambiguities and hiding.
                    var cast = type.TypeKind == TypeKind.Delegate || SymbolEqualityComparer.Default.Equals(member.ContainingType, type) ?
                        null : member.ContainingType.ToDisplayString(typeFormat);

                    switch (member)
                    {
                        case IMethodSymbol { MethodKind: MethodKind.Ordinary or MethodKind.DelegateInvoke } method:
                            RenderMethod(method, cast);
                            break;
                        case IPropertySymbol { IsIndexer: false } property:
                            RenderProperty(property, cast);
                            break;
                        case IPropertySymbol { IsIndexer: true } property:
                            RenderIndexer(property, cast);
                            break;
                    }
                }

                // Events are rendered last since their raise methods are skipped if they collide with other members.
                foreach (var @event in candidates.OfType<IEventSymbol>())
                    RenderRaise(@event);

                if (members.Length == 0)
                    return null;

                var source = new StringBuilder()
                    .Append("// <auto-generated/>\n")
                    .Append("#nullable enable\n")
                    .Append("#pragma warning disable CS0612, CS0618\n")
                    .Append('\n')
                    .Append("namespace Moq\n")
                    .Append("{\n")
                    .Append("    static partial class ").Append(ClassName).Append('\n')
                    .Append("    {\n")
                    .Append("        extension(global::Moq.IMock<").Append(typeName).Append("> ").Append(self).Append(")\n")
                    .Append("        {\n")
                    .Append(members.ToString().TrimEnd('\n')).Append('\n')
                    .Append("        }\n");

                if (delegates.Length > 0)
                {
                    source
                        .Append('\n')
                        .Append("        /// <summary>\n")
                        .Append("        /// Delegates for setting up members of <c>").Append(EscapeXml(type.ToDisplayString(docFormat))).Append("</c> that can't be represented with <c>Func</c> or <c>Action</c>.\n")
                        .Append("        /// </summary>\n")
                        .Append("        public static class ").Append(delegatesClass).Append('\n')
                        .Append("        {\n")
                        .Append(delegates.ToString())
                        .Append("        }\n");
                }

                return source
                    .Append("    }\n")
                    .Append("}\n")
                    .ToString();
            }

            void RenderMethod(IMethodSymbol method, string? cast)
            {
                var name = Escape(method.Name);
                if (reserved.Contains(method.Name) ||
                    (names.TryGetValue(method.Name, out var kind) && kind != MemberKind.Method) ||
                    method.ReturnsByRef || method.ReturnsByRefReadonly ||
                    method.TypeParameters.Any(x => x.AllowsRefLikeType) ||
                    !IsSupported(method.ReturnType) || !method.Parameters.All(x => IsSupported(x.Type)) ||
                    !signatures.Add($"{method.Name}`{method.TypeParameters.Length}({string.Join(",", method.Parameters.Select(Key))})"))
                    return;

                names[method.Name] = MemberKind.Method;

                var typeParameters = method.TypeParameters.Length == 0 ? "" :
                    $"<{string.Join(", ", method.TypeParameters.Select(x => Escape(x.Name)))}>";
                var constraints = RenderConstraints(method.TypeParameters);
                var setup = SetupType(method, typeParameters, constraints);

                AppendDocs(method, $"Sets up <c>{EscapeXml(method.ToDisplayString(docFormat))}</c>.");
                members.Append("            public ").Append(setup).Append(' ').Append(name).Append(typeParameters)
                    .Append('(').Append(string.Join(", ", method.Parameters.Select(x => RenderParameter(x, withDefault: true)))).Append(')')
                    .Append(constraints).Append('\n');

                var typeArguments = setup.Substring(Setup.Length + 1);
                var target = Target(self, cast);
                if (method.Parameters.Any(x => x.RefKind != RefKind.None))
                {
                    var used = new HashSet<string>(method.Parameters.Select(x => x.Name)) { self };
                    var lambda = method.Parameters.Select(parameter => (parameter, lambdaName: Unique("moq", used, suffix: true))).ToArray();

                    members.Append("            {\n");
                    foreach (var parameter in method.Parameters.Where(x => x.RefKind == RefKind.Out))
                        members.Append("                ").Append(Escape(parameter.Name)).Append(" = default!;\n");

                    members.Append("                return ").Append(Factory).Append(".Capture<").Append(typeArguments).Append("((")
                        .Append(string.Join(", ", lambda.Select(x => RenderParameter(x.parameter, x.lambdaName))))
                        .Append(") => ").Append(target).Append('.').Append(name).Append(typeParameters).Append('(')
                        .Append(string.Join(", ", lambda.Select(x => RenderArgument(x.parameter, x.lambdaName))))
                        .Append("), ")
                        .Append(string.Join(", ", method.Parameters.Select(x => Escape(x.Name))))
                        .Append(");\n")
                        .Append("            }\n");
                }
                else
                {
                    members.Append("                => ").Append(Factory).Append(".Capture<").Append(typeArguments).Append("(() => ")
                        .Append(target).Append('.').Append(name).Append(typeParameters).Append('(')
                        .Append(string.Join(", ", method.Parameters.Select(RenderArgument)))
                        .Append("));\n");
                }

                members.Append('\n');
            }

            void RenderProperty(IPropertySymbol property, string? cast)
            {
                if (!CanGet(property) || property.ReturnsByRef || property.ReturnsByRefReadonly || !IsSupported(property.Type) ||
                    reserved.Contains(property.Name) || names.ContainsKey(property.Name))
                    return;

                names[property.Name] = MemberKind.Property;

                var value = property.Type.ToDisplayString(typeFormat);
                var getter = $"global::System.Func<{value}>";
                var access = $"{Cast("x", cast)}.{Escape(property.Name)}";

                AppendDocs(property, $"Sets up the <c>{EscapeXml(property.ToDisplayString(docFormat))}</c> property.");
                if (CanSet(property))
                {
                    var setter = $"global::System.Action<{value}>";
                    members.Append("            public ").Append(PropertySetup).Append('<').Append(getter).Append(", ").Append(setter).Append(", ").Append(value).Append("> ")
                        .Append(Escape(property.Name)).Append('\n')
                        .Append("                => ").Append(Factory).Append(".Property<").Append(typeName).Append(", ").Append(getter).Append(", ").Append(setter).Append(", ").Append(value).Append(">(")
                        .Append(self).Append(", \"").Append(property.Name).Append("\", static x => _ = ").Append(access)
                        .Append(", static (x, value) => ").Append(access).Append(" = value);\n");
                }
                else
                {
                    members.Append("            public ").Append(PropertySetup).Append('<').Append(getter).Append(", ").Append(value).Append("> ")
                        .Append(Escape(property.Name)).Append('\n')
                        .Append("                => ").Append(Factory).Append(".Property<").Append(typeName).Append(", ").Append(getter).Append(", ").Append(value).Append(">(")
                        .Append(self).Append(", \"").Append(property.Name).Append("\", static x => _ = ").Append(access).Append(");\n");
                }

                members.Append('\n');
            }

            void RenderIndexer(IPropertySymbol property, string? cast)
            {
                var name = property.MetadataName;
                if (!CanGet(property) || property.ReturnsByRef || property.ReturnsByRefReadonly ||
                    property.Parameters.Length >= MaxDelegateParameters ||
                    !IsSupported(property.Type) || !property.Parameters.All(x => IsSupported(x.Type)))
                    return;

                if (indexers)
                {
                    if (!signatures.Add($"this[]({string.Join(",", property.Parameters.Select(Key))})"))
                        return;
                }
                else if (reserved.Contains(name) ||
                    (names.TryGetValue(name, out var kind) && kind != MemberKind.Method) ||
                    !signatures.Add($"{name}`0({string.Join(",", property.Parameters.Select(Key))})"))
                {
                    return;
                }
                else
                {
                    names[name] = MemberKind.Method;
                }

                var parameterNames = new HashSet<string>(property.Parameters.Select(x => x.Name)) { self };
                var x = Escape(Unique("x", parameterNames));
                var v = Escape(Unique("value", parameterNames));

                var value = property.Type.ToDisplayString(typeFormat);
                var keys = string.Join(", ", property.Parameters.Select(p => p.Type.ToDisplayString(typeFormat)));
                var getter = $"global::System.Func<{keys}, {value}>";
                var access = $"{Cast(x, cast)}[{string.Join(", ", property.Parameters.Select(RenderArgument))}]";
                var parameters = string.Join(", ", property.Parameters.Select(p => RenderParameter(p, withDefault: true)));

                AppendDocs(property, $"Sets up the <c>{EscapeXml(property.ToDisplayString(docFormat))}</c> indexer.");
                members.Append("            public ").Append(PropertySetup).Append('<').Append(getter);

                var setter = CanSet(property) ? $"global::System.Action<{keys}, {value}>" : null;
                if (setter != null)
                    members.Append(", ").Append(setter);

                members.Append(", ").Append(value).Append("> ")
                    .Append(indexers ? $"this[{parameters}]" : $"{Escape(name)}({parameters})").Append('\n')
                    .Append("                => ").Append(Factory).Append(".Indexer<").Append(typeName).Append(", ").Append(getter);

                if (setter != null)
                    members.Append(", ").Append(setter);

                members.Append(", ").Append(value).Append(">(")
                    .Append(self).Append(", \"").Append(name).Append("\", ").Append(x).Append(" => _ = ").Append(access);

                if (setter != null)
                    members.Append(", (").Append(x).Append(", ").Append(v).Append(") => ").Append(access).Append(" = ").Append(v);

                members.Append(");\n").Append('\n');
            }

            void RenderRaise(IEventSymbol @event)
            {
                var name = "Raise" + @event.Name;
                if (@event.Type is not INamedTypeSymbol { DelegateInvokeMethod: { } invoke } handler ||
                    invoke.Parameters.Any(x => x.RefKind != RefKind.None) ||
                    !invoke.Parameters.All(x => IsSupported(x.Type)) ||
                    reserved.Contains(name) || names.ContainsKey(name))
                    return;

                names[name] = MemberKind.Method;

                // The delegate constraint on the handler type argument requires a non-nullable delegate.
                var handlerType = handler.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(typeFormat);
                var invocation = $"{Factory}.GetEventHandler<{handlerType}>({self}, \"{@event.Name}\")?.Invoke";
                var summary = $"Raises the <c>{EscapeXml(@event.ToDisplayString(docFormat))}</c> event";
                var parameters = invoke.Parameters.Select(x => RenderParameter(x, withDefault: false)).ToArray();
                var arguments = invoke.Parameters.Select(x => Escape(x.Name)).ToArray();

                if (invoke.Parameters.Length == 2 && invoke.Parameters[0].Type.SpecialType == SpecialType.System_Object)
                {
                    // Senders are nullable by convention, even for handlers from nullable-oblivious assemblies.
                    parameters[0] = $"object? {arguments[0]}";

                    if (invoke.Parameters[1].Type.ToDisplayString(keyFormat) == "global::System.EventArgs")
                    {
                        AppendDocs(@event, summary + " with the mock as the sender and empty event arguments.");
                        members.Append("            public void ").Append(name).Append("()").Append('\n')
                            .Append("                => ").Append(invocation).Append('(').Append(self).Append(".Object, global::System.EventArgs.Empty);\n")
                            .Append('\n');
                    }

                    AppendDocs(@event, summary + " with the mock as the sender.");
                    members.Append("            public void ").Append(name).Append('(').Append(parameters[1]).Append(")\n")
                        .Append("                => ").Append(invocation).Append('(').Append(self).Append(".Object, ").Append(arguments[1]).Append(");\n")
                        .Append('\n');
                }

                AppendDocs(@event, summary + ".");
                members.Append("            public void ").Append(name).Append('(').Append(string.Join(", ", parameters)).Append(")\n")
                    .Append("                => ").Append(invocation).Append('(').Append(string.Join(", ", arguments)).Append(");\n")
                    .Append('\n');
            }

            /// <summary>
            /// Gets the setup type for the method, which is typed by a delegate matching its signature.
            /// </summary>
            string SetupType(IMethodSymbol method, string typeParameters, string constraints)
            {
                var result = method.ReturnsVoid ? null : method.ReturnType.ToDisplayString(typeFormat);
                string signature;
                if (type.TypeKind == TypeKind.Delegate)
                {
                    signature = typeName;
                }
                else if (method.Parameters.Length <= MaxDelegateParameters &&
                    method.Parameters.All(x => x.RefKind == RefKind.None && x.ScopedKind == ScopedKind.None))
                {
                    var types = method.Parameters.Select(x => x.Type.ToDisplayString(typeFormat)).ToList();
                    if (result != null)
                        types.Add(result);

                    signature = types.Count == 0 ?
                        "global::System.Action" :
                        $"global::System.{(result == null ? "Action" : "Func")}<{string.Join(", ", types)}>";
                }
                else
                {
                    var name = Unique(method.Name, delegateNames, suffix: true);
                    delegates.Append("            /// <summary>\n")
                        .Append("            /// Matches the signature of <c>").Append(EscapeXml(method.ToDisplayString(docFormat))).Append("</c>.\n")
                        .Append("            /// </summary>\n")
                        .Append("            public delegate ").Append(result ?? "void").Append(' ').Append(Escape(name)).Append(typeParameters)
                        .Append('(').Append(string.Join(", ", method.Parameters.Select(x => RenderParameter(x, withDefault: false)))).Append(')')
                        .Append(constraints).Append(";\n");

                    signature = $"global::Moq.{ClassName}.{delegatesClass}.{Escape(name)}{typeParameters}";
                }

                return result == null ? $"{Setup}<{signature}>" : $"{Setup}<{signature}, {result}>";
            }

            void AppendDocs(ISymbol member, string summary)
            {
                members.Append("            /// <summary>\n")
                    .Append("            /// ").Append(summary).Append('\n')
                    .Append("            /// </summary>\n");

                if (GetObsolete(member) is { } obsolete)
                    members.Append("            ").Append(obsolete).Append('\n');
            }

            bool IsAccessible(ISymbol member) => compilation.IsSymbolAccessibleWithin(member, compilation.Assembly);

            bool CanGet(IPropertySymbol property) => property.GetMethod is { } getter && !IsObsoleteError(getter) && IsAccessible(getter);

            bool CanSet(IPropertySymbol property) => property.SetMethod is { IsInitOnly: false } setter && !IsObsoleteError(setter) && IsAccessible(setter);
        }

        static HashSet<string> GetReserved(Compilation compilation, INamedTypeSymbol mockType)
        {
            // Members of IMock<T>, Mock<T> (including user-provided partial members) and object 
            // would shadow or conflict with the extensions.
            var reserved = new HashSet<string>(mockType.AllInterfaces.Add(mockType)
                .SelectMany(x => x.MemberNames)
                .Concat(compilation.GetSpecialType(SpecialType.System_Object).MemberNames))
            {
                "Sdk"
            };

            for (var arity = 1; arity <= 9; arity++)
            {
                if (compilation.GetTypeByMetadataName($"Moq.Mock`{arity}") is { } mock)
                {
                    foreach (var member in mock.GetMembers().Where(x => x.CanBeReferencedByName))
                        reserved.Add(member.Name);
                }
            }

            return reserved;
        }

        static IEnumerable<string> GetParameterNames(ISymbol member) => member switch
        {
            IMethodSymbol method => method.Parameters.Select(x => x.Name).Concat(method.TypeParameters.Select(x => x.Name)),
            IPropertySymbol property => property.Parameters.Select(x => x.Name),
            IEventSymbol { Type: INamedTypeSymbol { DelegateInvokeMethod: { } invoke } } => invoke.Parameters.Select(x => x.Name),
            _ => Enumerable.Empty<string>(),
        };

        static IEnumerable<ISymbol> GetMembers(INamedTypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Delegate)
                return type.DelegateInvokeMethod is { } invoke ? new[] { invoke } : Array.Empty<ISymbol>();

            if (type.TypeKind == TypeKind.Interface)
            {
                return new[] { type }.Concat(type.AllInterfaces)
                    .SelectMany(x => x.GetMembers())
                    .Where(x => x.IsAbstract || x.IsVirtual);
            }

            // Base members overridden further down the hierarchy (i.e. by a sealed override) 
            // dispatch to the override, so they can only be intercepted via the override itself.
            var members = new List<ISymbol>();
            var overridden = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
            for (var current = type; current is { SpecialType: not SpecialType.System_Object }; current = current.BaseType)
            {
                foreach (var member in current.GetMembers())
                {
                    if (GetOverridden(member) is { } overriddenMember)
                        overridden.Add(overriddenMember);

                    if (!overridden.Contains(member) && (member.IsAbstract || member.IsVirtual || member.IsOverride) && !member.IsSealed)
                        members.Add(member);
                }
            }

            return members;
        }

        static ISymbol? GetOverridden(ISymbol member) => member switch
        {
            IMethodSymbol method => method.OverriddenMethod,
            IPropertySymbol property => property.OverriddenProperty,
            IEventSymbol @event => @event.OverriddenEvent,
            _ => null,
        };

        static string Target(string self, string? cast) => Cast($"{self}.Object", cast);

        static string Cast(string target, string? cast) => cast is null ? target : $"(({cast}){target})";

        static string RenderParameter(IParameterSymbol parameter, bool withDefault)
            => RenderParameter(parameter, Escape(parameter.Name), withDefault);

        static string RenderParameter(IParameterSymbol parameter, string name, bool withDefault = false)
        {
            var builder = new StringBuilder();
            if (parameter.IsParams)
                builder.Append("params ");
            if (parameter.ScopedKind == ScopedKind.ScopedValue ||
                (parameter.ScopedKind == ScopedKind.ScopedRef && parameter.RefKind != RefKind.Out))
                builder.Append("scoped ");

            builder.Append(parameter.RefKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                RefKind.RefReadOnlyParameter => "ref readonly ",
                _ => "",
            });

            builder.Append(parameter.Type.ToDisplayString(typeFormat)).Append(' ').Append(name);

            if (withDefault && parameter.HasExplicitDefaultValue)
                builder.Append(" = ").Append(RenderDefault(parameter));

            return builder.ToString();
        }

        static string RenderArgument(IParameterSymbol parameter) => RenderArgument(parameter, Escape(parameter.Name));

        static string RenderArgument(IParameterSymbol parameter, string name) => parameter.RefKind switch
        {
            RefKind.Ref => "ref ",
            RefKind.Out => "out ",
            RefKind.In or RefKind.RefReadOnlyParameter => "in ",
            _ => "",
        } + name;

        static string RenderDefault(IParameterSymbol parameter)
        {
            var type = parameter.Type.ToDisplayString(typeFormat);
            return parameter.ExplicitDefaultValue switch
            {
                null => "default",
                double value when double.IsNaN(value) => $"({type})(double.NaN)",
                double value when double.IsPositiveInfinity(value) => $"({type})(double.PositiveInfinity)",
                double value when double.IsNegativeInfinity(value) => $"({type})(double.NegativeInfinity)",
                float value when float.IsNaN(value) => $"({type})(float.NaN)",
                float value when float.IsPositiveInfinity(value) => $"({type})(float.PositiveInfinity)",
                float value when float.IsNegativeInfinity(value) => $"({type})(float.NegativeInfinity)",
                var value => $"({type})({SymbolDisplay.FormatPrimitive(value, quoteStrings: true, useHexadecimalNumbers: false)})",
            };
        }

        static string RenderConstraints(ImmutableArray<ITypeParameterSymbol> typeParameters)
        {
            var builder = new StringBuilder();
            foreach (var typeParameter in typeParameters)
            {
                var constraints = new List<string>();
                if (typeParameter.HasReferenceTypeConstraint)
                    constraints.Add(typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
                else if (typeParameter.HasUnmanagedTypeConstraint)
                    constraints.Add("unmanaged");
                else if (typeParameter.HasValueTypeConstraint)
                    constraints.Add("struct");
                else if (typeParameter.HasNotNullConstraint)
                    constraints.Add("notnull");

                constraints.AddRange(typeParameter.ConstraintTypes.Select(x => x.ToDisplayString(typeFormat)));

                if (typeParameter.HasConstructorConstraint && !typeParameter.HasValueTypeConstraint)
                    constraints.Add("new()");

                if (constraints.Count > 0)
                    builder.Append(" where ").Append(Escape(typeParameter.Name)).Append(" : ").Append(string.Join(", ", constraints));
            }

            return builder.ToString();
        }

        static string? GetObsolete(ISymbol member)
        {
            var attribute = FindObsolete(member) ??
                (member is IPropertySymbol property ? FindObsolete(property.GetMethod) ?? FindObsolete(property.SetMethod) : null);

            if (attribute is null)
                return null;

            var arguments = string.Join(", ", attribute.ConstructorArguments.Select(x => x.ToCSharpString()));
            return $"[global::System.Obsolete({arguments})]";
        }

        static bool IsObsoleteError(ISymbol member) => FindObsolete(member) is { } attribute &&
            attribute.ConstructorArguments.Length == 2 &&
            attribute.ConstructorArguments[1].Value is true;

        static AttributeData? FindObsolete(ISymbol? member) => member?.GetAttributes().FirstOrDefault(x =>
            x.AttributeClass is { Name: "ObsoleteAttribute", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } });

        /// <summary>
        /// Whether the type can be used as a generic type argument of the setup and delegate types.
        /// </summary>
        static bool IsSupported(ITypeSymbol type) => type switch
        {
            IPointerTypeSymbol or IFunctionPointerTypeSymbol or IErrorTypeSymbol => false,
            { IsRefLikeType: true } => false,
            ITypeParameterSymbol { AllowsRefLikeType: true } => false,
            IArrayTypeSymbol array => IsSupported(array.ElementType),
            INamedTypeSymbol named => named.TypeArguments.All(IsSupported),
            _ => true,
        };

        /// <summary>
        /// Renders a parameter for signature comparison, where C# does not distinguish
        /// between ref kinds, nullable reference annotations or method type parameter names.
        /// </summary>
        static string Key(IParameterSymbol parameter) => (parameter.RefKind == RefKind.None ? "" : "&") + Key(parameter.Type);

        static string Key(ITypeSymbol type) => type switch
        {
            ITypeParameterSymbol { TypeParameterKind: TypeParameterKind.Method } parameter => "!!" + parameter.Ordinal,
            IArrayTypeSymbol array => $"{Key(array.ElementType)}[{new string(',', array.Rank - 1)}]",
            INamedTypeSymbol { IsGenericType: true } named =>
                $"{named.OriginalDefinition.ToDisplayString(keyFormat)}<{string.Join(",", named.TypeArguments.Select(Key))}>",
            _ => type.ToDisplayString(keyFormat),
        };

        static string Unique(string name, HashSet<string> names, bool suffix = false)
        {
            var candidate = name;
            for (var i = 2; !names.Add(candidate); i++)
                candidate = suffix ? name + i.ToString(CultureInfo.InvariantCulture) : "_" + candidate;

            return candidate;
        }

        static string Escape(string name) => SyntaxFacts.GetKeywordKind(name) == SyntaxKind.None ? name : "@" + name;

        static string EscapeXml(string value) => value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

        static string GetSafeName(string typeId)
        {
            var name = typeId.StartsWith("T:", StringComparison.Ordinal) ? typeId.Substring(2) : typeId;
            var safe = new string(name.Select(x => char.IsLetterOrDigit(x) || x is '.' or '_' or '-' ? x : '_').ToArray());

            // Sanitizing may map distinct types to the same name, so disambiguate with a stable hash.
            if (safe != name)
                safe += "." + Fnv1a(typeId).ToString("x8", CultureInfo.InvariantCulture);

            return safe;
        }

        static uint Fnv1a(string value)
        {
            var hash = 2166136261;
            foreach (var c in value)
                hash = (hash ^ c) * 16777619;

            return hash;
        }
    }
}
