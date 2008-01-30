using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq
{
	/// <summary>
	/// Used in conjuction with a <see cref="MockFactory"/>, 
	/// determines how the factory will verify mocks when 
	/// it's disposed.
	/// </summary>
	/// <seealso cref="MockFactory"/>
	public enum MockVerification
	{
		/// <summary>
		/// No verification should be performed on mocks.
		/// </summary>
		None,
		/// <summary>
		/// Should verify all mocks expectations by 
		/// invoking <see cref="Mock{T}.VerifyAll"/> 
		/// on all mocks.
		/// </summary>
		All,
		/// <summary>
		/// Should verify only verifiable expectations on 
		/// mocks by invoking <see cref="Mock{T}.Verify"/> 
		/// on all mocks.
		/// </summary>
		Verifiable
	}
}
