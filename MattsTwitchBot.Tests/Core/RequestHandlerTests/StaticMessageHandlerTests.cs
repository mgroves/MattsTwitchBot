using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class StaticMessageHandlerTests
    {
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<ITwitchClient> _mockTwitchClient;
        private Mock<IBucket> _mockBucket;
        private StaticMessageHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);
            _mockTwitchClient = new Mock<ITwitchClient>();
            _handler = new StaticMessageHandler(_mockBucketProvider.Object, _mockTwitchClient.Object);
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
            _mockBucket.Setup(x => x.GetAsync<ValidStaticCommands>("staticContentCommands"))
                .ReturnsAsync(new FakeOperationResult<ValidStaticCommands> { Value = content});
            var request = new StaticMessage(commandName, channel);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            // _twitchClient.SendMessage(request.Channel, command.Content);
            _mockTwitchClient.Verify(x => x.SendMessage(channel,expectedContent, false), Times.Once);
        }
    }
}