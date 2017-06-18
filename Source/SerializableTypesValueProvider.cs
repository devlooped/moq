#if FEATURE_SERIALIZATION

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Moq
{
	/// <summary>
	/// A <see cref="IDefaultValueProvider"/> that returns an empty default value 
	/// for serializable types that do not implement <see cref="ISerializable"/> properly, 
	/// and returns the value provided by the decorated provider otherwise.
	/// </summary>
	internal class SerializableTypesValueProvider : IDefaultValueProvider
	{
		private readonly IDefaultValueProvider decorated;
		private readonly EmptyDefaultValueProvider emptyDefaultValueProvider = new EmptyDefaultValueProvider();

		public SerializableTypesValueProvider(IDefaultValueProvider decorated)
		{
			this.decorated = decorated;
		}

		public void DefineDefault<T>(T value)
		{
			decorated.DefineDefault(value);
		}

		public object ProvideDefault(MethodInfo member)
		{
			return IsSerializableWithIncorrectImplementationForISerializable(member.ReturnType)
				? emptyDefaultValueProvider.ProvideDefault(member)
				: decorated.ProvideDefault(member);
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
