using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Query;
using MattsTwitchBot.Core.RequestHandlers.Chat;
using MattsTwitchBot.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.IntegrationTests.RequestHandlers.ChatTests
{
    [TestFixture]
    [Category("SkipWhenLiveUnitTesting")]

    public class StoreMessageHandlerTests : UsesDatabase
    {
        private StoreMessageHandler _handler;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            _handler = new StoreMessageHandler(BucketProvider, TestKeyGen);
        }

        [Test]
        public async Task Is_chat_message_stored()
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
            var n1ql = $"SELECT RAW COUNT(*) FROM `{Bucket.Name}` WHERE message = $expectedMessage";
            var result = await TestCluster.QueryAsync<int>(n1ql,
                QueryOptions.Create().Parameter("expectedMessage", expectedMessage).ScanConsistency(QueryScanConsistency.RequestPlus));
            var count = await result.Rows.FirstOrDefaultAsync();
            Assert.That(count, Is.EqualTo(1));
        }
    }
}