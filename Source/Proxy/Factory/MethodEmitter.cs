using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Moq.Proxy.Factory
{
	internal class MethodEmitter
	{
		private static readonly ConstructorInfo methodCallCtor = Reflect.Constructor(() => new MethodCall(null, null, null));
		private static readonly MethodInfo makeGenericMethodMethod = Reflect.Method<MethodInfo>(m => m.MakeGenericMethod());
		private static readonly MethodInfo getTypeFromHandleMethod = Reflect.Method(() => Type.GetTypeFromHandle(default(RuntimeTypeHandle)));
		private static readonly MethodInfo getCurrentMethodMethod = Reflect.Method(() => MethodBase.GetCurrentMethod());
		private static readonly MethodInfo interceptorInvokeMethod = Reflect.Method<IInterceptor>(i => i.Invoke(null));
		private static readonly MethodInfo outputsGetMethod = Reflect.Property<IMethodReturn>(m => m.OutArgs).GetGetMethod();
		private static readonly MethodInfo returnValueGetMethod = Reflect.Property<IMethodReturn>(m => m.ReturnValue).GetGetMethod();

		private static readonly IEqualityComparer<MethodInfo> methodInfoComparer = new MethodInfoEqualityComparer();

		private readonly FieldBuilder interceptorField;
		private readonly IDictionary<Type, Type> genericMappings;
		private readonly TypeBuilder TypeBuilder;

		internal MethodEmitter(TypeBuilder typeBuilder, FieldBuilder interceptorField, IDictionary<Type, Type> genericMappings)
		{
			this.TypeBuilder = typeBuilder;
			this.genericMappings = genericMappings;
			this.interceptorField = interceptorField;
		}

		public MethodBuilder Build(MethodInfo method)
		{
			var paramaterTypes = method.GetParameterTypes().ToArray();

			var methodBuilder = this.TypeBuilder.DefineMethod(
				method.Name,
				GetAttributes(method),
				method.ReturnType,
				paramaterTypes);

			var genericArguments = method.GetGenericArguments();

			if (method.IsGenericMethodDefinition)
			{
				this.BuildGenericArguments(methodBuilder, genericArguments);
			}

			var il = methodBuilder.GetILGenerator();
			var arguments = il.DeclareLocal(typeof(object[]));

			var outputs = EmitStoreParameters(methodBuilder, arguments, method.GetParameters());
			this.EmitInterceptorCall(il, arguments, method, genericArguments);
			EmitReturn(il, outputs, method);
			return methodBuilder;
		}

		private static LocalBuilder EmitCurrentMetod(ILGenerator il, MethodInfo method)
		{
			var currentMethod = il.DeclareLocal(typeof(MethodInfo));
			il.Emit(OpCodes.Ldtoken, method.DeclaringType);
			il.Emit(OpCodes.Call, getTypeFromHandleMethod);
			il.Emit(OpCodes.Call, getCurrentMethodMethod);
			il.Emit(OpCodes.Isinst, currentMethod.LocalType);
			il.Emit(OpCodes.Call, Reflect.Method(() => MethodEmitter.GetCurrentMethod(default(Type), default(MethodInfo))));
			il.Emit(OpCodes.Stloc, currentMethod);
			return currentMethod;
		}

		public static object GetCurrentMethod(Type type, MethodInfo methodInfo)
		{
			if (methodInfo.IsGenericMethodDefinition)
			{
				return type.GetMethods(MemberBindingFlags.InstanceMembers).First(m => methodInfoComparer.Equals(m, methodInfo));
			}

			return type.GetMethod(methodInfo.Name, MemberBindingFlags.InstanceMembers, null, methodInfo.GetParameterTypes().ToArray(), null);
		}

		private static LocalBuilder EmitGenericArguments(ILGenerator il, Type[] genericArguments)
		{
			var types = il.DeclareLocal(typeof(Type[]));
			il.Emit(OpCodes.Ldc_I4_S, genericArguments.Length);
			il.Emit(OpCodes.Newarr, typeof(Type));
			il.Emit(OpCodes.Stloc, types);

			var index = 0;
			foreach (var argument in genericArguments)
			{
				il.Emit(OpCodes.Ldloc, types);
				il.Emit(OpCodes.Ldc_I4, index++);
				il.Emit(OpCodes.Ldtoken, argument);
				il.Emit(OpCodes.Call, getTypeFromHandleMethod);
				il.Emit(OpCodes.Stelem_Ref);
			}

			return types;
		}

		private static void EmitReturn(ILGenerator il, IList<ParameterInfo> outputs, MethodInfo method)
		{
			var methodReturn = il.DeclareLocal(typeof(IMethodReturn));
			il.Emit(OpCodes.Stloc, methodReturn);

			if (!method.IsAbstract)
			{
				var skipBaseCaller = il.DefineLabel();

				il.Emit(OpCodes.Ldloc, methodReturn);
				il.Emit(OpCodes.Callvirt, returnValueGetMethod);
				il.Emit(OpCodes.Ldsfld, Reflect.Field(() => InvokeBaseMarker.Value));
				il.Emit(OpCodes.Call, Reflect.Method(() => object.ReferenceEquals(null, null)));
				il.Emit(OpCodes.Brfalse_S, skipBaseCaller);

				il.Emit(OpCodes.Ldarg_0);

				for (var index = 0; index < method.GetParameters().Length; index++)
				{
					il.Emit(OpCodes.Ldarg, index + 1);
				}

				il.Emit(OpCodes.Call, method);
				if (method.ReturnType != typeof(void))
				{
					il.Emit(OpCodes.Ret);
				}

				il.MarkLabel(skipBaseCaller);
			}

			if (method.ReturnType != typeof(void) || outputs.Count != 0)
			{
				var index = 0;
				foreach (var parameter in outputs)
				{
					il.Emit(OpCodes.Ldarg, parameter.Position + 1);
					il.Emit(OpCodes.Ldloc, methodReturn);
					il.Emit(OpCodes.Callvirt, outputsGetMethod);
					il.Emit(OpCodes.Ldc_I4, index++);
					il.Emit(OpCodes.Ldelem_Ref);

					var parameterType = parameter.ParameterType.GetElementType();
					if (parameterType.IsValueType || parameterType.IsGenericParameter)
					{
						il.Emit(OpCodes.Unbox_Any, parameterType);
					}
					else
					{
						il.Emit(OpCodes.Castclass, parameterType);
					}

					il.Emit(OpCodes.Stobj, parameterType);
				}

				if (method.ReturnType != typeof(void))
				{
					il.Emit(OpCodes.Ldloc, methodReturn);
					il.Emit(OpCodes.Callvirt, returnValueGetMethod);

					if (method.ReturnType.IsValueType || method.ReturnType.IsGenericParameter)
					{
						il.Emit(OpCodes.Unbox_Any, method.ReturnType);
					}
				}
			}

			il.Emit(OpCodes.Ret);
		}

		private static IList<ParameterInfo> EmitStoreParameters(MethodBuilder methodBuilder, LocalBuilder arguments, ParameterInfo[] parameters)
		{
			var il = methodBuilder.GetILGenerator();
			il.Emit(OpCodes.Ldc_I4, parameters.Length);
			il.Emit(OpCodes.Newarr, typeof(object));
			il.Emit(OpCodes.Stloc, arguments);

			var outputs = new List<ParameterInfo>();

			var index = 0;
			foreach (var parameter in parameters)
			{
				il.Emit(OpCodes.Ldloc, arguments);
				il.Emit(OpCodes.Ldc_I4, index++);

				methodBuilder.DefineParameter(index, parameter.Attributes, parameter.Name);
				il.Emit(OpCodes.Ldarg, index);

				var parameterType = parameter.ParameterType;
				if (parameterType.IsByRef)
				{
					outputs.Add(parameter);
					parameterType = parameterType.GetElementType();
					il.Emit(OpCodes.Ldobj, parameterType);
				}

				if (parameterType.IsValueType || parameterType.IsGenericParameter)
				{
					il.Emit(OpCodes.Box, parameterType);
				}

				il.Emit(OpCodes.Stelem_Ref);
			}

			return outputs;
		}

		private static MethodAttributes GetAttributes(MethodInfo method)
		{
			var methodAttributes = MethodAttributes.Virtual;
			if (method.IsFamily || method.IsFamilyAndAssembly || method.IsFamilyOrAssembly)
			{
				methodAttributes |= MethodAttributes.Family;
			}

			if (method.IsAssembly)
			{
				methodAttributes |= MethodAttributes.Assembly;
			}

			if (method.IsPublic)
			{
				methodAttributes |= MethodAttributes.Public;
			}

			return methodAttributes;
		}

		private void BuildGenericArguments(MethodBuilder methodBuilder, Type[] genericArguments)
		{
			var typeArguments = methodBuilder.DefineGenericParameters(genericArguments.Select(t => t.Name).ToArray())
				.Select((b, i) => new { Builder = b, Type = genericArguments[i] });

			foreach (var argument in typeArguments)
			{
				argument.Builder.SetGenericParameterAttributes(argument.Type.GenericParameterAttributes);
				foreach (var constraint in argument.Type.GetGenericParameterConstraints())
				{
					var contraintType = this.GetMappedType(constraint);
					if (contraintType.IsInterface)
					{
						argument.Builder.SetInterfaceConstraints(contraintType);
					}
					else
					{
						argument.Builder.SetBaseTypeConstraint(contraintType);
					}
				}
			}
		}

		private void EmitInterceptorCall(ILGenerator il, LocalBuilder arguments, MethodInfo method, Type[] genericArguments)
		{
			var currentMethod = EmitCurrentMetod(il, method);

			if (method.IsGenericMethod)
			{
				var parameterTypes = EmitGenericArguments(il, genericArguments);
				il.Emit(OpCodes.Ldloc, currentMethod);
				il.Emit(OpCodes.Ldloc, parameterTypes);
				il.Emit(OpCodes.Callvirt, makeGenericMethodMethod);
				il.Emit(OpCodes.Stloc, currentMethod);
			}

			il.Emit(OpCodes.Ldarg_0);

			il.Emit(OpCodes.Ldfld, this.interceptorField);
			il.Emit(OpCodes.Ldloc, currentMethod);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldloc, arguments);
			il.Emit(OpCodes.Newobj, methodCallCtor);
			il.Emit(OpCodes.Callvirt, interceptorInvokeMethod);
		}

		private Type GetMappedType(Type constraint)
		{
			return this.genericMappings.ContainsKey(constraint) ? this.genericMappings[constraint] : constraint;
		}

		internal sealed class InvokeBaseMarker
		{
			public static readonly InvokeBaseMarker Value = new InvokeBaseMarker();

			private InvokeBaseMarker()
			{
			}
		}
	}
}