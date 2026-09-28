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
        public static IMock<T> Get<T>(T instance) where T : class => new MockView<T>(instance);

        /// <summary>
        /// Creates the mock instance by using the specified types to 
        /// lookup the mock type in the assembly defining this class.
        /// </summary>
        static T Create<T>(MockBehavior behavior, object[] constructorArgs, params Type[] interfaces) where T : class
        {
            var mocked = (IMocked)MockFactory.Default.CreateMock(typeof(Mock).Assembly, typeof(T), interfaces, constructorArgs);

            mocked.Initialize(behavior);

            return (T)mocked;
        }
    }
}