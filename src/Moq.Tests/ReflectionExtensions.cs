// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Reflection;

namespace Moq.Tests
{
	public static class ReflectionExtensions
	{
		public static IEnumerable<MethodInfo> GetAccessors(this EventInfo @event, bool nonPublic = false)
		{
			yield return @event.GetAddMethod(nonPublic);
			yield return @event.GetRemoveMethod(nonPublic);
		}
	}
}
