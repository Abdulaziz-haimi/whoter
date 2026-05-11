using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace water3.DB
{
    public class DatabaseProvisioner
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public bool UseWindowsAuth { get; set; }
        public string SqlUser { get; set; }
        public string SqlPassword { get; set; }

        public string BuildMasterConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = "master",
                TrustServerCertificate = true,
                MultipleActiveResultSets = true,
                ConnectTimeout = 10
            };

            if (UseWindowsAuth)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = SqlUser;
                builder.Password = SqlPassword;
                builder.IntegratedSecurity = false;
            }

            return builder.ConnectionString;
        }

        public string BuildApplicationConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = DatabaseName,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true,
                ConnectTimeout = 10
            };

            if (UseWindowsAuth)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = SqlUser;
                builder.Password = SqlPassword;
                builder.IntegratedSecurity = false;
            }

            return builder.ConnectionString;
        }

        public void TestServerConnection()
        {
            using (var con = new SqlConnection(BuildMasterConnectionString()))
            {
                con.Open();
            }
        }

        public void CreateDatabaseIfNotExists()
        {
            ValidateDatabaseName(DatabaseName);

            using (var con = new SqlConnection(BuildMasterConnectionString()))
            {
                con.Open();

                string sql = $@"
IF DB_ID(N'{EscapeSqlLiteral(DatabaseName)}') IS NULL
BEGIN
    CREATE DATABASE [{EscapeSqlIdentifier(DatabaseName)}];
END";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InstallSchema()
        {
            string scriptPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "DB",
                "Scripts",
                "01_Schema.sql"
            );

            if (!File.Exists(scriptPath))
                throw new FileNotFoundException("لم يتم العثور على ملف سكربت قاعدة البيانات", scriptPath);

            string sql = File.ReadAllText(scriptPath, Encoding.UTF8);

            // استبدال اسم قاعدة البيانات الافتراضي باسم القاعدة الذي اختاره المستخدم
            sql = Regex.Replace(
                sql,
                @"USE\s+\[WaterBillingDB\]",
                $"USE [{EscapeSqlIdentifier(DatabaseName)}]",
                RegexOptions.IgnoreCase
            );

            var batches = SplitSqlBatches(sql);

            using (var con = new SqlConnection(BuildApplicationConnectionString()))
            {
                con.Open();

                foreach (string batch in batches)
                {
                    string cleaned = batch.Trim();

                    if (string.IsNullOrWhiteSpace(cleaned))
                        continue;

                    // تجاهل إنشاء المستخدم water_api لأنه خاص ببيئة معينة وقد يفشل إذا لم يكن Login موجودًا
                    if (ShouldSkipEnvironmentSpecificBatch(cleaned))
                        continue;

                    using (var cmd = new SqlCommand(cleaned, con))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SeedBasicData(string adminUserName, string adminFullName, string adminPasswordHash, decimal unitPrice, decimal serviceFees)
        {
            using (var con = new SqlConnection(BuildApplicationConnectionString()))
            {
                con.Open();

                string sql = @"
SET NOCOUNT ON;

------------------------------------------------------------
-- الأدوار الأساسية
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Admin')
    INSERT INTO dbo.Roles(RoleName) VALUES (N'Admin');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Accountant')
    INSERT INTO dbo.Roles(RoleName) VALUES (N'Accountant');

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = N'Collector')
    INSERT INTO dbo.Roles(RoleName) VALUES (N'Collector');

DECLARE @AdminRoleID INT;
SELECT @AdminRoleID = RoleID FROM dbo.Roles WHERE RoleName = N'Admin';

------------------------------------------------------------
-- المستخدم المدير
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserName = @AdminUserName)
BEGIN
    INSERT INTO dbo.Users
    (
        UserName,
        FullName,
        PasswordHash,
        RoleID,
        IsActive,
        CreatedDate
    )
    VALUES
    (
        @AdminUserName,
        @AdminFullName,
        @AdminPasswordHash,
        @AdminRoleID,
        1,
        GETDATE()
    );
END

------------------------------------------------------------
-- الحسابات الأساسية
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountCode = N'1100')
    INSERT INTO dbo.Accounts(AccountCode, AccountName, AccountType, IsControl, ParentAccountID)
    VALUES (N'1100', N'الصندوق', N'Asset', 0, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountCode = N'1200')
    INSERT INTO dbo.Accounts(AccountCode, AccountName, AccountType, IsControl, ParentAccountID)
    VALUES (N'1200', N'ذمم المشتركين', N'Asset', 1, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountCode = N'4100')
    INSERT INTO dbo.Accounts(AccountCode, AccountName, AccountType, IsControl, ParentAccountID)
    VALUES (N'4100', N'إيرادات المياه', N'Revenue', 0, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountCode = N'4110')
    INSERT INTO dbo.Accounts(AccountCode, AccountName, AccountType, IsControl, ParentAccountID)
    VALUES (N'4110', N'إيرادات رسوم الخدمة', N'Revenue', 0, NULL);

DECLARE
    @CashAccountID INT,
    @ReceivableAccountID INT,
    @WaterRevenueAccountID INT,
    @ServiceRevenueAccountID INT;

SELECT @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1100';
SELECT @ReceivableAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1200';
SELECT @WaterRevenueAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'4100';
SELECT @ServiceRevenueAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'4110';

------------------------------------------------------------
-- حسابات النظام
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.SystemAccounts2 WHERE ID = 1)
BEGIN
    INSERT INTO dbo.SystemAccounts2
    (
        ID,
        CashAccountID,
        ReceivableControlAccountID,
        WaterRevenueAccountID,
        ServiceRevenueAccountID
    )
    VALUES
    (
        1,
        @CashAccountID,
        @ReceivableAccountID,
        @WaterRevenueAccountID,
        @ServiceRevenueAccountID
    );
END
ELSE
BEGIN
    UPDATE dbo.SystemAccounts2
    SET
        CashAccountID = @CashAccountID,
        ReceivableControlAccountID = @ReceivableAccountID,
        WaterRevenueAccountID = @WaterRevenueAccountID,
        ServiceRevenueAccountID = @ServiceRevenueAccountID
    WHERE ID = 1;
END

------------------------------------------------------------
-- التعرفة الأساسية
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.BillingConstants)
BEGIN
    INSERT INTO dbo.BillingConstants
    (
        EffectiveFrom,
        UnitPrice,
        DefaultServiceFees,
        ServiceFees,
        IsActive,
        Notes,
        CreatedAt
    )
    VALUES
    (
        CAST(GETDATE() AS DATE),
        @UnitPrice,
        @ServiceFees,
        @ServiceFees,
        1,
        N'تعرفة تم إنشاؤها تلقائيًا عند إعداد النظام',
        GETDATE()
    );
END

------------------------------------------------------------
-- إعدادات الرسائل
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.MessageSettings WHERE SettingName = N'EnableSMS')
    INSERT INTO dbo.MessageSettings(SettingName, SettingValue, Description)
    VALUES (N'EnableSMS', N'0', N'تفعيل أو تعطيل الرسائل');

