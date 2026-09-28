using System;
using System.ComponentModel;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Extensions for configuring callbacks when invoking mocks.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class CallbackExtension
    {
        static TResult Callback<TResult>(this TResult target, Action<IArgumentCollection> callback)
        {
            var setup = MockContext.CurrentSetup;
            if (setup != null)
            {
                var mock = MockRuntime.Get(setup.Invocation.Target);

                mock.Invocations.Remove(setup.Invocation);
                var behavior = mock.GetPipeline(setup);

                // If there is already a behavior wrap it instead, 
                // so we can do a callback after even it if it's a 
                // short-circuiting one like Returns.
                if (behavior.Behaviors.Count > 0)
                {
                    var wrapped = behavior.Behaviors[behavior.Behaviors.Count - 1];
                    behavior.Behaviors.RemoveAt(behavior.Behaviors.Count - 1);
                    behavior.Behaviors.Add(new AnonymousMockBehavior(
                        (m, i, next) =>
                        {
                            // If the wrapped target does not invoke the next 
                            // behavior (us), then we invoke the callback explicitly.
                            var called = false;

                            // Note we're tweaking the GetNextBehavior to always 
                            // call us, before invoking the actual next behavior.
                            var result = wrapped.Execute(m, i, () => (IMockRuntime _, IMethodInvocation __, GetNextMockBehavior ___) =>
                            {
                                callback(i.Arguments);
                                called = true;
                                return next()(m, i, next);
                            });

                            // The Returns behavior does not invoke the GetNextBehavior, 
                            // and therefore we won't have been called in that case, 
                            // so call the callback before returning.
                            if (!called)
                                callback(i.Arguments);

                            return result;
                        }, "Callback")
                   );
                }
                else
                {
                    behavior.Behaviors.Add(new AnonymousMockBehavior(
                        (m, i, next) =>
                        {
                            callback(i.Arguments);
                            return next()(m, i, next);
                        }, "Callback")
                   );
                }
            }

            return target;
        }
    }
}
