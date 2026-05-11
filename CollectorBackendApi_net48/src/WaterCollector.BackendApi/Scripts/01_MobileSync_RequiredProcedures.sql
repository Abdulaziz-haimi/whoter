USE [WaterBillingDB]
GO

/*
  شغّل هذا الملف فقط إذا كانت الإجراءات التالية غير موجودة:
  dbo.usp_MobileSync_ExportReceivables
  dbo.usp_MobileSync_SaveBatch
  dbo.usp_MobileSync_GetImportDecisions

  الملف مصمم ليتوافق مع الجداول التي أرسلتها سابقًا.
*/

IF OBJECT_ID(N'dbo.usp_MobileSync_ExportReceivables', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_MobileSync_ExportReceivables;
GO
CREATE PROCEDURE dbo.usp_MobileSync_ExportReceivables
    @CollectorID INT,
    @AsOfDate DATE = NULL,
    @OnlyAssignedSubscribers BIT = 1,
    @IncludeAllIfNoAssignments BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @AsOfDate IS NULL SET @AsOfDate = CAST(GETDATE() AS DATE);

    DECLARE @HasAssignments BIT = 0;
    IF EXISTS (SELECT 1 FROM dbo.CollectorSubscribers WHERE CollectorID = @CollectorID AND IsActive = 1)
        SET @HasAssignments = 1;

    ;WITH ScopeSubscribers AS
    (
        SELECT S.SubscriberID
        FROM dbo.Subscribers S
        WHERE S.IsActive = 1
          AND
          (
              @OnlyAssignedSubscribers = 0
              OR EXISTS
              (
                  SELECT 1 FROM dbo.CollectorSubscribers CS
                  WHERE CS.CollectorID = @CollectorID
                    AND CS.SubscriberID = S.SubscriberID
                    AND CS.IsActive = 1
              )
              OR (@IncludeAllIfNoAssignments = 1 AND @HasAssignments = 0)
          )
    ),
    InvoiceBalances AS
    (
        SELECT
            I.InvoiceID,
            I.SubscriberID,
            I.MeterID,
            ISNULL(I.InvoiceNumber, CAST(I.InvoiceID AS NVARCHAR(30))) AS InvoiceNumber,
            I.InvoiceDate,
            CAST(ISNULL(I.Consumption,0) AS DECIMAL(18,2)) AS Consumption,
            CAST(ISNULL(I.UnitPrice,0) AS DECIMAL(18,2)) AS UnitPrice,
            CAST(ISNULL(I.ServiceFees,0) AS DECIMAL(18,2)) AS ServiceFees,
            CAST(ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS Arrears,
            CAST(ISNULL(I.TotalAmount,0) AS DECIMAL(18,2)) AS TotalAmount,
            CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS GrandTotal,
            CAST(ISNULL(PA.PaidTotal,0) AS DECIMAL(18,2)) AS PaidTotal,
            CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) - ISNULL(PA.PaidTotal,0) AS DECIMAL(18,2)) AS Remaining,
            I.Status,
            I.Notes
        FROM dbo.Invoices I
        OUTER APPLY
        (
            SELECT SUM(P.Amount) AS PaidTotal
            FROM dbo.Payments P
            WHERE P.InvoiceID = I.InvoiceID
        ) PA
        WHERE ISNULL(I.Status, N'') <> N'ملغاة'
          AND I.InvoiceDate <= @AsOfDate
    )
    SELECT
        @CollectorID AS CollectorID,
        @AsOfDate AS AsOfDate,
        GETDATE() AS ExportedAt,
        (SELECT COUNT(*) FROM ScopeSubscribers) AS SubscribersCount,
        (SELECT COUNT(*) FROM InvoiceBalances IB INNER JOIN ScopeSubscribers SS ON SS.SubscriberID = IB.SubscriberID WHERE IB.Remaining > 0) AS OpenInvoicesCount,
        (SELECT COUNT(*) FROM dbo.SubscriberCredits C INNER JOIN ScopeSubscribers SS ON SS.SubscriberID = C.SubscriberID WHERE C.AmountRemaining > 0) AS OpenCreditsCount;

    ;WITH ScopeSubscribers AS
    (
        SELECT S.SubscriberID
        FROM dbo.Subscribers S
        WHERE S.IsActive = 1
          AND
          (
              @OnlyAssignedSubscribers = 0
              OR EXISTS (SELECT 1 FROM dbo.CollectorSubscribers CS WHERE CS.CollectorID=@CollectorID AND CS.SubscriberID=S.SubscriberID AND CS.IsActive=1)
              OR (@IncludeAllIfNoAssignments = 1 AND @HasAssignments = 0)
          )
    ),
    PrimaryMeter AS
    (
        SELECT SM.SubscriberID, SM.MeterID, M.MeterNumber, M.Location,
               ROW_NUMBER() OVER(PARTITION BY SM.SubscriberID ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC) rn
        FROM dbo.SubscriberMeters SM
        INNER JOIN dbo.Meters M ON M.MeterID = SM.MeterID
    )
    SELECT
        S.SubscriberID,
        S.Name AS SubscriberName,
        S.PhoneNumber,
        S.Address,
        S.AccountID,
        S.TariffPlanID,
        S.IsActive,
        PM.MeterID AS PrimaryMeterID,
        PM.MeterNumber AS PrimaryMeterNumber,
        PM.Location AS PrimaryMeterLocation,
        CAST(ISNULL(Due.CurrentDue,0) AS DECIMAL(18,2)) AS CurrentDue,
        CAST(ISNULL(Cr.CurrentCredit,0) AS DECIMAL(18,2)) AS CurrentCredit,
        CAST(ISNULL(Due.CurrentDue,0) - ISNULL(Cr.CurrentCredit,0) AS DECIMAL(18,2)) AS CurrentBalance,
        LI.InvoiceID AS LastInvoiceID,
        LI.InvoiceNumber AS LastInvoiceNumber,
        LI.InvoiceDate AS LastInvoiceDate,
        LI.GrandTotal AS LastInvoiceTotal,
        LI.Remaining AS LastInvoiceRemaining
    FROM dbo.Subscribers S
    INNER JOIN ScopeSubscribers SS ON SS.SubscriberID = S.SubscriberID
    LEFT JOIN PrimaryMeter PM ON PM.SubscriberID = S.SubscriberID AND PM.rn = 1
    OUTER APPLY
    (
        SELECT SUM(CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) - ISNULL(PA.PaidTotal,0) AS DECIMAL(18,2))) AS CurrentDue
        FROM dbo.Invoices I
        OUTER APPLY (SELECT SUM(P.Amount) AS PaidTotal FROM dbo.Payments P WHERE P.InvoiceID = I.InvoiceID) PA
        WHERE I.SubscriberID = S.SubscriberID
          AND ISNULL(I.Status,N'') <> N'ملغاة'
          AND CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) - ISNULL(PA.PaidTotal,0) AS DECIMAL(18,2)) > 0
    ) Due
    OUTER APPLY
    (
        SELECT SUM(C.AmountRemaining) AS CurrentCredit
        FROM dbo.SubscriberCredits C
        WHERE C.SubscriberID = S.SubscriberID AND C.AmountRemaining > 0
    ) Cr
    OUTER APPLY
    (
        SELECT TOP 1
            I.InvoiceID,
            ISNULL(I.InvoiceNumber, CAST(I.InvoiceID AS NVARCHAR(30))) AS InvoiceNumber,
            I.InvoiceDate,
            CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS GrandTotal,
            CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) - ISNULL(PA.PaidTotal,0) AS DECIMAL(18,2)) AS Remaining
        FROM dbo.Invoices I
        OUTER APPLY (SELECT SUM(P.Amount) AS PaidTotal FROM dbo.Payments P WHERE P.InvoiceID = I.InvoiceID) PA
        WHERE I.SubscriberID = S.SubscriberID
          AND ISNULL(I.Status,N'') <> N'ملغاة'
        ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC
    ) LI
    ORDER BY S.Name;

    SELECT
        SM.SubscriberMeterID,
        SM.SubscriberID,
        M.MeterID,
        M.MeterNumber,
        M.MeterType,
        M.Location,
        M.IsActive,
        SM.IsPrimary,
        SM.LinkedAt
    FROM dbo.SubscriberMeters SM
    INNER JOIN dbo.Meters M ON M.MeterID = SM.MeterID
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = SM.SubscriberID
    WHERE S.IsActive = 1
      AND
      (
          @OnlyAssignedSubscribers = 0
          OR EXISTS (SELECT 1 FROM dbo.CollectorSubscribers CS WHERE CS.CollectorID=@CollectorID AND CS.SubscriberID=S.SubscriberID AND CS.IsActive=1)
          OR (@IncludeAllIfNoAssignments = 1 AND @HasAssignments = 0)
      )
    ORDER BY SM.SubscriberID, SM.IsPrimary DESC, M.MeterNumber;

    SELECT *
    FROM
    (
        SELECT
            I.InvoiceID,
            I.SubscriberID,
            I.MeterID,
            ISNULL(I.InvoiceNumber, CAST(I.InvoiceID AS NVARCHAR(30))) AS InvoiceNumber,
            I.InvoiceDate,
            CAST(ISNULL(I.Consumption,0) AS DECIMAL(18,2)) AS Consumption,
            CAST(ISNULL(I.UnitPrice,0) AS DECIMAL(18,2)) AS UnitPrice,
            CAST(ISNULL(I.ServiceFees,0) AS DECIMAL(18,2)) AS ServiceFees,
            CAST(ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS Arrears,
            CAST(ISNULL(I.TotalAmount,0) AS DECIMAL(18,2)) AS TotalAmount,
            CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS GrandTotal,
            CAST(ISNULL(PA.PaidTotal,0) AS DECIMAL(18,2)) AS PaidTotal,
            CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) - ISNULL(PA.PaidTotal,0) AS DECIMAL(18,2)) AS Remaining,
            I.Status,
            I.Notes
        FROM dbo.Invoices I
        OUTER APPLY (SELECT SUM(P.Amount) AS PaidTotal FROM dbo.Payments P WHERE P.InvoiceID = I.InvoiceID) PA
        WHERE ISNULL(I.Status, N'') <> N'ملغاة'
          AND I.InvoiceDate <= @AsOfDate
          AND
          (
              @OnlyAssignedSubscribers = 0
              OR EXISTS (SELECT 1 FROM dbo.CollectorSubscribers CS WHERE CS.CollectorID=@CollectorID AND CS.SubscriberID=I.SubscriberID AND CS.IsActive=1)
              OR (@IncludeAllIfNoAssignments = 1 AND @HasAssignments = 0)
          )
    ) X
    WHERE X.Remaining > 0
    ORDER BY X.InvoiceDate, X.InvoiceID;

    SELECT
        C.CreditID,
        C.SubscriberID,
        C.PaymentID,
        C.ReceiptID,
        C.MeterID,
        C.CreditDate,
        C.AmountTotal,
        C.AmountRemaining,
        C.Notes
    FROM dbo.SubscriberCredits C
    WHERE C.AmountRemaining > 0
      AND
      (
          @OnlyAssignedSubscribers = 0
          OR EXISTS (SELECT 1 FROM dbo.CollectorSubscribers CS WHERE CS.CollectorID=@CollectorID AND CS.SubscriberID=C.SubscriberID AND CS.IsActive=1)
          OR (@IncludeAllIfNoAssignments = 1 AND @HasAssignments = 0)
      )
    ORDER BY C.CreditDate, C.CreditID;
