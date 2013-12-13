#if !NET3x && !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Language.Flow
{
    /// <summary>
    /// Defines async extension methods on IReturns.
    /// </summary>
    public static class IReturnsExtensions
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
    }
}
#endif
