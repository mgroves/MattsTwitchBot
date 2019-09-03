using System;
using System.Text;
using Couchbase.Core;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery.Commands
{
    public class Help : ICommand
    {
        private readonly ChatMessage _message;

        public Help(ChatMessage chatMessage)
        {
            _message = chatMessage;
        }

        public void Execute(IBucket bucket, ITwitchClient client)
        {
            var sendWhisperTo = _message.Username;

            var sb = new StringBuilder();
            sb.AppendLine($"Hello {_message.Username}. Try these commands:");
            sb.AppendLine($"\t!help - Show this message");
            sb.AppendLine($"\t!currentproject - More info about what I'm working on");

            client.SendWhisper(sendWhisperTo, sb.ToString());

            Console.WriteLine($"I just whispered !help to {_message.Username}");
        }
    }
}