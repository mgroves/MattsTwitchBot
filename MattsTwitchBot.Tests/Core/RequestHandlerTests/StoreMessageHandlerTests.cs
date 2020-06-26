using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.RequestHandlers.Chat;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class StoreMessageHandlerTests : UnitTest
    {
        private StoreMessageHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new StoreMessageHandler(MockBucketProvider.Object, new KeyGenerator());
        }

        [Test]
        public async Task Should_insert_message_into_bucket()
        {
            // arrange
            var expectedUsername = "doesntmatter" + Guid.NewGuid();
            var expectedMessage = "some message whatever " + Guid.NewGuid();
            var expectedChannel = "mychannel" + Guid.NewGuid();
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(expectedUsername)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage(expectedMessage)
                .WithChannel(expectedChannel)
                .Build();
            var request = new StoreMessage(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockCollection.Verify(x => x.InsertAsync(It.IsAny<string>(), It.IsAny<ChatMessage>(), null), Times.Once());
            MockCollection.Verify(x => x.InsertAsync(It.IsAny<string>(), It.Is<ChatMessage>(d => d.Username == expectedUsername), null), Times.Once());
            MockCollection.Verify(x => x.InsertAsync(It.IsAny<string>(), It.Is<ChatMessage>(d => d.Message == expectedMessage), null), Times.Once());
            MockCollection.Verify(x => x.InsertAsync(It.IsAny<string>(), It.Is<ChatMessage>(d => d.Channel == expectedChannel), null), Times.Once());
        }
    }
}