using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Stunts;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Extensions for throwing exception from mock invocations.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ThrowsExtension
    {
        /// <summary>
        /// Specifies the exception to throw when the method is invoked.
        /// </summary>
        public static void Throws<TException>(this object target)
            where TException : Exception, new()
        {
            switch (target)
            {
                case Task task:
                    task.Throws(new TException());
                    break;
                case ValueTask value:
                    value.Throws(new TException());
                    break;
                default:
                    target.Throws(new TException());
                    break;
            }
        }

        /// <summary>
        /// Specifies the exception to throw when the method is invoked.
        /// </summary>
        public static void Throws(this object target, Exception exception)
        {
            var setup = MockContext.CurrentSetup;
            if (setup != null)
            {
                var mock = setup.Invocation.Target.AsMock();

                mock.Invocations.Remove(setup.Invocation);
                var behavior = mock.GetPipeline(setup);

                // Scenario for void-returning ValueTask/Task is already covered by the 
                // overloads and switch on <TException> overload above, so we only need 
                // to check for the ValueTask<T>/Task<T> cases here.
                if (setup.Invocation.MethodBase is MethodInfo method &&
                    method.ReturnType != null && method.ReturnType != typeof(void) &&
                    method.ReturnType.IsGenericType)
                {
                    object? returns = null;

                    if (method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        dynamic result = Activator.CreateInstance(typeof(TaskCompletionSource<>)
                            .MakeGenericType(method.ReturnType.GetGenericArguments()[0]));
                        result.SetException(exception);
                        returns = result.Task;
                    }
                    else if (method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                    {
                        dynamic result = Activator.CreateInstance(typeof(TaskCompletionSource<>)
                            .MakeGenericType(method.ReturnType.GetGenericArguments()[0]));
                        result.SetException(exception);
                        returns = Activator.CreateInstance(typeof(ValueTask<>)
                            .MakeGenericType(method.ReturnType.GetGenericArguments()[0]), (object)result.Task);
                    }

                    if (returns != null)
                    {
                        var returnBehavior = behavior.Behaviors.OfType<ReturnsBehavior>().FirstOrDefault();
                        if (returnBehavior != null)
                            returnBehavior.Value = returns;
                        else
                            behavior.Behaviors.Add(new ReturnsBehavior(returns));

                        return;
                    }
                }

                behavior.Behaviors.Add(new AnonymousMockBehavior(
                    (m, i, next) => i.CreateExceptionReturn(exception),
                    new Lazy<string>(() => $"Throws<{exception.GetType().Name}>(\"{exception.Message}\")")
               ));
            }
        }

        /// <summary>
        /// Specifies the exception to throw when the async method is invoked.
        /// </summary>
        public static void Throws(this Task target, Exception exception)
        {
            target.Returns(() =>
            {
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(exception);
                return tcs.Task;
            });
        }

        /// <summary>
        /// Specifies the exception to throw when the async method is invoked.
        /// </summary>
        public static void Throws<TResult>(this Task<TResult> target, Exception exception)
        {
            target.Returns(() =>
            {
                var tcs = new TaskCompletionSource<TResult>();
                tcs.SetException(exception);
                return tcs.Task;
            });
        }

        /// <summary>
        /// Specifies the exception to throw when the async method is invoked.
        /// </summary>
        public static void Throws(this ValueTask target, Exception exception)
        {
            target.Returns(() =>
            {
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(exception);
                return new ValueTask(tcs.Task);
            });
        }

        /// <summary>
        /// Specifies the exception to throw when the async method is invoked.
        /// </summary>
        public static void Throws<TResult>(this ValueTask<TResult> target, Exception exception)
        {
            target.Returns(() =>
            {
                var tcs = new TaskCompletionSource<TResult>();
                tcs.SetException(exception);
                return new ValueTask<TResult>(tcs.Task);
            });
        }
    }
}
