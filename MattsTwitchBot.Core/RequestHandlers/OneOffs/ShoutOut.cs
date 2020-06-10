using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
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