using System;
using System.Diagnostics;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Represents an executable behavior in the <see cref="IMockBehaviorPipeline.Behaviors"/>, whose 
    /// implementation is provided by the <see cref="ExecuteMockDelegate"/> provided in the 
    /// constructor.
    /// </summary>
    [DebuggerDisplay("{DisplayName}")]
    public class AnonymousMockBehavior : IMockBehavior
    {
        readonly Lazy<string> displayName;
        readonly ExecuteMockDelegate behavior;

        /// <summary>
        /// Creates an instance of the invokable behavior with the given 
        /// delegate and friendly display name.
        /// </summary>
        public AnonymousMockBehavior(ExecuteMockDelegate invoke, string displayName = null)
            : this(invoke, new Lazy<string>(() => displayName))
        {
        }

        /// <summary>
        /// Creates an instance of the invokable behavior with the given 
        /// delegate and friendly display name.
        /// </summary>
        /// <remarks>
        /// Use this constructor overload whenever constructing the display
        /// name is somewhat expensive.
        /// </remarks>
        public AnonymousMockBehavior(ExecuteMockDelegate invoke, Lazy<string> displayName)
        {
            behavior = invoke;
            this.displayName = displayName;
        }

        /// <summary>
        /// Executes the delegate received in the constructor.
        /// </summary>
        public IMethodReturn Execute(IMock mock, IMethodInvocation invocation, GetNextMockBehavior next)
            => behavior(mock, invocation, next);

        /// <summary>
        /// A friendly display name that describes what invoking the 
        /// provided <see cref="ExecuteMockDelegate"/> delegate does.
        /// </summary>
        public string DisplayName => displayName.Value;

        /// <summary>
        /// Returns the <see cref="DisplayName"/>.
        /// </summary>
        public override string ToString() => DisplayName ?? "<unnamed>";
    }
}