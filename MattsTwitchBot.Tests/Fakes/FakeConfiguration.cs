using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace MattsTwitchBot.Tests.Fakes
{
    public class FakeConfiguration : IConfiguration
    {
        public FakeConfiguration()
        {
            FakeValues = new Dictionary<string, object>();
        }

        public Dictionary<string, object> FakeValues { get; set; }

        public IConfigurationSection GetSection(string key)
        {
            return new FakeConfigurationSection { Value = FakeValues[key].ToString()};
        }

        #region unimplemented
        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public string this[string key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        #endregion
    }
}