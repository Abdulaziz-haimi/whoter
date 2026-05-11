using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace water3.Services
{
    public static class AppSchemaService
    {
        public static void EnsureProductionTables()
        {
            using (var con = Db.GetConnection())
            {
                con.Open();
                ExecuteNonQuery(con, @"
IF OBJECT_ID(N'dbo.AppSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppSettings
    (
        SettingKey nvarchar(100) NOT NULL CONSTRAINT PK_AppSettings PRIMARY KEY,
        SettingValue nvarchar(max) NULL,
        UpdatedAt datetime2(0) NOT NULL CONSTRAINT DF_AppSettings_UpdatedAt DEFAULT SYSUTCDATETIME()
    );
END;

IF OBJECT_ID(N'dbo.AppErrorLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppErrorLogs
    (
        ErrorID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppErrorLogs PRIMARY KEY,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AppErrorLogs_CreatedAt DEFAULT SYSUTCDATETIME(),
        Source nvarchar(200) NULL,
        Message nvarchar(max) NULL,
        StackTrace nvarchar(max) NULL,
        UserName nvarchar(100) NULL,
        MachineName nvarchar(100) NULL,
        AppVersion nvarchar(50) NULL,
        IsResolved bit NOT NULL CONSTRAINT DF_AppErrorLogs_IsResolved DEFAULT(0),
        ResolvedAt datetime2(0) NULL,
        ResolutionNote nvarchar(500) NULL
    );
END;

IF OBJECT_ID(N'dbo.AppSchemaMigrations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppSchemaMigrations
    (
        MigrationID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppSchemaMigrations PRIMARY KEY,
        MigrationKey nvarchar(150) NOT NULL,
        Title nvarchar(250) NULL,
        SqlText nvarchar(max) NULL,
        AppliedAt datetime2(0) NOT NULL CONSTRAINT DF_AppSchemaMigrations_AppliedAt DEFAULT SYSUTCDATETIME(),
        Success bit NOT NULL CONSTRAINT DF_AppSchemaMigrations_Success DEFAULT(1),
        ErrorMessage nvarchar(max) NULL,
        CONSTRAINT UQ_AppSchemaMigrations_Key UNIQUE(MigrationKey)
    );
END;

IF OBJECT_ID(N'dbo.AppVersionInfo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppVersionInfo
    (
        ID int NOT NULL CONSTRAINT PK_AppVersionInfo PRIMARY KEY,
        AppVersion nvarchar(50) NULL,
        DatabaseVersion nvarchar(50) NULL,
        LastUpdateAt datetime2(0) NULL,
        UpdateNotes nvarchar(max) NULL
    );

    INSERT INTO dbo.AppVersionInfo(ID, AppVersion, DatabaseVersion, LastUpdateAt, UpdateNotes)
    VALUES(1, N'1.0.0', N'1.0.0', SYSUTCDATETIME(), N'Initial production metadata');
END;

IF OBJECT_ID(N'dbo.AppBackupHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppBackupHistory
    (
        BackupID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppBackupHistory PRIMARY KEY,
        CreatedAt datetime2(0) NOT NULL CONSTRAINT DF_AppBackupHistory_CreatedAt DEFAULT SYSUTCDATETIME(),
        BackupPath nvarchar(500) NOT NULL,
        BackupSizeBytes bigint NULL,
        CreatedBy nvarchar(100) NULL,
        MachineName nvarchar(100) NULL,
        Notes nvarchar(500) NULL
    );
END;
");
            }
        }

        public static void InsertBackupHistory(string path, long sizeBytes, string notes)
        {
            EnsureProductionTables();
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.AppBackupHistory(BackupPath, BackupSizeBytes, CreatedBy, MachineName, Notes)
VALUES(@Path, @Size, @User, @Machine, @Notes);", con))
            {
                cmd.Parameters.AddWithValue("@Path", path ?? "");
                cmd.Parameters.AddWithValue("@Size", sizeBytes);
                cmd.Parameters.AddWithValue("@User", Environment.UserName ?? "");
                cmd.Parameters.AddWithValue("@Machine", Environment.MachineName ?? "");
                cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static bool IsMigrationApplied(string migrationKey)
        {
            EnsureProductionTables();
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.AppSchemaMigrations WHERE MigrationKey=@Key AND Success=1;", con))
            {
                cmd.Parameters.AddWithValue("@Key", migrationKey ?? "");
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public static void ApplyMigrationFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("ملف الترقية غير موجود.", filePath);

            EnsureProductionTables();
            string key = Path.GetFileName(filePath);
            if (IsMigrationApplied(key))
                throw new InvalidOperationException("هذا التحديث مطبق مسبقاً: " + key);

            string sql = File.ReadAllText(filePath, Encoding.UTF8);
            ApplyMigration(key, Path.GetFileNameWithoutExtension(filePath), sql);
        }

        public static void ApplyMigration(string migrationKey, string title, string sql)
        {
            if (string.IsNullOrWhiteSpace(migrationKey))
                throw new ArgumentException("MigrationKey مطلوب.");

            EnsureProductionTables();

            using (var con = Db.GetConnection())
            {
                con.Open();
                SqlTransaction tx = con.BeginTransaction();

                try
                {
                    foreach (string batch in SplitSqlBatches(sql))
                    {
                        string cleaned = batch.Trim();
                        if (string.IsNullOrWhiteSpace(cleaned)) continue;

                        using (var cmd = new SqlCommand(cleaned, con, tx))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    using (var cmd = new SqlCommand(@"
INSERT INTO dbo.AppSchemaMigrations(MigrationKey, Title, SqlText, Success)
VALUES(@Key, @Title, @Sql, 1);", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@Key", migrationKey);
                        cmd.Parameters.AddWithValue("@Title", (object)title ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Sql", (object)sql ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    try
                    {
                        using (var logCmd = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM dbo.AppSchemaMigrations WHERE MigrationKey=@Key)
BEGIN
    INSERT INTO dbo.AppSchemaMigrations(MigrationKey, Title, SqlText, Success, ErrorMessage)
    VALUES(@Key, @Title, @Sql, 0, @Error);
END", con))
                        {
                            logCmd.Parameters.AddWithValue("@Key", migrationKey);
                            logCmd.Parameters.AddWithValue("@Title", (object)title ?? DBNull.Value);
                            logCmd.Parameters.AddWithValue("@Sql", (object)sql ?? DBNull.Value);
                            logCmd.Parameters.AddWithValue("@Error", ex.ToString());
                            logCmd.ExecuteNonQuery();
                        }
                    }
                    catch { }

                    throw;
                }
            }
        }

        public static DataTable GetMigrationHistory()
        {
            EnsureProductionTables();
            var dt = new DataTable();
            using (var con = Db.GetConnection())
            using (var da = new SqlDataAdapter(@"
SELECT MigrationID AS [الرقم], MigrationKey AS [المفتاح], Title AS [العنوان],
       AppliedAt AS [تاريخ التطبيق], Success AS [نجح], ErrorMessage AS [الخطأ]
FROM dbo.AppSchemaMigrations
ORDER BY MigrationID DESC;", con))
            {
                da.Fill(dt);
            }
            return dt;
        }

        public static List<string> SplitSqlBatches(string sql)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(sql)) return result;

            var parts = Regex.Split(sql, @"^\s*GO\s*(?:--.*)?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (string part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                    result.Add(part);
            }
            return result;
        }

        private static void ExecuteNonQuery(SqlConnection con, string sql)
        {
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
