using System;
using System.Collections.Generic;
using MattsTwitchBot.Core;

namespace MattsTwitchBot.Tests.IntegrationTests.TestHelpers
{
    public class TestKeyGenerator : IKeyGenerator
    {
        private readonly List<string> _documentsToRemove;

        public TestKeyGenerator(List<string> documentsToRemove)
        {
            _documentsToRemove = documentsToRemove;
        }
        public string NewDocKey()
        {
            var key = Guid.NewGuid().ToString();
            _documentsToRemove.Add(key);
            return key;
        }
    }
}