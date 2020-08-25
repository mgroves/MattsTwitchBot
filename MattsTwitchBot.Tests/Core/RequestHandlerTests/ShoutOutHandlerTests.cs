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
    public class ShoutOutHandlerTests : UnitTest
    {
        private ShoutOutHandler _handler;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _handler = new ShoutOutHandler(MockApiWrapper.Object, MockTwitchClient.Object, MockBucketProvider.Object);
        }

        [TestCase(true, true, 1, Description = "user who is mod AND sub can shout out")]
        [TestCase(true, false, 1, Description = "user who is sub but not mod can shout out")]
        [TestCase(false, true, 1, Description = "user who is mod but not sub can shout out")]
        [TestCase(false, false, 0, Description = "user who is not mod or sub can NOT shout out")]
        public async Task ShoutOut_is_limited_to_mods_and_subs(bool isSub, bool isMod, int numVerified)
        {
            // arrange
            var userToShout = "someusername";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("NonSub")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithMessage($"!so {userToShout}")
                .WithIsSubscriber(isSub)
                .WithIsModerator(isMod)
                .Build();
            var request = new ShoutOut(chatMessage);
            MockApiWrapper.Setup(m =>
                m.DoesUserExist(It.IsAny<string>())).ReturnsAsync(true);
            MockCollection.Setup(m => m.ExistsAsync(userToShout, null))
                .ReturnsAsync(new FakeExistsResult(true));
            MockCollection.Setup(m => m.GetAsync(userToShout, null))
                .ReturnsAsync(new FakeGetResult(new TwitcherProfile()));

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false),
                Times.Exactly(numVerified));
        }

        [Test]
        public async Task ShoutOut_will_do_nothing_if_no_name_is_given()
        {
            // arrange
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(true)
                .WithMessage("!so")
                .Build();
            var request = new ShoutOut(chatMessage);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
        }

        [Test]
        public async Task ShoutOut_will_shout_the_given_username()
        {
            // arrange
            var userName = "shoutyMcShoutface";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(true)
                .WithMessage($"!so {userName}")
                .Build();
            MockApiWrapper.Setup(x => x.DoesUserExist(userName)).ReturnsAsync(true);
            MockCollection.Setup(x => x.ExistsAsync(userName.ToLower(), null))
                .ReturnsAsync(new FakeExistsResult(true));
            MockCollection.Setup(x => x.GetAsync(userName.ToLower(), null))
                .ReturnsAsync(new FakeGetResult(new TwitcherProfile()));
            var shout = new ShoutOut(chatMessage);
            var expectedMessage = $"Hey everyone, check out @{userName}'s Twitch stream at https://twitch.tv/{userName}";

            // act
            await _handler.Handle(shout, CancellationToken.None);

            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedMessage, false), Times.Once);
        }

        [Test]
        public async Task ShoutOut_will_not_shout_if_the_given_username_isnt_a_real_twitch_user()
        {
            // arrange
            var userLookup = "doesntmattereither";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(true)
                .WithMessage($"!so {userLookup}")
                .Build();
            MockApiWrapper.Setup(x => x.DoesUserExist(userLookup)).ReturnsAsync(false);
            var shout = new ShoutOut(chatMessage);
        
            // act
            await _handler.Handle(shout, CancellationToken.None);
        
            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
        }
        
        [Test]
        public async Task ShoutOut_WILL_shout_if_the_given_username_is_a_real_twitch_user()
        {
            // arrange
            var userLookup = "doesntmattereither";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(true)
                .WithMessage($"!so {userLookup}")
                .Build();
            MockApiWrapper.Setup(x => x.DoesUserExist(userLookup)).ReturnsAsync(true);
            MockCollection.Setup(x => x.ExistsAsync(userLookup.ToLower(), null))
                .ReturnsAsync(new FakeExistsResult(true));
            MockCollection.Setup(x => x.GetAsync(userLookup.ToLower(), null))
                .ReturnsAsync(new FakeGetResult(new TwitcherProfile()));

            var shout = new ShoutOut(chatMessage);
        
            // act
            await _handler.Handle(shout, CancellationToken.None);
        
            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Once);
        }
        
        [Test]
        public async Task ShoutOut_will_use_custom_message_if_there_is_one()
        {
            // arrange
            var userProfile = new TwitcherProfile {ShoutMessage = "hey hey look at me" + Guid.NewGuid()};
            var userName = "shoutyMcShoutface";
            var expectedShoutMessage = userProfile.ShoutMessage + $" https://twitch.tv/{userName}";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername("doesntmatter")
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithIsSubscriber(true)
                .WithMessage($"!so {userName}")
                .Build();
            MockApiWrapper.Setup(x => x.DoesUserExist(userName)).ReturnsAsync(true);
            MockCollection.Setup(x => x.ExistsAsync(userName.ToLower(), null))
                .ReturnsAsync(new FakeExistsResult(true));
            MockCollection.Setup(x => x.GetAsync(userName.ToLower(), null))
                .ReturnsAsync(new FakeGetResult(userProfile));
            var shout = new ShoutOut(chatMessage);
        
            // act
            await _handler.Handle(shout, CancellationToken.None);
        
            // assert
            MockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedShoutMessage, false), Times.Once);
        }
    }
}