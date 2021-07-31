// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	internal class SetupWithDepth : IEquatable<SetupWithDepth>
	{
		public int Depth { get; set; }
		public Setup Setup { get; set; }
		public SetupCollection ContainingMutableSetups { get; set; }

		public bool Equals(SetupWithDepth other)
		{
			return Setup == other.Setup;
		}

		public override int GetHashCode()
		{
			return Setup.GetHashCode();
		}

	}

	internal static class SetupFinder
	{
		private static void GetAllSetups(SetupCollection setups, List<SetupWithDepth> setupsWithDepth, int depth)
		{
			setupsWithDepth.AddRange(setups.ToArray().Select(s => new SetupWithDepth { Depth = depth, Setup = s, ContainingMutableSetups = setups }));
			foreach (var setup in setups)
			{
				if (setup.InnerMock != null)
				{
					GetAllSetups(setup.InnerMock.MutableSetups, setupsWithDepth, depth + 1);
				}
			}
		}
		private static void GetAllSetups(SetupCollection setups, List<SetupWithDepth> setupsWithDepth)
		{
			GetAllSetups(setups, setupsWithDepth, 0);
		}
		public static List<SetupWithDepth> GetAllSetups(Mock mock)
		{
			var setupsWithDepth = new List<SetupWithDepth>();
			GetAllSetups(mock.MutableSetups, setupsWithDepth);
			return setupsWithDepth;
		}
	}

}
