using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class Lurk : IRequest
    {
        public ChatMessage ChatMessage { get; }

        public Lurk(ChatMessage chatMessage)
        {
            ChatMessage = chatMessage;
        }
    }
}