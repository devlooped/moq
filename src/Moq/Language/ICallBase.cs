// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>CallBase</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICallBase : IFluentInterface
	{
		/// <summary>
		/// Calls the real method of the object.
		/// </summary>
		ICallBaseResult CallBase();
	}
}
