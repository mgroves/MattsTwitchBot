using System.Collections.Generic;

namespace MattsTwitchBot.Core.Models
{
    public class HomePageInfo
    {
        public List<SocialMediaBadge> Badges { get; set; }
    }

    public class SocialMediaBadge
    {
        public string Icon { get; set; }
        public string Text { get; set; }
    }
}