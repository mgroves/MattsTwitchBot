using System.Collections.Generic;
using System.Linq;
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

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class StaticCommandsLookupHandlerTests
    {
        private StaticCommandsLookupHandler _handler;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);

            _handler = new StaticCommandsLookupHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task Return_empty_list_if_there_are_no_commands_stored_in_database()
        {
            // arrange
            var request = new StaticCommandsLookup();
            _mockBucket.Setup(x => x.GetAsync<ValidStaticCommands>("staticContentCommands"))
                .ReturnsAsync(new FakeOperationResult<ValidStaticCommands> {Success = false});

            // act
            var result = await _handler.Handle(request, CancellationToken.None);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Commands == null || !result.Commands.Any(), Is.True);
        }

        [Test]
        public async Task Return_all_the_content_that_exists()
        {
            // arrange
            var request = new StaticCommandsLookup();
            var commands = new ValidStaticCommands
            {
                Commands = new List<StaticCommandInfo>
                {
                    new StaticCommandInfo {Command = "foo", Content = "bar"},
                    new StaticCommandInfo {Command = "foo2", Content = "bar2"},
                }
            };
            _mockBucket.Setup(x => x.GetAsync<ValidStaticCommands>("staticContentCommands"))
                .ReturnsAsync(new FakeOperationResult<ValidStaticCommands> { Success = true, Value = commands});

            // act
            var result = await _handler.Handle(request, CancellationToken.None);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Commands, Is.Not.Null);
            Assert.That(result.Commands.Any(), Is.True);
            Assert.That(result.Commands.Count, Is.EqualTo(commands.Commands.Count));
        }
    }
}