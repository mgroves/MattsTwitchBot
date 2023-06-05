using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
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

            GetUsersResponse userListResponse = await _api.Helix.Users.GetUsersAsync(logins: new List<string> { usernameWithNoAt });

            return userListResponse.Users.Length > 0;
        }
    }
}