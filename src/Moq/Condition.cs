// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
    sealed class Condition
    {
        Func<bool> condition;
        Action? success;

        public Condition(Func<bool> condition, Action? success = null, bool sequence = false)
        {
            this.condition = condition;
            this.success = success;
            this.sequence = sequence;
        }

        public bool IsTrue => this.condition.Invoke();

        public bool sequence { get; }

        public void SetupEvaluatedSuccessfully() => this.success?.Invoke();
    }
}
