using System.Linq;
using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class CouchbaseStatement : IRequest
    {
        public ChatMessage Message { get; }

        public CouchbaseStatement(ChatMessage message)
        {
            Message = message;
        }

        public static bool IsCouchMentioned(string message)
        {
            var lower = message.ToLower();
            var tokens = lower.Split(" ");
            return tokens.Any(t => t == "couch" || t == "couchdb");
        }
    }
}