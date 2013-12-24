using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Moq.Proxy.Factory
{
	internal class ConstructorEmitter
	{
		private TypeBuilder typeBuilder;
		private FieldBuilder interceptorField;

		internal ConstructorEmitter(TypeBuilder typeBuilder, FieldBuilder interceptorField)
		{
			this.typeBuilder = typeBuilder;
			this.interceptorField = interceptorField;
		}

		public ConstructorBuilder Build(ConstructorInfo constructor)
		{
			var parameters = new[] { typeof(IInterceptor) }
				.Concat(constructor.GetParameters()
				.Select(p => p.ParameterType))
				.ToArray();

			var ctorBuilder = this.typeBuilder.DefineConstructor(
				MethodAttributes.Public | MethodAttributes.HideBySig,
				CallingConventions.Standard,
				parameters);

			var il = ctorBuilder.GetILGenerator();

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Stfld, this.interceptorField);

			il.Emit(OpCodes.Ldarg_0);
			for (var index = 0; index < constructor.GetParameters().Length; index++)
			{
				il.Emit(OpCodes.Ldarg, index + 2);
			}
			Tuple.Create(1);
			il.Emit(OpCodes.Call, constructor);

			il.Emit(OpCodes.Ret);
			return ctorBuilder;
		}
	}
}