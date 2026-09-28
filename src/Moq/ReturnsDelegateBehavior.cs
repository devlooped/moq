using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    [DebuggerDisplay("{@delegate}", Name = "Returns", Type = nameof(ReturnsDelegateBehavior))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    class ReturnsDelegateBehavior : IMockBehavior
    {
        [DebuggerDisplay("<function>")]
        readonly Delegate @delegate;

        public ReturnsDelegateBehavior(Delegate @delegate) => this.@delegate = @delegate;

        public IMethodReturn Execute(IMockRuntime mock, IMethodInvocation invocation, GetNextMockBehavior next)
        {
            var values = invocation.Arguments.Select(prm => invocation.Arguments.GetValue(prm.Name)).ToArray();
            var returnValue = @delegate.DynamicInvoke(values);
            for (var i = 0; i < values.Length; i++)
                invocation.Arguments.SetValue(i, values[i]);

            return invocation.CreateValueReturn(returnValue, invocation.Arguments);
        }
    }
}
