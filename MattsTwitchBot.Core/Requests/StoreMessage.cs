using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
{
    public class StoreMessage : IRequest
    {
        public ChatMessage Message { get; private set; }

        public StoreMessage(ChatMessage chatMessage)
        {
            Message = chatMessage;
        }
    }
}