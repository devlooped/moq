using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/>, 
    /// which is set up by invoking its members on this mock.
    /// </summary>
    /// <remarks>
    /// This class is provided as source, so it can be customized by adding members 
    /// and constructors to it in a partial class, and by implementing 
    /// <see cref="OnCreated"/> to configure every mock it creates.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T> : IMock<T> where T : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IMock<T> view;

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : this(new MockView<T>(Mock.Create<T>(behavior, constructorArgs)), created: true) { }

        /// <summary>
        /// Initializes the mock over the <paramref name="view"/> of a mocked instance, 
        /// invoking <see cref="OnCreated"/> if it was just <paramref name="created"/>.
        /// </summary>
        internal Mock(MockView<T> view, bool created)
        {
            this.view = view;
            if (created)
                OnCreated();
        }

        /// <summary>
        /// Invoked after a new mock is created, to customize it.
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// The mocked object instance.
        /// </summary>
        public T Object => view.Object;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IMock.Object => Object;

        /// <summary>
        /// The behavior of the mock.
        /// </summary>
        public MockBehavior Behavior
        {
            get => view.Behavior;
            set => view.Behavior = value;
        }

        /// <summary>
        /// The provider of default values for members that are not set up.
        /// </summary>
        public DefaultValueProvider DefaultValue
        {
            get => view.DefaultValue;
            set => view.DefaultValue = value;
        }

        /// <summary>
        /// Whether the base member implementation is invoked by default 
        /// for members that are not set up.
        /// </summary>
        public bool CallBase
        {
            get => view.CallBase;
            set => view.CallBase = value;
        }

        /// <summary>
        /// Gets a mock for setting up the <typeparamref name="TInterface"/> 
        /// implemented by the same mocked object.
        /// </summary>
        /// <exception cref="InvalidCastException">The mock does not implement <typeparamref name="TInterface"/>.</exception>
        public Mock<TInterface> As<TInterface>() where TInterface : class
            => Mock.Get<TInterface>((object)Object);
    }
}
