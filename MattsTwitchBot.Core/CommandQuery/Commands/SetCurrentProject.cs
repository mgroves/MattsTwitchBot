using System;
using Couchbase.Core;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery.Commands
{
    public class SetCurrentProject : ICommand
    {
        private readonly ChatMessage _message;

        public SetCurrentProject(ChatMessage chatMessage)
        {
            _message = chatMessage;
        }

        public void Execute(IBucket bucket, ITwitchClient client)
        {
            // authorization - maybe this should be a seperate object or an attribute or something
            if (_message.Username != Config.Username)
                return;

            // parse out the url from the message and make sure it's valid
            var stripped = _message.Message.Replace("!setcurrentproject","").Trim();
            var isUri = Uri.TryCreate(stripped, UriKind.Absolute, out var uri);
            if (!isUri)
            {
                client.SendMessage(_message.Channel, "Sorry, I couldn't understand that URL!");
                return;
            }

            // store the current project info
            var currentProjectDocumentKey = "currentProject";
            var info = new CurrentProjectInfo();
            info.Url = uri;

            var result = bucket.Upsert(currentProjectDocumentKey, info);
            if(result == null || !result.Success)
                client.SendMessage(_message.Channel, "I was unable to store that, sorry!");
            else
                client.SendMessage(_message.Channel, "Okay, got it!");
        }
    }
}