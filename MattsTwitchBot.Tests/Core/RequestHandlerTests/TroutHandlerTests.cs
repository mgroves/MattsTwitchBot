using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.RequestHandlers.OneOffs;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class TroutHandlerTests
    {
        private TroutHandler _handler;
        private Mock<ITwitchClient> _mockTwitchClient;
        private Mock<ITwitchApiWrapper> _mockTwitchWrapper;

        [SetUp]
        public void Setup()
        {
            _mockTwitchClient = new Mock<ITwitchClient>();
            _mockTwitchWrapper = new Mock<ITwitchApiWrapper>();
            _handler = new TroutHandler(_mockTwitchClient.Object, _mockTwitchWrapper.Object);
        }

        [TestCase("!trout")]
        [TestCase("!trout ")]
        public async Task If_user_isnt_specified_with_command_say_error_message(string messageWithNoUser)
        {
            // arrange
            var expectedMessage = $"You must specify a user to trout.";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesn't matter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage(messageWithNoUser)
                .Build();

            var troutRequest = new Trout(chatMessage);

            // act
            await _handler.Handle(troutRequest, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(m =>
                m.SendMessage(It.IsAny<string>(), expectedMessage, false), Times.Once);
        }

        [TestCase("narendevseviltwin")]
        [TestCase("@narendevseviltwin")]
        public async Task If_user_doesnt_exist_trouting_say_error_message(string userThatDoesntExist)
        {
            // arrange
            var expectedMessage = $"User {userThatDoesntExist.Replace("@", "")} doesn't exist.";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesn't matter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage($"!trout {userThatDoesntExist}")
                .Build();
            _mockTwitchWrapper.Setup(m => m.DoesUserExist(userThatDoesntExist))
                .ReturnsAsync(false);

            var troutRequest = new Trout(chatMessage);

            // act
            await _handler.Handle(troutRequest, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(m => 
                m.SendMessage(It.IsAny<string>(), expectedMessage, false), Times.Once);
        }

        [TestCase("narendev")]
        [TestCase("@narendev")]
        public async Task If_user_exists_then_perform_trout_action_with_target_username(string userThatDoesExist)
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesn't matter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage($"!trout {userThatDoesExist}")
                .Build();
            _mockTwitchWrapper.Setup(m => m.DoesUserExist(It.IsAny<string>()))
                .ReturnsAsync(true);

            var troutRequest = new Trout(chatMessage);

            // act
            await _handler.Handle(troutRequest, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x =>
                x.SendMessage(It.IsAny<string>(), $"/me slaps @{userThatDoesExist.Replace("@","")} around a bit with a large trout.",It.IsAny<bool>())
                ,Times.Once);
        }
    }
}