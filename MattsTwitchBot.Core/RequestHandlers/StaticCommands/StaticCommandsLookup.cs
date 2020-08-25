using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.StaticCommands
{
    public class StaticCommandsLookup : IRequest<ValidStaticCommands>, IRequest<Unit>
    {

    }
}