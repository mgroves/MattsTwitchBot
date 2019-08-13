using Couchbase.Core;
using TwitchLib.Client;

namespace MattsTwitchBot.CommandQuery
{
    public class Commander : ICommander
    {
        private readonly IBucket _bucket;
        private readonly TwitchClient _twitch;

        public Commander(IBucket bucket, TwitchClient twitch)
        {
            _bucket = bucket;
            _twitch = twitch;
        }

        public void Execute(ICommand command)
        {
            command.Execute(_bucket, _twitch);
        }

        public T Query<T>(IQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}