using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MediatR;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class SetCurrentProjectHandler : IRequestHandler<SetCurrentProject>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IOptions<TwitchOptions> _twitchOptions;
        private readonly IBucket _bucket;

        public SetCurrentProjectHandler(ITwitchClient twitchClient, ITwitchBucketProvider twitchBucketProvider, IOptions<TwitchOptions> twitchOptions)
        {
            _twitchClient = twitchClient;
            _twitchOptions = twitchOptions;
            _bucket = twitchBucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(SetCurrentProject request, CancellationToken cancellationToken)
        {
            var message = request.Message;

            // authorization - maybe this should be a separate object or an attribute or something
            if (message.Username != _twitchOptions.Value.Username)
                return Unit.Value;

            // parse out the url from the message and make sure it's valid
            var stripped = message.Message.Replace("!setcurrentproject", "").Trim();
            var isUri = Uri.TryCreate(stripped, UriKind.Absolute, out var uri);
            if (!isUri)
            {
                _twitchClient.SendMessage(message.Channel, "Sorry, I couldn't understand that URL!");
                return Unit.Value;
            }

            // store the current project info
            var currentProjectDocumentKey = "currentProject";
            var info = new CurrentProjectInfo();
            info.Url = uri;

            var result = await _bucket.UpsertAsync(currentProjectDocumentKey, info);
            if (result == null || !result.Success)
                _twitchClient.SendMessage(message.Channel, "I was unable to store that, sorry!");
            else
                _twitchClient.SendMessage(message.Channel, "Okay, got it!");
            return Unit.Value;
        }
    }
}