using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.RequestHandlerTests
{
    [TestFixture]
    public class StoreMessageHandlerTests
    {
        private StoreMessageHandler _handler;
        private Mock<ITwitchBucketProvider> _mockTwitchBucketProvider;
        private Mock<IBucket> _mockBucket;

        [SetUp]
        public void Setup()
        {
            _mockTwitchBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucket = new Mock<IBucket>();
            _mockTwitchBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);
            _handler = new StoreMessageHandler(_mockTwitchBucketProvider.Object);
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
            _mockBucket.Verify(x => x.InsertAsync(It.Is<Document<ChatMessage>>(d => d.Id != Guid.Empty.ToString())), Times.Once());
            _mockBucket.Verify(x => x.InsertAsync(It.Is<Document<ChatMessage>>(d => d.Content.Username == expectedUsername)), Times.Once());
            _mockBucket.Verify(x => x.InsertAsync(It.Is<Document<ChatMessage>>(d => d.Content.Message == expectedMessage)), Times.Once());
            _mockBucket.Verify(x => x.InsertAsync(It.Is<Document<ChatMessage>>(d => d.Content.Channel == expectedChannel)), Times.Once());
        }
    }
}