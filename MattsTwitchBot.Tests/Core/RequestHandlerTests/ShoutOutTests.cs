using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.RequestHandlers.OneOffs;
using MattsTwitchBot.Tests.Fakes;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models.Builders;

namespace MattsTwitchBot.Tests.Core.RequestHandlerTests
{
    [TestFixture]
    public class ShoutOutTests
    {
        private ShoutOutHandler _handler;
        private Mock<ITwitchApiWrapper> _mockApiWrapper;
        private Mock<ITwitchClient> _mockTwitchClient;
        private Mock<ITwitchBucketProvider> _mockBucketProvider;
        private Mock<IBucket> _mockBucket;

        [SetUp]
        public void Setup()
        {
            _mockApiWrapper = new Mock<ITwitchApiWrapper>();
            _mockTwitchClient = new Mock<ITwitchClient>();
            _mockBucket = new Mock<IBucket>();
            _mockBucketProvider = new Mock<ITwitchBucketProvider>();
            _mockBucketProvider.Setup(x => x.GetBucket()).Returns(_mockBucket.Object);
            _handler = new ShoutOutHandler(_mockApiWrapper.Object, _mockTwitchClient.Object, _mockBucketProvider.Object);
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
            _mockApiWrapper.Setup(m =>
                m.DoesUserExist(It.IsAny<string>())).ReturnsAsync(true);

            // act
            await _handler.Handle(request, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false),
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
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
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
            _mockApiWrapper.Setup(x => x.DoesUserExist(userName)).ReturnsAsync(true);
            _mockBucket.Setup(x => x.Exists(userName.ToLower())).Returns(true);
            _mockBucket.Setup(x => x.Get<TwitcherProfile>(userName.ToLower()))
                .Returns(new FakeOperationResult<TwitcherProfile> { Value = new TwitcherProfile()});
            var shout = new ShoutOut(chatMessage);
            var expectedMessage = $"Hey everyone, check out @{userName}'s Twitch stream at https://twitch.tv/{userName}";

            // act
            await _handler.Handle(shout, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedMessage, false), Times.Once);
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
            _mockApiWrapper.Setup(x => x.DoesUserExist(userLookup)).ReturnsAsync(false);
            var shout = new ShoutOut(chatMessage);

            // act
            await _handler.Handle(shout, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Never);
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
            _mockApiWrapper.Setup(x => x.DoesUserExist(userLookup)).ReturnsAsync(true);
            var shout = new ShoutOut(chatMessage);

            // act
            await _handler.Handle(shout, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false), Times.Once);
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
            _mockApiWrapper.Setup(x => x.DoesUserExist(userName)).ReturnsAsync(true);
            _mockBucket.Setup(x => x.Exists(userName.ToLower())).Returns(true);
            _mockBucket.Setup(x => x.Get<TwitcherProfile>(userName.ToLower()))
                .Returns(new FakeOperationResult<TwitcherProfile> { Value = userProfile });
            var shout = new ShoutOut(chatMessage);
            var expectedMessage = $"Hey everyone, check out @{userName}'s Twitch stream at https://twitch.tv/{userName}";

            // act
            await _handler.Handle(shout, CancellationToken.None);

            // assert
            _mockTwitchClient.Verify(x => x.SendMessage(It.IsAny<string>(), expectedShoutMessage, false), Times.Once);
        }
    }
}