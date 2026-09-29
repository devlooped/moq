using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1))), created: true) { }
    }

    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1, T2> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2))), created: true) { }
    }

    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1, T2, T3> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3))), created: true) { }
    }

    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1, T2, T3, T4> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4))), created: true) { }
    }

    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1, T2, T3, T4, T5> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))), created: true) { }
    }

    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1, T2, T3, T4, T5, T6> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))), created: true) { }
    }

    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1, T2, T3, T4, T5, T6, T7> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7))), created: true) { }
    }

    /// <summary>
    /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
    /// and also implements the additional interfaces, which can be set up through 
    /// <see cref="Mock{T}.As{TInterface}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [CompilerGenerated]
    partial class Mock<T, T1, T2, T3, T4, T5, T6, T7, T8> : Mock<T> where T : class
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(params object[] constructorArgs)
            : this(MockBehavior.Loose, constructorArgs) { }

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and also implements the additional interfaces.
        /// </summary>
        /// <param name="behavior">Whether the mock is loose or strict.</param>
        /// <param name="constructorArgs">Optional constructor arguments for a mocked class.</param>
        [MockGenerator]
        public Mock(MockBehavior behavior, params object[] constructorArgs)
            : base(new MockView<T>(Mock.Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8))), created: true) { }
    }
}
