using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace MattsTwitchBot.Core
{
    public class ChatWebPageHub : Hub<IChatWebPageHub>
    {
        public async Task ReceiveFanfare(FanfareInfo fanfareInfo)
        {
            await Clients.All.ReceiveFanfare(fanfareInfo);
        }

        public async Task ReceiveSoundEffect(string soundEffectName)
        {
            await Clients.All.ReceiveSoundEffect(soundEffectName);
        }
    }

    public interface IChatWebPageHub
    {
        Task ReceiveFanfare(FanfareInfo fanfareInfo);
        Task ReceiveSoundEffect(string soundEffectName);
    }
}