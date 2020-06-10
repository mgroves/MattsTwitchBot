using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Dashboard
{
    public class SaveDashboardDataHandler : IRequestHandler<SaveDashboardData>
    {
        private readonly IBucket _bucket;

        public SaveDashboardDataHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(SaveDashboardData request, CancellationToken cancellationToken)
        {
            await _bucket.UpsertAsync("homePageInfo", request.HomePageInfo);
            await _bucket.UpsertAsync("staticContentCommands", request.StaticCommandInfo);
            await _bucket.UpsertAsync("triviaMessages", request.TriviaMessages);
            return default;
        }
    }
}