using Couchbase.Core;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.CommandQuery
{
    public interface ICommand
    {
        void Execute(IBucket bucket, ITwitchClient client);
    }
}