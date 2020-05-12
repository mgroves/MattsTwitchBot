using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class GetTriviaQuestionHandler : IRequestHandler<GetTriviaQuestion, TriviaQuestion>
    {
        private readonly IBucket _bucket;

        public GetTriviaQuestionHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<TriviaQuestion> Handle(GetTriviaQuestion request, CancellationToken cancellationToken)
        {
            var result = await _bucket.GetAsync<TriviaQuestion>(request.Id);
            var question = result.Value;
            question.Id = request.Id;
            return question;
        }
    }
}