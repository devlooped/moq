using System.Collections.Generic;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Decorator implementation over an <see cref="IMockRuntime"/>.
    /// </summary>
    public abstract class MockRuntimeDecorator : IMockRuntime
    {
        readonly IMockRuntime runtime;

        /// <summary>
        /// Initializes the decorator with the given underlying <see cref="IMockRuntime"/> 
        /// to use as default pass-through.
        /// </summary>
        protected MockRuntimeDecorator(IMockRuntime runtime) => this.runtime = runtime;

        /// <summary>
        /// See <see cref="IMockRuntime.Invocations"/>.
        /// </summary>
        public virtual ICollection<IMethodInvocation> Invocations => runtime.Invocations;

        /// <summary>
        /// See <see cref="IMockRuntime.Object"/>.
        /// </summary>
        public virtual object Object => runtime.Object;

        /// <summary>
        /// See <see cref="IMockRuntime.State"/>.
        /// </summary>
        public virtual StateBag State
        {
            get => runtime.State;
            set => runtime.State = value;
        }

        /// <summary>
        /// See <see cref="IMockRuntime.Setups"/>.
        /// </summary>
        public virtual IEnumerable<IMockBehaviorPipeline> Setups => runtime.Setups;

        /// <summary>
        /// See <see cref="IStunt.Behaviors"/>.
        /// </summary>
        public virtual IList<IStuntBehavior> Behaviors => runtime.Behaviors;

        /// <summary>
        /// See <see cref="IMockRuntime.GetPipeline(IMockSetup)"/>.
        /// </summary>
        public virtual IMockBehaviorPipeline GetPipeline(IMockSetup setup) => runtime.GetPipeline(setup);
    }
}
