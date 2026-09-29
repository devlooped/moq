using System;
using System.Reflection;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Static class intended to be imported statically, like 
    /// <c>using static Moq.Syntax;</c>. Groups functionality 
    /// <see cref="Arg"/> and <see cref="Moq.Raise"/> 
    /// so it makes more sense in a statically imported context.
    /// </summary>
    public static class Syntax
    {
        /// <summary>
        /// Matches any value of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public static T Any<T>() => Arg.Any<T>();

        /// <summary>
        /// Matches a value of the given type if it satisfies the specified condition.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="condition">The condition to check against actual invocation values.</param>
        public static T Any<T>(Func<T?, bool> condition) => Arg.Any(condition);

        /// <summary>
        /// Matches all values that do not equal the specified value.
        /// </summary>
        public static T Not<T>(T value) => Arg.Not(value);

        /// <summary>
        /// Raises the event being attached to, passing the target mock 
        /// as the sender, and <see cref="EventArgs.Empty"/> args.
        /// </summary>
        public static EventHandler? Raise() => Moq.Raise.Event();

        /// <summary>
        /// Raises the event being attached to, passing the target mock 
        /// as the sender, and the given <paramref name="args"/> as the 
        /// event arguments.
        /// </summary>
        public static EventHandler<TEventArgs>? Raise<TEventArgs>(TEventArgs args) => Moq.Raise.Event<TEventArgs>(args);

        /// <summary>
        /// Raises the event being attached to, passing the target mock 
        /// as the sender, and the given <paramref name="args"/> as the 
        /// event arguments.
        /// </summary>
        public static EventHandler? Raise(EventArgs args) => Moq.Raise.Event(args);

        /// <summary>
        /// Raises the event being attached to, passing the target mock 
        /// as the sender, and the given <paramref name="args"/> as the 
        /// event arguments.
        /// </summary>
        public static TEventHandler? Raise<TEventHandler>(EventArgs args) => Moq.Raise.Event<TEventHandler>(args);

        /// <summary>
        /// Raises the event being attached to, passing the given 
        /// <paramref name="args"/> as the event arguments.
        /// </summary>
        public static TEventHandler? Raise<TEventHandler>(params object[] args) => Moq.Raise.Event<TEventHandler>(args);

        /// <summary>
        /// Marks a code block as being setup for mocks. Usage: <c>using (Setup()) { ... }</c>.
        /// </summary>
        /// <seealso cref="SetupScope"/>
        public static IDisposable Setup() => new SetupScope();

        /// <summary>
        /// Sets up the last member invoked on a mock by the <paramref name="member"/> function, 
        /// which can be a recursive call (i.e. <c>Setup(() => mock.Object.Child.Value)</c>). 
        /// Handlers for the returned setup receive the invocation arguments.
        /// </summary>
        public static ISetup<Func<IArgumentCollection, TResult>, TResult> Setup<TResult>(Func<TResult> member)
        {
            using (SetupFactory.Begin())
            {
                member();
                return new SetupHandle<Func<IArgumentCollection, TResult>, TResult>(SetupFactory.Current(), untyped: true);
            }
        }

        /// <summary>
        /// Sets up the last void member invoked on a mock by the <paramref name="member"/> action, 
        /// which can be a recursive call (i.e. <c>Setup(() => mock.Object.Child.Execute())</c>). 
        /// Handlers for the returned setup receive the invocation arguments.
        /// </summary>
        public static ISetup<Action<IArgumentCollection>> Setup(Action member)
        {
            using (SetupFactory.Begin())
            {
                member();
                return new SetupHandle<Action<IArgumentCollection>>(SetupFactory.Current(), untyped: true);
            }
        }

        /// <summary>
        /// Sets up the mock member referenced by <paramref name="member"/> for any argument values, 
        /// typically used to access and set ref/out arguments via a custom delegate with the same signature, 
        /// like <c>SetupRef&lt;TryParse&gt;(mock.Object.TryParse)</c>.
        /// </summary>
        [SetupScope]
        public static ISetupRef<TDelegate> SetupRef<TDelegate>(TDelegate member) where TDelegate : Delegate
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            using (SetupFactory.Begin())
            {
                var parameters = member.GetMethodInfo().GetParameters();
                var arguments = new object?[parameters.Length];
                var defaults = new DefaultValueProvider(false);
                for (var i = 0; i < arguments.Length; i++)
                {
                    var parameter = parameters[i];
                    var type = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType()! : parameter.ParameterType;
                    MockSetup.Push(new AnyMatcher(type));
                    if (!parameter.IsOut)
                        arguments[i] = defaults.GetDefault(type);
                }

                member.DynamicInvoke(arguments);
                return new SetupHandle<TDelegate>(SetupFactory.Current());
            }
        }

        /// <summary>
        /// Sets up the mock member referenced by the delegate returned from <paramref name="member"/> 
        /// for any argument values. Use this overload when there is a recursive mock involved, 
        /// like <c>SetupRef&lt;TryParse&gt;(() => mock.Object.Parser.TryParse)</c>.
        /// </summary>
        [SetupScope]
        public static ISetupRef<TDelegate> SetupRef<TDelegate>(Func<TDelegate> member) where TDelegate : Delegate
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            TDelegate target;
            using (new SetupScope())
            {
                target = member();
            }

            return SetupRef(target);
        }
    }
}
