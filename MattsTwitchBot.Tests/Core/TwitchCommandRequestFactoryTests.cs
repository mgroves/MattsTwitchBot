using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core
{
    [TestFixture]
    public class TwitchCommandRequestFactoryTests
    {
        private TwitchCommandRequestFactory _factory;
        private Mock<IMediator> _mockMediator;
        private TwitchLibMessage _twitchLibMessage;

        [SetUp]
        public void Setup()
        {
            _twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            _mockMediator = new Mock<IMediator>();
            _factory = new TwitchCommandRequestFactory(_mockMediator.Object);

            // setup a soundeffect to be a valid sound effect
            _mockMediator.Setup(x => x.Send<ValidSoundEffects>(It.IsAny<SoundEffectLookup>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidSoundEffects { SoundEffects = new List<SoundEffectInfo> { new SoundEffectInfo { SoundEffectName = "!soundeffect" } }});
        }

        [TestCase("!help",typeof(Help))]
        [TestCase("!currentproject", typeof(SayCurrentProject))]
        [TestCase("!currentproject the rest of this is ignored", typeof(SayCurrentProject))]
        [TestCase("!setcurrentproject", typeof(SetCurrentProject))]
        [TestCase("!setcurrentproject etc etc etc", typeof(SetCurrentProject))]
        [TestCase("!so somebody", typeof(ShoutOut))]
        [TestCase("!so", typeof(StoreMessage))] // !so followed by nothing should just store the message
        [TestCase("!profile", typeof(ModifyProfile))]
        [TestCase("anything else", typeof(StoreMessage))]
        [TestCase("!soundeffect", typeof(SoundEffect))]
        public async Task Commands_returns_the_correct_requests(string command, Type type)
        {
            // arrange
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(_twitchLibMessage)
                .WithMessage(command)
                .Build();

            // act
            var result = await _factory.BuildCommand(chatMessage);

            // assert
            Assert.That(result, Is.TypeOf(type));
        }
    }
}