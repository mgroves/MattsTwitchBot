using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers.StaticCommands
{
    public class StaticMessageHandler : IRequestHandler<StaticMessage>
    {
        private readonly IBucket _bucket;
        private readonly ITwitchClient _twitchClient;

        public StaticMessageHandler(ITwitchBucketProvider bucketProvider, ITwitchClient twitchClient)
        {
            _bucket = bucketProvider.GetBucket();
            _twitchClient = twitchClient;
        }

        public async Task<Unit> Handle(StaticMessage request, CancellationToken cancellationToken)
        {
            // assumes that the request is valid

            // look up the content
            var result = await _bucket.GetAsync<ValidStaticCommands>("staticContentCommands");
            var contents = result.Value;
            var command = contents.Commands.First(c => c.Command == request.Command);

            // write the content to the chat room
            _twitchClient.SendMessage(request.Channel, command.Content);

            return default;
        }
    }
}