using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Query;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetRandomTriviaQuestionsHandler : IRequestHandler<GetRandomTriviaQuestions, IAsyncEnumerable<TriviaQuestion>>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetRandomTriviaQuestionsHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<IAsyncEnumerable<TriviaQuestion>> Handle(GetRandomTriviaQuestions request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var cluster = bucket.Cluster;

            var n1ql = @"SELECT META(t).id, t.*
                FROM `" + bucket.Name + @"`._default.trivia t
                WHERE t.approved = true
                ORDER BY RANDOM()
                LIMIT $limit;";

            var results = await cluster.QueryAsync<TriviaQuestion>(n1ql,
                QueryOptions.Create()
                    .Parameter("limit", request.NumQuestions));

            return results.Rows;
        }
    }
}