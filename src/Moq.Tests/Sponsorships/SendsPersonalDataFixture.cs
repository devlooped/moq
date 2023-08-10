using System;
using System.Threading.Tasks;

using Moq.Sponsorships;
using Moq.Sponsorships.Interfaces;

using Xunit;

namespace Moq.Tests.Sponsorships
{
    public class SendsPersonalDataFixture
    {
        public class MockCollector : IDataCollector
        {
            public Task CollectData(DataCollection collection)
            {
                collection.Add("Mock?", "Mock!");
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void CollectorRuns()
        {
            MockCollector collector = new MockCollector();

            Sponsorship sponsorship = new Sponsorship();
            sponsorship.Collectors.Add( collector );

            DataCollection collection = new DataCollection();

            var isSponsor = sponsorship.Iliiiliiuilkadaojdawdhiaudhawd(collection).GetAwaiter().GetResult();

            Assert.False(collection.Has("SomeRandomThing"));
            Assert.True(collection.Has("TW9jaz8="));
        }
    }
}
