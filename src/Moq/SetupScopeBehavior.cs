using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// A behavior that skips all behaviors that do not apply during a setup scope.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    class SetupScopeBehavior : IStuntBehavior
    {
        static readonly HashSet<Type> setupScopeBehaviors = new HashSet<Type>
        {
            typeof(DefaultValueBehavior),
            typeof(MockContextBehavior),
            typeof(RecursiveMockBehavior),
            typeof(ConfigurePipelineBehavior)
        };

        /// <summary>
        /// Applies only if <see cref="SetupScope.IsActive"/> is <see langword="true"/>.
        /// </summary>
        public bool AppliesTo(IMethodInvocation invocation) => SetupScope.IsActive;

        /// <summary>
        /// Skips all non-setup behaviors from execution.
        /// </summary>
        public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
        {
            foreach (var behavior in invocation.Target.AsMock().Behaviors.Where(x => !setupScopeBehaviors.Contains(x.GetType())))
            {
                invocation.SkipBehaviors.Add(behavior.GetType());
            }

            return next.Invoke(invocation, next);
        }
    }
}
