using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
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