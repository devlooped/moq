// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Moq
{
	internal sealed class AwaitOperatorObserver : IDisposable
	{
		[ThreadStatic]
		private static Stack<AwaitOperatorObserver> activations;

		public static AwaitOperatorObserver Activate()
		{
			var activation = new AwaitOperatorObserver();

			var activations = AwaitOperatorObserver.activations;
			if (activations == null)
			{
				AwaitOperatorObserver.activations = activations = new Stack<AwaitOperatorObserver>();
			}
			activations.Push(activation);

			return activation;
		}

		public static bool IsActive(out AwaitOperatorObserver observer)
		{
			var activations = AwaitOperatorObserver.activations;

			if (activations != null && activations.Count > 0)
			{
				observer = activations.Peek();
				return true;
			}
			else
			{
				observer = null;
				return false;
			}
		}

		private int timestamp;
		private List<int> observations;

		private AwaitOperatorObserver()
		{
		}

		public void Dispose()
		{
			var activations = AwaitOperatorObserver.activations;
			Debug.Assert(activations != null && activations.Count > 0);
			activations.Pop();
		}

		/// <summary>
		///   Returns the current timestamp. The next call will return a timestamp greater than this one,
		///   allowing you to order invocations and matcher observations.
		/// </summary>
		public int GetNextTimestamp()
		{
			return ++this.timestamp;
		}

		/// <summary>
		///   Adds the specified <see cref="Match"/> as an observation.
		/// </summary>
		public void OnAwaitOperator()
		{
			if (this.observations == null)
			{
				this.observations = new List<int>();
			}

			this.observations.Add(this.GetNextTimestamp());
		}

		public bool HasAwaitOperatorBetween(int fromTimestampInclusive, int toTimestampExclusive)
		{
			if (this.observations != null)
			{
				return this.observations.Any(o => fromTimestampInclusive <= o && o < toTimestampExclusive);
			}
			else
			{
				return false;
			}
		}
	}
}
