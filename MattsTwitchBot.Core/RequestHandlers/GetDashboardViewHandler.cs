using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class GetDashboardViewHandler : IRequestHandler<GetDashboardView, DashboardView>
    {
        private readonly IBucket _bucket;

        public GetDashboardViewHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<DashboardView> Handle(GetDashboardView request, CancellationToken cancellationToken)
        {
            // create homepageinfo and staticcontent docs if they don't already exist
            if (!(await _bucket.ExistsAsync("homePageInfo")))
                await _bucket.InsertAsync("homePageInfo", new HomePageInfo());
            if (!(await _bucket.ExistsAsync("staticContentCommands")))
                await _bucket.InsertAsync("staticContentCommands", new HomePageInfo());

            // get the homepageinfo and staticcontent docs with two KV calls
            var homePageInfo = (await _bucket.GetAsync<HomePageInfo>("homePageInfo")).Value;
            var staticContentCommands = (await _bucket.GetAsync<ValidStaticCommands>("staticContentCommands")).Value;

            // get a list of everyone with a profile with a N1QL
            var profilesN1ql = $"SELECT RAW META(t).id FROM `{_bucket.Name}` t WHERE t.type = 'profile'";
            var profilesQuery = QueryRequest.Create(profilesN1ql);
            var profilesResult = await _bucket.QueryAsync<string>(profilesQuery, cancellationToken);
            var profiles = profilesResult.Rows;

            // stuff everything into a DTO type object to return
            var view = new DashboardView();
            view.HomePageInfo = homePageInfo;
            view.StaticContentCommands = staticContentCommands;
            view.Profiles = profiles;
            return view;
        }
    }
}