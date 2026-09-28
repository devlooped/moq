using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Default implementation of the mock runtime API <see cref="IMockRuntime"/>, 
    /// which also ensures that the <see cref="IStunt.Behaviors"/> contains 
    /// the <see cref="MockContextBehavior"/> when initially created.
    /// </summary>
    [DebuggerDisplay("Invocations = {Invocations.Count}", Name = nameof(IMocked) + "." + nameof(IMocked.Runtime))]
    public class DefaultMockRuntime : IMockRuntime
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IStunt stunt;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ConcurrentDictionary<IMockSetup, IMockBehaviorPipeline> setupBehaviorMap = new ConcurrentDictionary<IMockSetup, IMockBehaviorPipeline>();

        /// <summary>
        /// Initializes the default <see cref="IMockRuntime"/> implementation for the given <paramref name="stunt"/>.
        /// </summary>
        public DefaultMockRuntime(IStunt stunt)
        {
            this.stunt = stunt ?? throw new ArgumentNullException(nameof(stunt));
            var behaviors = stunt.Behaviors;

            if (!behaviors.OfType<MockContextBehavior>().Any())
                behaviors.Insert(0, new MockContextBehavior());

            if (!behaviors.OfType<MockRecordingBehavior>().Any())
                behaviors.Insert(1, new MockRecordingBehavior());

            if (behaviors is INotifyCollectionChanged notify)
                notify.CollectionChanged += OnBehaviorsChanged;
        }

        /// <inheritdoc />
        public IList<IStuntBehavior> Behaviors => stunt.Behaviors;

        /// <inheritdoc />
        public ICollection<IMethodInvocation> Invocations { get; } = new List<IMethodInvocation>();

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object Object => stunt;

        /// <inheritdoc />
        public StateBag State { get; set; } = new StateBag();

        /// <inheritdoc />
        public IEnumerable<IMockBehaviorPipeline> Setups => setupBehaviorMap.Values;

        /// <inheritdoc />
        public IMockBehaviorPipeline GetPipeline(IMockSetup setup)
            => setupBehaviorMap.GetOrAdd(setup, x =>
            {
                var behaviors = stunt.Behaviors;
                var behavior = new MockBehaviorPipeline(x);

                // The tracking behavior must appear before the mock behaviors.
                var context = behaviors.OfType<MockContextBehavior>().FirstOrDefault();
                // If there is a recording behavior, it must be before mock behaviors too.
                var recording = behaviors.OfType<MockRecordingBehavior>().FirstOrDefault();

                var index = context == null ? 0 : behaviors.IndexOf(context);
                if (recording != null)
                    index = Math.Max(index, behaviors.IndexOf(recording));

                // NOTE: latest setup wins, since it goes to the top of the list.
                behaviors.Insert(++index, behavior);
                return behavior;
            });

        void OnBehaviorsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var behaviors = stunt.Behaviors;
                    // TODO: optimize these two checks
                    // Can't have more than one of MockContextBehavior, since that causes problems.
                    if (behaviors.OfType<MockContextBehavior>().Skip(1).Any())
                        throw new InvalidOperationException(ThisAssembly.Strings.DuplicateContextBehavior);
                    // Can't have more than one of MockRecordingBehavior, since that causes problems.
                    if (behaviors.OfType<MockRecordingBehavior>().Skip(1).Any())
                        throw new InvalidOperationException(ThisAssembly.Strings.DuplicateRecordingBehavior);

                    foreach (var behavior in e.NewItems.OfType<IMockBehaviorPipeline>())
                        setupBehaviorMap.TryAdd(behavior.Setup, behavior);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var behavior in e.OldItems.OfType<IMockBehaviorPipeline>())
                        setupBehaviorMap.TryRemove(behavior.Setup, out _);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var behavior in e.OldItems.OfType<IMockBehaviorPipeline>())
                        setupBehaviorMap.TryRemove(behavior.Setup, out _);
                    foreach (var behavior in e.NewItems.OfType<IMockBehaviorPipeline>())
                        setupBehaviorMap.TryAdd(behavior.Setup, behavior);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    setupBehaviorMap.Clear();
                    foreach (var behavior in stunt.Behaviors.OfType<IMockBehaviorPipeline>())
                        setupBehaviorMap.TryAdd(behavior.Setup, behavior);
                    break;
            }
        }
    }
}