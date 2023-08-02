// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class VerifyInvocationCount : Behavior
    After:
        sealed class VerifyInvocationCount : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class VerifyInvocationCount : Behavior
    After:
        sealed class VerifyInvocationCount : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class VerifyInvocationCount : Behavior
    After:
        sealed class VerifyInvocationCount : Behavior
    */
    sealed class VerifyInvocationCount : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private int count;
            private readonly Times times;
            private readonly MethodCall setup;
    After:
            int count;
            readonly Times times;
            readonly MethodCall setup;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private int count;
            private readonly Times times;
            private readonly MethodCall setup;
    After:
            int count;
            readonly Times times;
            readonly MethodCall setup;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private int count;
            private readonly Times times;
            private readonly MethodCall setup;
    After:
            int count;
            readonly Times times;
            readonly MethodCall setup;
    */
    {
        int count;
        readonly Times times;
        readonly MethodCall setup;

        public VerifyInvocationCount(MethodCall setup, Times times)
        {
            this.setup = setup;
            this.times = times;
            this.count = 0;
        }

        public void Reset()
        {
            this.count = 0;
        }

        public override void Execute(Invocation invocation)
        {
            ++this.count;
            this.VerifyUpperBound();
        }

        public void Verify()
        {
            if (!this.times.Validate(this.count))
            {
                throw MockException.IncorrectNumberOfCalls(this.setup, this.times, this.count);
            }
        }

        public void VerifyUpperBound()
        {
            var (_, maxCount) = this.times;
            if (this.count > maxCount)
            {
                throw MockException.IncorrectNumberOfCalls(this.setup, this.times, this.count);
            }
        }
    }
}
