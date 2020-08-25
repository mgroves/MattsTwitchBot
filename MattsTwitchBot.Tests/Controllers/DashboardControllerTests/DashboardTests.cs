using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Dashboard;
using MattsTwitchBot.Web.Controllers;
using MattsTwitchBot.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Controllers.DashboardControllerTests
{
    [TestFixture]
    public class DashboardTests
    {
        private Mock<IMediator> _mockMediator;
        private DashboardController _controller;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new DashboardController(_mockMediator.Object);
        }

        [Test]
        public async Task It_should_call_mediator_with_a_GetDashboardView_request()
        {
            // arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDashboardView>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DashboardView());

            // act
            await _controller.Dashboard();

            // assert
            _mockMediator.Verify(m => m.Send(It.IsAny<GetDashboardView>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task It_should_return_a_view_with_a_DashboardViewModel_object()
        {
            // arrange
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDashboardView>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DashboardView());

            // act
            var result = await _controller.Dashboard();

            // assert
            Assert.That(result,Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.TypeOf<DashboardViewModel>());
        }
    }
}