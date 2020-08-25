using System;

namespace MattsTwitchBot.Core
{
    public interface IKeyGenerator
    {
        string NewDocKey();
    }

    public class KeyGenerator : IKeyGenerator
    {
        public string NewDocKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}