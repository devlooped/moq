using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq
{
    /// <summary>
    /// Extensions for throwing exception from mock invocations.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ThrowsExtension
    {
        static readonly MethodInfo fromException = typeof(Task).GetMethods()
            .First(x => x.Name == nameof(Task.FromException) && x.IsGenericMethodDefinition);

        /// <summary>
        /// Specifies the exception to throw when the member is invoked. 
        /// Async members return a faulted task instead.
        /// </summary>
        public static TSetup Throws<TSetup>(this TSetup setup, Exception exception) where TSetup : ISetup
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var returnType = (setup.Sdk.Invocation.MethodBase as MethodInfo)?.ReturnType;
            if (returnType == typeof(Task))
                setup.SetReturnValue(_ => Task.FromException(exception));
            else if (returnType == typeof(ValueTask))
                setup.SetReturnValue(_ => new ValueTask(Task.FromException(exception)));
            else if (returnType?.IsGenericType == true && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                setup.SetReturnValue(_ => fromException.MakeGenericMethod(returnType.GetGenericArguments()).Invoke(null, new object[] { exception }));
            else if (returnType?.IsGenericType == true && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                setup.SetReturnValue(_ => Activator.CreateInstance(returnType,
                    fromException.MakeGenericMethod(returnType.GetGenericArguments()).Invoke(null, new object[] { exception })));
            else
                setup.SetException(exception);

            return setup;
        }

        /// <summary>
        /// Specifies the type of exception to throw when the member is invoked. 
        /// Async members return a faulted task instead.
        /// </summary>
        public static ISetup Throws<TException>(this ISetup setup) where TException : Exception, new()
            => setup.Throws(new TException());
    }
}
