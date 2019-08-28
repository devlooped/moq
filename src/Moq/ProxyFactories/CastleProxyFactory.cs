// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
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

#if FEATURE_CAS
using System.Security.Permissions;
using Castle.DynamicProxy.Generators;
#endif

#if FEATURE_COM
using System.Runtime.InteropServices;
#endif

namespace Moq
{
	/// <summary>
	/// An implementation of <see cref="ProxyFactory"/> that is based on Castle DynamicProxy.
	/// </summary>
	internal sealed class CastleProxyFactory : ProxyFactory
	{
		private ProxyGenerationOptions generationOptions;
		private ProxyGenerator generator;

		#if FEATURE_CAS || FEATURE_COM
		static CastleProxyFactory()
		{
			#if FEATURE_CAS
			AttributesToAvoidReplicating.Add<SecurityPermissionAttribute>();
			AttributesToAvoidReplicating.Add<ReflectionPermissionAttribute>();
			AttributesToAvoidReplicating.Add<PermissionSetAttribute>();
			AttributesToAvoidReplicating.Add<UIPermissionAttribute>();
			#endif

			#if FEATURE_COM
			AttributesToAvoidReplicating.Add<MarshalAsAttribute>();
			AttributesToAvoidReplicating.Add<TypeIdentifierAttribute>();
			#endif
		}
		#endif

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

			private Moq.IInterceptor underlying;

			internal Interceptor(Moq.IInterceptor underlying)
			{
				this.underlying = underlying;
			}

			public void Intercept(Castle.DynamicProxy.IInvocation invocation)
			{
				// This implements the `IProxy.Interceptor` property:
				if (invocation.Method == proxyInterceptorGetter)
				{
					invocation.ReturnValue = this.underlying;
					return;
				}

				this.underlying.Intercept(new Invocation(underlying: invocation));
			}
		}

		private sealed class Invocation : Moq.Invocation
		{
			private Castle.DynamicProxy.IInvocation underlying;

			internal Invocation(Castle.DynamicProxy.IInvocation underlying) : base(underlying.Proxy.GetType(), underlying.Method, underlying.Arguments)
			{
				this.underlying = underlying;
			}

			public override void Return()
			{
				Debug.Assert(this.underlying != null);
				Debug.Assert(this.underlying.Method.ReturnType == typeof(void));

				this.underlying = null;
			}

			public override void ReturnBase()
			{
				Debug.Assert(this.underlying != null);

				this.underlying.Proceed();
				this.underlying = null;
			}

			public override void Return(object value)
			{
				Debug.Assert(this.underlying != null);
				Debug.Assert(this.underlying.Method.ReturnType != typeof(void));

				this.SetReturnValue(value);

				this.underlying.ReturnValue = value;
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
