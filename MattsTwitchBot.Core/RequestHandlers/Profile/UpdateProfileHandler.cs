using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfile>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public UpdateProfileHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(UpdateProfile request, CancellationToken cancellationToken)
        {
            var profile = new TwitcherProfile();
            profile.Fanfare = new FanfareInfo();

            profile.ShoutMessage = request.ShoutMessage;
            profile.HasFanfare = request.FanfareEnabled;
            profile.Fanfare.Message = request.FanfareMessage;
            profile.Fanfare.Timeout = request.FanfareTimeout;
            profile.Fanfare.YouTubeStartTime = request.FanfareYouTubeStartTime;
            profile.Fanfare.YouTubeEndTime = request.FanfareYouTubeEndTime;
            profile.Fanfare.YouTubeCode = request.FanfareYouTubeCode;

            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            await collection.UpsertAsync(request.TwitchUsername, profile);
            return default;
        }
    }
}