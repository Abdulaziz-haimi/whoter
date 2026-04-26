using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;
using WaterCollector.BackendApi.App_Start;
using WaterCollector.BackendApi.Controllers;
using WaterCollector.BackendApi.Data;
using WaterCollector.BackendApi.Filters;
using WaterCollector.BackendApi.Services;

namespace WaterCollector.BackendApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var sqlConnectionFactory = new SqlConnectionFactory();
            var authService = new AuthService(sqlConnectionFactory);
            var mobileSyncService = new MobileSyncService(sqlConnectionFactory);

            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new SimpleResolver(
                sqlConnectionFactory,
                authService,
                mobileSyncService);

            config.Filters.Add(new ApiExceptionFilterAttribute());
        }
    }

    internal sealed class SimpleResolver : IDependencyResolver
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly IAuthService _authService;
        private readonly IMobileSyncService _mobileSyncService;

        public SimpleResolver(
            ISqlConnectionFactory connectionFactory,
            IAuthService authService,
            IMobileSyncService mobileSyncService)
        {
            _connectionFactory = connectionFactory;
            _authService = authService;
            _mobileSyncService = mobileSyncService;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ISqlConnectionFactory))
                return _connectionFactory;

            if (serviceType == typeof(IAuthService))
                return _authService;

            if (serviceType == typeof(IMobileSyncService))
                return _mobileSyncService;

            if (serviceType == typeof(AuthController))
                return new AuthController(_authService);

            if (serviceType == typeof(MobileSyncController))
                return new MobileSyncController(_mobileSyncService);

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var service = GetService(serviceType);
            return service == null ? new object[0] : new[] { service };
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}