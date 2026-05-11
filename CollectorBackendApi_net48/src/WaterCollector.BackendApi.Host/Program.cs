using System;
using WaterCollector.BackendApi.Hosting;

namespace WaterCollector.BackendApi.Host
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            using (var host = new ApiHostService())
            {
                try
                {
                    host.Start();
                    Console.WriteLine("WaterCollector.BackendApi is running.");
                    Console.WriteLine("Base URL: " + host.BaseUrl);
                    Console.WriteLine("Health: " + host.BaseUrl.Replace("+", "localhost") + "api/mobile-sync/health");
                    Console.WriteLine("اضغط Enter للإغلاق...");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("فشل تشغيل API:");
                    Console.WriteLine(ex);
                    Console.ReadLine();
                }
            }
        }
    }
}
