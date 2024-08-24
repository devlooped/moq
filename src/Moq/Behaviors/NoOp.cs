// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
    sealed class NoOp : Behavior
    {
        public static readonly NoOp Instance = new NoOp();

        NoOp()
        {
        }

        public override void Execute(Invocation invocation)
        {
        }
    }
}
