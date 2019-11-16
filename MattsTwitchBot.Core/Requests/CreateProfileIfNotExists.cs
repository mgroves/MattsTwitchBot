using MediatR;

namespace MattsTwitchBot.Core.Requests
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