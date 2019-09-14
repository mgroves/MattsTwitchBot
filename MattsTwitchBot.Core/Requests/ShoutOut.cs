using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
{
    public class ShoutOut : IRequest
    {
        public ChatMessage Message { get; private set; }

        public ShoutOut(ChatMessage chatMessage)
        {
            Message = chatMessage;
        }
    }
}