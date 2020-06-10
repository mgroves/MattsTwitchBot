using System.Collections.Generic;
using MattsTwitchBot.Core.Models;
using Newtonsoft.Json;

namespace MattsTwitchBot.Web.Models
{
    public class DashboardViewModel
    {
        public void Map(DashboardView model)
        {
            this.StaticContentCommandsJson = JsonConvert.SerializeObject(model.StaticContentCommands, Formatting.Indented);
            this.HomePageInfoJson = JsonConvert.SerializeObject(model.HomePageInfo, Formatting.Indented);
            this.TriviaMessagesJson = JsonConvert.SerializeObject(model.TriviaMessages, Formatting.Indented);
            this.Profiles = model.Profiles;
        }

        public string HomePageInfoJson { get; set; }
        public string StaticContentCommandsJson { get; set; }
        public string TriviaMessagesJson { get; set; }

        public List<string> Profiles { get; set; }
    }
}