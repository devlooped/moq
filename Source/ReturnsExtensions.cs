#if !NET3x
using System;
using System.Threading.Tasks;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
    /// <summary>
    /// Defines async extension methods on IReturns.
    /// </summary>
    public static class ReturnsExtensions
    {
        /// <summary>
        /// Allows to specify the return value of an asynchronous method.
        /// </summary>
        public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, TResult value) where TMock : class
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(value);

            return mock.Returns(tcs.Task);
        }

        /// <summary>
        /// Allows to specify the exception thrown by an asynchronous method.
        /// </summary>
        public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Exception exception) where TMock : class
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);

            return mock.Returns(tcs.Task);
        }

        private static readonly Random Random = new Random();

        /// <summary>
        /// Allows to specify the delayed return value of an asynchronous method.
        /// </summary>
        public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
            TResult value, TimeSpan delay) where TMock : class
        {
            return DelayedResult(mock, value, delay);
        }

        /// <summary>
        /// Allows to specify the delayed return value of an asynchronous method.
        /// </summary>
        public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
            TResult value, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
        {
            var delay = GetDelay(minDelay, maxDelay, Random);

            return DelayedResult(mock, value, delay);
        }

        /// <summary>
        /// <para>Allows to specify the delayed return value of an asynchronous method.</para>
        /// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
        /// </summary>
        public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
            TResult value, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
        {
            if(random == null)
                throw new ArgumentNullException(nameof(random));

            var delay = GetDelay(minDelay, maxDelay, random);

            return DelayedResult(mock, value, delay);
        }

        /// <summary>
        /// Allows to specify the exception thrown by an asynchronous method.
        /// </summary>
        public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
            Exception exception, TimeSpan delay) where TMock : class
        {
            return DelayedException(mock, exception, delay);
        }

        /// <summary>
        /// Allows to specify the exception thrown by an asynchronous method.
        /// </summary>
        public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
            Exception exception, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
        {
            var delay = GetDelay(minDelay, maxDelay, Random);

            return DelayedException(mock, exception, delay);
        }

        /// <summary>
        /// <para>Allows to specify the exception thrown by an asynchronous method.</para> 
        /// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
        /// </summary>
        public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
            Exception exception, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            var delay = GetDelay(minDelay, maxDelay, random);

            return DelayedException(mock, exception, delay);
        }

        private static TimeSpan GetDelay(TimeSpan minDelay, TimeSpan maxDelay, Random random)
        {
            if (!(minDelay < maxDelay))
                throw new ArgumentException("Mininum delay has to be lower than maximum delay.");

            var min = (int)minDelay.Ticks;
            var max = (int)maxDelay.Ticks;

            return new TimeSpan(random.Next(min, max));
        }

        private static void GuardPositiveDelay(TimeSpan delay)
        {
            if (!(delay > TimeSpan.Zero))
                throw new ArgumentException("Delays have to be greater than zero to ensure an async callback is used.");
        }

        private static IReturnsResult<TMock> DelayedResult<TMock, TResult>(IReturns<TMock, Task<TResult>> mock,
            TResult value, TimeSpan delay)
            where TMock : class
        {
            GuardPositiveDelay(delay);

            var tcs = new TaskCompletionSource<TResult>();
            Task.Delay(delay).ContinueWith(t => tcs.SetResult(value));

            return mock.Returns(tcs.Task);
        }

        private static IReturnsResult<TMock> DelayedException<TMock, TResult>(IReturns<TMock, Task<TResult>> mock,
            Exception exception, TimeSpan delay)
            where TMock : class
        {
            GuardPositiveDelay(delay);

            var tcs = new TaskCompletionSource<TResult>();
            Task.Delay(delay).ContinueWith(t => tcs.SetException(exception));

            return mock.Returns(tcs.Task);
        }
    }
}
#endif
