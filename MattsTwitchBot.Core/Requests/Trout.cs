using System.Threading.Channels;
using Couchbase.Annotations;
using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.Requests
{
    public class Trout : IRequest
    {
        public string UserToTrout { get; }
        public string Channel { get; }

        public Trout(ChatMessage chatMessage)
        {
            UserToTrout = chatMessage.Message.Replace("!trout ", "");
            Channel = chatMessage.Channel;
        }
    }
}