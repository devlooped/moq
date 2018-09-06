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
		private Dictionary<Type, Type> delegateInterfaceCache;
		private int delegateInterfaceSuffix;
		private ProxyGenerationOptions generationOptions;
		private ProxyGenerator generator;

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

		public CastleProxyFactory()
		{
			this.delegateInterfaceCache = new Dictionary<Type, Type>();
			this.generationOptions = new ProxyGenerationOptions { Hook = new IncludeObjectMethodsHook(), BaseTypeForInterfaceProxy = typeof(InterfaceProxy) };
			this.generator = new ProxyGenerator();
		}

		/// <inheritdoc />
		public override object CreateProxy(Type mockType, Moq.IInterceptor interceptor, Type[] interfaces, object[] arguments)
		{
			if (mockType.GetTypeInfo().IsInterface)
			{
				// While `CreateClassProxy` could also be used for interface types,
				// `CreateInterfaceProxyWithoutTarget` is much faster (about twice as fast):
				return generator.CreateInterfaceProxyWithoutTarget(mockType, interfaces, this.generationOptions, new Interceptor(interceptor));
			}

			try
			{
				return generator.CreateClassProxy(mockType, interfaces, this.generationOptions, arguments, new Interceptor(interceptor));
			}
			catch (TypeLoadException e)
			{
				throw new ArgumentException(Resources.InvalidMockClass, e);
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

		/// <inheritdoc />
		public override Type GetDelegateProxyInterface(Type delegateType, out MethodInfo delegateInterfaceMethod)
		{
			Type delegateInterfaceType;

			lock (this)
			{
				if (!delegateInterfaceCache.TryGetValue(delegateType, out delegateInterfaceType))
				{
					var interfaceName = String.Format(CultureInfo.InvariantCulture, "DelegateInterface_{0}_{1}",
					                                  delegateType.Name, delegateInterfaceSuffix++);

					var moduleBuilder = generator.ProxyBuilder.ModuleScope.ObtainDynamicModule(true);
					var newTypeBuilder = moduleBuilder.DefineType(interfaceName,
					                                              TypeAttributes.Public | TypeAttributes.Interface |
					                                              TypeAttributes.Abstract);

					var invokeMethodOnDelegate = delegateType.GetMethod("Invoke");

					var delegateParameters = invokeMethodOnDelegate.GetParameters();
					var delegateParameterTypes = delegateParameters.Select(p => p.ParameterType).ToArray();

					var delegateReturnParameter = invokeMethodOnDelegate.ReturnParameter;
					var delegateReturnType = delegateReturnParameter.ParameterType;

					// Note: The following conditional compilation symbol is currently never defined.
					// The CLR / CoreCLR do not currently perform a strict signature check for delegates
					// (see e.g. https://github.com/dotnet/coreclr/issues/18401), so custom attribute
					// replication is not required. This saves us from adding an additional
					// 'netstandard1.5' (or later) TFM to Moq's NuGet package.
					//
					// However, if delegate mocking suddenly starts malfunctioning, it could be that CLR
					// matches delegate signatures more strictly. In that case, ensure Moq has a
					// 'netstandard1.5' (or later) TFM, and define the following compilation symbol:

#if FEATURE_REFLECTION_EMIT_CMODS
					Type[][] delegateParameterTypeModreqs = delegateParameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
					Type[][] delegateParameterTypeModopts = delegateParameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

					Type[] delegateReturnTypeModreqs = delegateReturnParameter.GetRequiredCustomModifiers();
					Type[] delegateReturnTypeModopts = delegateReturnParameter.GetOptionalCustomModifiers();
#else
					Type[][] delegateParameterTypeModreqs = null;
					Type[][] delegateParameterTypeModopts = null;

					Type[] delegateReturnTypeModreqs = null;
					Type[] delegateReturnTypeModopts = null;
#endif

					// Create a method on the interface with the same signature as the delegate.
					var newMethBuilder = newTypeBuilder.DefineMethod(
						"Invoke",
						MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract,
						CallingConventions.HasThis,
						delegateReturnType, delegateReturnTypeModreqs, delegateReturnTypeModopts,
						delegateParameterTypes, delegateParameterTypeModreqs, delegateParameterTypeModopts);

					foreach (var param in invokeMethodOnDelegate.GetParameters())
					{
						newMethBuilder.DefineParameter(param.Position + 1, param.Attributes, param.Name);
					}

					delegateInterfaceType = newTypeBuilder.CreateTypeInfo().AsType();
					delegateInterfaceCache[delegateType] = delegateInterfaceType;
				}
			}

			delegateInterfaceMethod = delegateInterfaceType.GetMethod("Invoke");
			return delegateInterfaceType;
 		}

		private sealed class Interceptor : Castle.DynamicProxy.IInterceptor
		{
			private Moq.IInterceptor underlying;

			internal Interceptor(Moq.IInterceptor underlying)
			{
				this.underlying = underlying;
			}

			public void Intercept(Castle.DynamicProxy.IInvocation invocation)
			{
				this.underlying.Intercept(new Invocation(underlying: invocation));
			}
		}

		private sealed class Invocation : Moq.Invocation
		{
			private Castle.DynamicProxy.IInvocation underlying;

			internal Invocation(Castle.DynamicProxy.IInvocation underlying) : base(underlying.Method, underlying.Arguments)
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
