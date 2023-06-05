using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.Notifications
{
    public class BroadcastNotifications : INotificationHandler<MinuteHeartbeatNotification>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IOptions<TwitchOptions> _twitchOptions;
        private readonly ITwitchBucketProvider _bucketProvider;
        private readonly Random _rand;

        public BroadcastNotifications(ITwitchClient twitchClient, IOptions<TwitchOptions> twitchOptions, ITwitchBucketProvider bucketProvider)
        {
            _twitchClient = twitchClient;
            _twitchOptions = twitchOptions;
            _bucketProvider = bucketProvider;
            _rand = new Random();
        }

        public async Task Handle(MinuteHeartbeatNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var bucket = await _bucketProvider.GetBucketAsync();
                var coll = await bucket.CollectionAsync("config");
                var chatNotificationInfoDoc = await coll.GetAsync("chatNotificationInfo");
                var chatNotificationInfo = chatNotificationInfoDoc.ContentAs<ChatNotificationInfo>();

                foreach (var entry in chatNotificationInfo.Notifications)
                {
                    var exactMinuteMatch = entry.EveryNMinute == notification.NumMinutes;
                    var minuteMultiplierMatch = false;
                    if(entry.EveryNMinute > 1 && notification.NumMinutes > 0)
                        minuteMultiplierMatch = (notification.NumMinutes % entry.EveryNMinute) == 0;
                    if (exactMinuteMatch || minuteMultiplierMatch)
                    {
                        _twitchClient.SendMessage(_twitchOptions.Value.Username, entry.RandomMessage);
                    }
                }
            }
            catch
            {
                // TODO: log this somewhere maybe?
            }
        }
    }
}