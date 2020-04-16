using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.Requests
{
    public class GetHomePageInfo : IRequest, IRequest<HomePageInfo>
    {
        
    }
}