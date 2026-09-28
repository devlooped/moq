using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Helpers for applying behaviors to setups.
    /// </summary>
    static class SetupExtensions
    {
        public static IMockRuntime GetRuntime(this ISetup setup) => MockRuntime.Get(setup.Sdk.Invocation.Target);

        public static IMockBehaviorPipeline GetPipeline(this ISetup setup) => setup.GetRuntime().GetPipeline(setup.Sdk);

        public static bool IsUntyped(this ISetup setup) => setup is IUntypedSetup { Untyped: true };

        public static void SetReturnValue(this ISetup setup, object? value)
        {
            var pipeline = setup.GetPipeline();
            if (pipeline.Behaviors.OfType<ReturnsBehavior>().FirstOrDefault() is { } returns)
                returns.Value = value;
            else
                pipeline.Behaviors.Add(new ReturnsBehavior(value));
        }

        public static void SetReturnValue(this ISetup setup, Func<IArgumentCollection, object?> getter)
        {
            var pipeline = setup.GetPipeline();
            if (pipeline.Behaviors.OfType<ReturnsBehavior>().FirstOrDefault() is { } returns)
                returns.ValueGetter = getter;
            else
                pipeline.Behaviors.Add(new ReturnsBehavior(getter));
        }

        public static void SetException(this ISetup setup, Exception exception)
        {
            var pipeline = setup.GetPipeline();
            if (pipeline.Behaviors.OfType<ReturnsBehavior>().FirstOrDefault() is { } returns)
                returns.Exception = exception;
            else
                pipeline.Behaviors.Add(new ReturnsBehavior(exception));
        }

        public static void AddCallback(this ISetup setup, Action<IArgumentCollection> callback, bool setsOutputs = false)
        {
            var pipeline = setup.GetPipeline();
            // Callbacks run in the order they were added, before any other behaviors.
            pipeline.Behaviors.Insert(pipeline.Behaviors.OfType<CallbackBehavior>().Count(), new CallbackBehavior(callback, setsOutputs));
        }

        /// <summary>
        /// Invokes a delegate matching the member signature with the invocation arguments, 
        /// propagating back any ref/out values it sets.
        /// </summary>
        public static object? InvokeWith(this Delegate handler, IArgumentCollection arguments)
        {
            var values = new object?[arguments.Count];
            for (var i = 0; i < values.Length; i++)
                values[i] = arguments.GetValue(i);

            object? result;
            try
            {
                result = handler.DynamicInvoke(values);
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                throw;
            }

            for (var i = 0; i < values.Length; i++)
            {
                if (arguments[i].Parameter.ParameterType.IsByRef)
                    arguments.SetValue(i, values[i]);
            }

            return result;
        }

        /// <summary>
        /// Whether the delegate sets any ref/out values.
        /// </summary>
        public static bool HasRefOut(this Delegate handler)
            => handler.GetMethodInfo().GetParameters().Any(x => x.ParameterType.IsByRef) ||
               handler.GetType().GetMethod("Invoke")?.GetParameters().Any(x => x.ParameterType.IsByRef) == true;
    }
}
