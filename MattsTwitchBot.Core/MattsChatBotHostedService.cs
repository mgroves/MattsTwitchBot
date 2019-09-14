using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MediatR;
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

        public MattsChatBotHostedService(IMediator mediator, ITwitchClient twitchClient)
        {
            _mediator = mediator;
            _twitchClient = twitchClient;
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
