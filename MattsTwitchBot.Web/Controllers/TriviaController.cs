using System;
using System.Threading.Tasks;
using MattsTwitchBot.Core.RequestHandlers;
using MattsTwitchBot.Core.Requests;
using MattsTwitchBot.Web.Filters;
using MattsTwitchBot.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet]
        [Route("/trivia/submit")]
        public async Task<IActionResult> SubmitTriviaQuestion()
        {
            return View(new TriviaQuestionEditModel());
        }

        [Authorize]
        [HttpPost]
        [Route("/trivia/submit")]
        public async Task<IActionResult> SubmitTriviaQuestion(TriviaQuestionEditModel model)
        {
            model.Validate(ModelState);

            if (ModelState.IsValid)
            {
                var req = model.MapToRequest();
                req.Approved = false;   // always false when first being submitted
                req.SubmittedBy = User.Identity.Name;
                await _mediator.Send(req);
                TempData["Flash"] = "Thank you for submitting a trivia question! Submit another one!";
                return RedirectToAction("SubmitTriviaQuestion");
            }

            return View(model);
        }

        [BearerToken]
        [Route("/trivia/manage")]
        public async Task<IActionResult> ManageTriviaQuestions(int pageNum = 0)
        {
            var respNumPages = await _mediator.Send(new GetTotalNumberOfTriviaPages());
            var respQuestions = await _mediator.Send(new GetPageOfTriviaQuestions(pageNum));
            var viewModel = new ManageTriviaQuestionsViewModel { Questions = respQuestions, CurrentPageNum = pageNum, TotalPages = respNumPages };
            return View(viewModel);
        }

        [BearerToken]
        [HttpGet]
        [Route("/trivia/manage/{id}")]
        public async Task<IActionResult> ManageTriviaQuestion(string id)
        {
            var resp = await _mediator.Send(new GetTriviaQuestion(id));
            var viewModel = new TriviaQuestionEditModel(resp);

            return View(viewModel);
        }

        [BearerToken]
        [HttpPost]
        [Route("/trivia/manage/{id}")]
        public async Task<IActionResult> ManageTriviaQuestion(TriviaQuestionEditModel model)
        {
            model.Validate(ModelState);

            if (ModelState.IsValid)
            {
                var req = model.MapToRequest();
                await _mediator.Send(req);
                TempData["Flash"] = "Trivia question updated";
                return RedirectToAction("ManageTriviaQuestions");
            }

            return View(model);
        }
    }
}