using System;
using System.Collections.Generic;
using System.Linq;
using Avatars;

namespace Moq.Sdk
{
    public partial class MockSetup
    {
        /// <summary>
        /// Pushes an argument matcher in the current <see cref="CallContext{T}"/> with a 
        /// key of <see cref="Queue{IArgumentMatcher}"/>.
        /// </summary>
        public static void Push(IArgumentMatcher matcher) => Push<object>(matcher);

        /// <summary>
        /// Pushes an argument matcher in the current <see cref="CallContext{T}"/> with a 
        /// key of <see cref="Queue{IArgumentMatcher}"/> and returns a default value for <typeparamref name="T"/>.
        /// </summary>
        public static T Push<T>(IArgumentMatcher matcher)
        {
            CallContext<Queue<IArgumentMatcher>>.GetData(() => new Queue<IArgumentMatcher>())
               ?.Enqueue(matcher);

            return default!;
        }

        /// <summary>
        /// Freezes the argument matchers for the given method invocation, taking the collected 
        /// matchers so far in the <see cref="CallContext{T}"/> with the key <see cref="Queue{IArgumentMatcher}"/>.
        /// </summary>
        /// <param name="invocation">The invocation to freeze.</param>
        /// <returns>An <see cref="IMockSetup"/> that can be used to filter invocations in a behavior pipeline.</returns>
        internal static IMockSetup Freeze(IMethodInvocation invocation)
        {
            var currentMatchers = CallContext<Queue<IArgumentMatcher>>.GetData(() => new Queue<IArgumentMatcher>())
                ?? throw new InvalidOperationException(ThisAssembly.Strings.UnexpectedNullContextState(typeof(Queue<IArgumentMatcher>).FullName));

            var finalMatchers = new List<IArgumentMatcher>();
            var defaultValue = (invocation.Target as IMocked)?.
                Mock.Behaviors.OfType<DefaultValueBehavior>().FirstOrDefault()?.Provider ??
                new DefaultValueProvider();

            for (var i = 0; i < invocation.Arguments.Count; i++)
            {
                var argument = invocation.Arguments.GetValue(i);
                var parameter = invocation.Arguments[i].Parameter;

                // This is a bit fuzzy since we compare the actual argument value against the 
                // default value for the parameter type, or the type of the matcher in the 
                // queue of argument matchers to see if applies instead.
                if (Equals(argument, defaultValue.GetDefault(parameter.ParameterType)) &&
                    currentMatchers.Count != 0 &&
                    parameter.ParameterType.IsAssignableFrom(currentMatchers.Peek().ArgumentType))
                {
                    finalMatchers.Add(currentMatchers.Dequeue());
                }
                else
                {
                    finalMatchers.Add(new ValueMatcher(parameter.ParameterType, argument));
                }
            }

            return new MockSetup(invocation, finalMatchers.ToArray());
        }
    }
}
