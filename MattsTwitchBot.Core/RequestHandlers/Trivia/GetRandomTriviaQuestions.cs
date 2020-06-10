using System.Collections.Generic;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetRandomTriviaQuestions : IRequest<List<TriviaQuestion>>
    {
        public int NumQuestions { get; }

        public GetRandomTriviaQuestions(int numQuestions = 10)
        {
            NumQuestions = numQuestions;
        }    
    }
}