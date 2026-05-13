using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace water3.Licensing
{
    public static class LicenseService
    {
        private const string PublicKeyXml = @"PUT_PUBLIC_KEY_HERE";

        public static LicenseStatus CheckCurrentLicense()
        {
            string licenseKey = LicenseStore.Load();

            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                return Invalid("النظام غير مفعل. يرجى إدخال مفتاح التفعيل.");
            }

            return ValidateLicense(licenseKey);
        }

        public static LicenseStatus Activate(string licenseKey)
        {
            LicenseStatus status = ValidateLicense(licenseKey);

            if (status.IsValid)
                LicenseStore.Save(licenseKey);

            return status;
        }

        private static LicenseStatus ValidateLicense(string licenseKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(licenseKey))
                    return Invalid("مفتاح التفعيل فارغ.");

                if (PublicKeyXml == "PUT_PUBLIC_KEY_HERE")
                    return Invalid("لم يتم إعداد مفتاح التحقق العام Public Key داخل النظام.");

                string[] parts = licenseKey.Trim().Split('.');

                if (parts.Length != 2)
                    return Invalid("صيغة مفتاح التفعيل غير صحيحة.");

                byte[] payloadBytes = Base64Url.Decode(parts[0]);
                byte[] signatureBytes = Base64Url.Decode(parts[1]);

                bool signatureOk;

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
                {
                    rsa.FromXmlString(PublicKeyXml);

                    signatureOk = rsa.VerifyData(
                        payloadBytes,
                        CryptoConfig.MapNameToOID("SHA256"),
                        signatureBytes
                    );
                }

                if (!signatureOk)
                    return Invalid("مفتاح التفعيل غير صحيح أو تم التلاعب به.");

                string json = Encoding.UTF8.GetString(payloadBytes);

                LicenseInfo info = JsonConvert.DeserializeObject<LicenseInfo>(json);

                if (info == null)
                    return Invalid("بيانات الترخيص غير صالحة.");

                string currentMachineId = MachineFingerprint.GetMachineId();

                if (!string.Equals(info.MachineId, currentMachineId, StringComparison.OrdinalIgnoreCase))
                {
                    return Invalid(
                        "هذا الترخيص لا يخص هذا الجهاز.\n\n" +
                        "رقم هذا الجهاز:\n" + currentMachineId
                    );
                }

                DateTime expiry;

                if (!DateTime.TryParse(
                    info.ExpiryDateUtc,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal,
                    out expiry))
                {
                    return Invalid("تاريخ انتهاء الترخيص غير صحيح.");
                }

                if (DateTime.UtcNow.Date > expiry.Date)
                {
                    return Invalid(
                        "انتهى اشتراك النظام بتاريخ: " +
                        expiry.ToLocalTime().ToString("yyyy/MM/dd") +
                        "\nيرجى التواصل مع الدعم لتجديد الاشتراك."
                    );
                }

                return new LicenseStatus
                {
                    IsValid = true,
                    Message = "الترخيص صحيح.",
                    Info = info
                };
            }
            catch (Exception ex)
            {
                return Invalid("تعذر قراءة أو التحقق من مفتاح التفعيل.\n\n" + ex.Message);
            }
        }

        private static LicenseStatus Invalid(string message)
        {
            return new LicenseStatus
            {
                IsValid = false,
                Message = message,
                Info = null
            };
        }
    }
}