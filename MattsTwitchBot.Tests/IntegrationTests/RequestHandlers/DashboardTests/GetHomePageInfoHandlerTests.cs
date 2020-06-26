using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Dashboard;
using MattsTwitchBot.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.IntegrationTests.RequestHandlers.DashboardTests
{
    [TestFixture]
    [Category("SkipWhenLiveUnitTesting")]
    public class GetHomePageInfoHandlerTests : UsesDatabase
    {
        private GetHomePageInfoHandler _handler;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            _handler = new GetHomePageInfoHandler(BucketProvider);
        }

        [Test]
        public async Task Home_page_info_is_shown()
        {
            // arrange
            var expectedHomePageInfo = new HomePageInfo();
            expectedHomePageInfo.TickerMessages = new List<string> { "message 1", "2nd message"};
            await InsertTestDocument("homePageInfo", expectedHomePageInfo);
            var request = new GetHomePageInfo();

            // act
            var result = await _handler.Handle(request, CancellationToken.None);

            // assert
            Assert.That(result.TickerMessages.Count, Is.EqualTo(expectedHomePageInfo.TickerMessages.Count));
        }

        [Test]
        public async Task Empty_home_page_info_if_no_home_page_info_created_yet()
        {
            // arrange
            var request = new GetHomePageInfo();

            // act
            var result = await _handler.Handle(request, CancellationToken.None);

            // assert
            Assert.That(result.TickerMessages, Is.Null);
        }
    }
}