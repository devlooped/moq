using System;
using System.Reflection;
using Stunts;
using Moq.Sdk;

namespace Moq
{
    static class Extensions
    {
        const string TaskFullName = "System.Threading.Tasks.Task";

        public static void EnsureCompatible(this IMethodInvocation invocation, Delegate @delegate)
        {
            var method = @delegate.GetMethodInfo();
            if (invocation.Arguments.Count != method.GetParameters().Length)
                throw new ArgumentException("Callback arguments do not match target invocation arguments.");

            // TODO: validate assignability
        }

        public static bool CanBeIntercepted(this Type type)
            => !type.IsValueType &&
               !type.FullName.StartsWith(TaskFullName, StringComparison.Ordinal) &&
               (type.IsInterface ||
               (type.IsClass && !type.IsSealed));

        public static IMoq<T> AsMoq<T>(this T instance) where T : class => new Moq<T>(instance.AsMock());
    }
}
