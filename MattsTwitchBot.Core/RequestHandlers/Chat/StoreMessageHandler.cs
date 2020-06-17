using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Chat
{
    public class StoreMessageHandler : IRequestHandler<StoreMessage>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public StoreMessageHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(StoreMessage request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            var message = request.Message;
            await collection.InsertAsync(Guid.NewGuid().ToString(), message);
            return default;
        }
    }
}