// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	internal readonly struct Pair<T1, T2> : IEquatable<Pair<T1, T2>>
	{
		public readonly T1 Item1;
		public readonly T2 Item2;

		public Pair(T1 item1, T2 item2)
		{
			this.Item1 = item1;
			this.Item2 = item2;
		}

		public void Deconstruct(out T1 item1, out T2 item2)
		{
			item1 = this.Item1;
			item2 = this.Item2;
		}

		public bool Equals(Pair<T1, T2> other)
		{
			return object.Equals(this.Item1, other.Item1)
				&& object.Equals(this.Item2, other.Item2);
		}

		public override bool Equals(object obj)
		{
			return obj is Pair<T1, T2> other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return unchecked(1001 * this.Item1?.GetHashCode() ?? 101 + this.Item2?.GetHashCode() ?? 11);
		}
	}
}
