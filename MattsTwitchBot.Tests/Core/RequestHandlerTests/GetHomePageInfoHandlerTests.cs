using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.RequestHandlers.Dashboard;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class GetHomePageInfoHandlerTests
    {
        private GetHomePageInfoHandler _handler;
        private Mock<IBucket> _mockBucket;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);

            _handler = new GetHomePageInfoHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task If_homePageInfo_document_doesnt_exist_return_empty_object()
        {
            // arrange
            _mockBucket.Setup(m => m.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            // act
            var result = await _handler.Handle(new GetHomePageInfo(), CancellationToken.None);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Badges, Is.Null);
        }

        [Test]
        public async Task If_homePageInfo_retrieve_fails_then_return_empty_object()
        {
            // arrange
            _mockBucket.Setup(m => m.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockBucket.Setup(m => m.GetAsync<HomePageInfo>(It.IsAny<string>()))
                .ReturnsAsync(new FakeOperationResult<HomePageInfo> {Success = false});

            // act
            var result = await _handler.Handle(new GetHomePageInfo(), CancellationToken.None);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Badges, Is.Null);
        }

        [Test]
        public async Task Return_homePageInfo_from_database()
        {
            // arrange
            var info = new HomePageInfo();
            info.Badges = new List<SocialMediaBadge>();
            info.Badges.Add(new SocialMediaBadge { Icon = "foo", Text = "bar"});
            _mockBucket.Setup(m => m.ExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockBucket.Setup(m => m.GetAsync<HomePageInfo>(It.IsAny<string>()))
                .ReturnsAsync(new FakeOperationResult<HomePageInfo> { Success = true, Value = info});

            // act
            var result = await _handler.Handle(new GetHomePageInfo(), CancellationToken.None);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(info));
        }
    }
}