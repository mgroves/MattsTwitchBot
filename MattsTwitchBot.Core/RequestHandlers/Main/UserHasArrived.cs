using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.Main
{
    public class UserHasArrived : IRequest
    {
        public ChatMessage Message { get; }
        public UserHasArrived(ChatMessage chatMessage)
        {
            Message = chatMessage;
        }
    }
}