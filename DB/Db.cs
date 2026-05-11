using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace water3
{
    public static class Db
    {
        private const string ConnName = "WaterBillingDB";
        private const string UserConnKey = "UserConnString";

        private static string UserConnectionFilePath
        {
            get
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "water3"
                );

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return Path.Combine(dir, "connection.txt");
            }
        }

        public static string ConnectionString
        {
            get
            {
                // 1) من Settings User Scope
                try
                {
                    object value = Properties.Settings.Default[UserConnKey];
                    string userCs = value as string;

                    if (!string.IsNullOrWhiteSpace(userCs))
                        return userCs;
                }
                catch
                {
                    // في حال لم يكن UserConnString موجودًا داخل Settings.settings
                }

                // 2) من ملف محلي داخل AppData
                try
                {
                    if (File.Exists(UserConnectionFilePath))
                    {
                        string fileCs = File.ReadAllText(UserConnectionFilePath, Encoding.UTF8);
                        if (!string.IsNullOrWhiteSpace(fileCs))
                            return fileCs;
                    }
                }
                catch
                {
                    // تجاهل
                }

                // 3) من App.config
                var cs = ConfigurationManager.ConnectionStrings[ConnName];

                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                    throw new InvalidOperationException("ConnectionString (WaterBillingDB) غير موجود. افتح شاشة إعداد الاتصال أولاً.");

                return cs.ConnectionString;
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static string BuildConnectionString(
            string server,
            string database,
            bool windowsAuth,
            string sqlUser,
            string sqlPassword,
            int timeoutSeconds = 10)
        {
            if (string.IsNullOrWhiteSpace(server))
                server = ".";

            if (string.IsNullOrWhiteSpace(database))
                database = "master";

            var csb = new SqlConnectionStringBuilder
            {
                DataSource = server.Trim(),
                InitialCatalog = database.Trim(),
                IntegratedSecurity = windowsAuth,
                TrustServerCertificate = true,
                MultipleActiveResultSets = true,
                ConnectTimeout = timeoutSeconds
            };

            if (!windowsAuth)
            {
                csb.UserID = (sqlUser ?? "").Trim();
                csb.Password = sqlPassword ?? "";
                csb.IntegratedSecurity = false;
            }

            return csb.ConnectionString;
        }

        public static string BuildMasterConnectionString(
            string server,
            bool windowsAuth,
            string sqlUser,
            string sqlPassword)
        {
            return BuildConnectionString(server, "master", windowsAuth, sqlUser, sqlPassword);
        }

        public static void TestConnection(string connectionString)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                using (var cmd = new SqlCommand("SELECT 1", con))
                {
                    cmd.ExecuteScalar();
                }
            }
        }

        public static void SaveConnectionString(string newConnectionString)
        {
            bool savedToSettings = false;

            try
            {
                Properties.Settings.Default[UserConnKey] = newConnectionString;
                Properties.Settings.Default.Save();
                savedToSettings = true;
            }
            catch
            {
                savedToSettings = false;
            }

            if (!savedToSettings)
            {
                File.WriteAllText(UserConnectionFilePath, newConnectionString, Encoding.UTF8);
            }
        }

        public static bool DatabaseExists(string masterConnectionString, string databaseName)
        {
            ValidateDatabaseName(databaseName);

            using (var con = new SqlConnection(masterConnectionString))
            using (var cmd = new SqlCommand("SELECT DB_ID(@db)", con))
            {
                cmd.Parameters.AddWithValue("@db", databaseName);
                con.Open();

                object result = cmd.ExecuteScalar();
                return result != DBNull.Value && result != null;
            }
        }

        public static void CreateDatabaseIfNotExists(string masterConnectionString, string databaseName)
        {
            ValidateDatabaseName(databaseName);

            using (var con = new SqlConnection(masterConnectionString))
            {
                con.Open();

                string sql = @"
IF DB_ID(@db) IS NULL
BEGIN
    DECLARE @sql nvarchar(max);
    SET @sql = N'CREATE DATABASE ' + QUOTENAME(@db);
    EXEC(@sql);
END";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@db", databaseName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool IsSchemaInstalled(string appConnectionString)
        {
            using (var con = new SqlConnection(appConnectionString))
            using (var cmd = new SqlCommand(@"
SELECT COUNT(*)
FROM sys.tables
WHERE name IN
(
    N'Users',
    N'Roles',
    N'Subscribers',
    N'Meters',
    N'Invoices',
    N'Payments',
    N'Receipts',
    N'Accounts',
    N'BillingConstants'
);", con))
            {
                con.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count >= 8;
            }
        }

        public static void InstallSchemaFromFile(string appConnectionString, string databaseName, string scriptFilePath)
        {
            ValidateDatabaseName(databaseName);

            if (!File.Exists(scriptFilePath))
                throw new FileNotFoundException("لم يتم العثور على سكربت قاعدة البيانات.", scriptFilePath);

            string sql = File.ReadAllText(scriptFilePath, Encoding.UTF8);

            sql = Regex.Replace(
                sql,
                @"USE\s+\[[^\]]+\]",
                "USE [" + EscapeSqlIdentifier(databaseName) + "]",
                RegexOptions.IgnoreCase
            );

            List<string> batches = SplitSqlBatches(sql);

            using (var con = new SqlConnection(appConnectionString))
            {
                con.Open();

                foreach (string batch in batches)
                {
                    string cleaned = batch.Trim();

                    if (string.IsNullOrWhiteSpace(cleaned))
                        continue;

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
        public static void CreateSqlLoginAndUser(
    string masterConnectionString,
    string databaseName,
    string loginName,
    string loginPassword)
        {
            ValidateDatabaseName(databaseName);

            if (string.IsNullOrWhiteSpace(loginName))
                throw new Exception("اسم مستخدم SQL مطلوب.");

            if (string.IsNullOrWhiteSpace(loginPassword))
                throw new Exception("كلمة مرور مستخدم SQL مطلوبة.");

            using (var con = new SqlConnection(masterConnectionString))
            {
                con.Open();

                string sql = @"
DECLARE @LoginName sysname = @pLoginName;
DECLARE @Password nvarchar(200) = @pPassword;
DECLARE @DatabaseName sysname = @pDatabaseName;

DECLARE @Sql nvarchar(max);

------------------------------------------------------------
-- 1) إنشاء SQL Login على مستوى السيرفر
------------------------------------------------------------
IF NOT EXISTS
(
    SELECT 1
    FROM sys.server_principals
    WHERE name = @LoginName
)
BEGIN
    SET @Sql =
        N'CREATE LOGIN ' + QUOTENAME(@LoginName) +
        N' WITH PASSWORD = ' + QUOTENAME(@Password, '''') +
        N', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;';

    EXEC(@Sql);
END

------------------------------------------------------------
-- 2) إنشاء User داخل قاعدة البيانات
------------------------------------------------------------
SET @Sql = N'
USE ' + QUOTENAME(@DatabaseName) + N';

IF NOT EXISTS
(
    SELECT 1
    FROM sys.database_principals
    WHERE name = N''' + REPLACE(@LoginName, '''', '''''') + N'''
)
BEGIN
    CREATE USER ' + QUOTENAME(@LoginName) + N'
    FOR LOGIN ' + QUOTENAME(@LoginName) + N'
    WITH DEFAULT_SCHEMA = dbo;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.database_principals
    WHERE name = N''water_app_role''
      AND type = N''R''
)
BEGIN
    CREATE ROLE water_app_role;
END;

ALTER ROLE water_app_role ADD MEMBER ' + QUOTENAME(@LoginName) + N';

IF IS_ROLEMEMBER(N''db_datareader'', N''' + REPLACE(@LoginName, '''', '''''') + N''') <> 1
    ALTER ROLE db_datareader ADD MEMBER ' + QUOTENAME(@LoginName) + N';

IF IS_ROLEMEMBER(N''db_datawriter'', N''' + REPLACE(@LoginName, '''', '''''') + N''') <> 1
    ALTER ROLE db_datawriter ADD MEMBER ' + QUOTENAME(@LoginName) + N';

GRANT EXECUTE TO water_app_role;
';

EXEC(@Sql);
";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@pLoginName", loginName.Trim());
                    cmd.Parameters.AddWithValue("@pPassword", loginPassword);
                    cmd.Parameters.AddWithValue("@pDatabaseName", databaseName.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void SeedBasicData(
            string appConnectionString,
            string adminUserName,
            string adminFullName,
            string adminPassword,
            decimal unitPrice,
            decimal serviceFees)
        {
            if (string.IsNullOrWhiteSpace(adminUserName))
                adminUserName = "admin";

            if (string.IsNullOrWhiteSpace(adminFullName))
                adminFullName = "مدير النظام";

            if (string.IsNullOrWhiteSpace(adminPassword))
                adminPassword = "123";

            if (unitPrice <= 0)
                unitPrice = 1.00m;

            if (serviceFees < 0)
                serviceFees = 0m;

            string adminHash = Sha256Hex(adminPassword);

            using (var con = new SqlConnection(appConnectionString))
            {
                con.Open();

                string sql = @"
SET NOCOUNT ON;

------------------------------------------------------------
-- 1) الحسابات الأساسية
------------------------------------------------------------
DECLARE @Accounts TABLE
(
    AccountCode nvarchar(20) NOT NULL,
    AccountName nvarchar(100) NOT NULL,
    AccountType nvarchar(20) NOT NULL,
    IsControl bit NOT NULL
);

INSERT INTO @Accounts(AccountCode, AccountName, AccountType, IsControl)
VALUES
(N'1100', N'الصندوق', N'Asset', 0),
(N'4100', N'إيرادات المياه', N'Revenue', 0),
(N'1000', N'البنك', N'Asset', 0),
(N'1200', N'ذمم العملاء', N'Asset', 1),
(N'5100', N'مصروف أجور', N'Expense', 0),
(N'100',  N'الاستثمار', N'Receivable', 0),
(N'4110', N'إيرادات رسوم الخدمة', N'Revenue', 0),
(N'5110', N'مصروف مواد خام', N'Expense', 0),
(N'5120', N'مصروف صيانة', N'Expense', 0),
(N'5130', N'مصروف نقل ووقود', N'Expense', 0),
(N'5140', N'خسائر وتوالف', N'Expense', 0),
(N'1500', N'معدات وأصول صغيرة', N'Asset', 0),
(N'2100', N'ذمم موردين', N'Liability', 0);

MERGE dbo.Accounts AS T
USING @Accounts AS S
ON T.AccountCode = S.AccountCode
WHEN MATCHED THEN
    UPDATE SET
        T.AccountName = S.AccountName,
        T.AccountType = S.AccountType,
        T.IsControl = S.IsControl
WHEN NOT MATCHED THEN
    INSERT(AccountCode, AccountName, AccountType, IsControl, ParentAccountID)
    VALUES(S.AccountCode, S.AccountName, S.AccountType, S.IsControl, NULL);

------------------------------------------------------------
-- 2) حسابات النظام
------------------------------------------------------------
DECLARE
    @CashAccountID int,
    @BankAccountID int,
    @ReceivableAccountID int,
    @WaterRevenueAccountID int,
    @ServiceRevenueAccountID int;

SELECT @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1100';
SELECT @BankAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1000';
SELECT @ReceivableAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1200';
SELECT @WaterRevenueAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'4100';
SELECT @ServiceRevenueAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'4110';

IF OBJECT_ID(N'dbo.SystemAccounts2', N'U') IS NOT NULL
BEGIN
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
END

------------------------------------------------------------
-- 3) تصنيفات المصروفات والمشتريات والخسائر
------------------------------------------------------------
DECLARE @ExpenseCategories TABLE
(
    CategoryName nvarchar(100) NOT NULL,
    CategoryType nvarchar(20) NOT NULL,
    AccountCode nvarchar(20) NOT NULL,
    Notes nvarchar(200) NULL,
    IsActive bit NOT NULL
);

INSERT INTO @ExpenseCategories(CategoryName, CategoryType, AccountCode, Notes, IsActive)
VALUES
(N'أجور عمل', N'Expense', N'5100', N'رواتب وأجور يومية', 1),
(N'مشتروات القات', N'Expense', N'5130', NULL, 1),
(N'مواد خام', N'Purchase', N'5110', N'شراء خامات ومواد', 1),
(N'معدات', N'Purchase', N'1500', N'شراء معدات وأدوات', 1),
(N'خسائر وتالف', N'Loss', N'5140', N'تلف أو فاقد أو خسائر تشغيلية', 1);

MERGE dbo.ExpenseCategories AS T
USING
(
    SELECT
        C.CategoryName,
        C.CategoryType,
        A.AccountID AS DefaultAccountID,
        C.Notes,
        C.IsActive
    FROM @ExpenseCategories C
    INNER JOIN dbo.Accounts A ON A.AccountCode = C.AccountCode
) AS S
ON T.CategoryName = S.CategoryName
WHEN MATCHED THEN
    UPDATE SET
        T.CategoryType = S.CategoryType,
        T.DefaultAccountID = S.DefaultAccountID,
        T.Notes = S.Notes,
        T.IsActive = S.IsActive
WHEN NOT MATCHED THEN
    INSERT(CategoryName, CategoryType, DefaultAccountID, Notes, IsActive)
    VALUES(S.CategoryName, S.CategoryType, S.DefaultAccountID, S.Notes, S.IsActive);

------------------------------------------------------------
-- 4) إعدادات الرسائل
------------------------------------------------------------
DECLARE @MessageSettings TABLE
(
    SettingName nvarchar(100) NOT NULL,
    SettingValue nvarchar(200) NOT NULL,
    Description nvarchar(200) NULL
);

INSERT INTO @MessageSettings(SettingName, SettingValue, Description)
VALUES
(N'EnableSMS', N'1', N'تفعيل نظام الرسائل النصية'),
(N'SendInvoiceSMS', N'1', N'إرسال رسالة عند إصدار الفاتورة'),
(N'SendPaymentSMS', N'1', N'إرسال رسالة عند السداد'),
(N'SendLateSMS', N'1', N'إرسال رسائل المتأخرات'),
(N'LateDays', N'7', N'عدد أيام التأخير قبل إرسال التنبيه');

MERGE dbo.MessageSettings AS T
USING @MessageSettings AS S
ON T.SettingName = S.SettingName
WHEN MATCHED THEN
    UPDATE SET
        T.SettingValue = S.SettingValue,
        T.Description = S.Description
WHEN NOT MATCHED THEN
    INSERT(SettingName, SettingValue, Description)
    VALUES(S.SettingName, S.SetingValue, S.Description);
";

                // إصلاح خطأ كتابي متعمّد في النص أعلاه قبل التنفيذ
                sql = sql.Replace("S.SetingValue", "S.SettingValue");

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                }

                string sql2 = @"
SET NOCOUNT ON;

------------------------------------------------------------
-- 5) قوالب الرسائل
------------------------------------------------------------
DECLARE @Templates TABLE
(
    TemplateName nvarchar(100) NOT NULL,
    TemplateText nvarchar(max) NOT NULL,
    TemplateType nvarchar(50) NOT NULL,
    IsActive bit NOT NULL,
    [Language] nvarchar(10) NULL
);

INSERT INTO @Templates(TemplateName, TemplateText, TemplateType, IsActive, [Language])
VALUES
(
    N'فاتورة جديدة',
    N'استهلكت {Consumption}ح +ل{CurrentReading} +م{Arrears} عليكم {GrandTotal}',
    N'Invoice',
    1,
    N'AR'
),
(
    N'إشعار سداد',
    N'الأخ{SubscriberName}
تم استلام مبلغ {Amount} ريال
عن الفاتورة رقم {InvoiceID}
شكراً لتعاملكم معنا.',
    N'Payment',
    0,
    N'AR'
),
(
    N'ترحيب',
    N'مرحبا',
    N'Welcome Message',
    0,
    N'AR'
),
(
    N'انذار بالفصل',
    N'عزيزي المشترك {subdpaname}
يرجاء سرعات سداد مبلغ {totel}',
    N'Service Alert',
    1,
    N'AR'
),
(
    N'اشعار سداد1',
    N'تم استلام مبلغ {TotalReceived} ريال. المتبقي علىكم   {RemainingBalance}  - {CreditAmount}ريال. شكراً لكم.',
    N'Payment',
    1,
    N'AR'
),
(
    N'جديد',
    N'استهلكت {Consumption}ح +ل{CurrentReading} +م{Arrears} عليكم {GrandTotal} ريال. الرصيد الحالي {BalanceDirection} {RemainingBalanceAbs} ريال.',
    N'Invoice',
    1,
    N'AR'
),
(
    N'جديد سداد',
    N'تم استلام مبلغ {TotalReceived} ريال. الرصيد الحالي {BalanceDirection} {RemainingBalanceAbs} ريال. شكراً لكم.',
    N'Payment',
    1,
    N'AR'
);

MERGE dbo.MessageTemplates AS T
USING @Templates AS S
ON T.TemplateName = S.TemplateName
AND T.TemplateType = S.TemplateType
AND ISNULL(T.[Language], N'AR') = ISNULL(S.[Language], N'AR')
WHEN MATCHED THEN
    UPDATE SET
        T.TemplateText = S.TemplateText,
        T.IsActive = S.IsActive,
        T.[Language] = S.[Language]
WHEN NOT MATCHED THEN
    INSERT(TemplateName, TemplateText, TemplateType, IsActive, [Language], CreatedAt)
    VALUES(S.TemplateName, S.TemplateText, S.TemplateType, S.IsActive, S.[Language], GETDATE());

------------------------------------------------------------
-- 6) ربط القوالب الأساسية إن كان الجدول موجودًا
------------------------------------------------------------
IF OBJECT_ID(N'dbo.MessageTemplateMap', N'U') IS NOT NULL
BEGIN
    DECLARE @InvoiceTemplateID int;
    DECLARE @PaymentTemplateID int;

    SELECT TOP 1 @InvoiceTemplateID = TemplateID
    FROM dbo.MessageTemplates
    WHERE TemplateType = N'Invoice'
      AND IsActive = 1
      AND ISNULL([Language], N'AR') = N'AR'
    ORDER BY TemplateID DESC;

    SELECT TOP 1 @PaymentTemplateID = TemplateID
    FROM dbo.MessageTemplates
    WHERE TemplateType = N'Payment'
      AND IsActive = 1
      AND ISNULL([Language], N'AR') = N'AR'
    ORDER BY TemplateID DESC;

    IF @InvoiceTemplateID IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey = N'Invoice')
            INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled, UpdatedAt)
            VALUES(N'Invoice', @InvoiceTemplateID, 1, GETDATE());
        ELSE
            UPDATE dbo.MessageTemplateMap
            SET TemplateID = @InvoiceTemplateID, IsEnabled = 1, UpdatedAt = GETDATE()
            WHERE EventKey = N'Invoice';
    END

    IF @PaymentTemplateID IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey = N'Payment')
            INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled, UpdatedAt)
            VALUES(N'Payment', @PaymentTemplateID, 1, GETDATE());
        ELSE
            UPDATE dbo.MessageTemplateMap
            SET TemplateID = @PaymentTemplateID, IsEnabled = 1, UpdatedAt = GETDATE()
            WHERE EventKey = N'Payment';
    END
