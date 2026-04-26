using System.Threading.Tasks;
using System.Web.Http;
using WaterCollector.BackendApi.Services;


namespace WaterCollector.BackendApi.Controllers
{
    [RoutePrefix("api/auth")]
    public sealed class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
