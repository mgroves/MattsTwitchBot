using System;
using Couchbase;
using Couchbase.Core;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery
{
    public class StoreMessage : ICommand
    {
        private readonly ChatMessage _message;

        public StoreMessage(ChatMessage message)
        {
            _message = message;
        }

        public void Execute(IBucket bucket, ITwitchClient twitchClient)
        {
            var result = bucket.Insert(new Document<dynamic>
            {
                Id = Guid.NewGuid().ToString(),
                Content = _message
            });
            Console.WriteLine($"Logged a message from {_message.DisplayName}.");
        }
    }
}