END
";

                using (var cmd = new SqlCommand(sql2, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                }

                string sql3 = @"
SET NOCOUNT ON;

------------------------------------------------------------
-- 7) الأدوار
------------------------------------------------------------
DECLARE @Roles TABLE(RoleName nvarchar(50) NOT NULL);

INSERT INTO @Roles(RoleName)
VALUES
(N'Admin'),
(N'Collector'),
(N'ادارة المحصلين'),
(N'فني الصيانة'),
(N'محصل'),
(N'مدخل البيانات'),
(N'مدير الشكاوى'),
(N'مدير النظام'),
(N'مراجع الحسابات'),
(N'مراقب الجودة'),
(N'مستخدم العداد'),
(N'مشرف المنطقة'),
(N'موظف الفواتير');

MERGE dbo.Roles AS T
USING @Roles AS S
ON T.RoleName = S.RoleName
WHEN NOT MATCHED THEN
    INSERT(RoleName) VALUES(S.RoleName);

------------------------------------------------------------
-- 8) مستخدم المدير
------------------------------------------------------------
DECLARE @AdminRoleID int;

SELECT TOP 1 @AdminRoleID = RoleID
FROM dbo.Roles
WHERE RoleName = N'Admin';

IF @AdminRoleID IS NULL
    SELECT TOP 1 @AdminRoleID = RoleID
    FROM dbo.Roles
    WHERE RoleName = N'مدير النظام';

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
ELSE
BEGIN
    UPDATE dbo.Users
    SET
        FullName = @AdminFullName,
        RoleID = @AdminRoleID,
        IsActive = 1
    WHERE UserName = @AdminUserName;
