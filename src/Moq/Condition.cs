// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	internal sealed class Condition
	{
		private Func<bool> condition;
		private Action success;

		public Condition(Func<bool> condition, Action success = null)
		{
			this.condition = condition;
			this.success = success;
		}

		public bool IsTrue => this.condition?.Invoke() == true;

		public void SetupEvaluatedSuccessfully() => this.success?.Invoke();
	}
}
