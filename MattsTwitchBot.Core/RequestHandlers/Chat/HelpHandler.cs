using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.StaticCommands;
using MediatR;
using TwitchLib.Client.Interfaces;

namespace MattsTwitchBot.Core.RequestHandlers.Chat
{
    public class HelpHandler : IRequestHandler<Help>
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IMediator _mediator;

        public HelpHandler(ITwitchClient twitchClient, IMediator mediator)
        {
            _twitchClient = twitchClient;
            _mediator = mediator;
       }

        public async Task<Unit> Handle(Help request, CancellationToken cancellationToken)
        {
            var helpMessages = await BuildHelpMessageDictionary();

            var message = request.Message;
            var channel = request.Message.Channel;

            if (helpMessages.ContainsKey(message.Message))
                _twitchClient.SendMessage(channel, helpMessages[message.Message]);
            else
                _twitchClient.SendMessage(channel, helpMessages["!help"]);

            return default;
        }

        private async Task<Dictionary<string, string>> BuildHelpMessageDictionary()
        {
            var helpMessages = new Dictionary<string, string>();
            
            // hardcoded commands
            var commands = new List<string>();
            commands.Add("!help");
            commands.Add("!currentproject");
            commands.Add("!so");
            commands.Add("!profile");
            commands.Add("!laugh");
            commands.Add("!rimshot");
            commands.Add("!badumtss");
            commands.Add("!trout");
            commands.Add("!lurk");

            // static content commands
            var staticCommands = await _mediator.Send<ValidStaticCommands>(new StaticCommandsLookup());
            staticCommands.Commands.ForEach(c => commands.Add("!" + c.Command));
            staticCommands.Commands.ForEach(c => helpMessages.Add("!help " + c.Command,"This command displays an interesting message."));

            helpMessages.Add("!help", $"Try these commands: {string.Join(" ", commands)} - You can also get specific help. Example: !help rimshot");
            helpMessages.Add("!help currentproject", "!currentproject will announce a URL for more information about the current live coding project.");
            helpMessages.Add("!help so", "!so <username> will shout out the user (subscribers and mods only)");
            helpMessages.Add("!help profile", "!profile will create a user profile for you. !profile-shout <message> will set your shout out message.");
            helpMessages.Add("!help laugh", "!laugh causes a laugh sound effect to be played on the stream (max once every 5 minutes)");
            helpMessages.Add("!help rimshot", "!rimshot causes a rimshot sound effect to be played on the stream (max once every 5 minutes)");
            helpMessages.Add("!help badumtss", "!badumtss causes a rimshot sound effect to be played on the stream (max once every 5 minutes)");
            helpMessages.Add("!help trout", "!trout <username> to slap someone with a trout");
            helpMessages.Add("!help lurk", "!lurk just to let me know that you're here but lurking");
            return helpMessages;
        }
    }
}