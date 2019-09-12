using System;
using Couchbase;
using Couchbase.Core;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery
{
    public class StoreMessage : ICommand
    {
        private readonly ChatMessage _message;
        private readonly IBucket _bucket;

        public StoreMessage(ChatMessage message, IBucket bucket)
        {
            _message = message;
            _bucket = bucket;
        }

        public void Execute()
        {
            var result = _bucket.Insert(new Document<dynamic>
            {
                Id = Guid.NewGuid().ToString(),
                Content = _message
            });
            Console.WriteLine($"Logged a message from {_message.DisplayName}.");
        }
    }
}