using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.StaticCommands
{
    public class StaticCommandsLookupHandler : IRequestHandler<StaticCommandsLookup, ValidStaticCommands>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public StaticCommandsLookupHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<ValidStaticCommands> Handle(StaticCommandsLookup request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            try
            {
                var result = await collection.GetAsync("staticContentCommands");
                return result.ContentAs<ValidStaticCommands>();
            }
            catch
            {
                return new ValidStaticCommands();
            }
        }
    }
}