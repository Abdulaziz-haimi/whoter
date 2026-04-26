using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace WaterCollector.BackendApi.Filters
{
    public sealed class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var status = HttpStatusCode.InternalServerError;
            if (context.Exception is UnauthorizedAccessException) status = HttpStatusCode.Unauthorized;
            else if (context.Exception is InvalidOperationException || context.Exception is ArgumentException) status = HttpStatusCode.BadRequest;

            context.Response = context.Request.CreateResponse(status, new
            {
                success = false,
                message = context.Exception.Message
            });
        }
    }
}
