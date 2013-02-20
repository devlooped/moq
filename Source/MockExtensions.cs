using Moq.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Moq
{

    /// <summary>
    /// Provides additional methods on mocks.
    /// </summary>
    /// <remarks>
    /// Those methods are useful for Testeroids support. 
    /// </remarks>
    public static class MockExtensionsTesteroids
    {
        /// <summary>
        /// Resets the current count of calls for the specified method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="me"></param>
        /// <param name="expression"></param>
        public static void ResetCallCounts<T, TResult>(this Mock<T> me, Expression<Func<T, TResult>> expression) where T : class
        {
            var interceptor = me.GetInterceptor(expression);
            ((IList)interceptor.ActualCalls).Clear();
        }

        private static Interceptor GetInterceptor(this Mock mock, Expression fluentExpression)
        {
            var targetExpression = FluentMockVisitor.Accept(fluentExpression);
            var targetLambda = Expression.Lambda<Func<Mock>>(Expression.Convert(targetExpression, typeof(Mock)));

            var targetObject = targetLambda.Compile()();
            return targetObject.Interceptor;
        }
    }
}
