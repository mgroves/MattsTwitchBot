using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetTotalNumberOfTriviaPagesHandler : IRequestHandler<GetTotalNumberOfTriviaPages, int>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetTotalNumberOfTriviaPagesHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<int> Handle(GetTotalNumberOfTriviaPages request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var cluster = bucket.Cluster;

            var n1ql = $"SELECT VALUE COUNT(*) FROM {bucket.Name}._default.trivia";
            var result = await cluster.QueryAsync<int>(n1ql);
            var numQuestions = (await result.Rows.FirstOrDefaultAsync(cancellationToken: cancellationToken)) * 1.0;
            return Convert.ToInt32(Math.Ceiling(numQuestions / Globals.MANAGE_TRIVIA_PAGE_SIZE));
        }
    }
}