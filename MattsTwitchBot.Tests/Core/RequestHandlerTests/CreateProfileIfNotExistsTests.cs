using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Profile;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class CreateProfileIfNotExistsTests : UnitTest
    {
        private CreateProfileIfNotExistsHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new CreateProfileIfNotExistsHandler(MockBucketProvider.Object);
        }

        [Test]
        public async Task If_profile_does_exist_then_upsert_doesnt_happen()
        {
            // arrange
            var username = "MusicalBookworm";
            var request = new CreateProfileIfNotExists(username);
            MockCollection.Setup(m => m.ExistsAsync(username, null)).ReturnsAsync(new FakeExistsResult(true));

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockCollection.Verify(m => m.UpsertAsync(It.IsAny<string>(), It.IsAny<TwitcherProfile>(), null), Times.Never);
        }

        [Test]
        public async Task If_profile_doesnt_exist_then_upsert_does_happen()
        {
            // arrange
            var username = "MusicalBookworm";
            var request = new CreateProfileIfNotExists(username);
            MockCollection.Setup(m => m.ExistsAsync(username, null)).ReturnsAsync(new FakeExistsResult(false));
        
            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockCollection.Verify(m => m.UpsertAsync(
                username,
                It.Is<TwitcherProfile>(x => x != null),
                null));
        }
    }
}