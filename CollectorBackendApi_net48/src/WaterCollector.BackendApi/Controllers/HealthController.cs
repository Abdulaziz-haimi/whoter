using System;
using System.Web.Http;

namespace WaterCollector.BackendApi.Controllers
{
    [RoutePrefix("api/health")]
    public sealed class HealthController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                success = true,
                app = "WaterCollector.BackendApi",
                time = DateTime.Now
            });
        }
    }
}
