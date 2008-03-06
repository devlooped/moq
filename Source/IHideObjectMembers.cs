using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Moq
{
	/// <summary>
	/// Base interface for all mock verbs.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHideObjectMembers
	{
		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);
	}
}
