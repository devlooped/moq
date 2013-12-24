using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Moq.Properties;

namespace Moq.Proxy.Factory
{
	internal class ProxyFactory : IProxyFactory
	{
		private const string TypeNameFormat = "<Mock#_{0}>";
		private static readonly DynamicAssembly assembly = DynamicAssembly.Current;

		private static readonly Dictionary<Type, Type> delegateInterfaceCache = new Dictionary<Type, Type>();
		private static int delegateInterfaceSuffix;

		public object CreateProxy(Type mockType, ICallInterceptor interceptor, Type[] interfaces, params object[] arguments)
		{
			Guard.NotNull(() => interceptor, interceptor);
			Guard.NotNull(() => mockType, mockType);
			Guard.NotNull(() => interfaces, interfaces);

			if (interfaces.Any(i => !i.IsInterface))
			{
				throw new ArgumentException(); // TODO set a proper message
			}

			if (mockType.IsInterface)
			{
				interfaces = ExpandInterfaces(new[] { mockType }.Concat(interfaces)).ToArray();
				mockType = typeof(object);
			}
			else
			{
				interfaces = ExpandInterfaces(interfaces).ToArray();
			}

			try
			{
				var proxyType = assembly.GetType(new[] { mockType }.Concat(interfaces), () => CreateType(mockType, interfaces));
				return Activator.CreateInstance(proxyType, new[] { new Interceptor(interceptor) }.Concat(arguments ?? new object[0]).ToArray());
			}
			catch (MissingMethodException e)
			{
				throw new ArgumentException(Resources.ConstructorNotFound, e);
			}
		}

		public Type GetDelegateProxyInterface(Type delegateType, out MethodInfo delegateInterfaceMethod)
		{
			Type delegateInterfaceType;

			lock (this)
			{
				if (!delegateInterfaceCache.TryGetValue(delegateType, out delegateInterfaceType))
				{
					var interfaceName = string.Format(CultureInfo.InvariantCulture, "DelegateInterface_{0}_{1}",
													  delegateType.Name, delegateInterfaceSuffix++);

					var moduleBuilder = DynamicAssembly.Current.ModuleBuilder;
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

		private static void BuildGenericArguments(Type[] genericArguments, TypeBuilder typeBuilder)
		{
			var typeParameters = typeBuilder.DefineGenericParameters(genericArguments.Select(t => t.Name).ToArray())
				.Select((b, i) => new { ParameterBuilder = b, GenericType = genericArguments[i] });

			foreach (var typeParameter in typeParameters)
			{
				typeParameter.ParameterBuilder.SetGenericParameterAttributes(typeParameter.GenericType.GenericParameterAttributes);
				foreach (var constraint in typeParameter.GenericType.GetGenericParameterConstraints())
				{
					typeParameter.ParameterBuilder.SetBaseTypeConstraint(constraint);
				}
			}
		}

		private static Type CreateType(Type type, IEnumerable<Type> interfaces)
		{
			var typeName = string.Format(CultureInfo.InvariantCulture, TypeNameFormat, Guid.NewGuid().ToString("n"));
			var typeBuilder = assembly.DefineType(typeName, TypeAttributes.Public, type, interfaces);
			if (type.IsGenericTypeDefinition)
			{
				BuildGenericArguments(type.GetGenericArguments(), typeBuilder);
			}

			var interceptorField = typeBuilder.DefineField(
				"interceptor",
				typeof(IInterceptor),
				FieldAttributes.Private | FieldAttributes.InitOnly);

			var baseConstructors = type.GetConstructors(MemberBindingFlags.InstanceMembers)
				.Where(c => c.IsPublic || c.IsFamily || c.IsFamilyOrAssembly);

			foreach (var constructor in baseConstructors)
			{
				new ConstructorEmitter(typeBuilder, interceptorField).Build(constructor);
			}

			EmitImplementation(type, typeBuilder, interceptorField);

			foreach (var interfaceType in interfaces)
			{
				EmitImplementation(interfaceType, typeBuilder, interceptorField);
			}

			return typeBuilder.CreateType();
		}

		private static void EmitImplementation(Type type, TypeBuilder typeBuilder, FieldBuilder interceptorField)
		{
			var genericMappings = GetGenericMappings(type);
			var methodEmiter = new MethodEmitter(typeBuilder, interceptorField, genericMappings);

			foreach (var method in type.GetMethods(MemberBindingFlags.InstanceMembers).Where(m => m.CanOverride()))
			{
				methodEmiter.Build(method);
			}
		}

		private static IEnumerable<Type> ExpandInterfaces(IEnumerable<Type> interfaces)
		{
			return interfaces.Union(interfaces.SelectMany(i => i.GetInterfaces()));
		}

		private static IDictionary<Type, Type> GetGenericMappings(Type type)
		{
			if (type.IsGenericType)
			{
				var argumentTypes = type.GetGenericArguments();
				return type.GetGenericTypeDefinition().GetGenericArguments()
					.Select((a, i) => new { Argument = a, Type = argumentTypes[i] })
					.ToDictionary(m => m.Argument, m => m.Type);
			}

			return new Dictionary<Type, Type>(0);
		}

		private class Interceptor : IInterceptor
		{
			private ICallInterceptor interceptor;

			internal Interceptor(ICallInterceptor interceptor)
			{
				this.interceptor = interceptor;
			}

			public IMethodReturn Invoke(IMethodCall input)
			{
				var context = new CallContext(input);
				this.interceptor.Intercept(context);
				return new MethodReturn(context.ReturnValue, context.OutArgs);
			}
		}

		private class CallContext : ICallContext
		{
			private IMethodCall invocation;
			public Dictionary<int, object> outArgs = new Dictionary<int, object>();

			internal CallContext(IMethodCall invocation)
			{
				this.invocation = invocation;
			}

			public object[] Arguments
			{
				get { return this.invocation.InArgs; }
			}

			public MethodInfo Method
			{
				get { return this.invocation.Method; }
			}

			public object ReturnValue { get; set; }

			public object[] OutArgs
			{
				get
				{
					var outArgs = new List<object>();
					foreach (var parameter in invocation.Method.GetParameters().Where(p => p.ParameterType.IsByRef))
					{
						object value;
						if (this.outArgs.TryGetValue(parameter.Position, out value))
						{
							outArgs.Add(value);
						}
						else
						{
							outArgs.Add(parameter.IsRefArgument() ? this.Arguments[parameter.Position] : null); // TODO should be default
						}
					}

					return outArgs.ToArray();
				}
			}

			public void InvokeBase()
			{
				this.ReturnValue = MethodEmitter.InvokeBaseMarker.Value;
			}

			public void SetArgumentValue(int index, object value)
			{
				this.outArgs.Add(index, value);
			}
		}
	}
}