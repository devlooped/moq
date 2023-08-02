// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class SequenceSetup : SetupWithOutParameterSupport
    After:
        sealed class SequenceSetup : SetupWithOutParameterSupport
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class SequenceSetup : SetupWithOutParameterSupport
    After:
        sealed class SequenceSetup : SetupWithOutParameterSupport
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class SequenceSetup : SetupWithOutParameterSupport
    After:
        sealed class SequenceSetup : SetupWithOutParameterSupport
    */
    /// <summary>
    ///   Programmable setup used by <see cref="Mock.SetupSequence(Mock, LambdaExpression)"/>.
    /// </summary>
    sealed class SequenceSetup : SetupWithOutParameterSupport

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private ConcurrentQueue<Behavior> behaviors;
    After:
            ConcurrentQueue<Behavior> behaviors;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private ConcurrentQueue<Behavior> behaviors;
    After:
            ConcurrentQueue<Behavior> behaviors;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private ConcurrentQueue<Behavior> behaviors;
    After:
            ConcurrentQueue<Behavior> behaviors;
    */
    {
        // contains the behaviors set up with the `CallBase`, `Pass`, `Returns`, and `Throws` verbs
        ConcurrentQueue<Behavior> behaviors;

        public SequenceSetup(Expression originalExpression, Mock mock, MethodExpectation expectation)
            : base(originalExpression, mock, expectation)
        {
            this.behaviors = new ConcurrentQueue<Behavior>();
        }

        public void AddBehavior(Behavior behavior)
        {
            Debug.Assert(behavior != null);

            this.behaviors.Enqueue(behavior);
        }

        protected override void ExecuteCore(Invocation invocation)
        {
            if (this.behaviors.TryDequeue(out var behavior))
            {
                behavior.Execute(invocation);
            }
            else
            {
                // we get here if there are more invocations than configured behaviors.
                // if the setup method does not have a return value, we don't need to do anything;
                // if it does have a return value, we produce the default value.

                var returnType = invocation.Method.ReturnType;
                if (returnType != typeof(void))
                {
                    invocation.ReturnValue = returnType.GetDefaultValue();
                }
            }
        }
    }
}
