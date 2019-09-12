using Couchbase.Core;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.CommandQuery
{
    public class Commander : ICommander
    {
        public void Execute(ICommand command)
        {
            command.Execute();
        }

        public T Query<T>(IQuery query)
        {
            throw new System.NotImplementedException();
        }
    }
}