using System;
using Couchbase.Core;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery.Commands
{
    public class CurrentProject : ICommand
    {
        private readonly ChatMessage _message;

        public CurrentProject(ChatMessage chatMessage)
        {
            _message = chatMessage;
        }

        public void Execute(IBucket bucket, ITwitchClient client)
        {
            var currentProjectDocumentKey = "currentProject";

            var currentProjectResult = bucket.Get<CurrentProjectInfo>(currentProjectDocumentKey);
            if (currentProjectResult == null || !currentProjectResult.Success)
            {
                client.SendMessage(_message.Channel, "I haven't set any current project yet, sorry!");
                return;
            }

            var currentProjectDoc = currentProjectResult.Value;
            client.SendMessage(_message.Channel, $"Current Project is: " + currentProjectDoc.Url);
        }
    }

    public class CurrentProjectInfo
    {
        public Uri Url { get; set; }
    }
}