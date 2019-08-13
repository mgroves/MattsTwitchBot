using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using MattsTwitchBot.CommandQuery;
using MattsTwitchBot.CommandQuery.Commands;
using Moq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace MattsTwitchBot
{
    class Program
    {
        private static Commander _commander;

        static void Main(string[] args)
        {
            // connect to couchbase
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://localhost:8091") }
            });
            cluster.Authenticate("Administrator", "password");
            var bucket = cluster.OpenBucket("twitchchat");
            //var bucket = new Mock<IBucket>().Object;

            // connect to twitch
            var credentials = new ConnectionCredentials(Config.Username, Config.OauthKey);
            var twitchClient = new TwitchClient();

            // setup commander
            _commander = new Commander(bucket, twitchClient);

            // start listening to twitch
            twitchClient.Initialize(credentials, "matthewdgroves");
            twitchClient.OnMessageReceived += Client_OnMessageReceived;
            twitchClient.Connect();

            // don't end until I press enter
            Console.WriteLine("Press ENTER to stop this madness.");
            Console.ReadLine();

            cluster.Dispose();
            twitchClient.Disconnect();
        }

        // listen for Twitch messages and store them in Couchbase
        private static void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var command = InstantiateCommand(e.ChatMessage);
            _commander.Execute(command);
        }

        private static ICommand InstantiateCommand(ChatMessage chatMessage)
        {
            var messageText = chatMessage.Message;
            switch (messageText)
            {
                case var x when x.StartsWith("!help"):
                    return new Help(chatMessage);
                case var x when x.StartsWith("!currentproject"):
                    return new CurrentProject(chatMessage);
                case var x when x.StartsWith("!setcurrentproject"):
                    return new SetCurrentProject(chatMessage);
                default:
                    return new StoreMessage(chatMessage);
            }
        }
    }
}
