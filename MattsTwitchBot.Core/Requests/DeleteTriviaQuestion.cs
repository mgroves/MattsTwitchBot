using MediatR;

namespace MattsTwitchBot.Core.Requests
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