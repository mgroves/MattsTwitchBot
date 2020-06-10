using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.Chat
{
    public class Help : IRequest
    {
        public ChatMessage Message { get; private set; }

        public Help(ChatMessage chatMessage)
        {
            Message = chatMessage;
        }
    }
}