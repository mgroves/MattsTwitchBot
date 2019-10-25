using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.Requests
{
    public class StaticCommandsLookup : IRequest<ValidStaticCommands>, IRequest<Unit>
    {

    }
}