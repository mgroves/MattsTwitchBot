using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.OneOffs;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class SetCurrentProjectHandlerTests : UnitTest
    {
        private SetCurrentProjectHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new SetCurrentProjectHandler(MockTwitchClient.Object, MockBucketProvider.Object, MockTwitchOptions.Object);
            MockTwitchOptions.Setup(x => x.Value).Returns(new TwitchOptions
            {
                Username = "someusername"
            });
        }

        [Test]
        public async Task Only_the_bot_user_itself_can_set_the_current_project()
        {
            // arrange
            var notTheBotUser = MockTwitchOptions.Object.Value.Username + Guid.NewGuid();
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
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            MockCollection.Verify(x => x.UpsertAsync(It.IsAny<string>(), It.IsAny<CurrentProjectInfo>(), null),
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
                .WithUsername(MockTwitchOptions.Object.Value.Username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage(message)
                .Build();
            var request = new SetCurrentProject(chatMessage);
        
            // act
            await _handler.Handle(request, CancellationToken.None);
        
            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), "Sorry, I couldn't understand that URL!", false), Times.Once);
        }
        
        [Test]
        public async Task Says_an_error_message_if_unable_to_store()
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(MockTwitchOptions.Object.Value.Username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!setcurrentproject http://validurl.com")
                .Build();
            var request = new SetCurrentProject(chatMessage);
            MockCollection.Setup(x => x.UpsertAsync(It.IsAny<string>(), It.IsAny<CurrentProjectInfo>(), null))
                .Throws<Exception>();
        
            // act
            await _handler.Handle(request, CancellationToken.None);
        
            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), "I was unable to store that, sorry!", false), Times.Once);
        }
        
        [Test]
        public async Task Says_okay_if_able_to_store()
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(MockTwitchOptions.Object.Value.Username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!setcurrentproject http://validurl.com")
                .Build();
            var request = new SetCurrentProject(chatMessage);
            MockCollection.Setup(x => x.UpsertAsync(It.IsAny<string>(), It.IsAny<CurrentProjectInfo>(), null))
                .ReturnsAsync(new FakeMutationResult());
        
            // act
            await _handler.Handle(request, CancellationToken.None);
        
            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), "Okay, got it!", false), Times.Once);
        }
    }
}