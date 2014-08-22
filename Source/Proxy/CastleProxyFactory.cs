//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//http://code.google.com/p/moq/
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
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Moq.Properties;
using System.Diagnostics.CodeAnalysis;

namespace Moq.Proxy
{
	internal class CastleProxyFactory : IProxyFactory
	{
		private static readonly ProxyGenerator generator = CreateProxyGenerator();

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "By Design")]
		static CastleProxyFactory()
		{
#pragma warning disable 618
			AttributesToAvoidReplicating.Add<SecurityPermissionAttribute>();
#pragma warning restore 618

#if !SILVERLIGHT
			AttributesToAvoidReplicating.Add<ReflectionPermissionAttribute>();
			AttributesToAvoidReplicating.Add<PermissionSetAttribute>();
			AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.MarshalAsAttribute>();
			AttributesToAvoidReplicating.Add<UIPermissionAttribute>();
#if !NET3x
			AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.TypeIdentifierAttribute>();
#endif
#endif
			proxyOptions = new ProxyGenerationOptions { Hook = new ProxyMethodHook(), BaseTypeForInterfaceProxy = typeof(InterfaceProxy) };
		}

		/// <inheritdoc />
		public object CreateProxy(Type mockType, ICallInterceptor interceptor, Type[] interfaces, object[] arguments)
		{
			if (mockType.IsInterface) {
				return generator.CreateInterfaceProxyWithoutTarget(mockType, interfaces, proxyOptions, new Interceptor(interceptor));
			}

			try
			{
				return generator.CreateClassProxy(mockType, interfaces, proxyOptions, arguments, new Interceptor(interceptor));
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

		private static readonly Dictionary<Type, Type> delegateInterfaceCache = new Dictionary<Type, Type>();
		private static readonly ProxyGenerationOptions proxyOptions;
		private static int delegateInterfaceSuffix;

		/// <inheritdoc />
		public Type GetDelegateProxyInterface(Type delegateType, out MethodInfo delegateInterfaceMethod)
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
					newTypeBuilder.DefineMethod("Invoke",
					                            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract,
					                            CallingConventions.HasThis,
					                            invokeMethodOnDelegate.ReturnType, delegateParameterTypes);

					delegateInterfaceType = newTypeBuilder.CreateType();
					delegateInterfaceCache[delegateType] = delegateInterfaceType;
 				}
 			}

			delegateInterfaceMethod = delegateInterfaceType.GetMethod("Invoke");
			return delegateInterfaceType;
 		}

		private static ProxyGenerator CreateProxyGenerator()
		{
			return new ProxyGenerator();
		}

		private class Interceptor : IInterceptor
		{
			private ICallInterceptor interceptor;

			internal Interceptor(ICallInterceptor interceptor)
			{
				this.interceptor = interceptor;
			}

			public void Intercept(IInvocation invocation)
			{
				this.interceptor.Intercept(new CallContext(invocation));
			}
		}

		private class CallContext : ICallContext
		{
			private IInvocation invocation;

			internal CallContext(IInvocation invocation)
			{
				this.invocation = invocation;
			}

			public object[] Arguments
			{
				get { return this.invocation.Arguments; }
			}

			public MethodInfo Method
			{
				get { return this.invocation.Method; }
			}

			public object ReturnValue
			{
				get { return this.invocation.ReturnValue; }
				set { this.invocation.ReturnValue = value; }
			}

			public void InvokeBase()
			{
				this.invocation.Proceed();
			}

			public void SetArgumentValue(int index, object value)
			{
				this.invocation.SetArgumentValue(index, value);
			}
		}
	}
}
