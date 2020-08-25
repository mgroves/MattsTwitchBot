using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class SayCurrentProjectHandler : IRequestHandler<SayCurrentProject>
    {
        private readonly ITwitchBucketProvider _bucketProvider;
        private readonly ITwitchClient _twitchClient;

        public SayCurrentProjectHandler(ITwitchBucketProvider bucketProvider, ITwitchClient twitchClient)
        {
            _bucketProvider = bucketProvider;
            _twitchClient = twitchClient;
        }

        public async Task<Unit> Handle(SayCurrentProject request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();
            var message = request.Message;
            var currentProjectDocumentKey = "currentProject";

            try
            {
                var currentProjectResult = await collection.GetAsync(currentProjectDocumentKey);
                var currentProjectDoc = currentProjectResult.ContentAs<CurrentProjectInfo>();
                _twitchClient.SendMessage(message.Channel, $"Current Project is: {currentProjectDoc.Url}");
                return default;
            }
            catch
            {
                _twitchClient.SendMessage(message.Channel, "I haven't set any current project yet, sorry!");
                return default;
            }
        }
    }
}