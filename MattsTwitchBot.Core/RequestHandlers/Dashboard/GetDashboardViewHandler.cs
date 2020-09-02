using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Dashboard
{
    public class GetDashboardViewHandler : IRequestHandler<GetDashboardView, DashboardView>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public GetDashboardViewHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<DashboardView> Handle(GetDashboardView request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();
            var cluster = bucket.Cluster;

            // create homepageinfo and staticcontent and pre-show message docs if they don't already exist
            if (!(await collection.ExistsAsync("homePageInfo")).Exists)
                await collection.InsertAsync("homePageInfo", new HomePageInfo());
            if (!(await collection.ExistsAsync("staticContentCommands")).Exists)
                await collection.InsertAsync("staticContentCommands", new HomePageInfo());
            if (!(await collection.ExistsAsync("triviaMessages")).Exists)
                await collection.InsertAsync("triviaMessages", new TriviaMessages());
            if (!(await collection.ExistsAsync("chatNotificationInfo")).Exists)
                await collection.InsertAsync("chatNotificationInfo", new ChatNotificationInfo());

            // get the homepageinfo and staticcontent and trivia message docs with three KV calls
            var homePageInfo = (await collection.GetAsync("homePageInfo")).ContentAs<HomePageInfo>();
            var staticContentCommands = (await collection.GetAsync("staticContentCommands")).ContentAs<ValidStaticCommands>();
            var triviaMessages = (await collection.GetAsync("triviaMessages")).ContentAs<TriviaMessages>();
            var chatNotificationInfo = (await collection.GetAsync("chatNotificationInfo")).ContentAs<ChatNotificationInfo>();

            // get a list of everyone with a profile with N1QL
            var profilesN1ql = $"SELECT RAW META(t).id FROM `{bucket.Name}` t WHERE t.type = 'profile'";
            var profilesResult = await cluster.QueryAsync<string>(profilesN1ql);
            var profiles = profilesResult.Rows;

            // stuff everything into a DTO type object to return
            var view = new DashboardView();
            view.HomePageInfo = homePageInfo;
            view.StaticContentCommands = staticContentCommands;
            view.Profiles = profiles == null ? null : await profiles.ToListAsync(cancellationToken);
            view.TriviaMessages = triviaMessages;
            view.ChatNotificationInfo = chatNotificationInfo;
            return view;
        }
    }
}