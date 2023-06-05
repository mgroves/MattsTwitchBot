﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Query;
using MattsTwitchBot.Core;
using MattsTwitchBot.Tests.IntegrationTests.TestHelpers;
using MediatR;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.IntegrationTests.HostedServiceTests
{
    [TestFixture]
    public class MattsChatBotHostedServiceTests : UsesDatabase
    {
        private MattsChatBotHostedService _service;
        private IMediator _mediator;
        private FakeTwitchClient _fakeTwitchClient;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            _fakeTwitchClient = new FakeTwitchClient();
            TestConfiguration.FakeValues["Twitch"] = "fake twitch";
            _mediator = TestMediator.Build(twitchClient: _fakeTwitchClient, bucketProvider: BucketProvider, keyGenerator: TestKeyGen, configuration: TestConfiguration);
            _service = new MattsChatBotHostedService(_mediator, _fakeTwitchClient, new TwitchCommandRequestFactory(_mediator));
        }

        [Test]
        public async Task Regular_comment_on_twitch_should_be_saved_to_database()
        {
            // arrange
            await _service.StartAsync(CancellationToken.None);
            var messageText = "hello just checking in " + Guid.NewGuid();
            var username = "user" + Guid.NewGuid();
            var arrivedKey = $"{username}::arrived_recently";
            DocumentsToRemove.Add(arrivedKey); // clean up the "arrived" document

            // act
            _fakeTwitchClient.FakeRaiseMessage(messageText, username: username);

            // assert - message was saved
            var findMessage = TestCluster.QueryAsync<dynamic>(
                $"SELECT m.message FROM `{Bucket.Name}`._default.messages m WHERE m.message = $message",
                QueryOptions.Create().Parameter("message", messageText).ScanConsistency(QueryScanConsistency.RequestPlus));
            var rows = await findMessage.Result.Rows.ToListAsync();
            Assert.That(rows.Count, Is.EqualTo(1));

            // assert - user arrived doc was saved
            var userArrivedExists = await Collection.ExistsAsync(arrivedKey);
            Assert.That(userArrivedExists.Exists, Is.True);
        }
    }
}