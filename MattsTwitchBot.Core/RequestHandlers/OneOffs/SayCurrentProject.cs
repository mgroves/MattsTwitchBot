using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class SayCurrentProject : IRequest
    {
        public ChatMessage Message { get; private set; }

        public SayCurrentProject(ChatMessage chatMessage)
        {
            Message = chatMessage;
        }
    }
}