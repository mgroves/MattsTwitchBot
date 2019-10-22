using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.Requests
{
    public class SoundEffectLookup : IRequest<ValidSoundEffects>, IRequest<Unit>
    {
        
    }
}