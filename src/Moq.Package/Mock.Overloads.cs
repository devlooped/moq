using System.Runtime.CompilerServices;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Instantiates stunts for the specified types.
    /// </summary>
    partial class Mock
    {
        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/>.
        /// </summary>
        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs);

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5, T6>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5, T6, T7>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5, T6, T7, T8>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/>.
        /// </summary>
        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs);

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5, T6>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5, T6, T7>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

        [MockGenerator]
        [CompilerGenerated]
        public static T Of<T, T1, T2, T3, T4, T5, T6, T7, T8>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and returns its <see cref="IMock{T}"/> for setting it up.
        /// </summary>
        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5, T6>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5, T6, T7>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5, T6, T7, T8>(params object[] constructorArgs) where T : class => Create<T>(MockBehavior.Loose, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)).AsMock();

        /// <summary>
        /// Creates a mock that inherits or implements the type <typeparamref name="T"/> 
        /// and returns its <see cref="IMock{T}"/> for setting it up.
        /// </summary>
        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5, T6>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5, T6, T7>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)).AsMock();

        [MockGenerator]
        [CompilerGenerated]
        public static IMock<T> Of2<T, T1, T2, T3, T4, T5, T6, T7, T8>(MockBehavior behavior, params object[] constructorArgs) where T : class => Create<T>(behavior, constructorArgs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)).AsMock();
    }
}