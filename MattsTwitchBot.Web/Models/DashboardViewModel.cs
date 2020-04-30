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
            this.Profiles = model.Profiles;
        }

        public List<string> Profiles { get; set; }

        // public HomePageInfo HomePageInfo { get; set; }
        //
        // public ValidStaticCommands StaticContentCommands { get; set; }

        public string HomePageInfoJson { get; set; }
        public string StaticContentCommandsJson { get; set; }

        // public string HomePageInfoJson => JsonConvert.SerializeObject(this.HomePageInfo, Formatting.Indented);
        // public string StaticContentCommandsJson => JsonConvert.SerializeObject(this.StaticContentCommands, Formatting.Indented);
    }
}