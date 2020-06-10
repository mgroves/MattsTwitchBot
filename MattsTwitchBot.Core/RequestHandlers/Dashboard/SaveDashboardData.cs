using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Dashboard
{
    public class SaveDashboardData : IRequest
    {
        public HomePageInfo HomePageInfo { get; }
        public ValidStaticCommands StaticCommandInfo { get; }
        public TriviaMessages TriviaMessages { get; }

        public SaveDashboardData(HomePageInfo homePageInfo, ValidStaticCommands staticCommandInfo, TriviaMessages triviaMessages)
        {
            HomePageInfo = homePageInfo;
            StaticCommandInfo = staticCommandInfo;
            TriviaMessages = triviaMessages;
        }
    }
}