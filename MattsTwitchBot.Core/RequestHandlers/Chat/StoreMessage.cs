using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.Chat
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