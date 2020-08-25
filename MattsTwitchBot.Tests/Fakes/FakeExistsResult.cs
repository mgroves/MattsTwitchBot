using System;
using Couchbase.KeyValue;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeExistsResult : IExistsResult
    {
        public FakeExistsResult(bool exists)
        {
            Exists = exists;
        }

        public bool Exists { get; }

        #region unimplemented
        public ulong Cas
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}