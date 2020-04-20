using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.Requests;
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

        // ***** EXPERIMENTAL
        [HttpPost]
        [Route("/couchbasenotify")]
        [BearerToken]
        public IActionResult CouchbaseNotify([FromBody] CouchbaseNotification body)
        {
            // this should be triggered by a Couchbase event
            // See NotifyTwitchBot.couchbase.eventing.js!
            return Content(body.Message);
        }
        // *****
    }

    public class CouchbaseNotification
    {
        public string Username { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
    }
}
