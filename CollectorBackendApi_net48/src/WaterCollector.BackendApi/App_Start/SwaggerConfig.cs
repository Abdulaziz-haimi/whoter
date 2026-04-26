using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(WaterCollector.BackendApi.App_Start.SwaggerConfig), "Register")]

namespace WaterCollector.BackendApi.App_Start
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "WaterCollector Backend API");

                    c.ApiKey("Bearer")
                        .Description("JWT Authorization header. Example: Bearer {token}")
                        .Name("Authorization")
                        .In("header");

                    c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
                })
                .EnableSwaggerUi(c =>
                {
                });
        }
    }
}