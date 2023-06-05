using System;
using Couchbase.KeyValue;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeGetResult : IGetResult
    {
        private readonly object _content;

        public FakeGetResult(object content)
        {
            _content = content;
        }

        public T ContentAs<T>()
        {
            return (T) _content;
        }

        #region unimplemented
        public ulong Cas => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TimeSpan? Expiry => throw new NotImplementedException();
        public DateTime? ExpiryTime { get; }

        #endregion  
    }
}