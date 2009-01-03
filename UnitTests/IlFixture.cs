using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Moq.Tests
{
	public class IlFixture
	{
		[Fact]
		public void ReadsIlfromBody()
		{
			string s = new string(' ', 0);
			Assert.Equal(0, s.Length);

			Action<WithEvent> a = w => w.Event += null;

			var value = new object();

			ExpectSet<WithEvent>(w => w.Value = value);

			// CCI
			//var method = a.Method;
			//var body = a.Method.GetMethodBody();
			//var an = AssemblyNode.GetAssembly(method.DeclaringType.Assembly.Location);
			//var type = method.DeclaringType;
			//var tn = GetTypeNode(an, type);

			//var parameters = new List<TypeNode>();
			//foreach (var paramType in method.GetParameters())
			//{
			//    var pan = AssemblyNode.GetAssembly(paramType.ParameterType.Assembly.Location);
			//    var ptn = GetTypeNode(pan, paramType.ParameterType);
			//    parameters.Add(ptn);
			//}

			//var mn = tn.GetMethod(Identifier.For(method.Name), parameters.ToArray());
			//foreach (var st in mn.Body.Statements)
			//{
			//    Console.WriteLine(st);
			//}

			var asm = AssemblyFactory.GetAssembly(a.Method.DeclaringType.Assembly.Location);
			var method = (from m in asm.Modules.Cast<ModuleDefinition>()
						  from t in m.Types.Cast<TypeDefinition>()
						  where t.Name == a.Method.DeclaringType.Name
						  let type = t
						  from md in type.Methods.Cast<MethodDefinition>()
						  where md.Name == a.Method.Name
						  // TODO: add Matches(MethodBase) extension method.
						  select md)
					   .First();

			foreach (var instruction in method.Body.Instructions.Cast<Instruction>())
			{
				if (instruction.OpCode == OpCodes.Callvirt)
				{
					var mref = (MethodReference)instruction.Operand;
					if (mref.Name.StartsWith("add_"))
					{
						var name = mref.DeclaringType.FullName;
						var ev = typeof(WithEvent).GetEvent(mref.Name.Replace("add_", ""));
						Assert.NotNull(ev);
					}
					else if (mref.Name.StartsWith("remove_"))
					{
					}
				}
			}

			//var assembly = Mono.Cecil.AssemblyFactory.DefineAssembly("foo", Mono.Cecil.AssemblyKind.Dll);
			//var module = new Mono.Cecil.ModuleDefinition("bar", assembly);
			//var t = module.Import(
			//    a.Method.DeclaringType);

			//assembly.Modules[0].Types[0].Methods[0].Body.Instructions[0].Accept

			//a.Method.GetMethodBody()
			//Microsoft.FxCop.Sdk.Method
		}

		private void ExpectSet<T>(Action<T> setter)
		{
			var asm = AssemblyFactory.GetAssembly(setter.Method.DeclaringType.Assembly.Location);
			var method = (from m in asm.Modules.Cast<ModuleDefinition>()
						  from t in m.Types.Cast<TypeDefinition>()
						  where t.Name == setter.Method.DeclaringType.Name
						  let type = t
						  from md in type.Methods.Cast<MethodDefinition>()
						  where md.Name == setter.Method.Name
						  // TODO: add Matches(MethodBase) extension method.
						  select md)
					   .First();

			foreach (var instruction in method.Body.Instructions.Cast<Instruction>())
			{
				if (instruction.OpCode == OpCodes.Callvirt)
				{
					var mref = (MethodReference)instruction.Operand;
					Console.WriteLine(mref);
				}
			}
		}

		//private static TypeNode GetTypeNode(AssemblyNode assembly, Type type)
		//{
		//    if (!type.IsNested)
		//        return assembly.GetType(Identifier.For(type.Namespace), Identifier.For(type.Name));

		//    string sname;
		//    if (type.IsNested)
		//        sname = type.FullName.Substring(
		//            type.FullName.LastIndexOf('.'),
		//            type.FullName.Length - type.FullName.IndexOf('+')) +
		//            "+" + type.Name;
		//    return sname;
		//}

		public class WithEvent
		{
			public event EventHandler Event;
			public virtual event EventHandler VirtualEvent;
			public object Value { get; set; }
		}
	}
}
