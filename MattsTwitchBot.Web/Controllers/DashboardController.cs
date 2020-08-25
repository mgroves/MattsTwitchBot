using System.Threading.Tasks;
using MattsTwitchBot.Core.Models;
using MattsTwitchBot.Core.RequestHandlers.Dashboard;
using MattsTwitchBot.Web.Extensions;
using MattsTwitchBot.Web.Filters;
using MattsTwitchBot.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MattsTwitchBot.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("/dashboard")]
        [BearerToken]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = await _mediator.Send(new GetDashboardView());
            
            var model = new DashboardViewModel();
            model.Map(dashboard);
            
            return View(model);
        }

        [Route("/dashboard")]
        [BearerToken]
        [HttpPost]
        public async Task<IActionResult> DashboardPost(string homePageInfo, string staticContentCommands, string triviaMessages)
        {
            // sanity check validation on the JSON
            if(!homePageInfo.IsSaneJsonForType<HomePageInfo>())
                ModelState.AddModelError("homePageInfoJson", "That doesn't look like valid JSON for Home Page Stuff");
            if (!staticContentCommands.IsSaneJsonForType<ValidStaticCommands>())
                ModelState.AddModelError("staticContentCommandsJson", "That doesn't look like valid JSON for Static Content Commands");
            if (!triviaMessages.IsSaneJsonForType<TriviaMessages>())
                ModelState.AddModelError("triviaMessagesJson", "That doesn't look like valid JSON for Trivia Messages");

            // TODO: could also do strict validation checks (i.e. badges is required, commands is required, etc)

            if (!ModelState.IsValid)
            {
                var dashboard = await _mediator.Send(new GetDashboardView());
                var model = new DashboardViewModel();
                model.Map(dashboard);
                return View("Dashboard", model);
            }

            var cmd = new SaveDashboardData(
                JsonConvert.DeserializeObject<HomePageInfo>(homePageInfo),
                JsonConvert.DeserializeObject<ValidStaticCommands>(staticContentCommands),
                JsonConvert.DeserializeObject<TriviaMessages>(triviaMessages)
            );
            await _mediator.Send(cmd);

            return RedirectToAction("Dashboard");
        }
    }
}
