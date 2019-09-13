using System;
using System.Text;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.CommandQuery.Commands
{
    public class Help : ICommand
    {
        private readonly ChatMessage _message;
        private readonly ITwitchClient _client;

        public Help(ChatMessage chatMessage, ITwitchClient client)
        {
            _message = chatMessage;
            _client = client;
        }

        public void Execute()
        {
            var sendWhisperTo = _message.Username;

            var sb = new StringBuilder();
            sb.AppendLine($"Hello {_message.Username}. Try these commands:");
            sb.AppendLine($"!help !currentproject !so");

            _client.SendWhisper(sendWhisperTo, sb.ToString());

            Console.WriteLine($"I just whispered !help to {_message.Username}");
        }
    }
}