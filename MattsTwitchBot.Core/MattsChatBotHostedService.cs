using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core
{
    public class MattsChatBotHostedService : IHostedService
    {
        private readonly IMediator _mediator;
        private readonly ITwitchClient _twitchClient;
        private readonly IHubContext<TwitchHub> _hub;

        public MattsChatBotHostedService(IMediator mediator, ITwitchClient twitchClient, IHubContext<TwitchHub> hub)
        {
            _mediator = mediator;
            _twitchClient = twitchClient;
            _hub = hub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _twitchClient.OnMessageReceived += Client_OnMessageReceived;
            _twitchClient.Connect();

            return Task.CompletedTask;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var req = InstantiateRequest(e.ChatMessage);
            _mediator.Send(req);
        }

        private IRequest InstantiateRequest(ChatMessage chatMessage)
        {
            var messageText = chatMessage.Message;
            switch (messageText)
            {
                case var x when x.StartsWith("!help"):
                    return new Help(chatMessage);
                case var x when x.StartsWith("!currentproject"):
                    return new SayCurrentProject(chatMessage);
                case var x when x.StartsWith("!setcurrentproject"):
                    return new SetCurrentProject(chatMessage);
                case var x when x.StartsWith("!so "):
                    return new ShoutOut(chatMessage);
                case var x when x.StartsWith("!profile"):
                    return new ModifyProfile(chatMessage);
                case var x when x == "!laugh":
                    return new SoundEffect("laugh");
                case var x when (x == "!rimshot" || x == "!badumtss"):
                    return new SoundEffect("rimshot");
                case var x when x.StartsWith("!sadtrombone"):
                    return new SoundEffect("sadtrombone");
                default:
                    return new StoreMessage(chatMessage);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _twitchClient.Disconnect();
            return Task.CompletedTask;
        }
    }
}
