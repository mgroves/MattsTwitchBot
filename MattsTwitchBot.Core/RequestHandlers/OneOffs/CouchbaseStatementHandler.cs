using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers.OneOffs
{
    public class CouchbaseStatementHandler : IRequestHandler<CouchbaseStatement>
    {
        private readonly ITwitchClient _twitchClient;

        public CouchbaseStatementHandler(ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
        }

        public Task<Unit> Handle(CouchbaseStatement request, CancellationToken cancellationToken)
        {
            _twitchClient.SendMessage(request.Message.Channel, "Do you mean 'Couchbase'? It's not the same thing as CouchDb! Check out this diagram: https://stackoverflow.com/a/44418950/40015");

            return default;
        }
    }
}