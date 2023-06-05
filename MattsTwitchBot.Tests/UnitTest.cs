using Couchbase;
using Couchbase.KeyValue;
using MattsTwitchBot.Core;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Tests
{
    public abstract class UnitTest
    {
        protected Mock<ITwitchBucketProvider> MockBucketProvider;
        protected Mock<IBucket> MockBucket;
        protected Mock<ICouchbaseCollection> MockCollection;
        protected Mock<ICluster> MockCluster;
        protected Mock<ITwitchClient> MockTwitchClient;
        protected Mock<IOptions<TwitchOptions>> MockTwitchOptions;
        protected Mock<IMediator> MockMediator;
        protected Mock<ITwitchApiWrapper> MockApiWrapper;

        [SetUp]
        public virtual void Setup()
        {
            MockCollection = new Mock<ICouchbaseCollection>();
            MockCluster = new Mock<ICluster>();
            MockBucket = new Mock<IBucket>();
            MockBucket.Setup(m => m.DefaultCollectionAsync()).ReturnsAsync(MockCollection.Object);
            MockBucket.Setup(m => m.Cluster).Returns(MockCluster.Object);
            MockBucketProvider = new Mock<ITwitchBucketProvider>();
            MockBucketProvider.Setup(m => m.GetBucketAsync()).ReturnsAsync(MockBucket.Object);
            MockTwitchClient = new Mock<ITwitchClient>();
            MockTwitchOptions = new Mock<IOptions<TwitchOptions>>();
            MockMediator = new Mock<IMediator>();
            MockApiWrapper = new Mock<ITwitchApiWrapper>();
        }

    }
}