using System;
using Couchbase.Core;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery.Commands
{
    public class CurrentProject : ICommand
    {
        private readonly ChatMessage _message;
        private readonly IBucket _bucket;
        private readonly ITwitchClient _client;

        public CurrentProject(ChatMessage chatMessage, IBucket bucket, ITwitchClient client)
        {
            _message = chatMessage;
            _bucket = bucket;
            _client = client;
        }

        public void Execute()
        {
            var currentProjectDocumentKey = "currentProject";

            var currentProjectResult = _bucket.Get<CurrentProjectInfo>(currentProjectDocumentKey);
            if (currentProjectResult == null || !currentProjectResult.Success)
            {
                _client.SendMessage(_message.Channel, "I haven't set any current project yet, sorry!");
                return;
            }

            var currentProjectDoc = currentProjectResult.Value;
            _client.SendMessage(_message.Channel, $"Current Project is: " + currentProjectDoc.Url);
        }
    }

    public class CurrentProjectInfo
    {
        public Uri Url { get; set; }
    }
}