using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class StoreMessageHandler : IRequestHandler<StoreMessage>
    {
        private readonly IBucket _bucket;

        public StoreMessageHandler(ITwitchBucketProvider twitchBucketProvider)
        {
            _bucket = twitchBucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(StoreMessage request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            await _bucket.InsertAsync(new Document<ChatMessage>
            {
                Id = Guid.NewGuid().ToString(),
                Content = message
            });
            return Unit.Value;
        }
    }
}