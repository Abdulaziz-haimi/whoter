using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WaterCollector.BackendApi.Contracts.MobileSync;
using WaterCollector.BackendApi.Security;
using WaterCollector.BackendApi.Services;

namespace WaterCollector.BackendApi.Controllers
{
    [RoutePrefix("api/mobile-sync")]
    public sealed class MobileSyncController : ApiController
    {
        private readonly IMobileSyncService _mobileSyncService;

        public MobileSyncController()
            : this(ApiServiceFactory.CreateMobileSyncService())
        {
        }

        public MobileSyncController(IMobileSyncService mobileSyncService)
        {
            if (mobileSyncService == null)
                throw new ArgumentNullException(nameof(mobileSyncService));

            _mobileSyncService = mobileSyncService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("health")]
        public IHttpActionResult Health()
        {
            return Ok(new { success = true, message = "API is running.", time = DateTime.Now });
        }

        [JwtAuthorize]
        [HttpGet]
        [Route("receivables")]
        public async Task<IHttpActionResult> GetReceivables([FromUri] DateTime? asOfDate = null)
        {
            try
            {
                var response = await _mobileSyncService.ExportReceivablesAsync(GetCollectorId(), asOfDate).ConfigureAwait(false);
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

        [JwtAuthorize]
        [HttpPost]
        [Route("upload-batch")]
        public async Task<IHttpActionResult> UploadBatch([FromBody] UploadBatchRequest request)
        {
            if (request == null)
                return BadRequest("بيانات الدفعة مطلوبة.");

            try
            {
                var response = await _mobileSyncService.UploadBatchAsync(GetCollectorId(), request).ConfigureAwait(false);
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

        [JwtAuthorize]
        [HttpGet]
        [Route("import-decisions")]
        public async Task<IHttpActionResult> GetImportDecisions([FromUri] string deviceCode, [FromUri] DateTime? changedAfter = null)
        {
            if (string.IsNullOrWhiteSpace(deviceCode))
                return BadRequest("deviceCode مطلوب.");

            try
            {
                var response = await _mobileSyncService.GetImportDecisionsAsync(GetCollectorId(), deviceCode.Trim(), changedAfter).ConfigureAwait(false);
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
        private int GetCollectorId()
        {
            var principal = User as ClaimsPrincipal;

            if (principal == null)
                throw new UnauthorizedAccessException("رمز الدخول غير صالح.");

            // مهم: نبحث عن رقم المحصل فقط، ولا نستخدم NameIdentifier لأنه غالبًا UserID
            string value =
                principal.Claims.FirstOrDefault(c => c.Type == "collector_id")?.Value ??
                principal.Claims.FirstOrDefault(c => c.Type == "collectorId")?.Value ??
                principal.Claims.FirstOrDefault(c => c.Type == "CollectorID")?.Value;

            int collectorId;

            if (!int.TryParse(value, out collectorId) || collectorId <= 0)
                throw new UnauthorizedAccessException("collector_id غير موجود داخل الرمز.");

            return collectorId;
        }
        //private int GetCollectorId()
        //{
        //    var principal = User as ClaimsPrincipal;

        //    string value = principal?.Claims.FirstOrDefault(c =>
        //        c.Type == "collector_id" || c.Type == "CollectorID" || c.Type == ClaimTypes.NameIdentifier
        //    )?.Value;

        //    int collectorId;
        //    if (!int.TryParse(value, out collectorId) || collectorId <= 0)
        //        throw new UnauthorizedAccessException("collector_id غير موجود داخل الرمز.");

        //    return collectorId;
        //}
    }
}
