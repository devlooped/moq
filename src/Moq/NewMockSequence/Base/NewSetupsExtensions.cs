// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	internal static class NewSetupsExtensions
	{
		public class NewSetupsResult
		{
			public bool NoChange { get; set; }
			public List<SetupWithDepth> NewSetups { get; set; }
			public SetupWithDepth TerminalSetup { get; set; }
		}

		public static NewSetupsResult NewSetups(this List<SetupWithDepth> before, List<SetupWithDepth> after)
		{
			if (after.Count == before.Count)
			{
				return new NewSetupsResult { NoChange = true };
			}

			var difference = after.Except(before, EqualityComparer<SetupWithDepth>.Default);
			var orderedByDepth = difference.OrderBy(sd => sd.Depth).ToList();
			var terminalSetup = orderedByDepth.Last();
			return new NewSetupsResult
			{
				NewSetups = orderedByDepth,
				TerminalSetup = terminalSetup
			};
		}
	}

}
