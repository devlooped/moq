using System;
using Avatars;

namespace Moq.Sdk
{
    /// <summary>
    /// An <see cref="IAvatarBehavior"/> that keeps track of property 
    /// get/set invocations so that a stunt behaves like a stub.
    /// </summary>
    public class PropertyBehavior : IAvatarBehavior
    {
        /// <summary>
        /// Whether invoking a property setter requires the mock to be in a 
        /// setup state (which is always required for strict mocks, but never 
        /// for loose mocks by default).
        /// </summary>
        public bool SetterRequiresSetup { get; set; }

        /// <summary>
        /// Determines whether the given invocation is a property 
        /// getter or setter.
        /// </summary>
        public bool AppliesTo(IMethodInvocation invocation)
            => invocation.MethodBase.IsSpecialName &&
              // Specifically excludes indexers which are better handled via Returns since they can contain matchers for the index.
              ((invocation.MethodBase.Name.StartsWith("get_", StringComparison.Ordinal) && invocation.MethodBase.GetParameters().Length == 0) ||
               (invocation.MethodBase.Name.StartsWith("set_", StringComparison.Ordinal) && invocation.MethodBase.GetParameters().Length == 1));

        /// <summary>
        /// Gets or sets the value of the given property as an entry in the mock <see cref="IMock.State"/>.
        /// </summary>
        public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));

            var state = (invocation.Target as IMocked ?? throw new ArgumentException(ThisAssembly.Strings.TargetNotMock, nameof(invocation)))
                .Mock.State;

            if (invocation.MethodBase.Name.StartsWith("get_", StringComparison.Ordinal) &&
                state.TryGetValue<object>("_" + invocation.MethodBase.Name.Substring(4), out var value))
                return invocation.CreateValueReturn(value);

            if (invocation.MethodBase.Name.StartsWith("set_", StringComparison.Ordinal) &&
                (!SetterRequiresSetup || SetupScope.IsActive))
            {
                state.Set("_" + invocation.MethodBase.Name.Substring(4), invocation.Arguments.GetValue(0));
                return invocation.CreateReturn();
            }

            return next(invocation, next);
        }
    }
}
