using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class CreateProfileIfNotExistsHandler : IRequestHandler<CreateProfileIfNotExists>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public CreateProfileIfNotExistsHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(CreateProfileIfNotExists request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            var doesProfileExistResult = await collection.ExistsAsync(request.TwitchUsername);

            if (doesProfileExistResult.Exists)
                return default;

            // create a barebones profile
            await collection.UpsertAsync(request.TwitchUsername, new TwitcherProfile { });

            return default;
        }
    }
}