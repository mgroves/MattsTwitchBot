using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.RequestHandlerTests
{
    [TestFixture]
    public class HelpHandlerTests
    {
        private HelpHandler _handler;
        private Mock<ITwitchClient> _mockTwitchClient;

        [SetUp]
        public void Setup()
        {
            _mockTwitchClient = new Mock<ITwitchClient>();
            _handler = new HelpHandler(_mockTwitchClient.Object);
        }

        [Test]
        public async Task Help_command_will_whisper_help_to_the_user_who_requested_it()
        {
            // arrange
            var expectedUsername = "ineedhelp";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(expectedUsername)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!help")
                .Build();
            var request = new Help(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendWhisper(expectedUsername, It.IsAny<string>(), false), Times.Once);
        }
    }
}