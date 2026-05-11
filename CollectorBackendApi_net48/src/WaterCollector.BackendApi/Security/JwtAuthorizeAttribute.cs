using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json.Linq;

namespace WaterCollector.BackendApi.Security
{
    public sealed class JwtAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any() ||
                actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any())
            {
                return;
            }

            try
            {
                var auth = actionContext.Request.Headers.Authorization;
                if (auth == null || !string.Equals(auth.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                    throw new UnauthorizedAccessException("Authorization Bearer token مطلوب.");

                JObject payload = new BearerTokenService().ValidateToken(auth.Parameter);

                var identity = new ClaimsIdentity("Bearer");
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, payload.Value<string>("sub") ?? string.Empty));
                identity.AddClaim(new Claim(ClaimTypes.Name, payload.Value<string>("name") ?? string.Empty));
                identity.AddClaim(new Claim("full_name", payload.Value<string>("full_name") ?? string.Empty));
                identity.AddClaim(new Claim("collector_id", payload.Value<string>("collector_id") ?? string.Empty));
                identity.AddClaim(new Claim("collector_name", payload.Value<string>("collector_name") ?? string.Empty));

                var principal = new ClaimsPrincipal(identity);
                Thread.CurrentPrincipal = principal;
                actionContext.RequestContext.Principal = principal;
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new { success = false, message = ex.Message }
                );
            }
        }
    }
}
