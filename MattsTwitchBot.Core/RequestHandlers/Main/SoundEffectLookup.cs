using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Main
{
    public class SoundEffectLookup : IRequest<ValidSoundEffects>, IRequest<Unit>
    {
        
    }
}