using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Dashboard
{
    public class GetHomePageInfoHandler : IRequestHandler<GetHomePageInfo, HomePageInfo>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetHomePageInfoHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<HomePageInfo> Handle(GetHomePageInfo request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = await bucket.CollectionAsync("config");

            try
            {
                var result = await collection.GetAsync("homePageInfo");
                return result.ContentAs<HomePageInfo>();
            }
            catch
            {
                return new HomePageInfo();
            }
        }
    }
}