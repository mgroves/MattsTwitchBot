using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class SayCurrentProjectHandler : IRequestHandler<SayCurrentProject>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IBucket _bucket;

        public SayCurrentProjectHandler(ITwitchBucketProvider bucketProvider, ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
            _bucket = bucketProvider.GetBucket();
        }

        public Task<Unit> Handle(SayCurrentProject request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            var currentProjectDocumentKey = "currentProject";

            var currentProjectResult = _bucket.Get<CurrentProjectInfo>(currentProjectDocumentKey);
            if (currentProjectResult == null || !currentProjectResult.Success)
            {
                _twitchClient.SendMessage(message.Channel, "I haven't set any current project yet, sorry!");
                return default;
            }

            var currentProjectDoc = currentProjectResult.Value;
            _twitchClient.SendMessage(message.Channel, $"Current Project is: " + currentProjectDoc.Url);
            return default;
        }
    }
}