using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using Moq;
using NUnit.Framework;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
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

        [Test]
        public async Task If_user_doesnt_exist_trouting_say_error_message()
        {
            // arrange
            var userThatDoesntExist = "narendevseviltwin";
            var expectedMessage = $"User {userThatDoesntExist} doesn't exist.";
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

        [Test]
        public async Task If_user_exists_then_perform_trout_action_with_target_username()
        {
            // arrange
            var userThatDoesExist = "narendev";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesn't matter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage($"!trout {userThatDoesExist}")
                .Build();
            _mockTwitchWrapper.Setup(m => m.DoesUserExist(userThatDoesExist))
                .ReturnsAsync(true);

            var troutRequest = new Trout(chatMessage);

            // act
            await _handler.Handle(troutRequest, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x =>
                x.SendMessage(It.IsAny<string>(), $"/me slaps @{userThatDoesExist} around a bit with a large trout.",It.IsAny<bool>())
                ,Times.Once);
        }
    }
}