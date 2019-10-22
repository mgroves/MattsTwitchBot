using System.Collections.Generic;
using System.Linq;

namespace MattsTwitchBot.Core.Models
{
    public class ValidSoundEffects
    {
        public List<SoundEffectInfo> SoundEffects { get; set; }

        public bool IsValid(string soundEffect)
        {
            if (SoundEffects == null)
                return false;
            if (!SoundEffects.Any())
                return false;
            return SoundEffects.Any(x => x.SoundEffectName.ToLower() == soundEffect.ToLower());
        }
    }
}