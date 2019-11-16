using MattsTwitchBot.Core.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MattsTwitchBot.Web.Models
{
    public class ProfileEditorViewModel
    {
        public string ShoutMessage { get; set; }
        public string TwitchUsername { get; set; }
        public bool FanfareEnabled { get; set; }
        public string FanfareYouTubeCode { get; set; }
        public int? FanfareYouTubeStartTime { get; set; }
        public int? FanfareYouTubeEndTime { get; set; }
        public string FanfareMessage { get; set; }
        public int? FanfareTimeout { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            if (FanfareEnabled)
            {
                if(!FanfareYouTubeStartTime.HasValue)
                    modelState.AddModelError("", "YouTubeStartTime is required");
                if(!FanfareYouTubeEndTime.HasValue)
                    modelState.AddModelError("", "YouTubeEndTime is required");
                if(string.IsNullOrEmpty(FanfareMessage))
                    modelState.AddModelError("", "FanfareMessage is required");
                if(!FanfareTimeout.HasValue)
                    modelState.AddModelError("", "FanfareTimeout is required");
                if(string.IsNullOrEmpty(FanfareYouTubeCode))
                    modelState.AddModelError("", "FanfareYouTubeCode is required");
            }
        }

        public void Map(TwitcherProfile twitcherProfile, string twitchUsername)
        {
            TwitchUsername = twitchUsername;
            ShoutMessage = twitcherProfile.ShoutMessage;
            if (twitcherProfile.Fanfare != null)
            {
                FanfareEnabled = twitcherProfile.Fanfare.Enabled ?? false;
                FanfareMessage = twitcherProfile.Fanfare.Message;
                FanfareTimeout = twitcherProfile.Fanfare.Timeout;
                FanfareYouTubeCode = twitcherProfile.Fanfare.YouTubeCode;
                FanfareYouTubeEndTime = twitcherProfile.Fanfare.YouTubeEndTime;
                FanfareYouTubeStartTime = twitcherProfile.Fanfare.YouTubeStartTime;
            }
        }
    }
}