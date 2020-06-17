using System;
using System.Collections.Generic;
using System.Linq;
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
    public class StaticCommandsLookupHandlerTests : UnitTest
    {
        private StaticCommandsLookupHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _handler = new StaticCommandsLookupHandler(MockBucketProvider.Object);
        }

        [Test]
        public async Task Return_empty_list_if_there_are_no_commands_stored_in_database()
        {
            // arrange
            var request = new StaticCommandsLookup();
            MockCollection.Setup(x => x.GetAsync("staticContentCommands", null))
                .Throws<Exception>();

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
            MockCollection.Setup(x => x.GetAsync("staticContentCommands", null))
                .ReturnsAsync(new FakeGetResult(commands));
        
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