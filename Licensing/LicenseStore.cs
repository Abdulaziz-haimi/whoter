using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace water3.Licensing
{
    public static class LicenseStore
    {
        private static string FolderPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Water3",
                    "License"
                );
            }
        }

        private static string FilePath
        {
            get { return Path.Combine(FolderPath, "license.dat"); }
        }

        public static void Save(string licenseKey)
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            byte[] plain = Encoding.UTF8.GetBytes(licenseKey);
            byte[] protectedData = ProtectedData.Protect(
                plain,
                null,
                DataProtectionScope.LocalMachine
            );

            File.WriteAllBytes(FilePath, protectedData);
        }

        public static string Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return null;

                byte[] protectedData = File.ReadAllBytes(FilePath);
                byte[] plain = ProtectedData.Unprotect(
                    protectedData,
                    null,
                    DataProtectionScope.LocalMachine
                );

                return Encoding.UTF8.GetString(plain);
            }
            catch
            {
                return null;
            }
        }

        public static void Delete()
        {
            try
            {
                if (File.Exists(FilePath))
                    File.Delete(FilePath);
            }
            catch
            {
            }
        }
    }
}
