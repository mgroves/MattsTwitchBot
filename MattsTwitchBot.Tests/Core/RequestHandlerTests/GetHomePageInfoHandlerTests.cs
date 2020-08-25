using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Dashboard;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class GetHomePageInfoHandlerTests : UnitTest
    {
        private GetHomePageInfoHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _handler = new GetHomePageInfoHandler(MockBucketProvider.Object);
        }

        [Test]
        public async Task If_homePageInfo_document_doesnt_exist_return_empty_object()
        {
            // arrange
            MockCollection.Setup(m => m.ExistsAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new FakeExistsResult(false));

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
            MockCollection.Setup(m => m.ExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(new FakeExistsResult(true));
            MockCollection.Setup(m => m.GetAsync(It.IsAny<string>(), null))
                .Throws(new Exception("retrieve fails"));
        
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
            MockCollection.Setup(m => m.ExistsAsync(It.IsAny<string>(),null)).ReturnsAsync(new FakeExistsResult(true));
            MockCollection.Setup(m => m.GetAsync(It.IsAny<string>(), null))
                .ReturnsAsync(new FakeGetResult(info));
        
            // act
            var result = await _handler.Handle(new GetHomePageInfo(), CancellationToken.None);
        
            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(info));
        }
    }
}