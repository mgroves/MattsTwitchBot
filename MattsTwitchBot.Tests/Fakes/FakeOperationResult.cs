using Couchbase.Core;
using Couchbase.Core.IO.Operations;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeOperationResult<T> : IOperationResult<T>
    {
        private OpCode _opCode;
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
        public ulong Cas { get; set; }
        public ResponseStatus Status { get; }
        public Durability Durability { get; set; }
        public string Id { get; }

        OpCode IOperationResult.OpCode => _opCode;

        #endregion

        public T? Content { get; }
    }
}