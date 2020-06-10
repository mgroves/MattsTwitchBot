using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class CreateProfileIfNotExists : IRequest
    {
        public string TwitchUsername { get; }

        public CreateProfileIfNotExists(string twitchUsername)
        {
            TwitchUsername = twitchUsername;
        }
    }
}