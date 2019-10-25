using MediatR;

namespace MattsTwitchBot.Core.Requests
{
    public class StaticMessage : IRequest
    {
        public string Command { get; }
        public string Channel { get; }

        public StaticMessage(string command, string channel)
        {
            Channel = channel;
            Command = command;
        }
    }
}