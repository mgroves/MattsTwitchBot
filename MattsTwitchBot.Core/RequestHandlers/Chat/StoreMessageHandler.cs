using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Chat
{
    public class StoreMessageHandler : IRequestHandler<StoreMessage>
    {
        private readonly ITwitchBucketProvider _bucketProvider;
        private readonly IKeyGenerator _keyGen;

        public StoreMessageHandler(ITwitchBucketProvider bucketProvider, IKeyGenerator keyGen)
        {
            _bucketProvider = bucketProvider;
            _keyGen = keyGen;
        }

        public async Task<Unit> Handle(StoreMessage request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("messages");

            var message = request.Message;
            await collection.InsertAsync(_keyGen.NewDocKey(), message);
            return default;
        }
    }
}