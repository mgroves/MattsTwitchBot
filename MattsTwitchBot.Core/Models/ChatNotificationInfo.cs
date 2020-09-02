using System;
using System.Collections.Generic;
using System.Linq;

namespace MattsTwitchBot.Core.Models
{
    public class ChatNotificationInfo
    {
        public List<Notification> Notifications { get; set; }
    }

    public class Notification
    {
        public int EveryNMinute { get; set; }
        public List<string> Messages { get; set; }

        public string RandomMessage
        {
            get
            {
                return Messages
                    .OrderBy(m => Guid.NewGuid()) // using Guid for RNG
                    .First();
            }
        }
        public bool ShouldSerializeRandomMessage() { return false; }
    }
}