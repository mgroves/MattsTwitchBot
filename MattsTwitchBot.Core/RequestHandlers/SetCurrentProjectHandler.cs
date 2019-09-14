using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class SetCurrentProjectHandler : IRequestHandler<SetCurrentProject>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IBucket _bucket;

        public SetCurrentProjectHandler(ITwitchClient twitchClient, ITwitchBucketProvider twitchBucketProvider)
        {
            _twitchClient = twitchClient;
            _bucket = twitchBucketProvider.GetBucket();
        }

        public Task<Unit> Handle(SetCurrentProject request, CancellationToken cancellationToken)
        {
            var message = request.Message;

            // authorization - maybe this should be a seperate object or an attribute or something
            if (message.Username != Config.Username)
                return default;

            // parse out the url from the message and make sure it's valid
            var stripped = message.Message.Replace("!setcurrentproject", "").Trim();
            var isUri = Uri.TryCreate(stripped, UriKind.Absolute, out var uri);
            if (!isUri)
            {
                _twitchClient.SendMessage(message.Channel, "Sorry, I couldn't understand that URL!");
                return default;
            }

            // store the current project info
            var currentProjectDocumentKey = "currentProject";
            var info = new CurrentProjectInfo();
            info.Url = uri;

            var result = _bucket.Upsert(currentProjectDocumentKey, info);
            if (result == null || !result.Success)
                _twitchClient.SendMessage(message.Channel, "I was unable to store that, sorry!");
            else
                _twitchClient.SendMessage(message.Channel, "Okay, got it!");
            return default;
        }
    }
}