using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.RequestHandlers.Profile;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class GetProfileHandlerTests
    {
        private GetProfileHandler _handler;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(m => m.GetBucket()).Returns(_mockBucket.Object);
            _handler = new GetProfileHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task Profile_is_looked_up_by_twitch_username()
        {
            // arrange
            var username = "fork04_";
            var request = new GetProfile(username);
            _mockBucket.Setup(m => m.GetAsync<TwitcherProfile>(It.IsAny<string>()))
                .ReturnsAsync(new FakeOperationResult<TwitcherProfile> {Value = new TwitcherProfile()});

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockBucket.Verify(m => m.GetAsync<TwitcherProfile>(username), Times.Once);
        }
    }
}