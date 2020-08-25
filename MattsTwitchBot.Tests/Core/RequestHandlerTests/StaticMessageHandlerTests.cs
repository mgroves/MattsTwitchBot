using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.StaticCommands;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class StaticMessageHandlerTests : UnitTest
    {
        private StaticMessageHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new StaticMessageHandler(MockBucketProvider.Object, MockTwitchClient.Object);
        }

        [Test]
        public async Task Gets_content_and_says_it_to_chat_room()
        {
            // arrange
            var channel = "somechannel" + Guid.NewGuid();
            var commandName = "!somemessage";
            var expectedContent = "blah blah blah whatever " + Guid.NewGuid();
            var content = new ValidStaticCommands
            {
                Commands = new List<StaticCommandInfo>
                {
                    new StaticCommandInfo
                    {
                        Command = commandName,
                        Content = expectedContent
                    }
                }
            };
            MockCollection.Setup(x => x.GetAsync("staticContentCommands", null))
                .ReturnsAsync(new FakeGetResult(content));
            var request = new StaticMessage(commandName, channel);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            // _twitchClient.SendMessage(request.Channel, command.Content);
            MockTwitchClient.Verify(x => x.SendMessage(channel,expectedContent, false), Times.Once);
        }
    }
}