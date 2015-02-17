using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApi.NoOwin.Filters
{
    public class CustomAuthenticationFilter : FilterAttribute, IAuthenticationFilter
    {
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Upn, "User"),
                new Claim(ClaimTypes.Role, "NonAdmin")
            };
            var id = new ClaimsIdentity(claims, "Token");
            var principal = new ClaimsPrincipal(new[] { id });
            // The request message contains valid credential
            context.Principal = principal;
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }

    public class CustomAuthorizationFilter : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = actionContext.Request.GetRequestContext().Principal as ClaimsPrincipal;
            return principal.Claims.Any(c => c.Type == ClaimTypes.Upn && c.Value == "User");
        }
    }
}