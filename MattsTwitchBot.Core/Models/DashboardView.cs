using System.Collections.Generic;

namespace MattsTwitchBot.Core.Models
{
    public class DashboardView
    {
        public HomePageInfo HomePageInfo { get; set; }
        public ValidStaticCommands StaticContentCommands { get; set; }
        public TriviaMessages TriviaMessages { get; set; }
        public List<string> Profiles { get; set; }
        public ChatNotificationInfo ChatNotificationInfo { get; set; }
    }
}