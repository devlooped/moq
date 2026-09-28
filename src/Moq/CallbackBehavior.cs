using System;
using System.ComponentModel;
using System.Diagnostics;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Invokes a callback before continuing with the rest of the setup behaviors.
    /// </summary>
    [DebuggerDisplay("Callback", Name = "Callback", Type = nameof(CallbackBehavior))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    class CallbackBehavior(Action<IArgumentCollection> callback, bool setsOutputs) : IMockBehavior
    {
        public IMethodReturn Execute(IMockRuntime mock, IMethodInvocation invocation, GetNextMockBehavior next)
        {
            callback(invocation.Arguments);
            var result = next().Invoke(mock, invocation, next);

            // Subsequent behaviors (i.e. the default value one) would otherwise 
            // replace the ref/out values set by the callback.
            if (setsOutputs && result.Exception == null)
                return invocation.CreateValueReturn(result.ReturnValue, invocation.Arguments);

            return result;
        }
    }
}
