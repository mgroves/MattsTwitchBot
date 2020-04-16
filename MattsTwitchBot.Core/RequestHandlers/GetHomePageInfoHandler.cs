using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class GetHomePageInfoHandler : IRequestHandler<GetHomePageInfo, HomePageInfo>
    {
        private readonly IBucket _bucket;

        public GetHomePageInfoHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<HomePageInfo> Handle(GetHomePageInfo request, CancellationToken cancellationToken)
        {
            var homePageInfoExists = await _bucket.ExistsAsync("homePageInfo");
            if(!homePageInfoExists)
                return new HomePageInfo();

            var result = await _bucket.GetAsync<HomePageInfo>("homePageInfo");
            if(!result.Success)
                return new HomePageInfo();

            return result.Value;
        }
    }
}