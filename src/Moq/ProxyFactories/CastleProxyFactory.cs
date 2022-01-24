// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Castle.DynamicProxy;

using Moq.Internals;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// An implementation of <see cref="ProxyFactory"/> that is based on Castle DynamicProxy.
	/// </summary>
	internal sealed class CastleProxyFactory : ProxyFactory
	{
		private ProxyGenerationOptions generationOptions;
		private ProxyGenerator generator;

		public CastleProxyFactory()
		{
			this.generationOptions = new ProxyGenerationOptions { Hook = new IncludeObjectMethodsHook(), BaseTypeForInterfaceProxy = typeof(InterfaceProxy) };
			this.generator = new ProxyGenerator();
		}

		/// <inheritdoc />
		public override object CreateProxy(Type mockType, Moq.IInterceptor interceptor, Type[] interfaces, object[] arguments)
		{
			// All generated proxies need to implement `IProxy`:
			var additionalInterfaces = new Type[1 + interfaces.Length];
			additionalInterfaces[0] = typeof(IProxy);
			Array.Copy(interfaces, 0, additionalInterfaces, 1, interfaces.Length);

			if (mockType.IsInterface)
			{
				// While `CreateClassProxy` could also be used for interface types,
				// `CreateInterfaceProxyWithoutTarget` is much faster (about twice as fast):
				return generator.CreateInterfaceProxyWithoutTarget(mockType, additionalInterfaces, this.generationOptions, new Interceptor(interceptor));
			}
			else if (mockType.IsDelegateType())
			{
				var options = new ProxyGenerationOptions();
				options.AddDelegateTypeMixin(mockType);
				var container = generator.CreateClassProxy(typeof(object), additionalInterfaces, options, new Interceptor(interceptor));
				return Delegate.CreateDelegate(mockType, container, container.GetType().GetMethod("Invoke"));
			}

			try
			{
				return generator.CreateClassProxy(mockType, additionalInterfaces, this.generationOptions, arguments, new Interceptor(interceptor));
			}
			catch (TypeLoadException e)
			{
				throw new ArgumentException(Resources.TypeNotMockable, e);
			}
			catch (MissingMethodException e)
			{
				throw new ArgumentException(Resources.ConstructorNotFound, e);
			}
		}

		public override bool IsMethodVisible(MethodInfo method, out string messageIfNotVisible)
		{
			return ProxyUtil.IsAccessible(method, out messageIfNotVisible);
		}

		public override bool IsTypeVisible(Type type)
		{
			return ProxyUtil.IsAccessible(type);
		}

		private sealed class Interceptor : Castle.DynamicProxy.IInterceptor
		{
			private static readonly MethodInfo proxyInterceptorGetter = typeof(IProxy).GetProperty(nameof(IProxy.Interceptor)).GetMethod;

			private Moq.IInterceptor interceptor;

			internal Interceptor(Moq.IInterceptor interceptor)
			{
				this.interceptor = interceptor;
			}

			public void Intercept(Castle.DynamicProxy.IInvocation underlying)
			{
				// This implements the `IProxy.Interceptor` property:
				if (underlying.Method == proxyInterceptorGetter)
				{
					underlying.ReturnValue = this.interceptor;
					return;
				}

				var invocation = new Invocation(underlying);
				try
				{
					this.interceptor.Intercept(invocation);
					underlying.ReturnValue = invocation.ReturnValue;
				}
				catch (Exception ex)
				{
					invocation.Exception = ex;
					throw;
				}
				finally
				{
					invocation.DetachFromUnderlying();
				}
			}
		}

		private sealed class Invocation : Moq.Invocation
		{
			private Castle.DynamicProxy.IInvocation underlying;

			internal Invocation(Castle.DynamicProxy.IInvocation underlying) : base(underlying.Proxy.GetType(), underlying.Method, underlying.Arguments)
			{
				this.underlying = underlying;
			}

			public override object CallBase()
			{
				Debug.Assert(this.underlying != null);

				this.underlying.Proceed();
				return this.underlying.ReturnValue;
			}

			public void DetachFromUnderlying()
			{
				this.underlying = null;
			}
		}

		/// <summary>
		/// This hook tells Castle DynamicProxy to proxy the default methods it suggests,
		/// plus some of the methods defined by <see cref="object"/>, e.g. so we can intercept
		/// <see cref="object.ToString()"/> and give mocks useful default names.
		/// </summary>
		private sealed class IncludeObjectMethodsHook : AllMethodsHook
		{
			public override bool ShouldInterceptMethod(Type type, MethodInfo method)
			{
				return base.ShouldInterceptMethod(type, method) || IsRelevantObjectMethod(method);
			}

			private static bool IsRelevantObjectMethod(MethodInfo method)
			{
				return method.DeclaringType == typeof(object) && (method.Name == nameof(object.ToString)
				                                              ||  method.Name == nameof(object.Equals)
				                                              ||  method.Name == nameof(object.GetHashCode));
			}
		}
	}
}
