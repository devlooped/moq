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
