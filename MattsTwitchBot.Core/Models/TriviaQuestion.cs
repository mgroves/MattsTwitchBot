using System.Collections.Generic;

namespace MattsTwitchBot.Core.Models
{
    public class TriviaQuestion
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int Answer { get; set; }
    }
}