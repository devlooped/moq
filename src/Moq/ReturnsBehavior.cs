using System;
using System.ComponentModel;
using System.Diagnostics;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// A custom behavior for returning values, so that 
    /// the actual value to return can be replaced on successive
    /// <see cref="ReturnsExtension"/> method calls.
    /// </summary>
    [DebuggerDisplay("{DebuggerValue}", Name = "Returns", Type = nameof(ReturnsBehavior))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    class ReturnsBehavior : IMockBehavior
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Func<IArgumentCollection, object?> getter;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object? value;

        public ReturnsBehavior(Func<IArgumentCollection, object?> valueGetter) => getter = valueGetter;

        public ReturnsBehavior(object? value)
        {
            Value = value;
            getter = _ => value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object? Value
        {
            set
            {
                this.value = value;
                getter = _ => this.value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Func<IArgumentCollection, object?> ValueGetter
        {
            get => getter;
            set
            {
                // Clear previous constant value, if any.
                this.value = null;
                getter = value;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object DebuggerValue => value ?? "<function>";

        public IMethodReturn Execute(IMockRuntime mock, IMethodInvocation invocation, GetNextMockBehavior next)
            => invocation.CreateValueReturn(getter(invocation.Arguments), invocation.Arguments);
    }
}
