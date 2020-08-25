using System;
using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Dashboard;
using MattsTwitchBot.Web.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("/")]
        [BearerToken]
        public async Task<IActionResult> Index()
        {
            var homePageInfo = await _mediator.Send<HomePageInfo>(new GetHomePageInfo());
            return View(homePageInfo);
        }
    }
}
