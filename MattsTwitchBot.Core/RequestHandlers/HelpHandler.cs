using System;
using System.Text;
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

        public HelpHandler(ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
        }

        public Task<Unit> Handle(Help request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            var sendWhisperTo = message.Username;

            // TODO: maybe this should provide specific help with commands too
            // e.g. "!help so" whispers help on what "!so" does

            var sb = new StringBuilder();
            sb.AppendLine($"Hello {sendWhisperTo}. Try these commands:");
            sb.AppendLine($"!help !currentproject !so !profile !laugh !rimshot !badumtss");

            _twitchClient.SendWhisper(sendWhisperTo, sb.ToString());

            Console.WriteLine($"I just whispered !help to {sendWhisperTo}");

            return default;
        }
    }
}