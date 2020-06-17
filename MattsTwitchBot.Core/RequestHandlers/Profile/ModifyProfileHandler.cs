using System.Threading;
using System.Threading.Tasks;
using Couchbase.KeyValue;
using MattsTwitchBot.Core.Models;
using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers.Profile
{
    public class ModifyProfileHandler : IRequestHandler<ModifyProfile>
    {
        private readonly ITwitchBucketProvider _bucketProvider;

        public ModifyProfileHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public async Task<Unit> Handle(ModifyProfile request, CancellationToken cancellationToken)
        {
            var bucket = await _bucketProvider.GetBucketAsync();
            var collection = bucket.DefaultCollection();

            var chatMessage = request.Message;

            if (!await UserExists(chatMessage.Username, collection))
                await CreateUserProfile(chatMessage, collection);

            // update individual profile properties
            if (chatMessage.Message.StartsWith("!profile-shout"))
                await UpdateProperty(chatMessage.Username, "shoutMessage", GetShoutMessage(chatMessage), collection);

            return default;
        }

        private string GetShoutMessage(ChatMessage chatMessage)
        {
            return chatMessage.Message
                .Replace("!profile-shout", "") // remove the command
                .Trim(); // trim any extra space
        }

        private async Task UpdateProperty<T>(string key, string property, T value, ICouchbaseCollection collection)
        {
            await collection.MutateInAsync(key.ToLower(), 
                builder => builder.Upsert(property, value), options => { });
        }

        private async Task CreateUserProfile(ChatMessage chatMessage, ICouchbaseCollection collection)
        {
            await collection.InsertAsync(chatMessage.Username, new TwitcherProfile());
        }

        private async Task<bool> UserExists(string username, ICouchbaseCollection collection)
        {
            var result = await collection.ExistsAsync(username);
            return result.Exists;
        }
    }
}