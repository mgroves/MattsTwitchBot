using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class GetTotalNumberOfTriviaPagesHandler : IRequestHandler<GetTotalNumberOfTriviaPages, int>
    {
        private readonly IBucket _bucket;

        public GetTotalNumberOfTriviaPagesHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<int> Handle(GetTotalNumberOfTriviaPages request, CancellationToken cancellationToken)
        {
            var n1ql = $"SELECT VALUE COUNT(*) FROM {_bucket.Name} t WHERE t.type = 'trivia'";
            var query = QueryRequest.Create(n1ql);
            var result = await _bucket.QueryAsync<int>(query, cancellationToken);
            var numQuestions = result.Rows.FirstOrDefault() * 1.0;
            return Convert.ToInt32(Math.Ceiling(numQuestions / Globals.MANAGE_TRIVIA_PAGE_SIZE));
        }
    }
}