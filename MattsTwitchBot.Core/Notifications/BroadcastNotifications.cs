using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.Notifications
{
    public class BroadcastNotifications : INotificationHandler<MinuteHeartbeatNotification>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IOptions<TwitchOptions> _twitchOptions;
        private Random _rand;

        public BroadcastNotifications(ITwitchClient twitchClient, IOptions<TwitchOptions> twitchOptions)
        {
            _twitchClient = twitchClient;
            _twitchOptions = twitchOptions;
            _rand = new Random();
        }

        public Task Handle(MinuteHeartbeatNotification notification, CancellationToken cancellationToken)
        {
            // TODO: this stuff should all be pulled from configuration and not hard coded

            if (notification.NumMinutes == 0)
            {
                _twitchClient.SendMessage(_twitchOptions.Value.Username, "Welcome to my Office Hours! Thanks for being here, and remember that the only stupid question is the one you don't ask.");
                return default;
            }

            // only show twitter message every 8 minutes
            if (notification.NumMinutes % 8 == 0) { 
                _twitchClient.SendMessage(_twitchOptions.Value.Username, "You can follow me on Twitter: https://twitter.com/mgroves");
                return default;
            }

            // show !help message every 10 minutes
            if (notification.NumMinutes % 10 == 0)
            {
                _twitchClient.SendMessage(_twitchOptions.Value.Username, "New here? Try using the !help command");
                return default;
            }

            // show couchbase message every 12 minutes
            if (notification.NumMinutes % 12 == 0)
            {
                var message = "";
                var num = _rand.Next(0, 3);
                switch (num)
                {
                    case 0:
                        message = "This stream wouldn't be possible without the support of Couchbase! Try out the NoEQUAL database: https://couchbase.com/downloads";
                        break;
                    case 1:
                        message = "Interested in learning more about Couchbase? Follow https://twitter.com/CouchbaseDev";
                        break;
                    case 2:
                        message = "Got something to say about Couchbase or NoSQL? Submit a session to the CFP today: https://sessionize.com/couchbase-connect-online";
                        break;
                }
                _twitchClient.SendMessage(_twitchOptions.Value.Username, message);
                return default;
            }

            return default;
        }
    }
}