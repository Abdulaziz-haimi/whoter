using System;
using System.IO;
using Newtonsoft.Json;

namespace water3.Services
{
    public class SmsGatewaySettings
    {
        public string PhoneIp { get; set; } = "192.168.1.20";
        public int Port { get; set; } = 8080;
        public string ApiKey { get; set; } = "123456";

        public static string GetFilePath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "water3"
            );
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, "sms_gateway.json");
        }

        public static SmsGatewaySettings Load()
        {
            try
            {
                var path = GetFilePath();
                if (!File.Exists(path)) return new SmsGatewaySettings();

                var json = File.ReadAllText(path);
                var s = JsonConvert.DeserializeObject<SmsGatewaySettings>(json);
                return s ?? new SmsGatewaySettings();
            }
            catch
            {
                return new SmsGatewaySettings();
            }
        }

        public void Save()
        {
            var path = GetFilePath();
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}