// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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

			protected internal override object CallBase()
			{
				Debug.Assert(this.underlying != null);

#if FEATURE_DEFAULT_INTERFACE_IMPLEMENTATIONS
				var method = this.Method;
				if (method.DeclaringType.IsInterface && !method.IsAbstract)
				{
					// As of version 4.4.0, DynamicProxy cannot proceed to default method implementations of interfaces.
					// In order to support this anyway, we use a dynamic thunk that calls it via a non-virtual `call`.
					// This is roughly equivalent to how you'd choose the interface default implementation in C#:
					// you'd cast the object to the desired interface, and call the method on that.
					var thunk = DefaultImplementationThunk.Get(method);
					return thunk.Invoke(this.underlying.Proxy, this.Arguments);
				}
#endif

				this.underlying.Proceed();
				return this.underlying.ReturnValue;
			}

			public void DetachFromUnderlying()
			{
				this.underlying = null;
			}
		}

#if FEATURE_DEFAULT_INTERFACE_IMPLEMENTATIONS
		// NOTE: In theory, this helper should work on any platform. It is excluded on `netstandard2.0`
		// and lower mostly to avoid an additional NuGet dependency required for `DynamicMethod`.
		private static class DefaultImplementationThunk
		{
			// We store previously generated dynamic thunks for reuse.
			private static ConcurrentDictionary<MethodInfo, Func<object, object[], object>> thunks;

			static DefaultImplementationThunk()
			{
				thunks = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();
			}

			public static Func<object, object[], object> Get(MethodInfo method)
			{
				return thunks.GetOrAdd(method, static method =>
				{
					var originalParameterTypes = method.GetParameterTypes();
					var n = originalParameterTypes.Count;

					var dynamicMethod = new DynamicMethod(string.Empty, returnType: typeof(object), parameterTypes: new[] { typeof(object), typeof(object[]) });
					dynamicMethod.InitLocals = true;
					var il = dynamicMethod.GetILGenerator();

					var arguments = new LocalBuilder[n];
					var returnValue = il.DeclareLocal(typeof(object));

					// Erase by-ref-ness of parameter types to get at the actual type of value.
					// We need this because we are handed `invocation.Arguments` as an `object[]` array.
					var parameterTypes = originalParameterTypes.ToArray();
					for (var i = 0; i < n; ++i)
					{
						if (parameterTypes[i].IsByRef)
						{
							parameterTypes[i] = parameterTypes[i].GetElementType();
						}
					}

					// Transfer `invocation.Arguments` into appropriately typed local variables.
					// This involves unboxing value-typed arguments, and possibly down-casting others from `object`.
					// The `unbox.any` instruction will do the right thing in both cases.
					for (var i = 0; i < n; ++i)
					{
						arguments[i] = il.DeclareLocal(parameterTypes[i]);

						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldc_I4, i);
						il.Emit(OpCodes.Ldelem_Ref);
						il.Emit(OpCodes.Unbox_Any, parameterTypes[i]);
						il.Emit(OpCodes.Stloc, arguments[i]);
					}

					// Now we're going to call the actual default implementation.

					// We do this inside a `try` block because we need to write back possibly modified
					// arguments to `invocation.Arguments` even if the called method throws.
					var returnLabel = il.DefineLabel();
					il.BeginExceptionBlock();

					// Perform the actual call.
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Castclass, method.DeclaringType);
					for (var i = 0; i < n; ++i)
					{
						il.Emit(originalParameterTypes[i].IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc, arguments[i]);
					}
					il.Emit(OpCodes.Call, method);

					// Put the return value in a local variable for later retrieval.
					if (method.ReturnType != typeof(void))
					{
						il.Emit(OpCodes.Box, method.ReturnType);
						il.Emit(OpCodes.Castclass, typeof(object));
						il.Emit(OpCodes.Stloc, returnValue);
					}
					il.Emit(OpCodes.Leave, returnLabel);

					il.BeginFinallyBlock();

					// Write back possibly modified arguments to `invocation.Arguments`.
					for (var i = 0; i < n; ++i)
					{
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldc_I4, i);
						il.Emit(OpCodes.Ldloc, arguments[i]);
						il.Emit(OpCodes.Box, arguments[i].LocalType);
						il.Emit(OpCodes.Stelem_Ref);
					}
					il.Emit(OpCodes.Endfinally);

					il.EndExceptionBlock();
					il.MarkLabel(returnLabel);

					il.Emit(OpCodes.Ldloc, returnValue);
					il.Emit(OpCodes.Ret);

					return (Func<object, object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object, object[], object>));
				});
			}
		}
#endif

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
