using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.KeyValue;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Profile;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class CreateProfileIfNotExistsTests
    {
        private CreateProfileIfNotExistsHandler _handler;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;
        private Mock<ICouchbaseCollection> _mockCollection;

        [SetUp]
        public void Setup()
        {
            _mockCollection = new Mock<ICouchbaseCollection>();
            _mockBucket = new Mock<IBucket>();
            _mockBucket.Setup(m => m.DefaultCollection()).Returns(_mockCollection.Object);
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(m => m.GetBucketAsync()).ReturnsAsync(_mockBucket.Object);
            _handler = new CreateProfileIfNotExistsHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task If_profile_does_exist_then_upsert_doesnt_happen()
        {
            // arrange
            var username = "MusicalBookworm";
            var request = new CreateProfileIfNotExists(username);
            _mockCollection.Setup(m => m.ExistsAsync(username, null)).ReturnsAsync(new FakeExistsResult(true));

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockCollection.Verify(m => m.UpsertAsync(It.IsAny<string>(), It.IsAny<TwitcherProfile>(), null), Times.Never);
        }

        [Test]
        public async Task If_profile_doesnt_exist_then_upsert_does_happen()
        {
            // arrange
            var username = "MusicalBookworm";
            var request = new CreateProfileIfNotExists(username);
            _mockCollection.Setup(m => m.ExistsAsync(username, null)).ReturnsAsync(new FakeExistsResult(false));
        
            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockCollection.Verify(m => m.UpsertAsync(
                username,
                It.Is<TwitcherProfile>(x => x != null),
                null));
        }
    }
}