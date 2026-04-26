USE [WaterBillingDB];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE dbo.usp_MobileSync_GetImportDecisions
    @CollectorID INT,
    @DeviceCode NVARCHAR(100),
    @ChangedAfter DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeviceID INT;

    SELECT @DeviceID = DeviceID
    FROM dbo.CollectorDevices
    WHERE DeviceCode = @DeviceCode
      AND CollectorID = @CollectorID;

    IF @DeviceID IS NULL
        THROW 68001, N'الجهاز غير مسجل أو غير مرتبط بهذا المحصل.', 1;

    ;WITH Decisions AS
    (
        SELECT
            I.ImportID,
            I.SyncBatchID,
            I.LocalPaymentGuid,
            I.LocalReceiptNo,
            I.ImportStatus,
            I.ApprovedReceiptID,
            R.ReceiptNumber,
            I.ApprovedAt,
            I.ApprovedByUserID,
            U.FullName AS ApprovedByUserName,
            I.RejectedReason,
            I.CreatedAt,
            CASE
                WHEN I.ImportStatus = N'Approved' THEN I.ApprovedAt
                WHEN I.ImportStatus = N'Rejected' THEN I.CreatedAt
                ELSE I.CreatedAt
            END AS ChangedAt
        FROM dbo.MobileReceiptImports I
        LEFT JOIN dbo.Receipts R ON R.ReceiptID = I.ApprovedReceiptID
        LEFT JOIN dbo.Users U ON U.UserID = I.ApprovedByUserID
        WHERE I.CollectorID = @CollectorID
          AND I.DeviceID = @DeviceID
          AND I.ImportStatus IN (N'Approved', N'Rejected')
    )
    SELECT
        ImportID,
        SyncBatchID,
        LocalPaymentGuid,
        LocalReceiptNo,
        ImportStatus,
        ApprovedReceiptID,
        ReceiptNumber,
        ApprovedAt,
        ApprovedByUserID,
        ApprovedByUserName,
        RejectedReason,
        CreatedAt,
        ChangedAt
    FROM Decisions
    WHERE @ChangedAfter IS NULL OR ChangedAt > @ChangedAfter
    ORDER BY ChangedAt, ImportID;
END
GO
