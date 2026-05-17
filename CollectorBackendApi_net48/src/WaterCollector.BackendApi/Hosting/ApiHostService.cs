using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace WaterCollector.BackendApi.Hosting
{
    public sealed class ApiHostService : IDisposable
    {
        private IDisposable _server;

        public string BaseUrl { get; private set; }

        public bool IsRunning
        {
            get { return _server != null; }
        }

        public void Start()
        {
            if (_server != null)
                return;

            string enabled = ConfigurationManager.AppSettings["ApiEnabled"];

            if (!string.IsNullOrWhiteSpace(enabled) &&
                enabled.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string scheme = ConfigurationManager.AppSettings["ApiScheme"];
            if (string.IsNullOrWhiteSpace(scheme))
                scheme = "https";

            scheme = scheme.Trim().ToLowerInvariant();

            if (scheme != "http" && scheme != "https")
                throw new InvalidOperationException("ApiScheme يجب أن يكون http أو https فقط.");

            string host = ConfigurationManager.AppSettings["ApiHost"];
            if (string.IsNullOrWhiteSpace(host))
                host = "+";

            string port = ConfigurationManager.AppSettings["ApiPort"];
            if (string.IsNullOrWhiteSpace(port))
                port = "8443";

            BaseUrl = scheme + "://" + host + ":" + port + "/";

            // مهم جدًا:
            // لا تستخدم WebApp.Start(BaseUrl)
            // لأنها تعتمد على البحث التلقائي عن Startup
            _server = WebApp.Start<Startup>(BaseUrl);
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
/*using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace WaterCollector.BackendApi.Hosting
{
    public sealed class ApiHostService : IDisposable
    {
        private IDisposable _server;

        public string BaseUrl { get; private set; }
        public bool IsRunning => _server != null;

        public void Start()
        {
            if (_server != null)
                return;

            string enabled = ConfigurationManager.AppSettings["ApiEnabled"];
            if (!string.IsNullOrWhiteSpace(enabled) && enabled.Equals("false", StringComparison.OrdinalIgnoreCase))
                return;

            string port = ConfigurationManager.AppSettings["ApiPort"];
            if (string.IsNullOrWhiteSpace(port))
                port = "8443";

            string host = ConfigurationManager.AppSettings["ApiHost"];
            if (string.IsNullOrWhiteSpace(host))
                host = "+";

            BaseUrl = "https://" + host + ":" + port + "/";
            _server = WebApp.Start<Startup>(BaseUrl);
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
*/