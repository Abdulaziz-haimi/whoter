using WaterCollector.BackendApi.Data;

namespace WaterCollector.BackendApi.Services
{
    public static class ApiServiceFactory
    {
        public static IAuthService CreateAuthService()
        {
            return new AuthService(new SqlConnectionFactory());
        }

        public static IMobileSyncService CreateMobileSyncService()
        {
            return new MobileSyncService(new SqlConnectionFactory());
        }
    }
}
