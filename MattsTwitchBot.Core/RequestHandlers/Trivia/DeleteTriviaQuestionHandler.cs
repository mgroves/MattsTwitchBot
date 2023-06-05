using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class DeleteTriviaQuestionHandler : IRequestHandler<DeleteTriviaQuestion>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public DeleteTriviaQuestionHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(DeleteTriviaQuestion request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("trivia");
            await collection.RemoveAsync(request.Id);
            return default;
        }
    }
}