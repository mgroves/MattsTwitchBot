using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetPageOfTriviaQuestionsHandler : IRequestHandler<GetPageOfTriviaQuestions, List<TriviaQuestion>>
    {
        private readonly IBucket _bucket;

        public GetPageOfTriviaQuestionsHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<List<TriviaQuestion>> Handle(GetPageOfTriviaQuestions request, CancellationToken cancellationToken)
        {
            var n1ql = @$"SELECT META(t).id, t.*
                FROM `{_bucket.Name}` t
                WHERE t.type = 'trivia'
                ORDER BY t.approved, t.question
                LIMIT $pageSize
                OFFSET $pageNum * $pageSize";
            var query = QueryRequest.Create(n1ql);
            query.AddNamedParameter("$pageSize", Globals.MANAGE_TRIVIA_PAGE_SIZE);
            query.AddNamedParameter("$pageNum", request.PageNum);
            var result = await _bucket.QueryAsync<TriviaQuestion>(query, cancellationToken);
            return result.Rows;
        }
    }
}