using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetRandomTriviaQuestionsHandler : IRequestHandler<GetRandomTriviaQuestions, List<TriviaQuestion>>
    {
        private readonly IBucket _bucket;

        public GetRandomTriviaQuestionsHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<List<TriviaQuestion>> Handle(GetRandomTriviaQuestions request, CancellationToken cancellationToken)
        {
            var n1ql = @"SELECT META(t).id, t.*
                FROM `" + _bucket.Name + @"` t
                WHERE t.type = 'trivia'
                AND t.approved = true
                ORDER BY RANDOM()
                LIMIT $limit;";
            var query = QueryRequest.Create(n1ql);
            query.AddNamedParameter("$limit", request.NumQuestions);
            var result = await _bucket.QueryAsync<TriviaQuestion>(query, cancellationToken);
            return result.Rows;
        }
    }
}