#if FEATURE_SERIALIZATION

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Moq
{
	/// <summary>
	/// A <see cref="DefaultValueProvider"/> that returns an empty default value 
	/// for serializable types that do not implement <see cref="ISerializable"/> properly, 
	/// and returns the value provided by the decorated provider otherwise.
	/// </summary>
	internal class SerializableTypesValueProvider : DefaultValueProvider
	{
		private DefaultValueProvider decorated;

		public SerializableTypesValueProvider(DefaultValueProvider decorated)
		{
			this.decorated = decorated;
		}

		internal override DefaultValue Kind => this.decorated.Kind;

		protected internal override object GetDefaultParameterValueImpl(ParameterInfo parameter, Mock mock)
		{
			return IsSerializableWithIncorrectImplementationForISerializable(parameter.ParameterType)
				? EmptyDefaultValueProvider.Instance.GetDefaultParameterValueImpl(parameter, mock)
				: decorated.GetDefaultParameterValueImpl(parameter, mock);
		}

		protected internal override object GetDefaultReturnValueImpl(MethodInfo method, Mock mock)
		{
			return IsSerializableWithIncorrectImplementationForISerializable(method.ReturnType)
				? EmptyDefaultValueProvider.Instance.GetDefaultReturnValueImpl(method, mock)
				: decorated.GetDefaultReturnValueImpl(method, mock);
		}

		protected internal override object GetDefaultValueImpl(Type type, Mock mock)
		{
			return IsSerializableWithIncorrectImplementationForISerializable(type)
				? EmptyDefaultValueProvider.Instance.GetDefaultValueImpl(type, mock)
				: decorated.GetDefaultValueImpl(type, mock);
		}

		private static bool IsSerializableWithIncorrectImplementationForISerializable(Type typeToMock)
		{
			return typeToMock.IsSerializable
				&& typeof(ISerializable).IsAssignableFrom(typeToMock)
				&& !(ContainsDeserializationConstructor(typeToMock) && IsGetObjectDataVirtual(typeToMock));
		}

		private static bool ContainsDeserializationConstructor(Type typeToMock)
		{
			return typeToMock.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null,
				new[] { typeof(SerializationInfo), typeof(StreamingContext) },
				null) != null;
		}

		private static bool IsGetObjectDataVirtual(Type typeToMock)
		{
			var getObjectDataMethod = typeToMock.GetInterfaceMap(typeof(ISerializable)).TargetMethods[0];
			return !getObjectDataMethod.IsPrivate && getObjectDataMethod.IsVirtual && !getObjectDataMethod.IsFinal;
		}
	}
}

#endif
