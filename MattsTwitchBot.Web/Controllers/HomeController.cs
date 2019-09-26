using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
