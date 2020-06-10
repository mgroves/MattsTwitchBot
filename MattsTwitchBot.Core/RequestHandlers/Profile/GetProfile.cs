using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class GetProfile : IRequest<TwitcherProfile>
    {
        public string TwitchUsername { get; }

        public GetProfile(string twitchUsername)
        {
            TwitchUsername = twitchUsername;
        }
    }
}