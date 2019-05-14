using System;
using System.Collections.Generic;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace MattsTwitchBot
{
    class Program
    {
        private static IBucket _bucket;
        private static TwitchClient _client;

        static void Main(string[] args)
        {
            // connect to couchbase
            var cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri("http://localhost:8091") }
            });
            cluster.Authenticate("Administrator", "password");
            _bucket = cluster.OpenBucket("twitchchat");

            // connect to twitch
            var credentials = new ConnectionCredentials(Config.Username, Config.OauthKey);
            _client = new TwitchClient();
            _client.Initialize(credentials, "matthewdgroves");
            _client.OnMessageReceived += Client_OnMessageReceived;
            _client.Connect();

            Console.WriteLine("Press ENTER to stop this madness.");
            Console.ReadLine();

            cluster.Dispose();
            _client.Disconnect();
        }

        // listen for Twitch messages and store them in Couchbase
        private static void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            _bucket.Insert(new Document<dynamic>
            {
                Id = Guid.NewGuid().ToString(),
                Content = e.ChatMessage
            });
            Console.WriteLine($"Logged a message from {e.ChatMessage.DisplayName}.");
        }
    }
}
