using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using MediatR;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class HelpHandlerTests
    {
        private HelpHandler _handler;
        private Mock<ITwitchClient> _mockTwitchClient;
        private Mock<IMediator> _mockMediator;
        private ValidStaticCommands _fakeStaticCommands;

        [SetUp]
        public void Setup()
        {
            _mockTwitchClient = new Mock<ITwitchClient>();
            _mockMediator = new Mock<IMediator>();
            _handler = new HelpHandler(_mockTwitchClient.Object, _mockMediator.Object);

            //_mediator.Send<ValidStaticCommands>(new StaticCommandsLookup());
            _fakeStaticCommands = new ValidStaticCommands
            {
                Commands = new List<StaticCommandInfo>
                {
                    new StaticCommandInfo {Command = "!foo", Content = "foo content " + Guid.NewGuid()},
                    new StaticCommandInfo {Command = "!bar", Content = "bar content " + Guid.NewGuid()},
                    new StaticCommandInfo {Command = "!baz", Content = "foo content " + Guid.NewGuid()},
                }
            };
            _mockMediator.Setup(m => 
                m.Send<ValidStaticCommands>(It.IsAny<StaticCommandsLookup>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(_fakeStaticCommands);
        }

        [Test]
        public async Task Help_command_will_say_help_to_the_channel()
        {
            // arrange
            var expectedChannel = "whateverchannel";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithChannel(expectedChannel)
                .WithMessage("!help")
                .Build();
            var request = new Help(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(expectedChannel,
                It.Is<string>(s => s.StartsWith("Try these commands:")), false), Times.Once);
        }

        [TestCase("!help laugh", "!laugh causes")]
        [TestCase("!help so", "!so <username> will")]
        [TestCase("!help rimshot", "!rimshot causes a rimshot")]
        [TestCase("!help lurk", "!lurk just to let me know")]
        public async Task Help_will_provide_details_on_specific_commands(string messageText, string startsWith)
        {
            // arrange
            var expectedChannel = "whateverchannel";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithChannel(expectedChannel)
                .WithMessage(messageText)
                .Build();
            var request = new Help(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(expectedChannel,
                It.Is<string>(s => s.StartsWith(startsWith)), false), Times.Once);
        }

        [Test]
        public async Task Help_with_invalid_arguments_will_just_say_generic_help()
        {
            // arrange
            var expectedChannel = "whateverchannel";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithChannel(expectedChannel)
                .WithMessage("!help somethingThatIsntACommand")
                .Build();
            var request = new Help(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(expectedChannel,
                It.Is<string>(s => s.StartsWith("Try these commands:")), false), Times.Once);
        }
    }
}