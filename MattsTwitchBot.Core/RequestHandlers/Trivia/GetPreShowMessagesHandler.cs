using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetPreShowMessagesHandler : IRequestHandler<GetPreShowMessages, TriviaMessages>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetPreShowMessagesHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<TriviaMessages> Handle(GetPreShowMessages request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("config");

            try
            {
                var result = await collection.GetAsync("triviaMessages");
                var messages = result.ContentAs<TriviaMessages>();
                return messages;
            }
            catch
            {
                return null;
            }
        }
    }
}