using System;
using Couchbase;
using Couchbase.Core.Buckets;
using Couchbase.IO;
using Couchbase.IO.Operations;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeOperationResult<T> : IOperationResult<T>
    {
        public T Value { get; set; }
        public bool Success { get; set; }

        #region not used
        public bool ShouldRetry()
        {
            throw new NotImplementedException();
        }

        public string Message { get; }
        public Exception Exception { get; }
        public bool IsNmv()
        {
            throw new NotImplementedException();
        }

        public MutationToken Token { get; }
        public ulong Cas { get; }
        public ResponseStatus Status { get; }
        public Durability Durability { get; set; }
        public string Id { get; }
        public OperationCode OpCode { get; }
        #endregion
    }
}