using System.Linq;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery.Commands
{
    public class ShoutOut : ICommand
    {
        private readonly ChatMessage _message;
        private readonly ITwitchClient _client;
        private readonly ITwitchApiWrapper _api;

        public ShoutOut(ChatMessage message, ITwitchClient client, ITwitchApiWrapper api)
        {
            _message = message;
            _client = client;
            _api = api;
        }

        // TODO: refactor this, because I'll have to change Execute every time any command
        // needs another dependency!
        public async void Execute()
        {
            if (!_message.IsSubscriber)
                return;

            var userToShout = ParseUserNameFromCommand(_message.Message);

            // if there is no username specified, then bail out
            if (string.IsNullOrEmpty(userToShout))
                return;

            if (!await _api.DoesUserExist(userToShout))
                return;

            var thisChannel = _message.Channel;
            var message = $"Hey everyone, check out @{userToShout}'s Twitch stream at https://twitch.tv/{userToShout}";

            _client.SendMessage(thisChannel, message);
        }

        private string ParseUserNameFromCommand(string message)
        {
            return message
                .Replace("!so", "")
                .Trim()
                .Split(' ')
                .FirstOrDefault();
        }
    }
}