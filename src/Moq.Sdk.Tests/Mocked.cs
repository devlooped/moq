using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Stunts;

namespace Moq.Sdk.Tests
{
    public class Mocked : IMocked, IStunt
    {
        IMock mock;
        readonly IList<IStuntBehavior> behaviors = new ObservableCollection<IStuntBehavior>();

        public IMock Mock => LazyInitializer.EnsureInitialized(ref mock, () => new DefaultMock(this));

        public IList<IStuntBehavior> Behaviors => behaviors;
    }
}
