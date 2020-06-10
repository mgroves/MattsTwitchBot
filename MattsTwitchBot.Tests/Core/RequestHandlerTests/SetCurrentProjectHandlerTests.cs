using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.RequestHandlers.OneOffs;
using MattsTwitchBot.Tests.Fakes;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class SetCurrentProjectHandlerTests
    {
        private SetCurrentProjectHandler _handler;
        private Mock<ITwitchClient> _mockTwitchClient;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;
        private Mock<IOptions<TwitchOptions>> _mockOptions;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);
            _mockTwitchClient = new Mock<ITwitchClient>();
            _mockOptions = new Mock<IOptions<TwitchOptions>>();
            _handler = new SetCurrentProjectHandler(_mockTwitchClient.Object, _mockBucketProvider.Object, _mockOptions.Object);
            _mockOptions.Setup(x => x.Value).Returns(new TwitchOptions
            {
                Username = "someusername"
            });
        }

        [Test]
        public async Task Only_the_bot_user_itself_can_set_the_current_project()
        {
            // arrange
            var notTheBotUser = _mockOptions.Object.Value.Username + Guid.NewGuid();
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(notTheBotUser)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("doesntmatter")
                .Build();
                
            var request = new SetCurrentProject(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert - twitch client never used, bucket never used
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            _mockBucket.Verify(x => x.Upsert(It.IsAny<Document<CurrentProjectInfo>>()),
                Times.Never);
        }

        [TestCase("!setcurrentproject")]
        [TestCase("!setcurrentproject ")]
        [TestCase("!setcurrentproject invalid url")]
        [TestCase("!setcurrentproject invalid_url")]
        public async Task Valid_url_must_be_passed_in(string message)
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(_mockOptions.Object.Value.Username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage(message)
                .Build();
            var request = new SetCurrentProject(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), "Sorry, I couldn't understand that URL!", false), Times.Once);
        }

        [Test]
        public async Task Says_an_error_message_if_unable_to_store()
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(_mockOptions.Object.Value.Username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!setcurrentproject http://validurl.com")
                .Build();
            var request = new SetCurrentProject(chatMessage);
            _mockBucket.Setup(x => x.UpsertAsync(It.IsAny<string>(), It.IsAny<CurrentProjectInfo>()))
                .ReturnsAsync(new FakeOperationResult<CurrentProjectInfo>
                {
                    Success = false
                });

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), "I was unable to store that, sorry!", false), Times.Once);
        }
        
        [Test]
        public async Task Says_okay_if_able_to_store()
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(_mockOptions.Object.Value.Username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!setcurrentproject http://validurl.com")
                .Build();
            var request = new SetCurrentProject(chatMessage);
            _mockBucket.Setup(x => x.UpsertAsync(It.IsAny<string>(), It.IsAny<CurrentProjectInfo>()))
                .ReturnsAsync(new FakeOperationResult<CurrentProjectInfo>
                {
                    Success = true
                });

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), "Okay, got it!", false), Times.Once);
        }
    }
}