using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MattsTwitchBot.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Controllers.DashboardControllerTests
{
    [TestFixture]
    public class DashboardPostTests
    {
        private Mock<IMediator> _mockMediator;
        private DashboardController _controller;
        private const string MinimumValidBadgesJson = "{\"Badges\": []}";
        private const string MinimumValidCommandJson = "{ \"Commands\" : [] }";

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new DashboardController(_mockMediator.Object);
        }

        [Test]
        public async Task SaveDashboardData_command_is_executed()
        {
            // arrange

            // act
            await _controller.DashboardPost(MinimumValidBadgesJson, MinimumValidCommandJson);

            // assert
            _mockMediator.Verify(m => m.Send(
                It.IsAny<SaveDashboardData>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveDashboardData_has_correct_serialized_objects_from_strings()
        {
            // arrange
            var homePageInfo = new HomePageInfo {Badges = new List<SocialMediaBadge> { new SocialMediaBadge { Icon = "twumblr", Text = "mgroves"}}};
            var staticContentCommands = new ValidStaticCommands {  Commands = new List<StaticCommandInfo> { new StaticCommandInfo { Command = "defend", Content = "defend the channel against invaders!"} } };
            var homePageInfoJson = JsonConvert.SerializeObject(homePageInfo);
            var staticContentCommandsJson = JsonConvert.SerializeObject(staticContentCommands);

            // act
            await _controller.DashboardPost(homePageInfoJson, staticContentCommandsJson);

            // assert
            _mockMediator.Verify(m => m.Send(
                It.Is<SaveDashboardData>(x =>
                    x.HomePageInfo.Badges[0].Icon == homePageInfo.Badges[0].Icon
                    &&
                    x.StaticCommandInfo.Commands[0].Command == staticContentCommands.Commands[0].Command),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestCase("{ \"foo\" : \"bar\" }", "{ \"foo\" : \"bar\" }", true, true)]
        [TestCase(MinimumValidBadgesJson, MinimumValidCommandJson, false, false)]
        [TestCase("{ \"foo\" : \"bar\" }", MinimumValidCommandJson, true, false)]
        [TestCase(MinimumValidBadgesJson, "{ \"foo\" : \"bar\" }", false, true)]
        [TestCase("", "", true, true)]
        [TestCase(null, "", true, true)]
        [TestCase("", null, true, true)]
        [TestCase(null, null, true, true)]
        public async Task Sanity_check_the_json(string homePageInfojson, string staticContentJson, bool shouldHaveHomePageError, bool shouldHaveStaticContentError)
        {
            // arrange - this is valid JSON but it is NOT what I want to accept
            // arrange mediator command to reload dashboard
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDashboardView>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DashboardView());

            // act
            var result = await _controller.DashboardPost(homePageInfojson, staticContentJson);

            // assert - if everything is valid, controller will redirect
            if (!shouldHaveHomePageError && !shouldHaveStaticContentError)
            {
                Assert.That(result, Is.TypeOf<RedirectToActionResult>());
                return;
            }

            // assert - if something isn't!
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = result as ViewResult;
            var modelState = viewResult.ViewData.ModelState;
            Assert.That(_controller.ModelState, Is.Not.Null);
            Assert.That(_controller.ModelState.Count, Is.GreaterThan(0));
            Assert.That(modelState.Keys.Contains("homePageInfoJson"), Is.EqualTo(shouldHaveHomePageError));
            Assert.That(modelState.Keys.Contains("staticContentCommandsJson"), Is.EqualTo(shouldHaveStaticContentError));
        }
    }
}