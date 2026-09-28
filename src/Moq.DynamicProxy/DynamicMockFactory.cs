using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Provides an <see cref="IMockFactory"/> that creates types at run-time using Castle DynamicProxy.
    /// </summary>
    public class DynamicMockFactory : DynamicStuntFactory, IMockFactory
    {
        /// <inheritdoc />
        public object CreateMock(Assembly mocksAssembly, Type baseType, Type[] implementedInterfaces, object[] constructorArguments)
            => CreateStunt(mocksAssembly, baseType, implementedInterfaces, constructorArguments);

        /// <summary>
        /// Creates the mock proxy.
        /// </summary>
        protected override object CreateProxy(Type baseType, Type[] implementedInterfaces, object[] constructorArguments, ProxyGenerationOptions options, Func<IInterceptor> getDefaultInterceptor)
        {
            if (!implementedInterfaces.Contains(typeof(IMocked)))
            {
                var fixedInterfaces = new Type[implementedInterfaces.Length + 1];
                fixedInterfaces[0] = typeof(IMocked);
                implementedInterfaces.CopyTo(fixedInterfaces, 1);
                implementedInterfaces = fixedInterfaces;
            }

            var mocked = (IMocked)Generator.CreateClassProxy(baseType, implementedInterfaces, options, constructorArguments,
                new IInterceptor[] { new MockInterceptor(), getDefaultInterceptor() });

            // Save for cloning purposes. We opened a generated proxy from DP to figure out the ctor signature it creates.
            // The lazy-calculated value allows us to provide a new interceptor for every retrieval. 
            // Add first-class support in statebag for this pattern of either Func<T> for values, or 
            // Lazy<T>, since both could be quite useful for expensive state that may be needed lazily.
            mocked.Runtime.State.Set(".ctor", () => new object[] { new IInterceptor[] { new MockInterceptor(), getDefaultInterceptor() } }.Concat(constructorArguments).ToArray());

            return mocked;
        }

        class MockInterceptor : IInterceptor
        {
            IMockRuntime? mock;

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.DeclaringType == typeof(IMocked))
                    invocation.ReturnValue = LazyInitializer.EnsureInitialized(ref mock, () => new DefaultMockRuntime((IStunt)invocation.Proxy));
                else
                    invocation.Proceed();
            }
        }
    }
}
