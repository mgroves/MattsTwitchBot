using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class DeleteTriviaQuestion : IRequest
    {
        public string Id { get; }

        public DeleteTriviaQuestion(string id)
        {
            Id = id;
        }
    }
}