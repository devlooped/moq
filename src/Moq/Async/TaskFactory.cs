// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq.Async
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class TaskFactory : AwaitableFactory<Task>
    After:
        sealed class TaskFactory : AwaitableFactory<Task>
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class TaskFactory : AwaitableFactory<Task>
    After:
        sealed class TaskFactory : AwaitableFactory<Task>
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class TaskFactory : AwaitableFactory<Task>
    After:
        sealed class TaskFactory : AwaitableFactory<Task>
    */
    sealed class TaskFactory : AwaitableFactory<Task>
    {
        public static readonly TaskFactory Instance = new TaskFactory();


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private TaskFactory()
        After:
                TaskFactory()
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private TaskFactory()
        After:
                TaskFactory()
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private TaskFactory()
        After:
                TaskFactory()
        */
        TaskFactory()
        {
        }

        public override Task CreateCompleted()
        {
            return Task.FromResult<object>(default);
        }

        public override Task CreateFaulted(Exception exception)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        public override Task CreateFaulted(IEnumerable<Exception> exceptions)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(exceptions);
            return tcs.Task;
        }
    }
}
