// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Reflection;

using Moq.Properties;

namespace Moq.Language.Flow
{
	// keeping the fluent API separate from `SequenceMethodCall` saves us from having to
	// define a generic variant `SequenceMethodCallReturn<TResult>`, which would be much more
	// work that having a generic fluent API counterpart `SequenceMethodCall<TResult>`.
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class SetupSequencePhrase : ISetupSequentialAction
	{
		private SequenceSetup setup;

		public SetupSequencePhrase(SequenceSetup setup)
		{
			this.setup = setup;
		}

		public ISetupSequentialAction Pass()
		{
			this.setup.AddPass();
			return this;
		}

		public ISetupSequentialAction Throws<TException>()
			where TException : Exception, new()
			=> this.Throws(new TException());

		public ISetupSequentialAction Throws(Exception exception)
		{
			this.setup.AddThrows(exception);
			return this;
		}
	}

	internal sealed class SetupSequencePhrase<TResult> : ISetupSequentialResult<TResult>
	{
		private SequenceSetup setup;

		public SetupSequencePhrase(SequenceSetup setup)
		{
			this.setup = setup;
		}

		public ISetupSequentialResult<TResult> CallBase()
		{
			this.setup.AddCallBase();
			return this;
		}

		public ISetupSequentialResult<TResult> Returns(TResult value)
		{
			this.setup.AddReturns(value);
			return this;
		}

		public ISetupSequentialResult<TResult> Returns(Func<TResult> valueFunction)
		{
			Guard.NotNull(valueFunction, nameof(valueFunction));

			// If `valueFunction` is `TResult`, that is someone is setting up the return value of a method
			// that returns a `TResult`, then we have arrived here because C# picked the wrong overload:
			// We don't want to invoke the passed delegate to get a return value; the passed delegate
			// already is the return value.
			if (valueFunction is TResult)
			{
				return this.Returns((TResult)(object)valueFunction);
			}

			this.setup.AddReturns(() => valueFunction());
			return this;
		}

		public ISetupSequentialResult<TResult> Throws(Exception exception)
		{
			this.setup.AddThrows(exception);
			return this;
		}

		public ISetupSequentialResult<TResult> Throws<TException>()
			where TException : Exception, new()
			=> this.Throws(new TException());
	}
}
