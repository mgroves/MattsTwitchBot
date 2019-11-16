using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    // this isn't a true "user has arrived"
    // but it's a command that will be run against EVERY message
    // to see if this is the first message the given user has sent "in a while"
    // and the idea would be to welcome that user (if their profile has any welcome farfare defined)
    public class UserHasArrivedHandler : IRequestHandler<UserHasArrived>
    {
        private readonly IBucket _bucket;
        private IHubContext<ChatWebPageHub, IChatWebPageHub> _hub;

        public UserHasArrivedHandler(ITwitchBucketProvider twitchBucketProvider, IHubContext<ChatWebPageHub, IChatWebPageHub> hub)
        {
            _hub = hub;
            _bucket = twitchBucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(UserHasArrived request, CancellationToken cancellationToken)
        {
            var hasUserArrivedRecently = await CheckIfUserHasArrivedRecently(request.Message.Username);

            // if the user HAS been around recently, then bail out, no need to send fanfare
            if (hasUserArrivedRecently)
                return default;

            await CreateUserArrivedRecentlyData(request.Message.Username);

            var profile = await GetUserProfile(request.Message.Username);

            // if there is no profile, then there can be no fanfare
            if (profile == null)
                return default;

            // if they have fanfare, tell signalr hub
            if (profile.HasFanfare.HasValue && profile.HasFanfare.Value)
                await _hub.Clients.All.ReceiveFanfare(request.Message.Username);

            return default;
        }

        // get their profile
        private async Task<TwitcherProfile> GetUserProfile(string username)
        {
            var result = await _bucket.GetAsync<TwitcherProfile>(username.ToLower());
            return result.Value;
        }

        // create an arrive_recently with a TTL of like... 12 hours
        private async Task CreateUserArrivedRecentlyData(string username)
        {
            var doc = new Document<dynamic>
            {
                Id = $"{username}::arrived_recently",
                Content = new {
                    Message = $"{username} arrived: " + DateTime.Now
                },
                Expiry = 12 * 60 * 60 * 1000 // 12 hours
            };
            await _bucket.UpsertAsync(doc);
        }

        // check to see if user_arrived_recently document exists
        private async Task<bool> CheckIfUserHasArrivedRecently(string username)
        {
            return await _bucket.ExistsAsync($"{username}::arrived_recently");
        }
    }
}