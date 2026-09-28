using System;
using System.ComponentModel;
using System.Diagnostics;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// A custom behavior for returning values (or throwing exceptions), so that 
    /// the actual outcome can be replaced on successive <see cref="ReturnsExtension"/> 
    /// and <see cref="ThrowsExtension"/> method calls.
    /// </summary>
    [DebuggerDisplay("{DebuggerValue}", Name = "Returns", Type = nameof(ReturnsBehavior))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    class ReturnsBehavior : IMockBehavior
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Func<IArgumentCollection, object?> getter;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object? value;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Exception? exception;

        public ReturnsBehavior(Func<IArgumentCollection, object?> valueGetter) => getter = valueGetter;

        public ReturnsBehavior(object? value)
        {
            Value = value;
            getter = _ => value;
        }

        public ReturnsBehavior(Exception exception)
        {
            getter = _ => null;
            Exception = exception;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object? Value
        {
            get => value;
            set
            {
                this.value = value;
                exception = null;
                getter = _ => this.value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<IArgumentCollection, object?> ValueGetter
        {
            get => getter;
            set
            {
                // Clear previous constant value or exception, if any.
                this.value = null;
                exception = null;
                getter = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Exception? Exception
        {
            get => exception;
            set
            {
                this.value = null;
                exception = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object DebuggerValue => (object?)exception ?? value ?? "<function>";

        public IMethodReturn Execute(IMockRuntime mock, IMethodInvocation invocation, GetNextMockBehavior next)
            => exception != null ?
                invocation.CreateExceptionReturn(exception) :
                invocation.CreateValueReturn(getter(invocation.Arguments), invocation.Arguments);
    }
}
