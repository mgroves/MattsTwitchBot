using Couchbase.Core;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.CommandQuery.Commands
{
    public class Null : ICommand
    {
        public void Execute(IBucket bucket, ITwitchClient client)
        {
            // do nothing
        }
    }
}