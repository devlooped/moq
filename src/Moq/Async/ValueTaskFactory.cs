// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moq.Async
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
    After:
        sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
    After:
        sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
    After:
        sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
    */
    sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
    {
        public static readonly ValueTaskFactory Instance = new ValueTaskFactory();


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private ValueTaskFactory()
        After:
                ValueTaskFactory()
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private ValueTaskFactory()
        After:
                ValueTaskFactory()
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private ValueTaskFactory()
        After:
                ValueTaskFactory()
        */
        ValueTaskFactory()
        {
        }

        public override ValueTask CreateCompleted()
        {
            return default;
        }

        public override ValueTask CreateFaulted(Exception exception)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(exception);
            return new ValueTask(tcs.Task);
        }

        public override ValueTask CreateFaulted(IEnumerable<Exception> exceptions)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(exceptions);
            return new ValueTask(tcs.Task);
        }
    }
}
