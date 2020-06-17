using System.Threading.Tasks;
using Couchbase;
using MattsTwitchBot.Core;

namespace MattsTwitchBot.Tests.IntegrationTests.TestHelpers
{
    public class IntegrationTestBucketProvider : ITwitchBucketProvider
    {
        private readonly IBucket _bucket;

        public IntegrationTestBucketProvider(IBucket bucket)
        {
            _bucket = bucket;
        }

        public string BucketName => _bucket.Name;

        public ValueTask<IBucket> GetBucketAsync()
        {
            return new ValueTask<IBucket>(_bucket);
        }
    }
}