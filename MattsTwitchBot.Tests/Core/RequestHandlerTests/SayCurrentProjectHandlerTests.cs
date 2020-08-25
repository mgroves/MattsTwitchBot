using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.OneOffs;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class SayCurrentProjectHandlerTests : UnitTest
    {
        private SayCurrentProjectHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new SayCurrentProjectHandler(MockBucketProvider.Object, MockTwitchClient.Object);
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
            MockCollection.Setup(m => m.GetAsync("currentProject", null))
                .Throws<Exception>();

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedDefaultMessage, false),
                Times.Once);
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
            MockCollection.Setup(x => x.GetAsync("currentProject", null))
                .ReturnsAsync(new FakeGetResult(projectInfo));
        
            // act
            await _handler.Handle(request, CancellationToken.None);
        
            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedMessage, false), Times.Once);
        }
    }
}