using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.KeyValue;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Main;
using MattsTwitchBot.Tests.Fakes;
using MattsTwitchBot.Tests.Helpers;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class UserHasArrivedHandlerTests : UnitTest
    {
        private UserHasArrivedHandler _handler;
        // private Mock<ITwitchBucketProvider> _mockBucketProvider;
        // private Mock<IBucket> _mockBucket;
        private Mock<IHubContext<ChatWebPageHub, IChatWebPageHub>> _mockHub;
        private Mock<IHubClients<IChatWebPageHub>> _mockHubClients;
        private Mock<IChatWebPageHub> _mockTwitchHub;

        [SetUp]
        public void Setup()
        {
            // setup signalr mocks
            _mockTwitchHub = new Mock<IChatWebPageHub>();
            _mockHubClients = new Mock<IHubClients<IChatWebPageHub>>();
            _mockHubClients.Setup(x => x.All).Returns(_mockTwitchHub.Object);
            _mockHub = new Mock<IHubContext<ChatWebPageHub, IChatWebPageHub>>();
            _mockHub.Setup(x => x.Clients).Returns(_mockHubClients.Object);

            _handler = new UserHasArrivedHandler(MockBucketProvider.Object, _mockHub.Object);
        }

        [Test]
        public async Task Create_recent_activity_marker_document_if_necessary()
        {
            // arrange
            var username = "someusername";
            MockCollection.Setup(x => x.ExistsAsync($"{username}::arrived_recently", null))
                .ReturnsAsync(new FakeExistsResult(false));
            var twitchLibMessage = TwitchLibMessageBuilder.Create().WithUsername(username).Build();
            var chatMessage = ChatMessageBuilder.Create().WithTwitchLibMessage(twitchLibMessage).Build();
            var request = new UserHasArrived(chatMessage);
            MockCollection.Setup(x => x.GetAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new FakeGetResult(new TwitcherProfile()));
            _mockTwitchHub.Setup(x => x.ReceiveFanfare(It.IsAny<FanfareInfo>()));

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockCollection.Verify(x => x.UpsertAsync(
                It.Is<string>(k => k == $"{username}::arrived_recently"),
                It.IsAny<UserHasArrivedMarker>(),
                It.IsAny<UpsertOptions>()), Times.Once);
            MockCollection.Verify(x => x.UpsertAsync(
                    It.IsAny<string>(),
                    It.IsAny<UserHasArrivedMarker>(),
                    It.Is<UpsertOptions>(u =>
                        (u.GetInternalPropertyValue<TimeSpan, UpsertOptions>("ExpiryValue")).Hours == 12)),
                Times.Once);
        }

        [TestCase(false, 0)]
        [TestCase(true, 1)]
        public async Task Send_farfare_notice_when_user_has_fanfare_and_is_arriving(bool hasFanfare, int timesSent)
        {
            // arrange
            var username = "someusername";
            var expectedFanfare = new FanfareInfo
            {
                Message = "message " + Guid.NewGuid(),
                Timeout = 12312,
                YouTubeCode = "ytcode" + Guid.NewGuid(),
                YouTubeStartTime = 111,
                YouTubeEndTime = 222
            };
            var twitchLibMessage = TwitchLibMessageBuilder.Create().WithUsername(username).Build();
            var chatMessage = ChatMessageBuilder.Create().WithTwitchLibMessage(twitchLibMessage).Build();
            var request = new UserHasArrived(chatMessage);
            MockCollection.Setup(x => x.ExistsAsync($"{username}::arrived_recently",null))
                .ReturnsAsync(new FakeExistsResult(false));
            MockCollection.Setup(x => x.GetAsync(It.IsAny<string>(),null))
                .ReturnsAsync(new FakeGetResult(new TwitcherProfile { HasFanfare = hasFanfare, Fanfare = expectedFanfare}));
        
            // act
            await _handler.Handle(request, CancellationToken.None);
        
            // assert
            _mockTwitchHub.Verify(x => x.ReceiveFanfare(expectedFanfare), Times.Exactly(timesSent));
        }
        
        [Test]
        public async Task No_fanfare_if_there_is_no_user_profile()
        {
            // arrange
            var username = "gamlor";
            var twitchLibMessage = TwitchLibMessageBuilder.Create().WithUsername(username).Build();
            var chatMessage = ChatMessageBuilder.Create().WithTwitchLibMessage(twitchLibMessage).Build();
            var request = new UserHasArrived(chatMessage);
            // setup: user has NOT arrive recently
            MockCollection.Setup(m => m.ExistsAsync($"{username}::arrived_recently", null))
                .ReturnsAsync(new FakeExistsResult(false));
            // setup: don't care about the arrive_recently document being added
            MockCollection.Setup(m => m.UpsertAsync(It.IsAny<string>(), It.IsAny<UserHasArrivedMarker>(), null));
            // setup: user does NOT have a profile
            MockCollection.Setup(m => m.GetAsync(username.ToLower(), null))
                .ReturnsAsync(new FakeGetResult(null));
        
            // act
            await _handler.Handle(request, CancellationToken.None);
        
            // assert
            _mockTwitchHub.Verify(m => m.ReceiveFanfare(It.IsAny<FanfareInfo>()), Times.Never);
        }
    }
}