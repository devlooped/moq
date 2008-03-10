using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	public interface IThrowsOnceVerifies : IThrows, IOnceVerifies, IHideObjectMembers
	{
	}
}
