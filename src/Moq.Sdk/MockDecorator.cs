using System.Collections.Generic;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Decorator implementation over an <see cref="IMock"/>.
    /// </summary>
    public abstract class MockDecorator : IMock
    {
        readonly IMock mock;

        /// <summary>
        /// Initializes the decorator with the given underlying <see cref="IMock"/> 
        /// to use as default pass-through.
        /// </summary>
        protected MockDecorator(IMock mock) => this.mock = mock;

        /// <summary>
        /// See <see cref="IMock.Invocations"/>.
        /// </summary>
        public virtual ICollection<IMethodInvocation> Invocations => mock.Invocations;

        /// <summary>
        /// See <see cref="IMock.Object"/>.
        /// </summary>
        public virtual object Object => mock.Object;

        /// <summary>
        /// See <see cref="IMock.State"/>.
        /// </summary>
        public virtual StateBag State
        {
            get => mock.State;
            set => mock.State = value;
        }

        /// <summary>
        /// See <see cref="IMock.Setups"/>.
        /// </summary>
        public virtual IEnumerable<IMockBehaviorPipeline> Setups => mock.Setups;

        /// <summary>
        /// See <see cref="IStunt.Behaviors"/>.
        /// </summary>
        public virtual IList<IStuntBehavior> Behaviors => mock.Behaviors;

        /// <summary>
        /// See <see cref="IMock.GetPipeline(IMockSetup)"/>.
        /// </summary>
        public virtual IMockBehaviorPipeline GetPipeline(IMockSetup setup) => mock.GetPipeline(setup);
    }
}
