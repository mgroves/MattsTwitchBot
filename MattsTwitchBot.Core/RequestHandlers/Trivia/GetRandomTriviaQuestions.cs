using System.Collections.Generic;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetRandomTriviaQuestions : IRequest<IAsyncEnumerable<TriviaQuestion>>
    {
        public int NumQuestions { get; }

        public GetRandomTriviaQuestions(int numQuestions = 10)
        {
            NumQuestions = numQuestions;
        }    
    }
}