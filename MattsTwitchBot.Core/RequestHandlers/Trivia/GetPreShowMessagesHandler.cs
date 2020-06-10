using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetPreShowMessagesHandler : IRequestHandler<GetPreShowMessages, TriviaMessages>
    {
        private readonly IBucket _bucket;

        public GetPreShowMessagesHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<TriviaMessages> Handle(GetPreShowMessages request, CancellationToken cancellationToken)
        {
            var result = await _bucket.GetAsync<TriviaMessages>("triviaMessages");
            if (!result.Success)
                return null;
            var messages = result.Value;
            if (!messages.ShowMessages)
                return null;
            return messages;
        }
    }
}