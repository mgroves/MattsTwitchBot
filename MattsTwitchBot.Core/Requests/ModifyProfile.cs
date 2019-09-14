using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
{
    public class ModifyProfile : IRequest
    {
        public ChatMessage Message { get; private set; }

        public ModifyProfile(ChatMessage chatMessage)
        {
            Message = chatMessage;
        }
    }
}