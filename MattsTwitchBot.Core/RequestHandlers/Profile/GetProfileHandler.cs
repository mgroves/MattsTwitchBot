using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class GetProfileHandler : IRequestHandler<GetProfile, TwitcherProfile>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetProfileHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<TwitcherProfile> Handle(GetProfile request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("profiles");

            var result = await collection.GetAsync(request.TwitchUsername);
            return result.ContentAs<TwitcherProfile>();
        }
    }
}