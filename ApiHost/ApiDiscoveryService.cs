using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace water3.ApiHost
{

   
        public sealed class ApiDiscoveryService : IDisposable
        {
            private UdpClient _udp;
            private CancellationTokenSource _cts;

            public bool IsRunning
            {
                get { return _udp != null; }
            }

            public void Start()
            {
                if (_udp != null)
                    return;

                string enabled = ConfigurationManager.AppSettings["DiscoveryEnabled"];
                if (!string.IsNullOrWhiteSpace(enabled) &&
                    enabled.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                int discoveryPort = GetIntSetting("DiscoveryPort", 37020);

                _cts = new CancellationTokenSource();

                _udp = new UdpClient(discoveryPort);
                _udp.EnableBroadcast = true;

                Task.Run(() => ListenLoop(_cts.Token));
            }

            private async Task ListenLoop(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        UdpReceiveResult result = await _udp.ReceiveAsync();

                        string message = Encoding.UTF8.GetString(result.Buffer);

                        if (!message.Equals("WATER_COLLECTOR_DISCOVER", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string apiUrl = BuildApiUrl();

                        string response = "WATER_COLLECTOR_API|" + apiUrl;
                        byte[] bytes = Encoding.UTF8.GetBytes(response);

                        await _udp.SendAsync(bytes, bytes.Length, result.RemoteEndPoint);
                    }
                    catch
                    {
                        if (token.IsCancellationRequested)
                            break;
                    }
                }
            }

            private string BuildApiUrl()
            {
                string scheme = ConfigurationManager.AppSettings["ApiScheme"];
                if (string.IsNullOrWhiteSpace(scheme))
                    scheme = "https";

                string port = ConfigurationManager.AppSettings["ApiPort"];
                if (string.IsNullOrWhiteSpace(port))
                    port = "8443";

                string ip = GetLocalIPv4();

                return scheme + "://" + ip + ":" + port + "/";
            }

            private static string GetLocalIPv4()
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up)
                        continue;

                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                        continue;

                    var props = ni.GetIPProperties();

                    foreach (var address in props.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        string ip = address.Address.ToString();

                        if (ip.StartsWith("169.254."))
                            continue;

                        return ip;
                    }
                }

                return "127.0.0.1";
            }

            private static int GetIntSetting(string key, int defaultValue)
            {
                string value = ConfigurationManager.AppSettings[key];

                int result;
                if (int.TryParse(value, out result))
                    return result;

                return defaultValue;
            }

            public void Stop()
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                    _cts.Dispose();
                    _cts = null;
                }

                if (_udp != null)
                {
                    _udp.Close();
                    _udp = null;
                }
            }

            public void Dispose()
            {
                Stop();
            }
        }
    }