using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.RequestHandlerTests
{
    [TestFixture]
    public class SoundEffectHandlerTests
    {
        private SoundEffectHandler _handler;
        private Mock<IHubContext<TwitchHub>> _mockHub;
        private Mock<IHubClients> _mockHubClients;
        private Mock<IClientProxy> _mockClientProxy;

        [SetUp]
        public void Setup()
        {
            _mockClientProxy = new Mock<IClientProxy>();
            _mockHubClients = new Mock<IHubClients>();
            _mockHubClients.Setup(x => x.All).Returns(_mockClientProxy.Object);
            _mockHub = new Mock<IHubContext<TwitchHub>>();
            _mockHub.Setup(x => x.Clients).Returns(_mockHubClients.Object);
            _handler = new SoundEffectHandler(_mockHub.Object);
        }

        [Test]
        public async Task If_a_sound_effect_name_isnt_valid_dont_call_signalr()
        {
            // arrange
            var invalidSoundEffectName = "sound-effect-" + Guid.NewGuid();
            var request = new SoundEffect(invalidSoundEffectName);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockHub.Verify(x => x.Clients, Times.Never);
        }

        // https://buildingsteps.wordpress.com/2018/06/12/testing-signalr-hubs-in-asp-net-core-2-1/
        [Test]
        public async Task Given_valid_sound_effect_tell_signalr()
        {
            // arrange
            var validSoundEffectName = "laugh";
            var request = new SoundEffect(validSoundEffectName);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockHub.Verify(x => x.Clients, Times.Once);
            _mockClientProxy.Verify(x => x.SendCoreAsync("SoundEffect", 
                It.Is<object[]>(o => o != null && o.Any(p => (string)p == validSoundEffectName)),
                CancellationToken.None),
                Times.Once);
        }
    }
}