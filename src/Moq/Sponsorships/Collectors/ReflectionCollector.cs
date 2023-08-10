using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Moq.Sponsorships.Interfaces;

namespace Moq.Sponsorships.Collectors
{
    /// <summary>
    /// We can use information about namespaces and classes to find people
    /// who are using our free and open source software without paying for it!
    /// </summary>
    internal class ReflectionCollector : IDataCollector
    {
        public class TypeInfoDataEntry : DataCollection.DataEntry
        {
            public TypeInfoDataEntry(string name, Type type) : base(name)
            {
                Value = type;
            }

            public Type Value { get; private set; }

            public string GetFields()
                => String.Join(";", Value.GetFields()
                    .Select(field => $"field {field.FieldType.FullName} {field.Name}"));

            public string GetProperties()
                => String.Join(";", Value.GetProperties()
                    .Select(property => $"property {property.PropertyType.FullName} {property.Name} {{ {(property.CanRead ? "get;" : "")}{(property.CanWrite ? "get;" : "")} }}"));

            public string GetMethods()
                => String.Join(";", Value.GetMethods()
                    .Select(method => $"method {method.ReturnType} {method.Name} ({method.GetParameters().Select(info => $"{info.ParameterType.FullName} {info.Name}")}) " +
                                      //    Just in case we need it to identify WHICH company is freeloading,
                                      //    we should dump the IL too.
                                      $"{ Convert.ToBase64String( method.GetMethodBody()?.GetILAsByteArray() ?? new byte[0] )}"));

            public override string ToString() =>
                Encode(Name,
                    Sanitize( String.Concat(";", new [] { GetFields(), GetProperties(), GetMethods() })));
        }

        public void WriteType(DataCollection collection, Type type)
            => collection.Add(new TypeInfoDataEntry(type.FullName, type));

        public void WriteAssembly(DataCollection collection, Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
                WriteType(collection, type);
        }

        /// <summary>
        /// Get all loaded assemblies and write their children to the collection
        /// </summary>
        public void WriteAssemblies(DataCollection collection)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                WriteAssembly(collection, assembly);
        }

        public Task CollectData(DataCollection collection)
        {
            WriteAssemblies(collection);

            return Task.CompletedTask;
        }
    }
}
