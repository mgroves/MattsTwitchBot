using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Main;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class SoundEffectLookupHandlerTests : UnitTest
    {
        private SoundEffectLookupHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new SoundEffectLookupHandler(MockBucketProvider.Object);
        }

        [Test]
        public async Task Returns_no_valid_sound_effects_if_validSoundEffects_data_doesnt_exist()
        {
            // arrange
            var request = new SoundEffectLookup();
            MockCollection.Setup(x => x.GetAsync("validSoundEffects", null))
                .Throws<Exception>();

            // act
            var result = await _handler.Handle(request, CancellationToken.None);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SoundEffects == null || !result.SoundEffects.Any() , Is.True);
        }

        [Test]
        public async Task Returns_valid_sound_effects()
        {
            // arrange
            var validSoundEffects = new ValidSoundEffects
            {
                SoundEffects = new List<SoundEffectInfo>
                {
                    new SoundEffectInfo {SoundEffectName = "woof" + Guid.NewGuid()},
                    new SoundEffectInfo {SoundEffectName = "meow" + Guid.NewGuid()},
                    new SoundEffectInfo {SoundEffectName = "carcrash"  + Guid.NewGuid()}
                }
            };
            var request = new SoundEffectLookup();
            MockCollection.Setup(x => x.GetAsync("validSoundEffects", null))
                .ReturnsAsync(new FakeGetResult(validSoundEffects));
        
            // act
            var result = await _handler.Handle(request, CancellationToken.None);
        
            // assert
            Assert.That(result.SoundEffects, Is.Not.Null);
            Assert.That(result.SoundEffects, Is.Not.Empty);
            Assert.That(result.SoundEffects.Count, Is.EqualTo(validSoundEffects.SoundEffects.Count));
            foreach (var effect in validSoundEffects.SoundEffects)
                Assert.That(result.SoundEffects.Any(x => x.SoundEffectName == effect.SoundEffectName), Is.True);
        }
    }
}