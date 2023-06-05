using System;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class SubmitTriviaQuestionHandler : IRequestHandler<SubmitTriviaQuestion>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public SubmitTriviaQuestionHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(SubmitTriviaQuestion request, CancellationToken cancellationToken)
        {
            var trivia = new TriviaQuestion();
            trivia.Answer = request.Answer;
            trivia.Question = request.Question;
            trivia.Options = request.Options;
            trivia.Approved = request.Approved;
            trivia.SubmittedBy = request.SubmittedBy;

            var id = request.Id;
            if (string.IsNullOrEmpty(id))
                id = Guid.NewGuid().ToString();

            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("trivia");

            await collection.UpsertAsync(id, trivia);

            return default;
        }
    }
}