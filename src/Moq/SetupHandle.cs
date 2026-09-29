using System.Diagnostics;
using Moq.Sdk;

namespace Moq
{
    interface ITypedSetup
    {
        /// <summary>
        /// Whether handlers receive the member arguments.
        /// A syntax setup sets this to <see langword="false"/> and handlers receive the invocation's argument collection.
        /// </summary>
        bool IsTyped { get; }
    }

    [DebuggerDisplay("{Sdk}")]
    class SetupHandle<TDelegate>(IMockSetup setup, bool IsTyped = true) : ISetupRef<TDelegate>, ITypedSetup
    {
        public IMockSetup Sdk => setup;

        public bool IsTyped { get; } = IsTyped;
    }

    [DebuggerDisplay("{Sdk}")]
    class SetupHandle<TDelegate, TResult>(IMockSetup setup, bool IsTyped = true) : ISetup<TDelegate, TResult>, ITypedSetup
    {
        public IMockSetup Sdk => setup;

        public bool IsTyped { get; } = IsTyped;
    }

    /// <summary>
    /// A lazy property (or indexer) setup which only invokes the getter or setter 
    /// on the mock when the setup is actually used.
    /// </summary>
    [DebuggerDisplay("{member,nq}")]
    class PropertySetupHandle<T, TGetter, TSetter, TValue> : IPropertySetup<TGetter, TSetter, TValue>, ITypedSetup where T : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IMock<T> mock;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly string member;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly System.Action<T> getter;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly System.Action<T, TValue>? setter;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IArgumentMatcher[] matchers;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IMockSetup? setup;

        public PropertySetupHandle(IMock<T> mock, string member, System.Action<T> getter, System.Action<T, TValue>? setter, bool indexer)
        {
            this.mock = mock;
            this.member = member;
            this.getter = getter;
            this.setter = setter;
            // Indexer argument matchers are evaluated before the indexer is accessed, 
            // so we need to replay them whenever we actually invoke the getter or setter.
            matchers = indexer ? MockSetup.TakeMatchers() : System.Array.Empty<IArgumentMatcher>();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IMockSetup Sdk => setup ??= SetupGetter();

        public bool IsTyped => true;

        public ISetup<TGetter, TValue> Get()
        {
            _ = Sdk;
            return this;
        }

        public ISetup<TSetter> Set(TValue value)
        {
            if (setter == null)
                throw new System.NotSupportedException();

            // The value matchers (if any) were pushed before our key matchers are replayed.
            var values = MockSetup.TakeMatchers();
            using (SetupFactory.Begin())
            {
                foreach (var matcher in matchers)
                    MockSetup.Push(matcher);
                foreach (var matcher in values)
                    MockSetup.Push(matcher);

                setter(mock.Object, value);
                return new SetupHandle<TSetter>(SetupFactory.Current());
            }
        }

        IMockSetup SetupGetter()
        {
            using (SetupFactory.Begin())
            {
                foreach (var matcher in matchers)
                    MockSetup.Push(matcher);

                getter(mock.Object);
                return SetupFactory.Current();
            }
        }
    }
}
