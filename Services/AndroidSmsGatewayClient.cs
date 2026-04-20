using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace water3.Services
{
    public class AndroidSmsGatewayClient
    {
        private readonly HttpClient _http;

        public AndroidSmsGatewayClient()
        {
            _http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
        }

        public string PhoneIp { get; set; } = "192.168.1.20";
        public int Port { get; set; } = 8080;
        public string ApiKey { get; set; } = "123456";

        private string BaseUrl => $"http://{PhoneIp}:{Port}";

        private void ApplyHeaders()
        {
            _http.DefaultRequestHeaders.Remove("X-API-KEY");
            _http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
        }

        public async Task<(bool ok, string raw)> HealthAsync()
        {
            try
            {
                var resp = await _http.GetAsync($"{BaseUrl}/health");
                var raw = await resp.Content.ReadAsStringAsync();
                return (resp.IsSuccessStatusCode, raw);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool ok, string status, string raw, long? outId)> SendSmsAsync(string to, string message)
        {
            try
            {
                ApplyHeaders();

                var payload = new { to, message };
                var json = JsonConvert.SerializeObject(payload);

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var resp = await _http.PostAsync($"{BaseUrl}/send-sms", content);
                    var raw = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                        return (false, "HTTP_" + (int)resp.StatusCode, raw, null);

                    // response: { ok:true, status:"QUEUED", outId:123 }
                    dynamic obj = JsonConvert.DeserializeObject(raw);
                    bool ok = obj.ok == true;
                    string status = obj.status != null ? (string)obj.status : "OK";
                    long? outId = null;
                    try { outId = obj.outId != null ? (long)obj.outId : (long?)null; } catch { }

                    return (ok, status, raw, outId);
                }
            }
            catch (Exception ex)
            {
                return (false, "EXCEPTION", ex.Message, null);
            }
        }
    }
}