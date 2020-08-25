using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Profile;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class ModifyProfileHandlerTests : UnitTest
    {
        private ModifyProfileHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new ModifyProfileHandler(MockBucketProvider.Object);
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
            MockCollection.Setup(m => m.ExistsAsync(username, null))
                .ReturnsAsync(new FakeExistsResult(false));

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockCollection.Verify(x => x.InsertAsync(username, It.Is<TwitcherProfile>(
                y => y.Type == "profile"), null), Times.Once);
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
        
            MockCollection.Setup(x => x.ExistsAsync(username, null))
                .ReturnsAsync(new FakeExistsResult(true));
        
            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockCollection.Verify(x => x.InsertAsync(username, It.IsAny<TwitcherProfile>(), null), Times.Never);
        }

        // TODO: this seems like it might be a better integration test
        // [Test]
        // public async Task User_can_add_a_shoutout_message_to_their_profile()
        // {
        //     // arrange
        //     var username = "someguy";
        //     var expectedShoutMessage = $"This is my shoutout message! Here is my URL:";
        //     var twitchLibMessage = TwitchLibMessageBuilder.Create()
        //         .WithUsername(username)
        //         .Build();
        //     var chatMessage = ChatMessageBuilder.Create()
        //         .WithTwitchLibMessage(twitchLibMessage)
        //         .WithMessage($"!profile-shout {expectedShoutMessage}")
        //         .Build();
        //     var request = new ModifyProfile(chatMessage);
        //     MockCollection.Setup(m => m.ExistsAsync(username, null))
        //         .ReturnsAsync(new FakeExistsResult(true));
        //
        //     // act
        //     await _handler.Handle(request, CancellationToken.None);
        //
        //     // assert
        //     MockCollection.Verify(x => x.MutateInAsync(username, It.IsAny<Action<MutateInSpecBuilder>>(), It.IsAny<Action<MutateInOptions>>())
        //         ,Times.Once());
        //     // _mockMutateInBuilder.Verify(x =>
        //     //         x.Upsert("shoutMessage", expectedShoutMessage, true),
        //     //     Times.Once);
        // }
    }
}