IF NOT EXISTS (SELECT 1 FROM dbo.MessageSettings WHERE SettingName = N'SendInvoiceSMS')
    INSERT INTO dbo.MessageSettings(SettingName, SettingValue, Description)
    VALUES (N'SendInvoiceSMS', N'0', N'إرسال رسالة عند إصدار الفاتورة');

IF NOT EXISTS (SELECT 1 FROM dbo.MessageSettings WHERE SettingName = N'SendPaymentSMS')
    INSERT INTO dbo.MessageSettings(SettingName, SettingValue, Description)
    VALUES (N'SendPaymentSMS', N'0', N'إرسال رسالة عند السداد');

IF NOT EXISTS (SELECT 1 FROM dbo.MessageSettings WHERE SettingName = N'SendLateSMS')
    INSERT INTO dbo.MessageSettings(SettingName, SettingValue, Description)
    VALUES (N'SendLateSMS', N'0', N'إرسال رسائل المتأخرات');

IF NOT EXISTS (SELECT 1 FROM dbo.MessageSettings WHERE SettingName = N'LateDays')
    INSERT INTO dbo.MessageSettings(SettingName, SettingValue, Description)
    VALUES (N'LateDays', N'7', N'عدد أيام التأخير قبل إرسال التنبيه');
";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@AdminUserName", adminUserName);
                    cmd.Parameters.AddWithValue("@AdminFullName", adminFullName);
                    cmd.Parameters.AddWithValue("@AdminPasswordHash", adminPasswordHash);
                    cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                    cmd.Parameters.AddWithValue("@ServiceFees", serviceFees);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static List<string> SplitSqlBatches(string sql)
        {
            var batches = new List<string>();
            var sb = new StringBuilder();

            using (var reader = new StringReader(sql))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, @"^\s*GO\s*$", RegexOptions.IgnoreCase))
                    {
                        batches.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            if (sb.Length > 0)
                batches.Add(sb.ToString());

            return batches;
        }

        private static bool ShouldSkipEnvironmentSpecificBatch(string batch)
        {
            return batch.IndexOf("CREATE USER [water_api]", StringComparison.OrdinalIgnoreCase) >= 0
                || batch.IndexOf("CREATE ROLE [water_app_role]", StringComparison.OrdinalIgnoreCase) >= 0
                || batch.IndexOf("ALTER ROLE [water_app_role]", StringComparison.OrdinalIgnoreCase) >= 0
                || batch.IndexOf("ALTER ROLE [db_datareader] ADD MEMBER [water_api]", StringComparison.OrdinalIgnoreCase) >= 0
                || batch.IndexOf("ALTER ROLE [db_datawriter] ADD MEMBER [water_api]", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void ValidateDatabaseName(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new Exception("اسم قاعدة البيانات مطلوب.");

            if (!Regex.IsMatch(databaseName, @"^[a-zA-Z0-9_\-]+$"))
                throw new Exception("اسم قاعدة البيانات يجب أن يحتوي على حروف إنجليزية أو أرقام أو _ أو - فقط.");
        }

        private static string EscapeSqlIdentifier(string value)
        {
            return value.Replace("]", "]]");
        }

        private static string EscapeSqlLiteral(string value)
        {
            return value.Replace("'", "''");
        }
    }
}