using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Dashboard
{
    public class GetHomePageInfo : IRequest, IRequest<HomePageInfo>
    {
        
    }
}