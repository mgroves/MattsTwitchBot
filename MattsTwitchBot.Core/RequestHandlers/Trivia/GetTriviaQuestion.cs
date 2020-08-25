using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetTriviaQuestion : IRequest<TriviaQuestion>
    {
        public string Id { get; }

        public GetTriviaQuestion(string id)
        {
            Id = id;
        }
    }
}