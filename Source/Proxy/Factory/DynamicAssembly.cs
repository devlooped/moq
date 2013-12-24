using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

namespace Moq.Proxy.Factory
{
	internal class DynamicAssembly
	{
		internal const string AssemblyName = "<Mock#_Assembly>";
		internal const string AssemblyPublicKey =
			"0024000004800000940000000602000000240000525341310004000001000100AD1A097DF919CDE9" +
			"C972BE002F9BF9DC641B6665F8044FD3938F4F45977D8A55D0A7BEAC29EA261480AE86B8C4EEFEE3" +
			"CBDAC80567C6DE6B94E38D92BC0576B6C66D9B04DEA3379A1C45899555D499F12F4BD79B45DDA823" +
			"39CDAE59B79DC22F75D3767A624033125AB635FD026B01407C195F8DF34FB29C99859298B27FC1B5";

		private static readonly object sync = new object();
		private static DynamicAssembly current;

		private readonly Dictionary<string, Type> dynamicTypes = new Dictionary<string, Type>();

		private DynamicAssembly()
		{
			var assemblyName = new AssemblyName(AssemblyName) { KeyPair = GetStrongNameKeyPair() };
			this.ModuleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect)
				.DefineDynamicModule(AssemblyName);
		}

		public static DynamicAssembly Current
		{
			get
			{
				if (current == null)
				{
					lock (sync)
					{
						if (current == null)
						{
							current = new DynamicAssembly();
						}
					}
				}

				return current;
			}
		}

		public ModuleBuilder ModuleBuilder { get; private set; }

		public TypeBuilder DefineType(string name, TypeAttributes attributes, Type parent, IEnumerable<Type> interfaces)
		{
			return this.ModuleBuilder.DefineType(name, attributes, parent, interfaces.ToArray());
		}

		public Type GetType(IEnumerable<Type> types, Func<Type> typeBuilder)
		{
			var typeKey = string.Join("\t", types.Select(t => t.FullName).OrderBy(n => n));

			Type dynamicType;
			if (!this.dynamicTypes.TryGetValue(typeKey, out dynamicType))
			{
				lock (this.dynamicTypes)
				{
					if (!this.dynamicTypes.TryGetValue(typeKey, out dynamicType))
					{
						dynamicType = typeBuilder();
						this.dynamicTypes.Add(typeKey, dynamicType);
					}
				}
			}

			return dynamicType;
		}

		private static StrongNameKeyPair GetStrongNameKeyPair()
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Moq.Proxy.Factory.KeyPair.snk"))
			{
				var buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				return new StrongNameKeyPair(buffer);
			}
		}
	}
}