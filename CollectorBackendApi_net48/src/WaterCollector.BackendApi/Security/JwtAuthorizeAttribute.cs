using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WaterCollector.BackendApi.Security
{
    public sealed class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var auth = actionContext.Request.Headers.Authorization;
            if (auth == null || !string.Equals(auth.Scheme, "Bearer", System.StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(auth.Parameter))
                return false;

            try
            {
                var principal = new BearerTokenService().Validate(auth.Parameter);
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null) HttpContext.Current.User = principal;
                actionContext.RequestContext.Principal = principal;
                return principal.Identity != null && principal.Identity.IsAuthenticated;
            }
            catch
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
            {
                success = false,
                message = "غير مصرح. تحقق من رمز الدخول."
            });
        }
    }
}
