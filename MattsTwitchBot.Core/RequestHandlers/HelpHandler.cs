using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class HelpHandler : IRequestHandler<Help>
    {
        private readonly ITwitchClient _twitchClient;
        private Dictionary<string, string> _helpMessages;

        public HelpHandler(ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
            _helpMessages = new Dictionary<string, string>();
            _helpMessages.Add("!help", "Try these commands: !help !currentproject !so !profile !laugh !rimshot !badumtss - You can also get specific help. Example: !help rimshot");
            _helpMessages.Add("!help currentproject", "!currentproject will announce a URL for more information about the current live coding project.");
            _helpMessages.Add("!help so", "!so <username> will shout out the user (subscribers only)");
            _helpMessages.Add("!help profile", "!profile will create a user profile for you. !profile-shout <message> will set your shout out message.");
            _helpMessages.Add("!help laugh", "!laugh causes a laugh sound effect to be played on the stream (max once every 5 minutes)");
            _helpMessages.Add("!help rimshot", "!rimshot causes a rimshot sound effect to be played on the stream (max once every 5 minutes)");
            _helpMessages.Add("!help badumtss", "!badumtss causes a rimshot sound effect to be played on the stream (max once every 5 minutes)");
        }

        public Task<Unit> Handle(Help request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            var sendWhisperTo = message.Username;

            if (_helpMessages.ContainsKey(message.Message))
                SendWhisper(sendWhisperTo, _helpMessages[message.Message]);
            else
                SendWhisper(sendWhisperTo, _helpMessages["!help"]);

            return Unit.Task;
        }

        private void SendWhisper(string sendWhisperTo, string helpMessage)
        {
            _twitchClient.SendWhisper(sendWhisperTo, helpMessage);
        }
    }
}