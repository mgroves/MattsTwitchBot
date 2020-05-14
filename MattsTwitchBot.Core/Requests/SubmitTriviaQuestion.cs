using System.Collections.Generic;
using MediatR;

namespace MattsTwitchBot.Core.Requests
{
    public class SubmitTriviaQuestion : IRequest
    {
        public string Question { get; set; }
        public int Answer { get; set; }
        public List<string> Options { get; set; }
        public bool Approved { get; set; }
        public string Id { get; set; }
        public string SubmittedBy { get; set; }
    }
}