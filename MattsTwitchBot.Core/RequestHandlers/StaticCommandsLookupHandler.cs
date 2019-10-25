using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class StaticCommandsLookupHandler : IRequestHandler<StaticCommandsLookup, ValidStaticCommands>
    {
        private readonly IBucket _bucket;

        public StaticCommandsLookupHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<ValidStaticCommands> Handle(StaticCommandsLookup request, CancellationToken cancellationToken)
        {
            var result = await _bucket.GetAsync<ValidStaticCommands>("staticContentCommands");
            if(!result.Success)
                return new ValidStaticCommands();
            return result.Value;
        }
    }
}