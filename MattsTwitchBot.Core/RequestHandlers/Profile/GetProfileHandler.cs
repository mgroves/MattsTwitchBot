using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class GetProfileHandler : IRequestHandler<GetProfile, TwitcherProfile>
    {
        private readonly IBucket _bucket;

        public GetProfileHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<TwitcherProfile> Handle(GetProfile request, CancellationToken cancellationToken)
        {
            var result = await _bucket.GetAsync<TwitcherProfile>(request.TwitchUsername);
            return result.Value;
        }
    }
}