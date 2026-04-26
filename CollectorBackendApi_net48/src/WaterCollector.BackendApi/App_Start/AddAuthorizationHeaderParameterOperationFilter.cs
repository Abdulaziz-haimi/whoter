using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using Swashbuckle.Swagger;
namespace WaterCollector.BackendApi.App_Start
{
    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation == null)
                return;

            var hasAuthorize =
                apiDescription.ActionDescriptor.GetCustomAttributes<System.Web.Http.AuthorizeAttribute>().Any()
                || apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<System.Web.Http.AuthorizeAttribute>().Any()
                || apiDescription.ActionDescriptor.GetCustomAttributes<WaterCollector.BackendApi.Security.JwtAuthorizeAttribute>().Any()
                || apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<WaterCollector.BackendApi.Security.JwtAuthorizeAttribute>().Any();

            var hasAllowAnonymous =
                apiDescription.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any();

            if (!hasAuthorize || hasAllowAnonymous)
                return;

            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            if (operation.parameters.All(p => p.name != "Authorization"))
            {
                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    type = "string",
                    required = false,
                    description = "Bearer {token}"
                });
            }
        }
    }
}