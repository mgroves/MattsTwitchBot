using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Main
{
    public class SoundEffect : IRequest
    {
        public string SoundEffectName { get; private set; }

        public SoundEffect(string soundEffect)
        {
            SoundEffectName = soundEffect;
        }
    }
}