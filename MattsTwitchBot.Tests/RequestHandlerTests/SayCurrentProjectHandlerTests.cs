using System;
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
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.RequestHandlerTests
{
    [TestFixture]
    public class SayCurrentProjectHandlerTests
    {
        private SayCurrentProjectHandler _handler;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<ITwitchClient> _mockTwitchClient;
        private Mock<IBucket> _mockBucket;

        [SetUp]
        public void Setup()
        {
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);
            _mockTwitchClient = new Mock<ITwitchClient>();
            _handler = new SayCurrentProjectHandler(_mockBucketProvider.Object, _mockTwitchClient.Object);
        }

        [Test]
        public async Task If_no_current_project_set_show_default_message()
        {
            // arrange
            var expectedDefaultMessage = "I haven't set any current project yet, sorry!";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatterusername")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!currentproject")
                .WithChannel("doesntmatterchannel")
                .Build();
            var request = new SayCurrentProject(chatMessage);
            _mockBucket.Setup(x => x.Get<CurrentProjectInfo>("currentProject"))
                .Returns(new FakeOperationResult<CurrentProjectInfo> {Success = false});

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedDefaultMessage, false), Times.Once);
        }

        [Test]
        public async Task Show_current_project_url()
        {
            // arrange
            var expectedUrl = "http://example.org/foo/bar";
            var expectedMessage = $"Current Project is: " + expectedUrl;
            var projectInfo = new CurrentProjectInfo { Url = new Uri(expectedUrl) };
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatterusername")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("!currentproject")
                .WithChannel("doesntmatterchannel")
                .Build();
            var request = new SayCurrentProject(chatMessage);
            _mockBucket.Setup(x => x.Get<CurrentProjectInfo>("currentProject"))
                .Returns(new FakeOperationResult<CurrentProjectInfo> { Success = true, Value = projectInfo });

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedMessage, false), Times.Once);
        }
    }
}