using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WaterCollector.BackendApi.Contracts.Auth;
using WaterCollector.BackendApi.Services;

namespace WaterCollector.BackendApi.Controllers
{
    [RoutePrefix("api/auth")]
    public sealed class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        public AuthController()
            : this(ApiServiceFactory.CreateAuthService())
        {
        }

        public AuthController(IAuthService authService)
        {
            if (authService == null)
                throw new ArgumentNullException(nameof(authService));

            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest("بيانات تسجيل الدخول مطلوبة.");

            try
            {
                var response = await _authService.LoginAsync(request).ConfigureAwait(false);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Content(HttpStatusCode.Unauthorized, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
