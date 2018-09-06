// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
