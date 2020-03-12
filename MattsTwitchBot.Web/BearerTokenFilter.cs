using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace MattsTwitchBot.Web
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

            if (!query.ContainsKey("token"))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //var validToken = "mytoken";
            var givenToken = query["token"].FirstOrDefault();
            if (givenToken == null || givenToken != _validToken)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}