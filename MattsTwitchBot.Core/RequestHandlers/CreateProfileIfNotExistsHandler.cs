using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class CreateProfileIfNotExistsHandler : IRequestHandler<CreateProfileIfNotExists>
    {
        private readonly IBucket _bucket;

        public CreateProfileIfNotExistsHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(CreateProfileIfNotExists request, CancellationToken cancellationToken)
        {
            var doesProfileExist = await _bucket.ExistsAsync(request.TwitchUsername);

            if (doesProfileExist)
                return default;

            // create a barebones profile
            await _bucket.UpsertAsync(new Document<TwitcherProfile>
            {
                Id = request.TwitchUsername,
                Content = new TwitcherProfile { }
            });

            return default;
        }
    }
}