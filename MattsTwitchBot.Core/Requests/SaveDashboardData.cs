using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.Requests
{
    public class SaveDashboardData : IRequest
    {
        public HomePageInfo HomePageInfo { get; }
        public ValidStaticCommands StaticCommandInfo { get; }

        public SaveDashboardData(HomePageInfo homePageInfo, ValidStaticCommands staticCommandInfo)
        {
            HomePageInfo = homePageInfo;
            StaticCommandInfo = staticCommandInfo;
        }
    }
}