END

------------------------------------------------------------
-- 9) الصلاحيات
------------------------------------------------------------
DECLARE @Permissions TABLE
(
    PermissionKey nvarchar(100) NOT NULL,
    PermissionName nvarchar(150) NOT NULL,
    Category nvarchar(100) NULL,
    IsActive bit NOT NULL
);

INSERT INTO @Permissions(PermissionKey, PermissionName, Category, IsActive)
VALUES
(N'USERS_VIEW', N'عرض المستخدمين', N'المستخدمون', 1),
(N'USERS_MANAGE', N'إدارة المستخدمين', N'المستخدمون', 1),
(N'USERS_RESET_PASSWORD', N'إعادة تعيين كلمة المرور', N'المستخدمون', 1),
(N'SUBSCRIBERS_VIEW', N'عرض المشتركين', N'المشتركين', 1),
(N'SUBSCRIBERS_MANAGE', N'إدارة المشتركين', N'المشتركين', 1),
(N'READINGS_VIEW', N'عرض القراءات', N'القراءات', 1),
(N'READINGS_ADD', N'إضافة قراءة', N'القراءات', 1),
(N'READINGS_IMPORT', N'استيراد القراءات', N'القراءات', 1),
(N'INVOICES_VIEW', N'عرض الفواتير', N'الفواتير', 1),
(N'INVOICES_PRINT', N'طباعة الفواتير', N'الفواتير', 1),
(N'PAYMENTS_VIEW', N'عرض التحصيلات', N'التحصيلات', 1),
(N'PAYMENTS_ADD', N'إضافة تحصيل', N'التحصيلات', 1),
(N'RECEIPTS_PRINT', N'طباعة سندات الاستلام', N'التحصيلات', 1),
(N'MOBILE_SYNC_VIEW', N'عرض شاشة المزامنة', N'المزامنة الجوالة', 1),
(N'MOBILE_BATCH_VIEW', N'عرض دفعات الهاتف', N'المزامنة الجوالة', 1),
(N'MOBILE_BATCH_APPROVE', N'اعتماد دفعات الهاتف', N'المزامنة الجوالة', 1),
(N'MOBILE_BATCH_REJECT', N'رفض دفعات الهاتف', N'المزامنة الجوالة', 1),
(N'REPORTS_VIEW', N'عرض التقارير', N'التقارير', 1),
(N'SETTINGS_VIEW', N'عرض الإعدادات', N'الإعدادات', 1),
(N'COLLECTORS_LINK_USER', N'ربط المحصل بالمستخدم', N'المحصلون', 1),
(N'COLLECTOR_DEVICES_VIEW', N'عرض أجهزة المحصلين', N'الأجهزة الجوالة', 1),
(N'COLLECTOR_DEVICES_MANAGE', N'إدارة أجهزة المحصلين', N'الأجهزة الجوالة', 1),
(N'COLLECTOR_DEVICES_APPROVE', N'اعتماد أجهزة المحصلين', N'الأجهزة الجوالة', 1),
(N'MOBILE_SYNC_TO_PHONE_VIEW', N'عرض شاشة المزامنة إلى الهاتف', N'المزامنة الجوالة', 1),
(N'MOBILE_SYNC_TO_PHONE_EXECUTE', N'تنفيذ المزامنة إلى الهاتف', N'المزامنة الجوالة', 1),
(N'DASHBOARD_VIEW', N'عرض لوحة التحكم', N'الرئيسية', 1),
(N'SUBSCRIBERS_REPORT_VIEW', N'عرض تقرير المشتركين', N'التقارير', 1),
(N'COLLECTORS_REPORT_VIEW', N'عرض تقرير تحصيلات المحصلين', N'التقارير', 1),
(N'ACCOUNT_STATEMENT_VIEW', N'عرض كشف حساب المشترك', N'التقارير', 1),
(N'INVOICE_PRINT_VIEW', N'طباعة الفواتير', N'الفواتير', 1),
(N'COLLECTORS_VIEW', N'عرض المحصلين', N'المحصلون', 1),
(N'COLLECTORS_MANAGE', N'إدارة المحصلين', N'المحصلون', 1),
(N'BILLING_CONSTANTS_VIEW', N'إدارة الثوابت', N'الإعدادات', 1),
(N'SMS_LOGS_VIEW', N'عرض سجل الرسائل', N'الرسائل', 1),
(N'SMS_REPORT_VIEW', N'عرض تقرير الرسائل', N'الرسائل', 1),
(N'MESSAGES_MANAGE', N'إدارة الرسائل', N'الرسائل', 1),
(N'DB_SETTINGS_VIEW', N'إدارة الاتصال', N'الإعدادات', 1),
(N'UNPAID_INVOICES_VIEW', N'عرض الفواتير غير المسددة', N'التقارير', 1),
(N'READING_ENTRY_VIEW', N'إدخال القراءات', N'القراءات', 1),
(N'USERS_CREATE', N'إنشاء مستخدم جديد', N'المستخدمون', 1),
(N'ROLES_MANAGE', N'إدارة الأدوار والصلاحيات', N'المستخدمون', 1),
(N'AUDITLOG_VIEW', N'عرض سجل العمليات', N'السجل الإداري', 1),
(N'REPORTS_CENTER_VIEW', N'عرض مركز التقارير', N'التقارير', 1),
(N'REPORT_INVOICES_VIEW', N'عرض تقرير الفواتير', N'التقارير', 1),
(N'REPORT_PAYMENTS_VIEW', N'عرض تقرير المدفوعات', N'التقارير', 1),
(N'REPORT_RECEIPTS_VIEW', N'عرض تقرير الإيصالات', N'التقارير', 1),
(N'REPORT_ACCOUNTSTATEMENT_VIEW', N'عرض كشف الحساب', N'التقارير', 1),
(N'REPORT_AGING_VIEW', N'عرض تقرير الأعمار', N'التقارير', 1),
(N'REPORT_GENERALJOURNAL_VIEW', N'عرض دفتر اليومية', N'التقارير', 1),
(N'REPORT_TRIALBALANCE_VIEW', N'عرض ميزان المراجعة', N'التقارير', 1),
(N'REPORT_EXPORT', N'تصدير التقارير', N'التقارير', 1),
(N'REPORT_PRINT', N'طباعة التقارير', N'التقارير', 1),
(N'REPORT_COLLECTOR_COLLECTIONS_VIEW', N'عرض تقرير تحصيلات المحصلين', N'تقارير التشغيل', 1),
(N'REPORT_COLLECTOR_DEVICES_VIEW', N'عرض تقرير أجهزة المحصلين', N'تقارير التشغيل', 1),
(N'REPORT_MOBILE_BATCHES_VIEW', N'عرض تقرير دفعات المزامنة', N'تقارير التشغيل', 1),
(N'REPORT_MOBILE_ERRORS_VIEW', N'عرض تقرير أخطاء المزامنة', N'تقارير التشغيل', 1),
(N'REPORT_COLLECTOR_SUBSCRIBERS_VIEW', N'عرض تقرير تخصيص المشتركين للمحصلين', N'تقارير التشغيل', 1),
(N'REPORT_PHONE_PUSH_VIEW', N'عرض تقرير التنزيل إلى الهاتف', N'تقارير التشغيل', 1),
(N'MAIN_METER_REPORT_VIEW', N'عرض تقرير العداد الرئيسي والفاقد', N'التقارير التشغيلية', 1),
(N'EXPENSES_VIEW', N'عرض المصروفات والمشتريات والمخاسير', N'المصروفات', 1),
(N'EXPENSES_MANAGE', N'إدارة المصروفات والمشتريات والمخاسير', N'المصروفات', 1),
(N'EXPENSES_PRINT', N'طباعة سندات المصروفات', N'المصروفات', 1),
(N'EXPENSE_CATEGORIES_MANAGE', N'إدارة تصنيفات المصروفات', N'المصروفات', 1),
(N'EXPENSE_REPORTS_VIEW', N'عرض تقارير المصروفات والمشتريات والخسائر', N'المصروفات', 1),
(N'EXPENSE_REPORTS_PRINT', N'طباعة تقارير وسندات المصروفات', N'المصروفات', 1);

