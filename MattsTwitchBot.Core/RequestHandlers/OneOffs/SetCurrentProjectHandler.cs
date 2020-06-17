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
        private readonly ITwitchBucketProvider _bucketProvider;
        private readonly IOptions<TwitchOptions> _twitchOptions;

        public SetCurrentProjectHandler(ITwitchClient twitchClient, ITwitchBucketProvider bucketProvider, IOptions<TwitchOptions> twitchOptions)
        {
            _twitchClient = twitchClient;
            _bucketProvider = bucketProvider;
            _twitchOptions = twitchOptions;
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

            try
            {
                // store the current project info
                var currentProjectDocumentKey = "currentProject";
                var info = new CurrentProjectInfo();
                info.Url = uri;

                var bucket = await _bucketProvider.GetBucketAsync();
                var collection = bucket.DefaultCollection();
                await collection.UpsertAsync(currentProjectDocumentKey, info);
                _twitchClient.SendMessage(message.Channel, "Okay, got it!");
            }
            catch
            {
                _twitchClient.SendMessage(message.Channel, "I was unable to store that, sorry!");
            }
            return default;
        }
    }
}