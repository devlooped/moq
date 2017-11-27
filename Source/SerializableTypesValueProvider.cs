#if FEATURE_SERIALIZATION

using System;
using System.Diagnostics;
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

		protected internal override object GetDefaultParameterValue(ParameterInfo parameter, Mock mock)
		{
			Debug.Assert(parameter != null);
			Debug.Assert(parameter.ParameterType != null);
			Debug.Assert(parameter.ParameterType != typeof(void));
			Debug.Assert(mock != null);

			return IsSerializableWithIncorrectImplementationForISerializable(parameter.ParameterType)
				? DefaultValueProvider.Empty.GetDefaultParameterValue(parameter, mock)
				: decorated.GetDefaultParameterValue(parameter, mock);
		}

		protected internal override object GetDefaultReturnValue(MethodInfo method, Mock mock)
		{
			Debug.Assert(method != null);
			Debug.Assert(method.ReturnType != null);
			Debug.Assert(method.ReturnType != typeof(void));
			Debug.Assert(mock != null);

			return IsSerializableWithIncorrectImplementationForISerializable(method.ReturnType)
				? DefaultValueProvider.Empty.GetDefaultReturnValue(method, mock)
				: decorated.GetDefaultReturnValue(method, mock);
		}

		protected internal override object GetDefaultValue(Type type, Mock mock)
		{
			Debug.Assert(type != null);
			Debug.Assert(type != typeof(void));
			Debug.Assert(mock != null);

			return IsSerializableWithIncorrectImplementationForISerializable(type)
				? DefaultValueProvider.Empty.GetDefaultValue(type, mock)
				: decorated.GetDefaultValue(type, mock);
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
