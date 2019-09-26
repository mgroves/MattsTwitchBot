using MediatR;

namespace MattsTwitchBot.Core.Requests
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