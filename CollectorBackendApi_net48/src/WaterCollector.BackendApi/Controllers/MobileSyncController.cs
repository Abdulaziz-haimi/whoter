using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WaterCollector.BackendApi.Contracts.MobileSync;
using WaterCollector.BackendApi.Security;
using WaterCollector.BackendApi.Services;

namespace WaterCollector.BackendApi.Controllers
{
    [JwtAuthorize]
    [RoutePrefix("api/mobile-sync")]
    public sealed class MobileSyncController : ApiController
    {
        private readonly IMobileSyncService _mobileSyncService;

        public MobileSyncController(IMobileSyncService mobileSyncService)
        {
            _mobileSyncService = mobileSyncService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("health")]
        public IHttpActionResult Health() => Ok(new { success = true, message = "API is running." });

        [HttpGet]
        [Route("receivables")]
        public async Task<IHttpActionResult> GetReceivables([FromUri] DateTime? asOfDate = null)
        {
            var response = await _mobileSyncService.ExportReceivablesAsync(GetCollectorId(), asOfDate).ConfigureAwait(false);
            return Ok(response);
        }

        [HttpPost]
        [Route("upload-batch")]
        public async Task<IHttpActionResult> UploadBatch([FromBody] UploadBatchRequest request)
        {
            var response = await _mobileSyncService.UploadBatchAsync(GetCollectorId(), request).ConfigureAwait(false);
            return Ok(response);
        }

        [HttpGet]
        [Route("import-decisions")]
        public async Task<IHttpActionResult> GetImportDecisions([FromUri] string deviceCode, [FromUri] DateTime? changedAfter = null)
        {
            if (string.IsNullOrWhiteSpace(deviceCode)) throw new InvalidOperationException("deviceCode مطلوب.");
            var response = await _mobileSyncService.GetImportDecisionsAsync(GetCollectorId(), deviceCode, changedAfter).ConfigureAwait(false);
            return Ok(response);
        }

        private int GetCollectorId()
        {
            var principal = User as ClaimsPrincipal;
            var value = principal?.Claims.FirstOrDefault(c => c.Type == "collector_id")?.Value;
            if (!int.TryParse(value, out var collectorId) || collectorId <= 0)
                throw new UnauthorizedAccessException("collector_id غير موجود داخل الرمز.");
            return collectorId;
        }
    }
}
