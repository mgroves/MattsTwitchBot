using MediatR;

namespace MattsTwitchBot.Core.Notifications
{
    public class MinuteHeartbeatNotification : INotification
    {
        public int NumMinutes { get; }

        public MinuteHeartbeatNotification(int numMinutes)
        {
            NumMinutes = numMinutes;
        }
    }
}