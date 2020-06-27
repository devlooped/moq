// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	/// <summary>
	/// Represents a switch, or a combination of switches, that can be either enabled or disabled.
	/// When set via <see cref="Mock.Switches"/> or <see cref="MockFactory.Switches"/>, they determine how a mock will operate.
	/// </summary>
	[Flags]
	public enum Switches
	{
		/// <summary>
		/// The default set of switches. The switches covered by this enumeration value may change between different versions of Moq.
		/// </summary>
		Default = 0,

		/// <summary>
		/// When enabled, specifies that source file information should be collected for each setup.
		/// This results in more helpful error messages, but may affect performance.
		/// </summary>
		CollectDiagnosticFileInfoForSetups = 1 << 0,
	}
}
