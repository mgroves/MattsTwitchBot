using System.Reflection;
using MattsTwitchBot.Core;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Tests.IntegrationTests.TestHelpers
{
    public static class TestMediator
    {
        public static IMediator Build(ITwitchClient twitchClient = null,
            ITwitchApiWrapper twitchApiWrapper = null,
            ITwitchBucketProvider bucketProvider = null,
            IKeyGenerator keyGenerator = null,
            IHubContext<ChatWebPageHub, IChatWebPageHub> hubContext = null,
            IConfiguration configuration = null)
        {
            var services = new ServiceCollection();

            services.AddMediatR(Assembly.GetAssembly(typeof(MattsChatBotHostedService)));

            services.AddTransient<TwitchCommandRequestFactory>();
            services.AddSingleton<ITwitchClient>(twitchClient ?? new Mock<ITwitchClient>().Object);
            services.AddSingleton<ITwitchApiWrapper>(twitchApiWrapper ?? new Mock<ITwitchApiWrapper>().Object);
            services.AddSingleton<ITwitchBucketProvider>(bucketProvider ?? new Mock<ITwitchBucketProvider>().Object);
            services.AddSingleton<IKeyGenerator>(x => keyGenerator ?? new Mock<IKeyGenerator>().Object);
            services.AddTransient<IHubContext<ChatWebPageHub, IChatWebPageHub>>(x => hubContext ?? new Mock<IHubContext<ChatWebPageHub, IChatWebPageHub>>().Object);
            services.Configure<TwitchOptions>(configuration.GetSection("Twitch"));

            var provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IMediator>();
        }
    }
}