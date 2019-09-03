using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
