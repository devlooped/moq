using System;
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
		private sealed class Factory : IDefaultValueProviderFactory
		{
			public Factory(IDefaultValueProvider decorated)
			{
				this.Decorated = decorated;
			}

			public IDefaultValueProvider Decorated { get; }

			public IDefaultValueProvider CreateProviderFor(Mock owner)
			{
				return new SerializableTypesValueProvider(this);
			}
		}

		private Factory factory;
		private readonly IDefaultValueProvider emptyDefaultValueProvider = new EmptyDefaultValueProvider();

		public SerializableTypesValueProvider(IDefaultValueProvider decorated)
		{
			this.factory = new Factory(decorated);
		}

		private SerializableTypesValueProvider(Factory factory)
		{
			this.factory = factory;
		}

		IDefaultValueProviderFactory IDefaultValueProvider.Factory => this.factory;

		public void DefineDefault<T>(T value)
		{
			this.factory.Decorated.DefineDefault(value);
		}

		public object ProvideDefault(MethodInfo member)
		{
			return !member.ReturnType.GetTypeInfo().IsSerializable || member.ReturnType.IsSerializableMockable()
				? this.factory.Decorated.ProvideDefault(member)
				: emptyDefaultValueProvider.ProvideDefault(member);
		}
	}
}
