using System;
using System.Collections;
using System.Collections.Generic;
using Couchbase.N1QL;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeQueryResult<T> : IQueryResult<T>
    {
        public List<T> FakeRows { private get; set; }

        public List<T> Rows => FakeRows;


        #region not implemented
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ShouldRetry()
        {
            throw new NotImplementedException();
        }

        public bool Success { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Guid RequestId { get; }
        public string ClientContextId { get; }
        public dynamic Signature { get; }
        public QueryStatus Status { get; }
        public List<Error> Errors { get; }
        public List<Warning> Warnings { get; }
        public Metrics Metrics { get; }
        #endregion
    }
}