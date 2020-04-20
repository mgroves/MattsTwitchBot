using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
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