using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class SubmitTriviaQuestionHandler : IRequestHandler<SubmitTriviaQuestion>
    {
        private readonly IBucket _bucket;

        public SubmitTriviaQuestionHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
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

            await _bucket.UpsertAsync(id, trivia);

            return default;
        }
    }
}