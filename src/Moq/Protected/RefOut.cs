using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Protected
{
	/// <summary>
	/// For use with mock.Protected() for methods with ref or out parameters
	/// e.g mock.Protected.Setup("RefMethod",RefOut.Arg(arg));
	/// </summary>
	public class RefOut
	{
		private readonly object arg;
		private readonly Type refType;
		internal object Argument => arg;
		internal Type RefType => refType; 

		private RefOut(object arg)
		{
			this.arg = arg;
			refType = arg.GetType().MakeByRefType();
		}

		/// <summary>
		/// Construct a RefOut for a ref or out parameter argument
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static RefOut Arg(object arg)
		{
			return new RefOut(arg);
		}

	}
}
