using System;
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
    public class SoundEffectLookupHandlerTests
    {
        private SoundEffectLookupHandler _handler;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);
            _handler = new SoundEffectLookupHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task Returns_no_valid_sound_effects_if_validSoundEffects_data_doesnt_exist()
        {
            // arrange
            var request = new SoundEffectLookup();
            _mockBucket.Setup(x => x.GetAsync<ValidSoundEffects>("validSoundEffects"))
                .ReturnsAsync(new FakeOperationResult<ValidSoundEffects> {Success = false});

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
            _mockBucket.Setup(x => x.GetAsync<ValidSoundEffects>("validSoundEffects"))
                .ReturnsAsync(new FakeOperationResult<ValidSoundEffects> { Success = true, Value = validSoundEffects });

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