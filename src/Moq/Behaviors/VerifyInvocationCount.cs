// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
    sealed class VerifyInvocationCount : Behavior
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
