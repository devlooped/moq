// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq
{
	/// <summary>
	///   Obsolete. Use <see cref="MockRepository"/> instead.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This class has been renamed to MockRepository. MockFactory will be retired in v5.", false)]
	public class MockFactory : MockRepository
	{
		/// <summary>
		///   Obsolete.
		/// </summary>
		public MockFactory(MockBehavior defaultBehavior)
			: base(defaultBehavior)
		{
		}
	}
}
