namespace MattsTwitchBot.Core.Models
{
    public class TwitcherProfile
    {
        public string Type => "profile";
        public string ShoutMessage { get; set; }
        public bool HasFanfare { get; set; }
    }
}