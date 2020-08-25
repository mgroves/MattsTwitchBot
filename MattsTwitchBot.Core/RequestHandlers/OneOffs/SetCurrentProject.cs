using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class SetCurrentProject : IRequest
    {
        public ChatMessage Message { get; private set; }

        public SetCurrentProject(ChatMessage chatMessage)
        {
            Message = chatMessage;
        }
    }
}