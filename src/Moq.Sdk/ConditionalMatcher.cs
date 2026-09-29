using System;
using System.Diagnostics;
using System.Reflection;
using TypeNameFormatter;

namespace Moq.Sdk
{
    /// <summary>
    /// Matches an argument with the given type <typeparamref name="T"/>, 
    /// as long as it satisfies a given condition.
    /// </summary>
    /// <typeparam name="T">Type of argument being conditioned.</typeparam>
    public class ConditionalMatcher<T> : IArgumentMatcher, IEquatable<ConditionalMatcher<T>>
    {
        static readonly bool IsValueType = typeof(T).IsValueType;
        static readonly bool IsNullable = typeof(T).IsGenericType &&
            typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly string name;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Func<T?, bool> condition;

        /// <summary>
        /// Initializes the matcher with the condition and optional friendly name.
        /// </summary>
        public ConditionalMatcher(Func<T?, bool> condition, string name = "condition")
        {
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the type of the argument this matcher supports.
        /// </summary>
        public Type ArgumentType => typeof(T);

        /// <summary>
        /// Evaluates whether the given value matches this instance.
        /// </summary>
        public bool Matches(object? value)
        {
            // Non-nullable value types never match against a null value.
            if (IsValueType && !IsNullable && value == null)
                return false;

            return (value == null ||
                typeof(T) == value.GetType() ||
                typeof(T).IsAssignableFrom(value.GetType())) &&
                condition((T?)value);
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
        public override string ToString() => "Any<" + ArgumentType.GetFormattedName() + ">(" + name + ")";

        #region Equality

        /// <summary>
        /// Checks whether <paramref name="other"/> has the same condition and friendly name.
        /// </summary>
        public bool Equals(ConditionalMatcher<T> other)
            => ReferenceEquals(condition, other.condition) && name.Equals(other.name);

        /// <summary>
        /// Checks whether <paramref name="other"/> has the same condition and friendly name.
        /// </summary>
        public override bool Equals(object other) => other is ConditionalMatcher<T> matcher && Equals(matcher);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(condition, name);

        #endregion
    }
}
