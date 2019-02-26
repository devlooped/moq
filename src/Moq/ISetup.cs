// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Reflection;

namespace Moq
{
	internal interface ISetup
	{
		MethodInfo Method { get; }
	}
}