MERGE dbo.Permissions AS T
USING @Permissions AS S
ON T.PermissionKey = S.PermissionKey
WHEN MATCHED THEN
    UPDATE SET
        T.PermissionName = S.PermissionName,
        T.Category = S.Category,
        T.IsActive = S.IsActive
WHEN NOT MATCHED THEN
    INSERT(PermissionKey, PermissionName, Category, IsActive)
    VALUES(S.PermissionKey, S.PermissionName, S.Category, S.IsActive);

------------------------------------------------------------
-- 10) منح كل الصلاحيات للمدير
------------------------------------------------------------
DECLARE @RoleIDs TABLE(RoleID int);

INSERT INTO @RoleIDs(RoleID)
SELECT RoleID
FROM dbo.Roles
WHERE RoleName IN (N'Admin', N'مدير النظام');

INSERT INTO dbo.RolePermissions(RoleID, PermissionID, IsAllowed)
SELECT R.RoleID, P.PermissionID, 1
FROM @RoleIDs R
CROSS JOIN dbo.Permissions P
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RolePermissions RP
    WHERE RP.RoleID = R.RoleID
      AND RP.PermissionID = P.PermissionID
);

UPDATE RP
SET IsAllowed = 1
FROM dbo.RolePermissions RP
INNER JOIN @RoleIDs R ON R.RoleID = RP.RoleID;
";

                using (var cmd = new SqlCommand(sql3, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@AdminUserName", adminUserName.Trim());
                    cmd.Parameters.AddWithValue("@AdminFullName", adminFullName.Trim());
                    cmd.Parameters.AddWithValue("@AdminPasswordHash", adminHash);
                    cmd.ExecuteNonQuery();
                }

                string sql4 = @"
SET NOCOUNT ON;

------------------------------------------------------------
-- 11) التعرفة الافتراضية
------------------------------------------------------------
IF OBJECT_ID(N'dbo.TariffPlans', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.TariffPlans WHERE PlanName = N'تعرفة افتراضية')
    BEGIN
        INSERT INTO dbo.TariffPlans
        (
            PlanName,
            PricingModel,
            FixedUnitPrice,
            DefaultServiceFees,
            IsActive,
            CreatedAt
        )
        VALUES
        (
            N'تعرفة افتراضية',
            N'Fixed',
            @UnitPrice,
            @ServiceFees,
            1,
            GETDATE()
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.TariffPlans
        SET
            PricingModel = N'Fixed',
            FixedUnitPrice = @UnitPrice,
            DefaultServiceFees = @ServiceFees,
            IsActive = 1
        WHERE PlanName = N'تعرفة افتراضية';
    END
END

------------------------------------------------------------
-- 12) ثوابت الفوترة
------------------------------------------------------------
IF OBJECT_ID(N'dbo.BillingConstants', N'U') IS NOT NULL
BEGIN
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
            CAST(GETDATE() AS date),
            @UnitPrice,
            @ServiceFees,
            @ServiceFees,
            1,
            N'تعرفة افتراضية تم إنشاؤها تلقائيًا',
            GETDATE()
        );
    END
