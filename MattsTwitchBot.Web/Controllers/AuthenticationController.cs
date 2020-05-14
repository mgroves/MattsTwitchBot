using System.Linq;
using System.Threading.Tasks;
using MattsTwitchBot.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MattsTwitchBot.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/signin")]
        public async Task<IActionResult> SignIn()
        {
            var returnUrlQuery = Request.Query["ReturnUrl"];
            if (returnUrlQuery.Any())
                ViewBag.ReturnUrl = returnUrlQuery.ToString();

            return View("SignIn");
        }

        [HttpPost("~/signin")]
        public async Task<IActionResult> SignIn([FromForm] string provider, [FromForm] string returnUrl = null)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return Challenge(new AuthenticationProperties {RedirectUri = returnUrl ?? "/"}, provider);
        }

        [HttpGet("~/signout"), HttpPost("~/signout")]
        public IActionResult SignOut()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            return SignOut(new AuthenticationProperties {RedirectUri = "/"},
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}