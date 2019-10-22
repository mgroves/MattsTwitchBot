using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class ModifyProfileHandlerTests
    {
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;
        private ModifyProfileHandler _handler;
        private Mock<IMutateInBuilder<dynamic>> _mockMutateInBuilder;

        [SetUp]
        public void Setup()
        {
            _mockMutateInBuilder = new Mock<IMutateInBuilder<dynamic>>();
            _mockMutateInBuilder.Setup(x => x.Upsert(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Returns(_mockMutateInBuilder.Object);

            _mockBucket = new Mock<IBucket>();
            _mockBucket.Setup(x => x.MutateIn<dynamic>(It.IsAny<string>()))
                .Returns(_mockMutateInBuilder.Object);

            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);

            _handler = new ModifyProfileHandler(_mockBucketProvider.Object);
        }

        [Test]
        public async Task Should_create_a_profile_if_one_doesnt_exist()
        {
            // arrange
            var username = "someguy";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("doesntmatter")
                .Build();
            var request = new ModifyProfile(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockBucket.Verify(x => x.InsertAsync(It.Is<IDocument<TwitcherProfile>>(
                y => 
                    y.Id == username
                    && y.Content.Type == "profile")), Times.Once);
        }

        [Test]
        public async Task Should_not_create_a_profile_if_one_already_exists()
        {
            // arrange
            var username = "someguythatalreadyexists";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage("notnull")
                .Build();
            var request = new ModifyProfile(chatMessage);

            _mockBucket.Setup(x => x.ExistsAsync(username)).ReturnsAsync(true);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockBucket.Verify(x => x.InsertAsync(It.Is<IDocument<TwitcherProfile>>(
                y =>
                    y.Id == username)), Times.Never);
        }

        [Test]
        public async Task User_can_add_a_shoutout_message_to_their_profile()
        {
            // arrange
            var username = "someguy";
            var expectedShoutMessage = $"This is my shoutout message! Here is my URL:";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage($"!profile-shout {expectedShoutMessage}")
                .Build();
            var request = new ModifyProfile(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockMutateInBuilder.Verify(x =>
                    x.Upsert("shoutMessage", expectedShoutMessage, true),
                Times.Once);
        }
    }
}