END
";

                using (var cmd = new SqlCommand(sql4, con))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                    cmd.Parameters.AddWithValue("@ServiceFees", serviceFees);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ExecuteTable(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.CommandTimeout = 0;
                if (p != null) p(cmd.Parameters);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            {
                con.Open();

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 0;
                    if (p != null) p(cmd.Parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            {
                con.Open();

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 0;
                    if (p != null) p(cmd.Parameters);
                    return cmd.ExecuteScalar();
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

            if (!Regex.IsMatch(databaseName.Trim(), @"^[a-zA-Z0-9_\-]+$"))
                throw new Exception("اسم قاعدة البيانات يجب أن يحتوي على حروف إنجليزية أو أرقام أو _ أو - فقط.");
        }

        private static string EscapeSqlIdentifier(string value)
        {
            return (value ?? "").Replace("]", "]]");
        }

        private static string Sha256Hex(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password ?? "");
                byte[] hash = sha.ComputeHash(bytes);

                var sb = new StringBuilder();

                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
/*using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace water3
{
    public static class Db
    {
        private const string ConnName = "WaterBillingDB";        // اسم عنصر connectionStrings في App.config
        private const string UserConnKey = "UserConnString";     // اسم المفتاح في Settings (User)

        public static string ConnectionString
        {
            get
            {
                // 1) اقرأ من User Settings (الأفضل للتثبيت على أجهزة أخرى)
                try
                {
                    var userCs = Properties.Settings.Default[UserConnKey] as string;
                    if (!string.IsNullOrWhiteSpace(userCs))
                        return userCs;
                }
                catch { }

                // 2) fallback: اقرأ من App.config
                var cs = ConfigurationManager.ConnectionStrings[ConnName];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                    throw new InvalidOperationException("ConnectionString (WaterBillingDB) غير موجود في App.config");
                return cs.ConnectionString;
            }
        }

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

        public static void TestConnection(string connectionString)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT 1", con))
                    cmd.ExecuteScalar();
            }
        }

        public static void SaveConnectionString(string newConnectionString)
        {
            // ✅ احفظ في Settings (User scope) لتجنب مشكلة عدم وجود صلاحيات كتابة في Program Files
            Properties.Settings.Default[UserConnKey] = newConnectionString;
            Properties.Settings.Default.Save();
        }

        public static DataTable ExecuteTable(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                p?.Invoke(cmd.Parameters);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            {
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    p?.Invoke(cmd.Parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            {
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    p?.Invoke(cmd.Parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
*/