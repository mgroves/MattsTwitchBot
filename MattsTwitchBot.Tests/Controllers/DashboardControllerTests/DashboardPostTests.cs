using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Dashboard;
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
        private const string MinimumValidBadgesJson = "{\"Badges\": [], \"TickerMessages\": []}";
        private const string MinimumValidCommandJson = "{ \"Commands\" : [] }";
        private const string MinimumValidTriviaMessagesJson = "{ \"Messages\" : [], \"ShowMessages\" : false }";
        private const string MinimumValidChatNotificationInfo = "{ \"Notifications\" : [] }";

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
            await _controller.DashboardPost(MinimumValidBadgesJson, MinimumValidCommandJson, MinimumValidTriviaMessagesJson, MinimumValidChatNotificationInfo);

            // assert
            _mockMediator.Verify(m => m.Send(
                It.IsAny<SaveDashboardData>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveDashboardData_has_correct_serialized_objects_from_strings()
        {
            // arrange
            var homePageInfo = new HomePageInfo {Badges = new List<SocialMediaBadge> { new SocialMediaBadge { Icon = "tumblr", Text = "mgroves"}}};
            var staticContentCommands = new ValidStaticCommands {  Commands = new List<StaticCommandInfo> { new StaticCommandInfo { Command = "defend", Content = "defend the channel against invaders!"} } };
            var triviaMessages = new TriviaMessages {Messages = new List<string> { "hey what's up" }};
            var chatNotificationInfo = new ChatNotificationInfo();

            var homePageInfoJson = JsonConvert.SerializeObject(homePageInfo);
            var chatNotificationInfoJson = JsonConvert.SerializeObject(chatNotificationInfo);
            var staticContentCommandsJson = JsonConvert.SerializeObject(staticContentCommands);
            var triviaMessagesJson = JsonConvert.SerializeObject(triviaMessages);

            // act
            await _controller.DashboardPost(homePageInfoJson, staticContentCommandsJson, triviaMessagesJson, chatNotificationInfoJson);

            // assert
            _mockMediator.Verify(m => m.Send(
                It.Is<SaveDashboardData>(x =>
                    x.HomePageInfo.Badges[0].Icon == homePageInfo.Badges[0].Icon
                    &&
                    x.StaticCommandInfo.Commands[0].Command == staticContentCommands.Commands[0].Command),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // TODO: this is out of hand, the dashboard should be broken up
        [TestCase(MinimumValidBadgesJson, MinimumValidCommandJson, "{ \"foo\" : \"bar\" }", MinimumValidChatNotificationInfo, false, false, true, false)]
        [TestCase(MinimumValidBadgesJson, "{ \"foo\" : \"bar\" }", "{ \"foo\" : \"bar\" }", MinimumValidChatNotificationInfo, false, true, true, false)]
        [TestCase("{ \"foo\" : \"bar\" }", MinimumValidCommandJson, MinimumValidTriviaMessagesJson, MinimumValidChatNotificationInfo, true, false, false, false)]
        [TestCase("{ \"foo\" : \"bar\" }", "{ \"foo\" : \"bar\" }", MinimumValidTriviaMessagesJson, MinimumValidChatNotificationInfo, true, true, false, false)]
        [TestCase("{ \"foo\" : \"bar\" }", "{ \"foo\" : \"bar\" }", "{ \"foo\" : \"bar\" }", MinimumValidChatNotificationInfo, true, true, true, false)]
        [TestCase("", "", "", "", true, true, true, true)]
        [TestCase(null, "", "", "", true, true, true, true)]
        public async Task Sanity_check_the_json(string homePageInfojson, string staticContentJson, string triviaMessagesJson, string chatNotificationInfo, bool shouldHaveHomePageError, bool shouldHaveStaticContentError, bool shouldHaveTriviaMessagesError, bool ShouldHaveChatNotificationError)
        {
            // arrange - this is valid JSON but it is NOT what I want to accept
            // arrange mediator command to reload dashboard
            _mockMediator.Setup(m => m.Send(It.IsAny<GetDashboardView>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DashboardView());

            // act
            var result = await _controller.DashboardPost(homePageInfojson, staticContentJson, triviaMessagesJson, chatNotificationInfo);

            // assert - if everything is valid, controller will redirect
            if (!shouldHaveHomePageError && !shouldHaveStaticContentError && !shouldHaveTriviaMessagesError)
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
            Assert.That(modelState.Keys.Contains("triviaMessagesJson"), Is.EqualTo(shouldHaveTriviaMessagesError));
        }
    }
}