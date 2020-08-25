using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Main
{
    public class SoundEffectLookupHandler : IRequestHandler<SoundEffectLookup, ValidSoundEffects>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public SoundEffectLookupHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<ValidSoundEffects> Handle(SoundEffectLookup request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            try
            {
                var soundEffectResult = await collection.GetAsync("validSoundEffects");
                return soundEffectResult.ContentAs<ValidSoundEffects>();
            }
            catch
            {
                return new ValidSoundEffects();
            }
        }
    }
}