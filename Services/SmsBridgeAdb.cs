
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace water3.Services
{
    public static class SmsBridgeAdb
    {
        // عدل المسار إذا adb ليس في PATH
        // مثال: @"C:\Android\platform-tools\adb.exe"
        public static string AdbPath = "adb";

        public const string Action = "com.water3.SEND_SMS_CMD";
        public const string ReceiverComponent = "com.water3.smsbridge1/.SmsCommandReceiver";
        public const string Token = "WATER3_SECRET_2026";

        public static (bool ok, string output) SendSms(string phone, string message, string reqId, int simSlot = 1)
        {
            // cmd (escape quotes)
            string Esc(string s) => (s ?? "").Replace("\"", "\\\"");

            // Windows CMD يحتاج quoting مضبوط
            string args =
                $"shell am broadcast -n {ReceiverComponent} " +
                $"-a {Action} " +
                $"--es phone \"{Esc(phone)}\" " +
                $"--es message \"{Esc(message)}\" " +
                $"--es reqId \"{Esc(reqId)}\" " +
                $"--es token \"{Token}\" " +
                $"--ei simSlot {simSlot}";

            return RunAdb(args);
        }

        private static (bool ok, string output) RunAdb(string args)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = AdbPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using (var p = Process.Start(psi))
                {
                    string stdout = p.StandardOutput.ReadToEnd();
                    string stderr = p.StandardError.ReadToEnd();
                    p.WaitForExit();

                    string all = (stdout + "\n" + stderr).Trim();

                    // نجاح broadcast لا يعني SMS نجحت، لكن يعني الأمر وصل للتطبيق
                    bool ok = p.ExitCode == 0 && (stdout.Contains("Broadcast completed") || stdout.Contains("result="));
                    return (ok, all);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}