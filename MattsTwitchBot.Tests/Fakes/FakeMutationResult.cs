using System;
using Couchbase.Core;
using Couchbase.KeyValue;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeMutationResult : IMutationResult
    {
        #region unimplemented
        public MutationToken MutationToken
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ulong Cas => throw new NotImplementedException();

        #endregion
    }
}