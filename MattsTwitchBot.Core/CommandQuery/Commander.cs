using Couchbase.Core;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.CommandQuery
{
    public class Commander : ICommander
    {
        private readonly IBucket _bucket;
        private readonly ITwitchClient _twitch;

        public Commander(IBucket bucket, ITwitchClient twitch)
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