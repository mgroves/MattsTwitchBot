using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class SoundEffectHandler : IRequestHandler<SoundEffect>
    {
        private readonly IHubContext<TwitchHub> _hub;

        public SoundEffectHandler(IHubContext<TwitchHub> hub)
        {
            _hub = hub;
        }

        public async Task<Unit> Handle(SoundEffect request, CancellationToken cancellationToken)
        {
            // this method assumes that the SoundEffect request is valid and won't do any more
            // checks on the sound effect
            await _hub.Clients.All.SendAsync("SoundEffect", request.SoundEffectName, cancellationToken: cancellationToken);
            return default;
        }
    }
}