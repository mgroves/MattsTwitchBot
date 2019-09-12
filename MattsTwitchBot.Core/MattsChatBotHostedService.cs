using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using MattsTwitchBot.Core.CommandQuery;
using MattsTwitchBot.Core.CommandQuery.Commands;
using Microsoft.Extensions.Hosting;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core
{
    public class MattsChatBotHostedService : IHostedService
    {
        private readonly ITwitchClient _twitchClient;
        private readonly IBucket _bucket;
        private readonly ConnectionCredentials _credentials;
        private Commander _commander;

        public MattsChatBotHostedService(ITwitchBucketProvider bucketProvider)
        {
            _credentials = new ConnectionCredentials(Config.Username, Config.OauthKey);
            _twitchClient = new TwitchClient();

            _bucket = bucketProvider.GetBucket();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // setup commander
            _commander = new Commander();

            // start listening to twitch
            _twitchClient.Initialize(_credentials, "matthewdgroves");
            _twitchClient.OnMessageReceived += Client_OnMessageReceived;
            _twitchClient.Connect();

            return Task.CompletedTask;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var command = InstantiateCommand(e.ChatMessage);
            _commander.Execute(command);
        }

        private ICommand InstantiateCommand(ChatMessage chatMessage)
        {
            var messageText = chatMessage.Message;
            switch (messageText)
            {
                case var x when x.StartsWith("!help"):
                    return new Help(chatMessage, _twitchClient);
                case var x when x.StartsWith("!currentproject"):
                    return new CurrentProject(chatMessage, _bucket, _twitchClient);
                case var x when x.StartsWith("!setcurrentproject"):
                    return new SetCurrentProject(chatMessage, _bucket, _twitchClient);
                case var x when x.StartsWith("!so "):
                    return new ShoutOut(chatMessage, _twitchClient);
                default:
                    return new StoreMessage(chatMessage,_bucket);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _twitchClient.Disconnect();
            return Task.CompletedTask;
        }
    }
}
