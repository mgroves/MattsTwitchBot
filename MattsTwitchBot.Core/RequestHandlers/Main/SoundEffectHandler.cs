using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MattsTwitchBot.Core.RequestHandlers.Main
{
    public class SoundEffectHandler : IRequestHandler<SoundEffect>
    {
        private IHubContext<ChatWebPageHub, IChatWebPageHub> _hub;

        public SoundEffectHandler(IHubContext<ChatWebPageHub, IChatWebPageHub> hub)
        {
            _hub = hub;
        }

        public async Task<Unit> Handle(SoundEffect request, CancellationToken cancellationToken)
        {
            // this method assumes that the SoundEffect request is valid and won't do any more
            // checks on the sound effect
            await _hub.Clients.All.ReceiveSoundEffect(request.SoundEffectName);
            return default;
        }
    }
}