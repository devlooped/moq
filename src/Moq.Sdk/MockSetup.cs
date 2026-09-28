using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Stunts;
using TypeNameFormatter;

namespace Moq.Sdk
{
    /// <summary>
    /// Default implementation of the configuration used to set up a <see cref="IMockBehaviorPipeline" />.
    /// </summary>
    public partial class MockSetup : IMockSetup, IEquatable<MockSetup>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IMethodInvocation invocation;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IArgumentMatcher[] matchers;

        /// <summary>
        /// Initializes the class with the <paramref name="invocation"/> and 
        /// <paramref name="matchers"/> that determine this setup instance.
        /// </summary>
        public MockSetup(IMethodInvocation invocation, IArgumentMatcher[] matchers)
        {
            this.invocation = invocation ?? throw new ArgumentNullException(nameof(invocation));
            this.matchers = matchers ?? throw new ArgumentNullException(nameof(matchers));
        }

        /// <inheritdoc />
        public IMethodInvocation Invocation => invocation;

        /// <inheritdoc />
        public IArgumentMatcher[] Matchers => matchers;

        /// <inheritdoc />
        public Times? Occurrence { get; set; }

        /// <inheritdoc />
        public StateBag State { get; } = new StateBag();

        /// <inheritdoc />
        public bool AppliesTo(IMethodInvocation actualInvocation)
        {
            if (actualInvocation == null)
                throw new ArgumentNullException(nameof(actualInvocation));

            if (invocation.MethodBase != actualInvocation.MethodBase)
                return false;

            if (actualInvocation.Arguments.Count != matchers.Length)
                return false;

            for (var i = 0; i < actualInvocation.Arguments.Count; i++)
            {
                if (!matchers[i].Matches(actualInvocation.Arguments.GetValue(i)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a friendly representation of the object.
        /// </summary>
        /// <devdoc>
        /// We don't want to optimize code coverage for this since it's a debugger aid only. 
        /// Annotating this method with DebuggerNonUserCode achieves that.
        /// No actual behavior depends on these strings.
        /// </devdoc>
        [DebuggerNonUserCode]
        public override string ToString()
        {
            var result = new StringBuilder();
            if (invocation.MethodBase is MethodInfo info)
            {
                if (info.ReturnType != typeof(void))
                    result.Append(info.ReturnType.GetFormattedName()).Append(" ");
                else
                    result.Append("void ");
            }

            if (invocation.MethodBase.IsSpecialName)
            {
                if (invocation.MethodBase.Name.StartsWith("get_", StringComparison.Ordinal) ||
                    invocation.MethodBase.Name.StartsWith("set_", StringComparison.Ordinal))
                {
                    result.Append(invocation.MethodBase.Name.Substring(4));
                }
                else if (invocation.MethodBase.Name.StartsWith("add_", StringComparison.Ordinal))
                {
                    result.Append(invocation.MethodBase.Name.Substring(4) + " += ");
                }
                else if (invocation.MethodBase.Name.StartsWith("remove_", StringComparison.Ordinal))
                {
                    result.Append(invocation.MethodBase.Name.Substring(7) + " -= ");
                }
            }
            else
            {
                result.Append(invocation.MethodBase.Name);
            }

            if (invocation.MethodBase.IsGenericMethod)
            {
                var generic = ((MethodInfo)invocation.MethodBase).GetGenericMethodDefinition();
                result
                    .Append("<")
                    .Append(string.Join(", ", generic.GetGenericArguments().Select(t => t.Name)))
                    .Append(">");
            }

            var parameters = invocation.MethodBase.GetParameters();
            // TODO: render indexer arguments?
            if (!invocation.MethodBase.IsSpecialName)
            {
                return result
                    .Append("(")
                    .Append(string.Join(", ", parameters.Select((p, i) =>
                        (p.IsOut ? "out " : (p.ParameterType.IsByRef ? "ref " : "")) +
                        p.ParameterType.GetFormattedName() + " " +
                        p.Name +
                        (p.IsOut ? "" : (" = " + matchers[i]))
                    )))
                    .Append(")")
                    .ToString();
            }

            return result.ToString();
        }

        #region Equality

        /// <inheritdoc />
        public bool Equals(IMockSetup? other)
            => other != null && Invocation.Equals(other.Invocation) && Matchers.SequenceEqual(other.Matchers);

        /// <inheritdoc />
        public bool Equals(MockSetup other)
            => Equals((IMockSetup)other);

        /// <inheritdoc />
        public override bool Equals(object obj)
            => Equals(obj as IMockSetup);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Invocation);
            foreach (var matcher in Matchers)
            {
                hash.Add(matcher);
            }
            return hash.ToHashCode();
        }

        #endregion
    }
}