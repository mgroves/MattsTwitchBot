using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfile>
    {
        private readonly IBucket _bucket;

        public UpdateProfileHandler(ITwitchBucketProvider twitchBucketProvider)
        {
            _bucket = twitchBucketProvider.GetBucket();
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

            await _bucket.UpsertAsync(request.TwitchUsername, profile);
            return default;
        }
    }
}