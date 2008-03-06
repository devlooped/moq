using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq.Language.Primitives
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOccurrence : IHideObjectMembers
	{
		/// <summary>
		/// 
		/// </summary>
		IVerifies AtMostOnce();

		///// <summary>
		///// 
		///// </summary>
		///// <param name="times"></param>
		//IVerifies Times(int times);
	}
}
