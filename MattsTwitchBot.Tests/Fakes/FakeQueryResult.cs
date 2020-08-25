using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Couchbase.Core.Retry;
using Couchbase.Query;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeQueryResult<T> : IQueryResult<T>
    {
        public List<T> FakeRows { private get; set; }

        public IAsyncEnumerable<T> Rows => FakeRows.ToAsyncEnumerable();

        #region unimplemented

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public RetryReason RetryReason { get; }
        public QueryMetaData? MetaData { get; }
        public List<Error> Errors { get; }

        #endregion  
    }
}