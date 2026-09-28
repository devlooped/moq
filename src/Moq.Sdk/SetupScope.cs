using System;
using System.Threading;

namespace Moq
{
    /// <summary>
    /// Marks the mock as being set up, meaning 
    /// invocation tracking and strict behavior 
    /// will be suspended until the scope is 
    /// is disposed. Scopes can be nested.
    /// </summary>
    /// <devdoc>
    /// We make an exception with the namespace for this 
    /// type, since it's intended to be used by users 
    /// quite regularly, unless they are leveraging the 
    /// <c>using static Moq.Syntax;</c> which allows the 
    /// simpler <c>using (Setup()) { ... }</c> syntax.
    /// </devdoc>
    public class SetupScope : IDisposable
    {
        static readonly AsyncLocal<bool?> setup = new AsyncLocal<bool?>();

        readonly bool? previous;

        /// <summary>
        /// Initializes the setup scope.
        /// </summary>
        public SetupScope()
        {
            previous = setup.Value;
            setup.Value = true;
        }

        /// <summary>
        /// Whether there is an active setup scope in the running 
        /// execution context.
        /// </summary>
        public static bool IsActive => setup.Value == true;

        /// <summary>
        /// Disposes the scope, restoring <see cref="IsActive"/> 
        /// to its value before the scope was created.
        /// </summary>
        public void Dispose() => setup.Value = previous;
    }
}
