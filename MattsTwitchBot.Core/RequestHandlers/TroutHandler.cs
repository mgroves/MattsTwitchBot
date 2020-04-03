using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class TroutHandler : IRequestHandler<Trout>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly ITwitchApiWrapper _apiWrapper;

        public TroutHandler(ITwitchClient twitchClient, ITwitchApiWrapper apiWrapper)
        {
            _twitchClient = twitchClient;
            _apiWrapper = apiWrapper;
        }

        public async Task<Unit> Handle(Trout request, CancellationToken cancellationToken)
        {
            var doesUserExist = await _apiWrapper.DoesUserExist(request.UserToTrout);
            if (!doesUserExist)
            {
                _twitchClient.SendMessage(request.Channel, $"User {request.UserToTrout} doesn't exist.");
                return default;
            }

            _twitchClient.SendMessage(request.Channel, $"/me slaps @{request.UserToTrout} around a bit with a large trout.");
            return default;
        }
    }
}