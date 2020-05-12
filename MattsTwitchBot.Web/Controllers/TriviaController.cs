using System.Threading.Tasks;
using MattsTwitchBot.Core.Requests;
using MattsTwitchBot.Web.Filters;
using MattsTwitchBot.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web.Controllers
{
    public class TriviaController : Controller
    {
        private readonly IMediator _mediator;

        public TriviaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("/trivia")]
        [BearerToken]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/gettriviadata")]
        [BearerToken]
        public async Task<IActionResult> GetRandomTriviaQuestions()
        {
            var resp = await _mediator.Send(new GetRandomTriviaQuestions());
            var viewModel = new TriviaQuestionViewModel {Questions = resp};
            return Ok(viewModel);
        }

        // TODO: create a (public) page to let people submit their own questions
        // - use oauth?
    }
}