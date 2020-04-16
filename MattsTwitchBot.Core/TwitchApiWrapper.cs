using System.Threading.Tasks;
using TwitchLib.Api.Interfaces;

namespace MattsTwitchBot.Core
{
    public interface ITwitchApiWrapper
    {
        Task<bool> DoesUserExist(string username);
    }

    public class TwitchApiWrapper : ITwitchApiWrapper
    {
        private readonly ITwitchAPI _api;

        public TwitchApiWrapper(ITwitchAPI api)
        {
            _api = api;
        }

        public async Task<bool> DoesUserExist(string username)
        {
            // defensively remove the @ symbol if it somehow makes it here
            var usernameWithNoAt = username.Replace("@", "");

            var result = await _api.V5.Users.GetUserByNameAsync(usernameWithNoAt);
            return result.Total > 0;
        }
    }
}