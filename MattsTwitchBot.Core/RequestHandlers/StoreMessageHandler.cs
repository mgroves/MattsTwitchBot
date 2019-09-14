using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class StoreMessageHandler : IRequestHandler<StoreMessage>
    {
        private readonly IBucket _bucket;

        public StoreMessageHandler(ITwitchBucketProvider twitchBucketProvider)
        {
            _bucket = twitchBucketProvider.GetBucket();
        }

        public Task<Unit> Handle(StoreMessage request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            var result = _bucket.Insert(new Document<dynamic>
            {
                Id = Guid.NewGuid().ToString(),
                Content = message
            });
            Console.WriteLine($"Logged a message from {message.DisplayName}.");
            return default;
        }
    }
}