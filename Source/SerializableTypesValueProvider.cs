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
            return !member.ReflectedType.IsSerializable || member.ReflectedType.IsSerializableMockable()
                       ? decorated.ProvideDefault(member)
                       : emptyDefaultValueProvider.ProvideDefault(member);
        }
    }
}