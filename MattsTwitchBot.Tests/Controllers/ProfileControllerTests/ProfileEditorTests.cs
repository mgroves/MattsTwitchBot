using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MattsTwitchBot.Web.Controllers;
using MediatR;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Controllers.ProfileControllerTests
{
    [TestFixture]
    public class ProfileEditorTests
    {
        private ProfileController _controller;
        private Mock<IMediator> _mockMediator;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new ProfileController(_mockMediator.Object);

            _mockMediator.Setup(m => m.Send(It.IsAny<GetProfile>(), CancellationToken.None))
                .ReturnsAsync(new TwitcherProfile());
        }

        [Test]
        public async Task Controller_will_send_CreateProfileIfNotExists_request()
        {
            // arrange
            var username = "CopperBeardy";

            // act
            await _controller.ProfileEditor(username);

            // assert
            _mockMediator.Verify(x =>
                x.Send(It.Is<CreateProfileIfNotExists>(y => y.TwitchUsername == username),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}