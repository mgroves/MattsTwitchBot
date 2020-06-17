using System.Threading;
using System.Threading.Tasks;
using Couchbase.KeyValue;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Profile;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class GetProfileHandlerTests : UnitTest
    {
        private GetProfileHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new GetProfileHandler(MockBucketProvider.Object);
        }

        [Test]
        public async Task Profile_is_looked_up_by_twitch_username()
        {
            // arrange
            var username = "fork04_";
            var request = new GetProfile(username);
            MockCollection.Setup(m => m.GetAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new FakeGetResult(new TwitcherProfile()));

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockCollection.Verify(m => m.GetAsync(username, It.IsAny<GetOptions>()), Times.Once);
        }
    }
}