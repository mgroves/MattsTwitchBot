namespace MattsTwitchBot.Core.Models
{
    public class TwitcherProfile
    {
        public string Type => "profile";
        public string ShoutMessage { get; set; }
        public bool? HasFanfare { get; set; }
        public FanfareInfo Fanfare { get; set; }
    }

    public class FanfareInfo
    {
        public string YouTubeCode { get; set; }
        public int? YouTubeStartTime { get; set; }
        public int? YouTubeEndTime { get; set; }
        public string Message { get; set; }
        public int? Timeout { get; set; }
    }
}