namespace WaterCollector.BackendApi.Services
{

    public sealed class LoginRequest
    {
        public LoginRequest()
        {
            UserName = string.Empty;
            Password = string.Empty;
            DeviceCode = string.Empty;
            DeviceName = null;
            DeviceModel = null;
            AppVersion = null;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string AppVersion { get; set; }
    }
}