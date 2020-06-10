using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Main
{
    public class SoundEffectLookupHandler : IRequestHandler<SoundEffectLookup, ValidSoundEffects>
    {
        private readonly IBucket _bucket;

        public SoundEffectLookupHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<ValidSoundEffects> Handle(SoundEffectLookup request, CancellationToken cancellationToken)
        {
            var soundEffectResult = await _bucket.GetAsync<ValidSoundEffects>("validSoundEffects");
            if(!soundEffectResult.Success)
                return new ValidSoundEffects();
            return soundEffectResult.Value;
        }
    }
}