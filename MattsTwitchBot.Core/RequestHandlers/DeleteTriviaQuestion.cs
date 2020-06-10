using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class DeleteTriviaQuestionHandler : IRequestHandler<DeleteTriviaQuestion>
    {
        private readonly IBucket _bucket;

        public DeleteTriviaQuestionHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(DeleteTriviaQuestion request, CancellationToken cancellationToken)
        {
            await _bucket.RemoveAsync(request.Id);
            return default;
        }
    }
}