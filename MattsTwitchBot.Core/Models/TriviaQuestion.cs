using System.Collections.Generic;

namespace MattsTwitchBot.Core.Models
{
    public class TriviaQuestion
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int Answer { get; set; }
        public bool Approved { get; set; }
        public string Type => "trivia";
        public string SubmittedBy { get; set; }

        public bool ShouldSerializeId()
        {
            return false;
        }
    }
}