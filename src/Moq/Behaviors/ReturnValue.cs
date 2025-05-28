// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
    sealed class ReturnValue : Behavior
    {
        readonly object? value;

        public ReturnValue(object? value)
        {
            this.value = value;
        }

        public object? Value => this.value;

        public override void Execute(Invocation invocation)
        {
            invocation.ReturnValue = this.value;
        }
    }
}
