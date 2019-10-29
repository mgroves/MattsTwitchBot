using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MattsTwitchBot.Core
{
    public class ChatWebPageHub : Hub<IChatWebPageHub>
    {
        public async Task ReceiveFanfare(string username)
        {
            await Clients.All.ReceiveFanfare(username);
        }

        public async Task ReceiveSoundEffect(string soundEffectName)
        {
            await Clients.All.ReceiveSoundEffect(soundEffectName);
        }
    }

    public interface IChatWebPageHub
    {
        Task ReceiveFanfare(string username);
        Task ReceiveSoundEffect(string soundEffectName);
    }
}