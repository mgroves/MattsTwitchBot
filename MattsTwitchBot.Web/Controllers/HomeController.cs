using MattsTwitchBot.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        [BearerToken]
        public IActionResult Index()
        {
            return View();
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
