using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class ShoutOutHandler : IRequestHandler<ShoutOut>
    {
        private readonly ITwitchApiWrapper _api;
        private readonly ITwitchClient _client;
        private readonly IBucket _bucket;

        public ShoutOutHandler(ITwitchApiWrapper apiWrapper, ITwitchClient client, ITwitchBucketProvider bucketProvider)
        {
            _api = apiWrapper;
            _client = client;
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(ShoutOut request, CancellationToken cancellationToken)
        {
            var message = request.Message;

            if (!message.IsSubscriber)
                return default;

            var userToShout = ParseUserNameFromCommand(message.Message);

            // if there is no username specified, then bail out
            if (string.IsNullOrEmpty(userToShout))
                return default;

            if (!await _api.DoesUserExist(userToShout))
                return default;

            var thisChannel = message.Channel;
            var shoutOutMessage = GetShoutOutMessage(userToShout);

            _client.SendMessage(thisChannel, shoutOutMessage);

            return default;
        }

        private string GetShoutOutMessage(string userToShout)
        {
            var defaultMessage = $"Hey everyone, check out @{userToShout}'s Twitch stream at https://twitch.tv/{userToShout}";
            if (!_bucket.Exists(userToShout.ToLower()))
                return defaultMessage;
            var userProfile = _bucket.Get<TwitcherProfile>(userToShout.ToLower()).Value;
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