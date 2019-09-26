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
            // validate the sound effect (TODO: consider switch this to Enum?)
            var validSoundEffects = new List<string> { "laugh", "rimshot" };
            if (!validSoundEffects.Contains(request.SoundEffectName))
                return default;

            await _hub.Clients.All.SendAsync("SoundEffect", request.SoundEffectName, cancellationToken: cancellationToken);
            return default;
        }
    }
}