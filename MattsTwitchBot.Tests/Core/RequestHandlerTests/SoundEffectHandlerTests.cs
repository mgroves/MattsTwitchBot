using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.RequestHandlers.Main;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class SoundEffectHandlerTests : UnitTest
    {
        private SoundEffectHandler _handler;
        private Mock<IHubContext<ChatWebPageHub, IChatWebPageHub>> _mockHub;
        private Mock<IHubClients<IChatWebPageHub>> _mockHubClients;
        private Mock<IChatWebPageHub> _mockTwitchHub;

        [SetUp]
        public void Setup()
        {
            _mockTwitchHub = new Mock<IChatWebPageHub>();
            _mockHubClients = new Mock<IHubClients<IChatWebPageHub>>();
            _mockHubClients.Setup(x => x.All).Returns(_mockTwitchHub.Object);
            _mockHub = new Mock<IHubContext<ChatWebPageHub, IChatWebPageHub>>();
            _mockHub.Setup(x => x.Clients).Returns(_mockHubClients.Object);
            _handler = new SoundEffectHandler(_mockHub.Object);
        }

        [Test]
        public async Task Given_valid_sound_effect_tell_signalr()
        {
            // arrange
            var validSoundEffectName = "laugh";
            var request = new SoundEffect(validSoundEffectName);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchHub.Verify(x => x.ReceiveSoundEffect(validSoundEffectName), Times.Once());
        }
    }
}