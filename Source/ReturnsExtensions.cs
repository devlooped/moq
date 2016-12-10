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
        /// Specifies the value to return from an asynchronous method.
        /// </summary>
        /// <typeparam name="TMock">Mocked type.</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
        /// <param name="value">The value to return, or <see longword="null"/>.</param>
        public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, TResult value) where TMock : class
        {
            return mock.ReturnsAsync(() => value);
        }

        /// <summary>
        /// Specifies a function that will calculate the value to return from the asynchronous method.
        /// </summary>
        /// <typeparam name="TMock">Mocked type.</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
        /// <param name="valueFunction">The function that will calculate the return value.</param>
        public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<TResult> valueFunction) where TMock : class
		{
		    return mock.Returns(() => Task.FromResult(valueFunction()));
		}

        /// <summary>
        /// Specifies the exception to throw when the asynchronous method is invoked.
        /// </summary>
        /// <typeparam name="TMock">Mocked type.</typeparam>
        /// <typeparam name="TResult">Type of the return value.</typeparam>
        /// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
        /// <param name="exception">Exception instance to throw.</param>
        public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Exception exception) where TMock : class
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);

            return mock.Returns(tcs.Task);
        }
    }
}
#endif
