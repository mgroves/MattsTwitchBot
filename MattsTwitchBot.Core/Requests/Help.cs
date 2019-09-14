using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
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