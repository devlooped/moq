using System.Reflection;
#if !NETCORE
using System.Runtime.Serialization;
#endif

namespace Moq
{
#if !NETCORE
	/// <summary>
	/// A <see cref="IDefaultValueProvider"/> that returns an empty default value 
	/// for serializable types that do not implement <see cref="ISerializable"/> properly, 
	/// and returns the value provided by the decorated provider otherwise.
	/// </summary>
#else
	/// <summary>
	/// A <see cref="IDefaultValueProvider"/> that returns an empty default value 
	/// for serializable types that do not implement ISerializable properly, 
	/// and returns the value provided by the decorated provider otherwise.
	/// </summary>
#endif
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
			return !member.ReturnType.GetTypeInfo().IsSerializable || member.ReturnType.IsSerializableMockable()
				       ? decorated.ProvideDefault(member)
				       : emptyDefaultValueProvider.ProvideDefault(member);
		}
	}
}