using System;

namespace WaterCollector.BackendApi.Contracts.Auth
{
    public sealed class LoginRequest
    {
        public LoginRequest()
        {
            UserName = string.Empty;
            Password = string.Empty;
            DeviceCode = string.Empty;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string AppVersion { get; set; }
    }

    public sealed class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int CollectorId { get; set; }
        public string CollectorName { get; set; }
        public string DeviceCode { get; set; }
    }
}
