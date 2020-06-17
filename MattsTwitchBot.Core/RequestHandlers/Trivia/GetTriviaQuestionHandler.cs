using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetTriviaQuestionHandler : IRequestHandler<GetTriviaQuestion, TriviaQuestion>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetTriviaQuestionHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<TriviaQuestion> Handle(GetTriviaQuestion request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            var result = await collection.GetAsync(request.Id);
            var question = result.ContentAs<TriviaQuestion>();
            question.Id = request.Id;
            return question;
        }
    }
}