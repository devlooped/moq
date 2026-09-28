using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Stunts;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Adds support for recursive mocks invoked during a setup, 
    /// so that types that can be intercepted (see <see cref="Extensions.CanBeIntercepted(Type)"/>)
    /// are turned into mocks automatically.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RecursiveMockBehavior : IStuntBehavior
    {
        /// <summary>
        /// Only applies if there is an active setup.
        /// </summary>
        public bool AppliesTo(IMethodInvocation invocation)
            => SetupScope.IsActive;

        /// <summary>
        /// Ensures that a recursive mock invocation during a setup returns a 
        /// new mock instead of null.
        /// </summary>
        public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
        {
            if (invocation.MethodBase is MethodInfo info &&
                info.ReturnType != typeof(void) &&
                info.ReturnType.CanBeIntercepted())
            {
                var result = next.Invoke(invocation, next);
                if (result.ReturnValue == null)
                {
                    // Turn the null value into a mock for the current invocation setup
                    var currentMock = ((IMocked)invocation.Target).Mock;
                    // NOTE: this invocation will throw if there isn't a matching 
                    // mock for the given return type in the same assembly as the 
                    // current mock. It might be tricky to diagnose at run-time, 
                    // but at design-time our recursive mock analyzer should catch 
                    // this with a diagnostic that the type hasn't been generated 
                    // yet.
                    var recursiveMock = ((IMocked)MockFactory.Default.CreateMock(
                        // Use the same assembly as the current target
                        invocation.Target.GetType().Assembly,
                        info.ReturnType,
                        new Type[0],
                        new object[0])).Mock;

                    // Clone the current mock's behaviors, except for the setups and the 
                    // context and recording behaviors which are added already by default.
                    foreach (var behavior in currentMock.Behaviors.Where(x =>
                        !(x is IMockBehaviorPipeline) &&
                        !(x is MockContextBehavior) &&
                        !(x is MockRecordingBehavior)))
                    {
                        recursiveMock.Behaviors.Add(behavior);
                    }

                    // Set up the current invocation to return the created value
                    var setup = currentMock.GetPipeline(MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>());
                    var returnBehavior = setup.Behaviors.OfType<ReturnsBehavior>().FirstOrDefault();
                    if (returnBehavior != null)
                        returnBehavior.Value = recursiveMock.Object;
                    else
                        setup.Behaviors.Add(new ReturnsBehavior(recursiveMock.Object));

                    // Copy over values from the result, so that outputs contain the default values.
                    var arguments = invocation.Arguments;
                    for (var i = 0; i < arguments.Count; i++)
                    {
                        var parameter = arguments[i].Parameter;
                        if (parameter.IsOut)
                            arguments.SetValue(i, result.Outputs.GetValue(parameter.Name));
                    }

                    return invocation.CreateValueReturn(recursiveMock.Object, arguments);
                }

                return result;
            }

            return next.Invoke(invocation, next);
        }
    }
}
