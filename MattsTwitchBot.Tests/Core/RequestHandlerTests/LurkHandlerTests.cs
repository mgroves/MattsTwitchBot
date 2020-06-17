using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.RequestHandlers.OneOffs;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class LurkHandlerTests : UnitTest
    {
        private LurkHandler _handler;
        private Mock<ITwitchClient> _mockTwitchClient;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _mockTwitchClient = new Mock<ITwitchClient>();
            _handler = new LurkHandler(_mockTwitchClient.Object);
        }

        [Test]
        public async Task A_random_lurk_message_is_sent_with_the_users_name()
        {
            // arrange
            var expectedUsername = "whatevername" + Guid.NewGuid();
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(expectedUsername)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!help")
                .Build();
            var req = new Lurk(chatMessage);

            // act
            await _handler.Handle(req, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(m => m.SendMessage(It.IsAny<string>(),
                It.Is<string>(s => s.Contains(expectedUsername)), false),Times.Once);
        }
    }
}