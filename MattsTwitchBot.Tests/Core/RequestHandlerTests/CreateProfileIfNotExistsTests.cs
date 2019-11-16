using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
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

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(m => m.GetBucket()).Returns(_mockBucket.Object);
            _handler = new CreateProfileIfNotExistsHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task If_profile_does_exist_then_upsert_doesnt_happen()
        {
            // arrange
            var username = "MusicalBookworm";
            var request = new CreateProfileIfNotExists(username);
            _mockBucket.Setup(m => m.ExistsAsync(username)).ReturnsAsync(true);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockBucket.Verify(m => m.UpsertAsync(It.IsAny<Document<TwitcherProfile>>()), Times.Never);
        }

        [Test]
        public async Task If_profile_doesnt_exist_then_upsert_does_happen()
        {
            // arrange
            var username = "MusicalBookworm";
            var request = new CreateProfileIfNotExists(username);
            _mockBucket.Setup(m => m.ExistsAsync(username)).ReturnsAsync(false);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockBucket.Verify(m => m.UpsertAsync(
                It.Is<Document<TwitcherProfile>>(x => 
                    x.Id == username 
                    && x.Content != null)));
        }
    }
}