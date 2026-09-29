using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Instantiates mocks for the specified types.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock
    {
        /// <summary>
        /// Gets the configuration and introspection for the given mocked instance.
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="instance"/> is not a mock.</exception>
        public static Mock<T> Get<T>(T instance) where T : class => new Mock<T>(new MockView<T>(instance), created: false);

        /// <summary>
        /// Gets the configuration and introspection for the given mocked instance 
        /// as the <typeparamref name="T"/> it implements.
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="instance"/> is not a mock.</exception>
        /// <exception cref="InvalidCastException">The mock does not implement <typeparamref name="T"/>.</exception>
        public static Mock<T> Get<T>(object instance) where T : class => new Mock<T>(new MockView<T>((T)instance), created: false);

        /// <summary>
        /// Creates the mock instance by using the specified types to 
        /// lookup the mock type in the assembly defining this class.
        /// </summary>
        internal static T Create<T>(MockBehavior behavior, object[] constructorArgs, params Type[] interfaces) where T : class
        {
            var mock = MockFactory.Default.CreateMock(typeof(Mock).Assembly, typeof(T), interfaces, constructorArgs);
            // Delegate mocks are delegates bound to the mocked instance.
            var mocked = (IMocked)(mock is Delegate @delegate ? @delegate.Target : mock);

            mocked.Initialize(behavior);

            return (T)mock;
        }
    }
}