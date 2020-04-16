using System.Threading;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Collections;
using Couchbase.Core;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
using MediatR;
using TwitchLib.Client.Models;

namespace MattsTwitchBot.Core.RequestHandlers
{
    public class ModifyProfileHandler : IRequestHandler<ModifyProfile>
    {
        private readonly IBucket _bucket;

        public ModifyProfileHandler(ITwitchBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket();
        }

        public async Task<Unit> Handle(ModifyProfile request, CancellationToken cancellationToken)
        {
            var chatMessage = request.Message;

            if (!await UserExists(chatMessage.Username))
                await CreateUserProfile(chatMessage);

            // update individual profile properties
            if (chatMessage.Message.StartsWith("!profile-shout"))
                await UpdateProperty(chatMessage.Username, "shoutMessage", GetShoutMessage(chatMessage));

            return default;
        }

        private string GetShoutMessage(ChatMessage chatMessage)
        {
            return chatMessage.Message
                .Replace("!profile-shout", "") // remove the command
                .Trim(); // trim any extra space
        }

        private async Task UpdateProperty<T>(string key, string property, T value)
        {
            await _bucket.MutateIn<dynamic>(key.ToLower())
                .Upsert(property, value)
                .ExecuteAsync();
        }

        private async Task CreateUserProfile(ChatMessage chatMessage)
        {
            var doc = new Document<TwitcherProfile>
            {
                Id = chatMessage.Username,
                Content = new TwitcherProfile()
            };
            await _bucket.InsertAsync(doc);
        }

        private async Task<bool> UserExists(string username)
        {
            return await _bucket.ExistsAsync(username);
        }
    }
}