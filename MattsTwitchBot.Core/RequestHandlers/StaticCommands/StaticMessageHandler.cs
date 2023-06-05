using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers.StaticCommands
{
    public class StaticMessageHandler : IRequestHandler<StaticMessage>
    {
        private readonly ITwitchBucketProvider _bucketProvider;
        private readonly ITwitchClient _twitchClient;

        public StaticMessageHandler(ITwitchBucketProvider bucketProvider, ITwitchClient twitchClient)
        {
            _bucketProvider = bucketProvider;
            _twitchClient = twitchClient;
        }

        public async Task<Unit> Handle(StaticMessage request, CancellationToken cancellationToken)
        {
            // assumes that the request is valid

            // look up the content
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("config");
            var result = await collection.GetAsync("staticContentCommands");
            var contents = result.ContentAs<ValidStaticCommands>();
            var command = contents.Commands.First(c => c.Command == request.Command);

            // write the content to the chat room
            _twitchClient.SendMessage(request.Channel, command.Content);

            return default;
        }
    }
}