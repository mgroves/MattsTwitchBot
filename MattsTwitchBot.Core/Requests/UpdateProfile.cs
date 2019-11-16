using MediatR;

namespace MattsTwitchBot.Core.Requests
{
    public class UpdateProfile : IRequest
    {
        public string TwitchUsername { get; set; }
        public string ShoutMessage { get; set; }
        public bool FanfareEnabled { get; set; }
        public string FanfareMessage { get; set; }
        public int? FanfareTimeout { get; set; }
        public string FanfareYouTubeCode { get; set; }
        public int? FanfareYouTubeEndTime { get; set; }
        public int? FanfareYouTubeStartTime { get; set; }
    }
}