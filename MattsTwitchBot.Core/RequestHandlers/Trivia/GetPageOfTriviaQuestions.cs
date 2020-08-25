using System.Collections.Generic;
using MattsTwitchBot.Core.Models;
using MediatR;

namespace MattsTwitchBot.Core.RequestHandlers.Trivia
{
    public class GetPageOfTriviaQuestions : IRequest<IAsyncEnumerable<TriviaQuestion>>
    {
        public int PageNum { get; }

        public GetPageOfTriviaQuestions(int? pageNum)
        {
            PageNum = pageNum ?? 0;
        }
    }
}