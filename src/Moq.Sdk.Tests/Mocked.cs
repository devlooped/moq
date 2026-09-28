using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Stunts;

namespace Moq.Sdk.Tests
{
    public class Mocked : IMocked, IStunt
    {
        IMockRuntime mock;
        readonly IList<IStuntBehavior> behaviors = new ObservableCollection<IStuntBehavior>();

        public IMockRuntime Runtime => LazyInitializer.EnsureInitialized(ref mock, () => new DefaultMockRuntime(this));

        public IList<IStuntBehavior> Behaviors => behaviors;
    }
}
