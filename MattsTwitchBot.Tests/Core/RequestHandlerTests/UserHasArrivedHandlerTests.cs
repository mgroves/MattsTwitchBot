using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using MattsTwitchBot.Tests.Fakes;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class UserHasArrivedHandlerTests
    {
        private UserHasArrivedHandler _handler;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;
        private Mock<IHubContext<ChatWebPageHub, IChatWebPageHub>> _mockHub;
        private Mock<IHubClients<IChatWebPageHub>> _mockHubClients;
        private Mock<IChatWebPageHub> _mockTwitchHub;

        [SetUp]
        public void Setup()
        {
            // setup database mocks
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);

            // setup signalr mocks
            _mockTwitchHub = new Mock<IChatWebPageHub>();
            _mockHubClients = new Mock<IHubClients<IChatWebPageHub>>();
            _mockHubClients.Setup(x => x.All).Returns(_mockTwitchHub.Object);
            _mockHub = new Mock<IHubContext<ChatWebPageHub, IChatWebPageHub>>();
            _mockHub.Setup(x => x.Clients).Returns(_mockHubClients.Object);

            _handler = new UserHasArrivedHandler(_mockBucketProvider.Object, _mockHub.Object);
        }

        [TestCase(false, 1)]        // if marker document doesn't exist, create it
        [TestCase(true, 0)]         // if marker document does exist, don't create/update it
        public async Task Create_recent_activity_marker_document_if_necessary(bool documentAlreadyExists, int timesCalled)
        {
            // arrange
            var username = "someusername";
            _mockBucket.Setup(x => x.ExistsAsync($"{username}::arrived_recently"))
                .ReturnsAsync(documentAlreadyExists);
            var twitchLibMessage = TwitchLibMessageBuilder.Create().WithUsername(username).Build();
            var chatMessage = ChatMessageBuilder.Create().WithTwitchLibMessage(twitchLibMessage).Build();
            var request = new UserHasArrived(chatMessage);
            _mockBucket.Setup(x => x.GetAsync<TwitcherProfile>(It.IsAny<string>()))
                .ReturnsAsync(new FakeOperationResult<TwitcherProfile> { Value = new TwitcherProfile() });
            _mockTwitchHub.Setup(x => x.ReceiveFanfare(It.IsAny<string>()));

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockBucket.Verify(x => x.UpsertAsync(

                It.Is<Document<dynamic>>(x =>
                        x.Id == $"{username}::arrived_recently"
                        && x.Expiry == 12 * 60 * 60 * 1000 // 12 hours
                )

            ), Times.Exactly(timesCalled));
        }

        [TestCase(false, 0)]
        [TestCase(true, 1)]
        public async Task Send_farfare_notice_when_user_has_fanfare_and_is_arriving(bool hasFanfare, int timesSent)
        {
            // arrange
            var username = "someusername";
            var twitchLibMessage = TwitchLibMessageBuilder.Create().WithUsername(username).Build();
            var chatMessage = ChatMessageBuilder.Create().WithTwitchLibMessage(twitchLibMessage).Build();
            var request = new UserHasArrived(chatMessage);
            _mockBucket.Setup(x => x.ExistsAsync($"{username}::arrived_recently"))
                .ReturnsAsync(false);
            _mockBucket.Setup(x => x.GetAsync<TwitcherProfile>(It.IsAny<string>()))
                .ReturnsAsync(new FakeOperationResult<TwitcherProfile> { Value = new TwitcherProfile { HasFanfare = hasFanfare} });

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchHub.Verify(x => x.ReceiveFanfare(username), Times.Exactly(timesSent));
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
            _mockBucket.Setup(m => m.ExistsAsync($"{username}::arrived_recently"))
                .ReturnsAsync(false);
            // setup: don't care about the arrive_recently document being added
            _mockBucket.Setup(m => m.UpsertAsync(It.IsAny<Document<dynamic>>()));
            // setup: user does NOT have a profile
            _mockBucket.Setup(m => m.GetAsync<TwitcherProfile>(username.ToLower()))
                .ReturnsAsync(new FakeOperationResult<TwitcherProfile> {Value = null});

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchHub.Verify(m => m.ReceiveFanfare(It.IsAny<string>()), Times.Never);
        }
    }
}