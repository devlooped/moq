// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	/// <summary>
	/// Tracks the current mock and interception context.
	/// </summary>
	internal class FluentMockContext : IDisposable
	{
		[ThreadStatic]
		private static FluentMockContext current;

		/// <summary>
		/// Having an active fluent mock context means that the invocation 
		/// is being performed in "trial" mode, just to gather the 
		/// target method and arguments that need to be matched later 
		/// when the actual invocation is made.
		/// </summary>
		public static bool IsActive(out FluentMockContext context)
		{
			var current = FluentMockContext.current;

			context = current;
			return current != null;
		}

		private List<Observation> observations;

		public FluentMockContext()
		{
			current = this;
		}

		public void Dispose()
		{
			if (this.observations != null)
			{
				for (var i = this.observations.Count - 1; i >= 0; --i)
				{
					this.observations[i].Dispose();
				}
			}

			current = null;
		}

		/// <summary>
		///   Adds the specified mock invocation as an observation.
		/// </summary>
		public void OnInvocation(Mock mock, Invocation invocation)
		{
			if (this.observations == null)
			{
				this.observations = new List<Observation>();
			}

			observations.Add(new InvocationObservation(mock, invocation));
		}

		/// <summary>
		///   Adds the specified <see cref="Match"/> as an observation.
		/// </summary>
		public void OnMatch(Match match)
		{
			if (this.observations == null)
			{
				this.observations = new List<Observation>();
			}

			this.observations.Add(new MatchObservation(match));
		}

		/// <summary>
		///   Checks whether the last observed thing was a mock invocation.
		///   If <see langword="true"/>, details about that mock invocation are provided via the <see langword="out"/> parameters.
		/// </summary>
		/// <param name="mock">The <see cref="Mock"/> on which an invocation was observed.</param>
		/// <param name="invocation">The observed <see cref="Invocation"/>.</param>
		/// <param name="matches">The <see cref="Match"/>es that were observed just before the invocation.</param>
		public bool LastObservationWasMockInvocation(out Mock mock, out Invocation invocation, out Matches matches)
		{
			if (this.observations != null)
			{
				var lastIndex = this.observations.Count - 1;

				if (this.observations[lastIndex] is InvocationObservation invocationRecord)
				{
					// Determine the first index of all recorded matches that immediately precede
					// the last invocation; up to the previous recorded invocation or the beginning
					// of the recording (whichever comes first):
					int offset = lastIndex;
					while (offset > 0 && this.observations[offset - 1] is MatchObservation)
					{
						--offset;
					}

					mock = invocationRecord.Mock;
					invocation = invocationRecord.Invocation;
					matches = new Matches(this, offset, lastIndex - offset);
					return true;
				}
			}

			mock = default;
			invocation = default;
			matches = default;
			return false;
		}

		/// <summary>
		///   Checks whether the last thing observed was a <see cref="Match"/> matcher.
		///   If <see langword="true"/>, details about that matcher are provided via the <see langword="out"/> parameter.
		/// </summary>
		/// <param name="match">The observed <see cref="Match"/> matcher.</param>
		public bool LastObservationWasMatcher(out Match match)
		{
			if (this.observations != null && this.observations[this.observations.Count - 1] is MatchObservation matchRecord)
			{
				match = matchRecord.Match;
				return true;
			}

			match = default;
			return false;
		}

		/// <summary>
		///   Allocation-free pseudo-collection (think `ReadOnlySpan&lt;Match&gt;`)
		///   used to access all <see cref="Match"/>es associated with a recorded invocation.
		/// </summary>
		public readonly struct Matches
		{
			private readonly FluentMockContext context;
			private readonly int offset;
			private readonly int count;

			public Matches(FluentMockContext context, int offset, int count)
			{
				this.context = context;
				this.offset = offset;
				this.count = count;
			}

			public int Count => this.count;

			public Match this[int index] => ((MatchObservation)this.context.observations[this.offset + index]).Match;
		}

		private abstract class Observation : IDisposable
		{
			protected Observation()
			{
			}

			public virtual void Dispose()
			{
			}
		}

		private sealed class InvocationObservation : Observation
		{
			public readonly Mock Mock;
			public readonly Invocation Invocation;

			private DefaultValueProvider defaultValueProvider;

			public InvocationObservation(Mock mock, Invocation invocation)
			{
				this.Mock = mock;
				this.Invocation = invocation;

				this.defaultValueProvider = mock.DefaultValueProvider;
				mock.DefaultValueProvider = DefaultValueProvider.Mock;
			}

			public override void Dispose()
			{
				this.Mock.DefaultValueProvider = this.defaultValueProvider;
			}
		}

		private sealed class MatchObservation : Observation
		{
			public readonly Match Match;

			public MatchObservation(Match match)
			{
				this.Match = match;
			}
		}
	}
}
