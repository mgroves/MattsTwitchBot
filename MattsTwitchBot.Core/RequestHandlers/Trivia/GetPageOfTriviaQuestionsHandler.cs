using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Query;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetPageOfTriviaQuestionsHandler : IRequestHandler<GetPageOfTriviaQuestions, IAsyncEnumerable<TriviaQuestion>>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetPageOfTriviaQuestionsHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<IAsyncEnumerable<TriviaQuestion>> Handle(GetPageOfTriviaQuestions request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var cluster = bucket.Cluster;

            var n1ql = @$"SELECT META(t).id, t.*
                FROM `{bucket.Name}` t
                WHERE t.type = 'trivia'
                ORDER BY t.approved, t.question
                LIMIT $pageSize
                OFFSET $pageNum * $pageSize";

            var result = await cluster.QueryAsync<TriviaQuestion>(n1ql,
                QueryOptions.Create()
                    .Parameter("pageSize", Globals.MANAGE_TRIVIA_PAGE_SIZE)
                    .Parameter("pageNum", request.PageNum));

            return result.Rows;
        }
    }
}