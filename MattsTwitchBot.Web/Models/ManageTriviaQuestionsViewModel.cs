using System.Collections.Generic;
using MattsTwitchBot.Core.Models;

namespace MattsTwitchBot.Web.Models
{
    public class ManageTriviaQuestionsViewModel
    {
        public List<TriviaQuestion> Questions { get; set; }
        public int CurrentPageNum { get; set; }
        public int TotalPages { get; set; }
    }
}