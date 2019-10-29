using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MediatR;
using Microsoft.Extensions.Hosting;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core
{
    public class MattsChatBotHostedService : IHostedService
    {
        private readonly IMediator _mediator;
        private readonly ITwitchClient _twitchClient;
        private readonly TwitchCommandRequestFactory _commandFactory;

        public MattsChatBotHostedService(IMediator mediator, ITwitchClient twitchClient, TwitchCommandRequestFactory commandFactory)
        {
            _mediator = mediator;
            _twitchClient = twitchClient;
            _commandFactory = commandFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _twitchClient.OnMessageReceived += Client_OnMessageReceived;
            _twitchClient.Connect();

            return Task.CompletedTask;
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // check to see if a user has "arrived" for the first time
            // and welcome them if applicable
            await _mediator.Send(new UserHasArrived(e.ChatMessage));

            var req = await _commandFactory.BuildCommand(e.ChatMessage);
            await _mediator.Send(req);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _twitchClient.Disconnect();
            return Task.CompletedTask;
        }
    }
}
