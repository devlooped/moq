// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Tests
{
	public static class StringExtensions
	{
		public static bool ContainsConsecutiveLines(this string str, params string[] lines)
		{
			var haystack = str.Replace("\r\n", "\n");
			var needle = string.Join("\n", lines);
			return haystack.Contains(needle);
		}
	}
}
