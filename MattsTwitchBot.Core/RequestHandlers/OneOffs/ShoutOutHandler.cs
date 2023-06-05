using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class ShoutOutHandler : IRequestHandler<ShoutOut>
    {
        private readonly ITwitchApiWrapper _api;
        private readonly ITwitchClient _client;
        private readonly ITwitchBucketProvider _bucketProvider;

        public ShoutOutHandler(ITwitchApiWrapper apiWrapper, ITwitchClient client, ITwitchBucketProvider bucketProvider)
        {
            _api = apiWrapper;
            _client = client;
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(ShoutOut request, CancellationToken cancellationToken)
        {
            var message = request.Message;

            if (message.IsSubscriber || message.IsModerator)
                ; // do nothing, shout out is allowed
            else
                return default; // return, no shout out allowed

            var userToShout = ParseUserNameFromCommand(message.Message);

            // if there is no username specified, then bail out
            if (string.IsNullOrEmpty(userToShout))
                return default;

            if (!await _api.DoesUserExist(userToShout))
                return default;

            var thisChannel = message.Channel;
            var shoutOutMessage = await GetShoutOutMessage(userToShout);

            _client.SendMessage(thisChannel, shoutOutMessage);

            return default;
        }

        private async Task<string> GetShoutOutMessage(string userToShout)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("profiles");

            var defaultMessage = $"Hey everyone, check out @{userToShout}'s Twitch stream at https://twitch.tv/{userToShout}";
            var doesUserExist = (await collection.ExistsAsync(userToShout.ToLower())).Exists;
            if (!doesUserExist)
                return defaultMessage;

            var userProfileResult = await collection.GetAsync(userToShout.ToLower());
            var userProfile = userProfileResult.ContentAs<TwitcherProfile>();
            if (string.IsNullOrEmpty(userProfile.ShoutMessage))
                return defaultMessage;

            return userProfile.ShoutMessage + $" https://twitch.tv/{userToShout}";
        }

        private string ParseUserNameFromCommand(string message)
        {
            return message
                .Replace("!so", "")
                .Trim()
                .Split(' ')
                .FirstOrDefault();
        }
    }
}