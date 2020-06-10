using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Chat;
using MattsTwitchBot.Core.RequestHandlers.Main;
using MattsTwitchBot.Core.RequestHandlers.OneOffs;
using MattsTwitchBot.Core.RequestHandlers.Profile;
using MattsTwitchBot.Core.RequestHandlers.StaticCommands;
using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core
{
    // builds mediatr requests given a twitch chat message
    public class TwitchCommandRequestFactory
    {
        private readonly IMediator _mediator;

        public TwitchCommandRequestFactory(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IRequest> BuildCommand(ChatMessage chatMessage)
        {
            // check for "!" right away, if no "!" then just go
            // directly to StoreMessage
            var messageText = chatMessage.Message;
            if(!messageText.StartsWith("!"))
                return new StoreMessage(chatMessage);

            switch (messageText)
            {
                case var x when x.StartsWith("!help"):
                    return new Help(chatMessage);
                case var x when x.StartsWith("!currentproject"):
                    return new SayCurrentProject(chatMessage);
                case var x when x.StartsWith("!setcurrentproject"):
                    return new SetCurrentProject(chatMessage);
                case var x when x.StartsWith("!so "):
                    return new ShoutOut(chatMessage);
                case var x when x.StartsWith("!profile"):
                    return new ModifyProfile(chatMessage);
                case var x when await IsASoundEffect(x):
                    return new SoundEffect(x.Replace("!", ""));
                case var x when await IsStaticCommand(x):
                    return new StaticMessage(x.Replace("!", ""), chatMessage.Channel);
                case var x when x.StartsWith("!trout"):
                    return new Trout(chatMessage);
                case var x when x.StartsWith("!lurk"):
                    return new Lurk(chatMessage);
                default:
                    return new StoreMessage(chatMessage);
            }
        }

        private async Task<bool> IsStaticCommand(string commandText)
        {
            if (!commandText.StartsWith("!"))
                return false;
            var commandName = commandText.Replace("!", "");
            var validStaticCommands = await _mediator.Send<ValidStaticCommands>(new StaticCommandsLookup());
            return validStaticCommands.IsValid(commandName);
        }

        private async Task<bool> IsASoundEffect(string commandText)
        {
            if (!commandText.StartsWith("!"))
                return false;
            var validSoundEffects = await _mediator.Send<ValidSoundEffects>(new SoundEffectLookup());
            return validSoundEffects.IsValid(commandText);
        }
    }
}