using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
{
    public class Trout : IRequest
    {
        public string UserToTrout { get; }
        public string Channel { get; }

        public Trout(ChatMessage chatMessage)
        {
            Channel = chatMessage.Channel;

            // everything AFTER the '!trout ' in this command
            UserToTrout = chatMessage.Message
                .Replace("!trout ", "")
                .Replace("!trout", "");

            // if there's nothing, we're done. This isn't going to be a valid trouting
            if (string.IsNullOrEmpty(UserToTrout))
                return;

            // only want the first token
            UserToTrout = UserToTrout.Split(" ")[0];

            // if there's an @, get rid of it
            UserToTrout = UserToTrout.Replace("@", "");
        }
    }
}