END
GO

IF OBJECT_ID(N'dbo.usp_MobileSync_SaveBatch', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_MobileSync_SaveBatch;
GO
CREATE PROCEDURE dbo.usp_MobileSync_SaveBatch
    @CollectorID INT,
    @SyncBatchID INT = NULL,
    @DeviceID INT,
    @DeviceCode NVARCHAR(100),
    @DeviceName NVARCHAR(100) = NULL,
    @DeviceModel NVARCHAR(100) = NULL,
    @AppVersion NVARCHAR(30) = NULL,
    @AutoCreateDevice BIT = 1,
    @Receipts dbo.MobileReceiptImportType READONLY,
    @Lines dbo.MobileReceiptImportLineType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @TotalRows INT = (SELECT COUNT(*) FROM @Receipts);
    DECLARE @InsertedCount INT = 0;
    DECLARE @DuplicateCount INT = 0;
    DECLARE @BatchStatus NVARCHAR(20) = N'Pending';

    BEGIN TRY
        BEGIN TRAN;

        IF @SyncBatchID IS NULL
        BEGIN
            INSERT INTO dbo.MobileSyncBatches(CollectorID, DeviceID, SyncDate, RecordsCount, SyncStatus, Notes, StartedAt)
            VALUES(@CollectorID, @DeviceID, GETDATE(), @TotalRows, N'Pending', NULL, GETDATE());
            SET @SyncBatchID = SCOPE_IDENTITY();
        END

        SELECT R.RowNo, R.LocalPaymentGuid, R.LocalReceiptNo, I.ImportID
        INTO #DupReceipts
        FROM @Receipts R
        INNER JOIN dbo.MobileReceiptImports I
            ON I.DeviceID = @DeviceID
           AND I.LocalPaymentGuid = R.LocalPaymentGuid;

        SELECT @DuplicateCount = COUNT(*) FROM #DupReceipts;

        SELECT R.*
        INTO #NewReceipts
        FROM @Receipts R
        LEFT JOIN #DupReceipts D ON D.RowNo = R.RowNo
        WHERE D.RowNo IS NULL;

        INSERT INTO dbo.MobileReceiptImports
        (
            SyncBatchID, CollectorID, DeviceID, LocalReceiptNo, LocalPaymentGuid,
            SubscriberID, PaymentDate, TotalReceived, PaymentMethod, Notes, ImportStatus, CreatedAt
        )
        SELECT
            @SyncBatchID, @CollectorID, @DeviceID, LocalReceiptNo, LocalPaymentGuid,
            SubscriberID, PaymentDate, TotalReceived, PaymentMethod, Notes, N'New', GETDATE()
        FROM #NewReceipts;

        SELECT @InsertedCount = @@ROWCOUNT;

        SELECT N.RowNo, I.ImportID
        INTO #Map
        FROM #NewReceipts N
        INNER JOIN dbo.MobileReceiptImports I
            ON I.DeviceID = @DeviceID
           AND I.LocalPaymentGuid = N.LocalPaymentGuid;

        INSERT INTO dbo.MobileReceiptImportLines(ImportID, InvoiceID, AppliedAmount, ApplicationType, Notes, CreatedAt)
        SELECT M.ImportID, L.InvoiceID, L.AppliedAmount, L.ApplicationType, L.Notes, GETDATE()
        FROM @Lines L
        INNER JOIN #Map M ON M.RowNo = L.ReceiptRowNo;

        SET @BatchStatus =
            CASE
                WHEN @InsertedCount > 0 AND @DuplicateCount = 0 THEN N'Imported'
                WHEN @InsertedCount > 0 AND @DuplicateCount > 0 THEN N'Partial'
                WHEN @InsertedCount = 0 AND @DuplicateCount > 0 THEN N'Rejected'
                ELSE N'Pending'
            END;

        UPDATE dbo.MobileSyncBatches
        SET SyncStatus = @BatchStatus,
            FinishedAt = GETDATE(),
            Notes = N'Total=' + CAST(@TotalRows AS NVARCHAR(20)) + N', Inserted=' + CAST(@InsertedCount AS NVARCHAR(20)) + N', Duplicates=' + CAST(@DuplicateCount AS NVARCHAR(20))
        WHERE SyncBatchID = @SyncBatchID;

        UPDATE dbo.CollectorDevices
        SET LastSyncAt = GETDATE(), DeviceName = COALESCE(@DeviceName, DeviceName), DeviceModel = COALESCE(@DeviceModel, DeviceModel), AppVersion = COALESCE(@AppVersion, AppVersion)
        WHERE DeviceID = @DeviceID;

        COMMIT;

        SELECT @SyncBatchID AS SyncBatchID, @DeviceID AS DeviceID, @TotalRows AS TotalRows, @InsertedCount AS InsertedCount, @DuplicateCount AS DuplicateCount, @BatchStatus AS BatchStatus;

        SELECT
            R.RowNo,
            R.LocalPaymentGuid,
            COALESCE(M.ImportID, D.ImportID) AS ImportID,
            CASE WHEN D.ImportID IS NOT NULL THEN N'Duplicate' ELSE N'Inserted' END AS SaveStatus
        FROM @Receipts R
        LEFT JOIN #Map M ON M.RowNo = R.RowNo
        LEFT JOIN #DupReceipts D ON D.RowNo = R.RowNo
        ORDER BY R.RowNo;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

IF OBJECT_ID(N'dbo.usp_MobileSync_GetImportDecisions', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_MobileSync_GetImportDecisions;
GO
CREATE PROCEDURE dbo.usp_MobileSync_GetImportDecisions
    @CollectorID INT,
    @DeviceCode NVARCHAR(100),
    @ChangedAfter DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

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
        CAST(COALESCE(I.ApprovedAt, I.CreatedAt) AS DATETIME) AS ChangedAt
    FROM dbo.MobileReceiptImports I
    INNER JOIN dbo.CollectorDevices D ON D.DeviceID = I.DeviceID
    LEFT JOIN dbo.Receipts R ON R.ReceiptID = I.ApprovedReceiptID
    LEFT JOIN dbo.Users U ON U.UserID = I.ApprovedByUserID
    WHERE I.CollectorID = @CollectorID
      AND D.DeviceCode = @DeviceCode
      AND (@ChangedAfter IS NULL OR COALESCE(I.ApprovedAt, I.CreatedAt) > @ChangedAfter)
    ORDER BY ChangedAt, I.ImportID;
END
GO
