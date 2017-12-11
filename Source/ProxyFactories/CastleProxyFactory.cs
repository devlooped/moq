//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

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
					var delegateParameterTypes = invokeMethodOnDelegate.GetParameters().Select(p => p.ParameterType).ToArray();

					// Create a method on the interface with the same signature as the delegate.
					var newMethBuilder = newTypeBuilder.DefineMethod("Invoke",
					                                                 MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract,
					                                                 CallingConventions.HasThis,
					                                                 invokeMethodOnDelegate.ReturnType, delegateParameterTypes);

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

			public void Intercept(IInvocation invocation)
			{
				this.underlying.Intercept(new Invocation(underlying: invocation));
			}
		}

		private sealed class Invocation : Moq.Invocation
		{
			private IInvocation underlying;

			internal Invocation(IInvocation underlying) : base(underlying.Method, underlying.Arguments)
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
