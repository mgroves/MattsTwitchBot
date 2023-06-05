using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.KeyValue;
using MattsTwitchBot.Core.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MattsTwitchBot.Core.RequestHandlers.Main
{
    // this isn't a true "user has arrived"
    // but it's a command that will be run against EVERY message
    // to see if this is the first message the given user has sent "in a while"
    // and the idea would be to welcome that user (if their profile has any welcome farfare defined)
    public class UserHasArrivedHandler : IRequestHandler<UserHasArrived>
    {
        private readonly ITwitchBucketProvider _bucketProvider;
        private readonly IHubContext<ChatWebPageHub, IChatWebPageHub> _hub;

        public UserHasArrivedHandler(ITwitchBucketProvider bucketProvider, IHubContext<ChatWebPageHub, IChatWebPageHub> hub)
        {
            _bucketProvider = bucketProvider;
            _hub = hub;
        }

        public async Task<Unit> Handle(UserHasArrived request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("arrived");

            var hasUserArrivedRecently = await CheckIfUserHasArrivedRecently(request.Message.Username, collection);

            // if the user HAS been around recently, then bail out, no need to send fanfare
            if (hasUserArrivedRecently)
                return default;

            await CreateUserArrivedRecentlyData(request.Message.Username, collection);

            var profile = await GetUserProfile(request.Message.Username, collection);

            // if there is no profile, then there can be no fanfare
            if (profile == null)
                return default;

            // if they have fanfare, tell signalr hub
            if (profile.HasFanfare.HasValue && profile.HasFanfare.Value)
                await _hub.Clients.All.ReceiveFanfare(profile.Fanfare);

            return default;
        }

        // get their profile
        private async Task<TwitcherProfile> GetUserProfile(string username, ICouchbaseCollection collection)
        {
            try
            {
                var result = await collection.GetAsync(username.ToLower());
                return result.ContentAs<TwitcherProfile>();
            }
            catch
            {
                return null;
            }
        }

        // create an arrive_recently with a TTL of like... 12 hours
        private async Task CreateUserArrivedRecentlyData(string username, ICouchbaseCollection collection)
        {
            var id = $"{username}::arrived_recently";
            var content = new UserHasArrivedMarker
            {
                Message = $"{username} arrived: " + DateTime.Now
            };
            await collection.UpsertAsync(id, content, 
                options => options.Expiry(new TimeSpan(12,0,0)));
        }

        // check to see if user arrived_recently document exists
        private async Task<bool> CheckIfUserHasArrivedRecently(string username, ICouchbaseCollection collection)
        {
            var result = await collection.ExistsAsync($"{username}::arrived_recently");
            return result.Exists;
        }
    }
}