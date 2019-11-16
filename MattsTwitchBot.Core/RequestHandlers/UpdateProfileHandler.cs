using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
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
            var cmd = _bucket.MutateIn<TwitcherProfile>(request.TwitchUsername);

            cmd = cmd.Upsert("fanfare.enabled", request.FanfareEnabled);

            if (!string.IsNullOrEmpty(request.ShoutMessage))
                cmd = cmd.Upsert("shoutMessage", request.ShoutMessage);
            if (!string.IsNullOrEmpty(request.FanfareMessage))
                cmd = cmd.Upsert("fanfare.message", request.FanfareMessage);
            if (request.FanfareTimeout.HasValue)
                cmd = cmd.Upsert("fanfare.timeout", request.FanfareTimeout.Value);
            if (request.FanfareYouTubeStartTime.HasValue)
                cmd = cmd.Upsert("fanfare.youTubeStartTime", request.FanfareYouTubeStartTime.Value);
            if (request.FanfareYouTubeEndTime.HasValue)
                cmd = cmd.Upsert("fanfare.youTubeEndTime", request.FanfareYouTubeEndTime.Value);
            if(!string.IsNullOrEmpty(request.FanfareYouTubeCode))
                cmd = cmd.Upsert("fanfare.youTubeCode", request.FanfareYouTubeCode);

            await cmd.ExecuteAsync();

            return default;
        }
    }
}