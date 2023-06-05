using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Trivia;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MattsTwitchBot.Web.Models
{
    public class TriviaQuestionEditModel
    {
        public TriviaQuestionEditModel()
        {
            
        }
        public TriviaQuestionEditModel(TriviaQuestion resp)
        {
            this.Id = resp.Id;
            this.Question = resp.Question;
            this.Answer = resp.Answer;
            this.Option0 = resp.Options[0];
            this.Option1 = resp.Options[1];
            this.Option2 = resp.Options[2];
            this.Option3 = resp.Options[3];
            this.Approved = resp.Approved;
            this.SubmittedBy = resp.SubmittedBy;
        }

        public string? Id { get; set; }
        public string Question { get; set; }
        public int? Answer { get; set; }
        public string Option0 { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public bool Approved { get; set; }
        public string? SubmittedBy { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            if(string.IsNullOrEmpty(Question))
                modelState.AddModelError("QuestionNullOrEmpty","Question is required");
            if(string.IsNullOrEmpty(Option0) || string.IsNullOrEmpty(Option1) || string.IsNullOrEmpty(Option2) || string.IsNullOrEmpty(Option3))
                modelState.AddModelError("OptionNullOrEmpty", "Four options are required");
            if(!Answer.HasValue || Answer.Value > 3 || Answer.Value < 0)
                modelState.AddModelError("AnswerNumberInvalid", "Please select the correct answer");
        }

        public SubmitTriviaQuestion MapToRequest()
        {
            var req = new SubmitTriviaQuestion();
            req.Question = this.Question;
            req.Answer = this.Answer.Value;
            req.Options = new List<string> {this.Option0, this.Option1, this.Option2, this.Option3};
            req.Approved = this.Approved;
            req.Id = this.Id;
            req.SubmittedBy = this.SubmittedBy;
            return req;
        }
    }
}