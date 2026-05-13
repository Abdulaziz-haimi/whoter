using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;

namespace water3.Licensing
{
    public static class MachineFingerprint
    {
        public static string GetMachineId()
        {
            string machineGuid = "";

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("MachineGuid");
                        if (value != null)
                            machineGuid = value.ToString();
                    }
                }
            }
            catch
            {
                machineGuid = "";
            }

            string raw = Environment.MachineName + "|" + Environment.UserDomainName + "|" + machineGuid;

            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                return BitConverter.ToString(hash).Replace("-", "").Substring(0, 32);
            }
        }
    }
}
