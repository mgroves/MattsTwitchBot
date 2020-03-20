using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace MattsTwitchBot.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BearerTokenAttribute : TypeFilterAttribute
    {
        public BearerTokenAttribute() : base(typeof(BearerTokenFilter))
        {
        }
    }

    public class BearerTokenFilter : IAuthorizationFilter
    {
        private readonly string _validToken;

        public BearerTokenFilter(IConfiguration config)
        {
            _validToken = config.GetValue<string>("BearerToken");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var query = context?.HttpContext?.Request?.Query;

            if (query == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var urlHasToken = query.ContainsKey("token");
            var doesCookieExist = context?.HttpContext?.Request?.Cookies?.ContainsKey("bearertoken") ?? false;

            if (!urlHasToken && !doesCookieExist)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (urlHasToken)
            {
                var givenToken = query["token"].FirstOrDefault();
                if (string.IsNullOrEmpty(givenToken) || givenToken != _validToken)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // put it into a cookie too for next time
                context.HttpContext.Response.Cookies.Append("bearertoken", _validToken);
                return;
            }

            var cookie = context?.HttpContext?.Request?.Cookies["bearertoken"];

            if (string.IsNullOrEmpty(cookie) || cookie != _validToken)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // either they have the valid token in the URL or in the cookie
            // so let them in!
            return;
        }
    }
}