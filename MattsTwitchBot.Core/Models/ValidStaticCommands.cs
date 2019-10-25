using System.Collections.Generic;
using System.Linq;

namespace MattsTwitchBot.Core.Models
{
    public class ValidStaticCommands
    {
        public List<StaticCommandInfo> Commands { get; set; }

        public bool IsValid(string soundEffect)
        {
            if (Commands == null)
                return false;
            if (!Commands.Any())
                return false;
            return Commands.Any(x => x.Command.ToLower() == soundEffect.ToLower());
        }
    }

    public class StaticCommandInfo
    {
        public string Command { get; set; }
        public string Content { get; set; }
    }
}