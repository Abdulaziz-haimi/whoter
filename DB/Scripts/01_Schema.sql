USE [WaterBillingDB]
GO
/****** Object:  User [water_api]    Script Date: 5/8/2026 3:43:47 AM ******/
CREATE USER [water_api] FOR LOGIN [water_api] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  DatabaseRole [water_app_role]    Script Date: 5/8/2026 3:43:47 AM ******/
CREATE ROLE [water_app_role]
GO
ALTER ROLE [water_app_role] ADD MEMBER [water_api]
GO
ALTER ROLE [db_datareader] ADD MEMBER [water_api]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [water_api]
GO
/****** Object:  UserDefinedTableType [dbo].[ExpenseLineType]    Script Date: 5/8/2026 3:43:48 AM ******/
CREATE TYPE [dbo].[ExpenseLineType] AS TABLE(
	[ItemName] [nvarchar](150) NULL,
	[Qty] [decimal](18, 2) NULL,
	[UnitPrice] [decimal](18, 2) NULL,
	[TargetAccountID] [int] NULL,
	[Notes] [nvarchar](200) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[MobileReceiptImportLineType]    Script Date: 5/8/2026 3:43:48 AM ******/
CREATE TYPE [dbo].[MobileReceiptImportLineType] AS TABLE(
	[ReceiptRowNo] [int] NOT NULL,
	[InvoiceID] [int] NULL,
	[AppliedAmount] [decimal](12, 2) NOT NULL,
	[ApplicationType] [nvarchar](30) NOT NULL,
	[Notes] [nvarchar](200) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[MobileReceiptImportType]    Script Date: 5/8/2026 3:43:48 AM ******/
CREATE TYPE [dbo].[MobileReceiptImportType] AS TABLE(
	[RowNo] [int] NOT NULL,
	[LocalPaymentGuid] [nvarchar](100) NOT NULL,
	[LocalReceiptNo] [nvarchar](50) NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[PaymentDate] [date] NOT NULL,
	[TotalReceived] [decimal](12, 2) NOT NULL,
	[PaymentMethod] [nvarchar](30) NOT NULL,
	[Notes] [nvarchar](200) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[SubscriberImportType]    Script Date: 5/8/2026 3:43:48 AM ******/
CREATE TYPE [dbo].[SubscriberImportType] AS TABLE(
	[RowNo] [int] NOT NULL,
	[SubscriberName] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Address] [nvarchar](150) NULL,
	[MeterNumber] [nvarchar](50) NULL,
	[MeterLocation] [nvarchar](150) NULL,
	[IsPrimary] [bit] NULL,
	[AccountCode] [nvarchar](50) NULL,
	[InitialReading] [decimal](18, 3) NULL
)
GO
/****** Object:  UserDefinedFunction [dbo].[fn_IsPeriodClosed]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_IsPeriodClosed](@Date DATE)
RETURNS BIT
AS
BEGIN
    DECLARE @Result BIT = 0;
    IF EXISTS (
        SELECT 1 FROM AccountingPeriods
        WHERE @Date BETWEEN FromDate AND ToDate
          AND IsClosed = 1
    )
        SET @Result = 1;
    RETURN @Result;
END;
GO
/****** Object:  Table [dbo].[Receipts]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Receipts](
	[ReceiptID] [int] IDENTITY(1,1) NOT NULL,
	[ReceiptNumber] [nvarchar](30) NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[CollectorID] [int] NULL,
	[PaymentDate] [date] NOT NULL,
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[PaymentMethod] [nvarchar](30) NOT NULL,
	[Notes] [nvarchar](200) NULL,
	[JournalID] [int] NULL,
	[Status] [nvarchar](20) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReceiptID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Meters]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Meters](
	[MeterID] [int] IDENTITY(1,1) NOT NULL,
	[MeterNumber] [nvarchar](50) NOT NULL,
	[MeterType] [nvarchar](20) NOT NULL,
	[Location] [nvarchar](150) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MeterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[MeterNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubscriberMeters]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriberMeters](
	[SubscriberMeterID] [int] IDENTITY(1,1) NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[MeterID] [int] NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[LinkedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubscriberMeterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Collectors]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Collectors](
	[CollectorID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[UserID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CollectorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subscribers]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscribers](
	[SubscriberID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[MeterNumber] [nvarchar](50) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Address] [nvarchar](150) NULL,
	[CreatedDate] [datetime] NULL,
	[AccountID] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[TariffPlanID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SubscriberID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[InvoiceDate] [date] NOT NULL,
	[Consumption] [decimal](10, 2) NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[ServiceFees] [decimal](10, 2) NULL,
	[Arrears] [decimal](10, 2) NULL,
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[Notes] [nvarchar](200) NULL,
	[ReadingID] [int] NULL,
	[InvoiceNumber] [nvarchar](30) NULL,
	[MeterID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[SubscriberID] [int] NOT NULL,
	[CollectorID] [int] NULL,
	[PaymentDate] [date] NOT NULL,
	[Amount] [decimal](12, 2) NOT NULL,
	[PaymentType] [nvarchar](30) NULL,
	[Notes] [nvarchar](200) NULL,
	[ReceiptNumber] [nvarchar](30) NULL,
	[PaymentCategory] [nvarchar](30) NULL,
	[ReceiptID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_PaymentsDisplay]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_PaymentsDisplay]
AS
SELECT
    P.PaymentID,
    P.PaymentDate AS [التاريخ],
    S.Name AS [المشترك],
    ISNULL(MI.MeterNumber, ISNULL(MP.MeterNumber, N'')) AS [رقم العداد],
    CASE 
        WHEN P.InvoiceID IS NULL THEN N'رصيد مقدم'
        ELSE CAST(P.InvoiceID AS NVARCHAR(20))
    END AS [رقم الفاتورة],
    P.Amount AS [المبلغ],
    CASE 
        WHEN P.PaymentCategory = N'AdvanceCredit' THEN N'رصيد مقدم'
        WHEN P.PaymentCategory = N'CreditSettlement' THEN N'تسوية من رصيد مقدم'
        ELSE N'سداد'
    END AS [النوع],
    CASE 
        WHEN P.PaymentType = N'Cash' THEN N'نقداً'
        WHEN P.PaymentType = N'Transfer' THEN N'تحويل'
        WHEN P.PaymentType = N'Cheque' THEN N'شيك'
        WHEN P.PaymentType = N'Other' THEN N'أخرى'
        ELSE ISNULL(P.PaymentType, N'')
    END AS [الطريقة],
    ISNULL(C.Name, N'') AS [المحصل],
    ISNULL(P.Notes, N'') AS [ملاحظات],
    R.ReceiptNumber AS [رقم الإيصال]
FROM dbo.Payments P
INNER JOIN dbo.Subscribers S ON P.SubscriberID = S.SubscriberID
LEFT JOIN dbo.Collectors C ON P.CollectorID = C.CollectorID
LEFT JOIN dbo.Receipts R ON R.ReceiptID = P.ReceiptID
LEFT JOIN dbo.Invoices I ON P.InvoiceID = I.InvoiceID
LEFT JOIN dbo.Meters MI ON I.MeterID = MI.MeterID
LEFT JOIN dbo.SubscriberMeters SM ON SM.SubscriberID = S.SubscriberID AND SM.IsPrimary = 1
LEFT JOIN dbo.Meters MP ON MP.MeterID = SM.MeterID;
GO
/****** Object:  Table [dbo].[AccountStatements]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountStatements](
	[StatementID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NOT NULL,
	[Details] [nvarchar](200) NULL,
	[Item] [nvarchar](100) NULL,
	[DocumentType] [nvarchar](50) NULL,
	[DocumentNumber] [nvarchar](50) NULL,
	[Credit] [decimal](12, 2) NULL,
	[Debit] [decimal](12, 2) NULL,
	[SubscriberID] [int] NOT NULL,
	[InvoiceID] [int] NULL,
	[PaymentID] [int] NULL,
	[BalanceAfter] [decimal](12, 2) NULL,
	[JournalID] [int] NULL,
	[MeterID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StatementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_AccountStatement]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   VIEW [dbo].[vw_AccountStatement]
AS
SELECT 
    st.StatementID,
    st.SubscriberID,
    st.MeterID,
    m.MeterNumber,
    m.Location AS MeterLocation,
    st.[Date],
    st.Details,
    st.Debit,
    st.Credit,
    st.InvoiceID,
    st.PaymentID,
    st.DocumentType,
    st.DocumentNumber,
    st.Item,
    st.JournalID,
    SUM(ISNULL(st.Debit,0) - ISNULL(st.Credit,0))
        OVER (PARTITION BY st.SubscriberID, st.MeterID
              ORDER BY st.[Date], st.StatementID) AS RunningBalance
FROM dbo.AccountStatements st
LEFT JOIN dbo.Meters m ON st.MeterID = m.MeterID;
GO
/****** Object:  Table [dbo].[BillingConstants]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BillingConstants](
	[ConstantID] [int] IDENTITY(1,1) NOT NULL,
	[EffectiveFrom] [date] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[DefaultServiceFees] [decimal](18, 2) NOT NULL,
	[Notes] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ServiceFees] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ConstantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_CurrentBillingConstants]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   C) View التعرفة الحالية: لا ترجع تعرفة مستقبلية
   ========================================================= */
CREATE   VIEW [dbo].[vw_CurrentBillingConstants]
AS
SELECT TOP (1) *
FROM dbo.BillingConstants
WHERE IsActive = 1
  AND EffectiveFrom <= CAST(GETDATE() AS DATE)
ORDER BY EffectiveFrom DESC, ConstantID DESC;
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[AccountCode] [nvarchar](20) NOT NULL,
	[AccountName] [nvarchar](100) NOT NULL,
	[AccountType] [nvarchar](20) NOT NULL,
	[IsControl] [bit] NULL,
	[ParentAccountID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Journals]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Journals](
	[JournalID] [int] IDENTITY(1,1) NOT NULL,
	[JournalDate] [date] NOT NULL,
	[Description] [nvarchar](200) NULL,
	[Source] [nvarchar](50) NULL,
	[SourceID] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[IsPosted] [bit] NULL,
	[PostedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[JournalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JournalEntries]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JournalEntries](
	[EntryID] [int] IDENTITY(1,1) NOT NULL,
	[JournalID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Debit] [decimal](12, 2) NULL,
	[Credit] [decimal](12, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[EntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_GeneralLedger]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   6) تحسين دفتر الأستاذ وميزان المراجعة
   ========================================================= */

CREATE   VIEW [dbo].[vw_GeneralLedger]
AS
SELECT
    A.AccountID,
    A.AccountCode,
    A.AccountName,
    J.JournalDate,
    J.JournalID,
    J.Description,
    J.Source,
    J.SourceID,
    E.EntryID,
    CAST(ISNULL(E.Debit, 0) AS DECIMAL(18,2)) AS Debit,
    CAST(ISNULL(E.Credit, 0) AS DECIMAL(18,2)) AS Credit,
    SUM(CAST(ISNULL(E.Debit, 0) - ISNULL(E.Credit, 0) AS DECIMAL(18,2)))
        OVER
        (
            PARTITION BY A.AccountID
            ORDER BY J.JournalDate, J.JournalID, E.EntryID
        ) AS RunningBalance
FROM dbo.JournalEntries E
INNER JOIN dbo.Journals J
    ON E.JournalID = J.JournalID
INNER JOIN dbo.Accounts A
    ON E.AccountID = A.AccountID
WHERE ISNULL(J.IsPosted, 0) = 1;
GO
/****** Object:  View [dbo].[vw_TrialBalance]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   VIEW [dbo].[vw_TrialBalance]
AS
SELECT
    A.AccountID,
    A.AccountCode,
    A.AccountName,
    CAST(ISNULL(SUM(CASE WHEN ISNULL(J.IsPosted, 0) = 1 THEN ISNULL(E.Debit, 0) ELSE 0 END), 0) AS DECIMAL(18,2)) AS TotalDebit,
    CAST(ISNULL(SUM(CASE WHEN ISNULL(J.IsPosted, 0) = 1 THEN ISNULL(E.Credit, 0) ELSE 0 END), 0) AS DECIMAL(18,2)) AS TotalCredit,
    CAST(ISNULL(SUM(CASE WHEN ISNULL(J.IsPosted, 0) = 1 THEN ISNULL(E.Debit, 0) - ISNULL(E.Credit, 0) ELSE 0 END), 0) AS DECIMAL(18,2)) AS Balance
FROM dbo.Accounts A
LEFT JOIN dbo.JournalEntries E
    ON A.AccountID = E.AccountID
LEFT JOIN dbo.Journals J
    ON E.JournalID = J.JournalID
GROUP BY
    A.AccountID,
    A.AccountCode,
    A.AccountName;
GO
/****** Object:  Table [dbo].[SubscriberCredits]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriberCredits](
	[CreditID] [int] IDENTITY(1,1) NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[PaymentID] [int] NOT NULL,
	[MeterID] [int] NULL,
	[CreditDate] [date] NOT NULL,
	[AmountTotal] [decimal](18, 2) NOT NULL,
	[AmountRemaining] [decimal](18, 2) NOT NULL,
	[Notes] [nvarchar](200) NULL,
	[ReceiptID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CreditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_SubscriberBalanceByMeter]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_SubscriberBalanceByMeter]
AS
WITH PrimaryMeter AS
(
    SELECT
        sm.SubscriberID,
        sm.MeterID,
        ROW_NUMBER() OVER
        (
            PARTITION BY sm.SubscriberID
            ORDER BY sm.IsPrimary DESC, sm.SubscriberMeterID DESC
        ) AS rn
    FROM dbo.SubscriberMeters sm
)
SELECT
    s.SubscriberID,
    m.MeterID,
    m.MeterNumber,
    m.Location AS MeterLocation,
    CAST(ISNULL(due.DueAmount, 0) - ISNULL(cr.CreditRemain, 0) AS DECIMAL(12,2)) AS Balance
FROM dbo.Subscribers s
JOIN dbo.SubscriberMeters sm ON sm.SubscriberID = s.SubscriberID
JOIN dbo.Meters m ON m.MeterID = sm.MeterID
OUTER APPLY
(
    -- متأخرات هذا العداد فقط
    SELECT
        CAST(ISNULL(SUM(
            CAST(I.TotalAmount AS DECIMAL(12,2)) -
            CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
        ), 0) AS DECIMAL(12,2)) AS DueAmount
    FROM dbo.Invoices I
    OUTER APPLY
    (
        SELECT SUM(P.Amount) AS Paid
        FROM dbo.Payments P
        WHERE P.InvoiceID = I.InvoiceID
    ) PA
    WHERE I.SubscriberID = s.SubscriberID
      AND I.MeterID = m.MeterID
      AND CAST(I.TotalAmount AS DECIMAL(12,2)) > CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
      AND ISNULL(I.Status, N'') <> N'ملغاة'
) due
OUTER APPLY
(
    SELECT
        CAST(ISNULL(SUM(CAST(c.AmountRemaining AS DECIMAL(12,2))), 0) AS DECIMAL(12,2)) AS CreditRemain
    FROM dbo.SubscriberCredits c
    LEFT JOIN PrimaryMeter pm
        ON pm.SubscriberID = c.SubscriberID AND pm.rn = 1
    WHERE c.SubscriberID = s.SubscriberID
      AND c.AmountRemaining > 0
      AND
      (
          c.MeterID = m.MeterID
          OR (c.MeterID IS NULL AND pm.MeterID = m.MeterID) -- الرصيد العام يُعرض على العداد الأساسي فقط
      )
) cr;
GO
/****** Object:  Table [dbo].[SystemAccounts2]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemAccounts2](
	[ID] [int] NOT NULL,
	[CashAccountID] [int] NOT NULL,
	[ReceivableControlAccountID] [int] NULL,
	[WaterRevenueAccountID] [int] NOT NULL,
	[ServiceRevenueAccountID] [int] NOT NULL,
 CONSTRAINT [PK_SystemAccounts2] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemAccounts]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemAccounts](
	[CashAccountID] [int] NULL,
	[RevenueAccountID] [int] NULL,
	[ServiceRevenueAccountID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_SystemAccountsUnified]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =========================================================
   B) View موحّد لحسابات النظام: يرجع صفًا واحدًا فقط
   ويفضل SystemAccounts2 على الجدول القديم
   ========================================================= */
CREATE   VIEW [dbo].[vw_SystemAccountsUnified]
AS
SELECT TOP (1)
    X.CashAccountID,
    X.ReceivableControlAccountID,
    X.WaterRevenueAccountID,
    X.ServiceRevenueAccountID
FROM
(
    SELECT
        1 AS PriorityOrder,
        SA2.CashAccountID,
        SA2.ReceivableControlAccountID,
        SA2.WaterRevenueAccountID,
        SA2.ServiceRevenueAccountID
    FROM dbo.SystemAccounts2 SA2

    UNION ALL

    SELECT
        2 AS PriorityOrder,
        SA.CashAccountID,
        NULL AS ReceivableControlAccountID,
        SA.RevenueAccountID AS WaterRevenueAccountID,
        SA.ServiceRevenueAccountID
    FROM dbo.SystemAccounts SA
) X
ORDER BY X.PriorityOrder;
GO
/****** Object:  View [dbo].[vw_ReceiptsSummary]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_ReceiptsSummary]
AS
SELECT
    R.ReceiptID,
    R.ReceiptNumber,
    R.SubscriberID,
    S.Name AS SubscriberName,
    R.CollectorID,
    C.Name AS CollectorName,
    R.PaymentDate,
    R.TotalAmount,
    R.PaymentMethod,
    R.Notes,
    R.JournalID,
    R.Status,
    R.CreatedAt
FROM dbo.Receipts R
INNER JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
LEFT JOIN dbo.Collectors C ON C.CollectorID = R.CollectorID;
GO
/****** Object:  Table [dbo].[ReceiptApplications]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReceiptApplications](
	[ReceiptApplicationID] [int] IDENTITY(1,1) NOT NULL,
	[ReceiptID] [int] NOT NULL,
	[InvoiceID] [int] NULL,
	[CreditID] [int] NULL,
	[AppliedAmount] [decimal](12, 2) NOT NULL,
	[ApplicationType] [nvarchar](30) NOT NULL,
	[PaymentID] [int] NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReceiptApplicationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_ReceiptApplicationsDetailed]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_ReceiptApplicationsDetailed]
AS
SELECT
    RA.ReceiptApplicationID,
    RA.ReceiptID,
    R.ReceiptNumber,
    R.PaymentDate,
    R.SubscriberID,
    S.Name AS SubscriberName,
    RA.InvoiceID,
    I.InvoiceDate,
    I.InvoiceNumber,
    RA.CreditID,
    RA.AppliedAmount,
    RA.ApplicationType,
    RA.PaymentID,
    RA.CreatedAt
FROM dbo.ReceiptApplications RA
INNER JOIN dbo.Receipts R ON R.ReceiptID = RA.ReceiptID
INNER JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
LEFT JOIN dbo.Invoices I ON I.InvoiceID = RA.InvoiceID;
GO
/****** Object:  View [dbo].[vw_SubscriberBalance]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_SubscriberBalance]
AS
SELECT
    s.SubscriberID,
    CAST(ISNULL(due.DueAmount, 0) - ISNULL(cr.CreditRemain, 0) AS DECIMAL(12,2)) AS Balance
FROM dbo.Subscribers s
OUTER APPLY
(
    SELECT
        CAST(ISNULL(SUM(
            CAST(I.TotalAmount AS DECIMAL(12,2)) -
            CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
        ), 0) AS DECIMAL(12,2)) AS DueAmount
    FROM dbo.Invoices I
    OUTER APPLY
    (
        SELECT SUM(P.Amount) AS Paid
        FROM dbo.Payments P
        WHERE P.InvoiceID = I.InvoiceID
    ) PA
    WHERE I.SubscriberID = s.SubscriberID
      AND CAST(I.TotalAmount AS DECIMAL(12,2)) > CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
      AND ISNULL(I.Status, N'') <> N'ملغاة'
) due
OUTER APPLY
(
    SELECT
        CAST(ISNULL(SUM(CAST(c.AmountRemaining AS DECIMAL(12,2))), 0) AS DECIMAL(12,2)) AS CreditRemain
    FROM dbo.SubscriberCredits c
    WHERE c.SubscriberID = s.SubscriberID
      AND c.AmountRemaining > 0
) cr;
GO
/****** Object:  View [dbo].[vw_AccountStatementShort]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_AccountStatementShort]
AS
SELECT
    R.ReceiptID,
    R.ReceiptNumber,
    R.PaymentDate AS [Date],
    R.SubscriberID,
    S.Name AS SubscriberName,
    R.TotalAmount AS Amount,
    R.PaymentMethod,
    R.Notes,
    R.CollectorID,
    C.Name AS CollectorName
FROM dbo.Receipts R
INNER JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
LEFT JOIN dbo.Collectors C ON C.CollectorID = R.CollectorID;
GO
/****** Object:  Table [dbo].[AccountingPeriods]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountingPeriods](
	[PeriodID] [int] IDENTITY(1,1) NOT NULL,
	[FiscalYearID] [int] NULL,
	[PeriodName] [nvarchar](20) NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
	[IsClosed] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[PeriodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[Action] [nvarchar](100) NULL,
	[TableName] [nvarchar](50) NULL,
	[RecordID] [int] NULL,
	[ActionDate] [datetime] NULL,
	[Details] [nvarchar](500) NULL,
	[EntityName] [nvarchar](150) NULL,
	[DeviceName] [nvarchar](100) NULL,
	[UserName] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CollectorDevices]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollectorDevices](
	[DeviceID] [int] IDENTITY(1,1) NOT NULL,
	[CollectorID] [int] NOT NULL,
	[DeviceCode] [nvarchar](100) NOT NULL,
	[DeviceName] [nvarchar](100) NULL,
	[IsApproved] [bit] NOT NULL,
	[LastSyncAt] [datetime] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[DeviceModel] [nvarchar](100) NULL,
	[AppVersion] [nvarchar](30) NULL,
	[IsActive] [bit] NOT NULL,
	[PhoneNumber] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[DeviceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CollectorSubscribers]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollectorSubscribers](
	[CollectorSubscriberID] [int] IDENTITY(1,1) NOT NULL,
	[CollectorID] [int] NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[AssignedAt] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CollectorSubscriberID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentSequences]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentSequences](
	[SequenceID] [int] IDENTITY(1,1) NOT NULL,
	[DocType] [nvarchar](20) NOT NULL,
	[Year] [int] NOT NULL,
	[NextNumber] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SequenceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_DocSeq] UNIQUE NONCLUSTERED 
(
	[DocType] ASC,
	[Year] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExpenseCategories]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpenseCategories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[CategoryType] [nvarchar](20) NOT NULL,
	[DefaultAccountID] [int] NULL,
	[Notes] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExpenseLines]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpenseLines](
	[ExpenseLineID] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseID] [int] NOT NULL,
	[ItemName] [nvarchar](150) NOT NULL,
	[Qty] [decimal](18, 2) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[LineTotal]  AS ([Qty]*[UnitPrice]) PERSISTED,
	[TargetAccountID] [int] NULL,
	[Notes] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ExpenseLineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Expenses]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Expenses](
	[ExpenseID] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseNumber] [nvarchar](30) NOT NULL,
	[ExpenseDate] [date] NOT NULL,
	[CategoryID] [int] NOT NULL,
	[SupplierName] [nvarchar](150) NULL,
	[Description] [nvarchar](250) NULL,
	[Notes] [nvarchar](250) NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[PaymentMethod] [nvarchar](30) NOT NULL,
	[CashAccountID] [int] NULL,
	[CounterAccountID] [int] NULL,
	[JournalID] [int] NULL,
	[CreatedBy] [int] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsPosted] [bit] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ExpenseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FiscalYears]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FiscalYears](
	[FiscalYearID] [int] IDENTITY(1,1) NOT NULL,
	[YearName] [nvarchar](20) NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[IsClosed] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[FiscalYearID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MainMeterReadings]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MainMeterReadings](
	[MainReadingID] [int] IDENTITY(1,1) NOT NULL,
	[MeterID] [int] NOT NULL,
	[ReadingDate] [date] NOT NULL,
	[PreviousReading] [decimal](18, 2) NOT NULL,
	[CurrentReading] [decimal](18, 2) NOT NULL,
	[Consumption] [decimal](18, 2) NOT NULL,
	[Notes] [nvarchar](200) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MainReadingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_MainMeterReadings_Meter_Date] UNIQUE NONCLUSTERED 
(
	[MeterID] ASC,
	[ReadingDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageSettings]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageSettings](
	[SettingID] [int] IDENTITY(1,1) NOT NULL,
	[SettingName] [nvarchar](100) NOT NULL,
	[SettingValue] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[SettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_MessageSettings_Name] UNIQUE NONCLUSTERED 
(
	[SettingName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageTemplateMap]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageTemplateMap](
	[MapID] [int] IDENTITY(1,1) NOT NULL,
	[EventKey] [nvarchar](50) NOT NULL,
	[TemplateID] [int] NULL,
	[IsEnabled] [bit] NOT NULL,
	[UpdatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MapID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[EventKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MessageTemplates]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MessageTemplates](
	[TemplateID] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [nvarchar](100) NOT NULL,
	[TemplateText] [nvarchar](max) NOT NULL,
	[TemplateType] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Language] [nvarchar](10) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeterReports]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterReports](
	[ReportID] [int] IDENTITY(1,1) NOT NULL,
	[ReportDate] [date] NOT NULL,
	[MainMeterPrev] [decimal](10, 2) NULL,
	[MainMeterCurr] [decimal](10, 2) NULL,
	[MainMeterDiff] [decimal](10, 2) NULL,
	[TotalSubMetersPrev] [decimal](10, 2) NULL,
	[TotalSubMetersCurr] [decimal](10, 2) NULL,
	[TotalSubMetersDiff] [decimal](10, 2) NULL,
	[WaterLoss] [decimal](10, 2) NULL,
	[TotalConsumptionAmount] [decimal](12, 2) NULL,
	[TotalServiceFees] [decimal](12, 2) NULL,
	[TotalDue] [decimal](12, 2) NULL,
	[TotalFromQasem] [decimal](12, 2) NULL,
	[RemainingWithQasem] [decimal](12, 2) NULL,
	[Arrears] [decimal](12, 2) NULL,
	[MainMeterID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileReceiptImportLines]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileReceiptImportLines](
	[ImportLineID] [int] IDENTITY(1,1) NOT NULL,
	[ImportID] [int] NOT NULL,
	[InvoiceID] [int] NULL,
	[AppliedAmount] [decimal](12, 2) NOT NULL,
	[ApplicationType] [nvarchar](30) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Notes] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ImportLineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileReceiptImports]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileReceiptImports](
	[ImportID] [int] IDENTITY(1,1) NOT NULL,
	[SyncBatchID] [int] NOT NULL,
	[CollectorID] [int] NOT NULL,
	[DeviceID] [int] NOT NULL,
	[LocalReceiptNo] [nvarchar](50) NOT NULL,
	[LocalPaymentGuid] [nvarchar](100) NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[PaymentDate] [date] NOT NULL,
	[TotalReceived] [decimal](12, 2) NOT NULL,
	[PaymentMethod] [nvarchar](30) NOT NULL,
	[Notes] [nvarchar](200) NULL,
	[ImportStatus] [nvarchar](20) NOT NULL,
	[ApprovedReceiptID] [int] NULL,
	[ApprovedAt] [datetime] NULL,
	[RejectedReason] [nvarchar](200) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ApprovedByUserID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ImportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileSyncBatches]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileSyncBatches](
	[SyncBatchID] [int] IDENTITY(1,1) NOT NULL,
	[CollectorID] [int] NOT NULL,
	[DeviceID] [int] NOT NULL,
	[SyncDate] [datetime] NOT NULL,
	[RecordsCount] [int] NOT NULL,
	[SyncStatus] [nvarchar](20) NOT NULL,
	[Notes] [nvarchar](200) NULL,
	[StartedAt] [datetime] NULL,
	[FinishedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SyncBatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MobileSyncErrors]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MobileSyncErrors](
	[SyncErrorID] [int] IDENTITY(1,1) NOT NULL,
	[SyncBatchID] [int] NULL,
	[ImportID] [int] NULL,
	[ErrorMessage] [nvarchar](500) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ErrorSource] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[SyncErrorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[PermissionID] [int] IDENTITY(1,1) NOT NULL,
	[PermissionKey] [nvarchar](100) NOT NULL,
	[PermissionName] [nvarchar](150) NOT NULL,
	[Category] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[PermissionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Readings]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Readings](
	[ReadingID] [int] IDENTITY(1,1) NOT NULL,
	[SubscriberID] [int] NOT NULL,
	[ReadingDate] [date] NOT NULL,
	[PreviousReading] [decimal](10, 2) NULL,
	[CurrentReading] [decimal](10, 2) NULL,
	[Consumption] [decimal](10, 2) NULL,
	[Notes] [nvarchar](200) NULL,
	[MeterID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReadingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportPresets]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportPresets](
	[PresetID] [int] IDENTITY(1,1) NOT NULL,
	[PresetName] [nvarchar](100) NOT NULL,
	[FormKey] [nvarchar](50) NOT NULL,
	[JsonOptions] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PresetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportTemplateColumns]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportTemplateColumns](
	[TemplateColumnID] [int] IDENTITY(1,1) NOT NULL,
	[TemplateID] [int] NOT NULL,
	[ColumnKey] [nvarchar](100) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsVisible] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TemplateColumnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportTemplateFilters]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportTemplateFilters](
	[TemplateFilterID] [int] IDENTITY(1,1) NOT NULL,
	[TemplateID] [int] NOT NULL,
	[FilterKey] [nvarchar](100) NOT NULL,
	[FilterOperator] [nvarchar](20) NOT NULL,
	[FilterValue] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemplateFilterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportTemplates]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportTemplates](
	[TemplateID] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [nvarchar](200) NOT NULL,
	[ReportKey] [nvarchar](100) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[RolePermissionID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
	[IsAllowed] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RolePermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_RolePermissions] UNIQUE NONCLUSTERED 
(
	[RoleID] ASC,
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SmsLogs]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SmsLogs](
	[SmsID] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceID] [int] NULL,
	[SubscriberID] [int] NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Message] [nvarchar](max) NULL,
	[Status] [nvarchar](20) NULL,
	[Reason] [nvarchar](200) NULL,
	[SentDate] [datetime] NULL,
	[CollectorID] [int] NULL,
	[TemplateID] [int] NULL,
	[RetryCount] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[MessageType] [nvarchar](20) NULL,
	[PaymentID] [int] NULL,
	[ReceiptID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SmsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TariffPlans]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TariffPlans](
	[TariffPlanID] [int] IDENTITY(1,1) NOT NULL,
	[PlanName] [nvarchar](100) NOT NULL,
	[PricingModel] [nvarchar](20) NOT NULL,
	[FixedUnitPrice] [decimal](18, 2) NULL,
	[DefaultServiceFees] [decimal](18, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TariffPlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TariffRates]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TariffRates](
	[TariffRateID] [int] IDENTITY(1,1) NOT NULL,
	[TariffPlanID] [int] NOT NULL,
	[FromQty] [decimal](18, 2) NOT NULL,
	[ToQty] [decimal](18, 2) NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TariffRateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[PasswordHash] [nvarchar](200) NOT NULL,
	[RoleID] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[Phone] [char](10) NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AccountingPeriods] ADD  DEFAULT ((0)) FOR [IsClosed]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT ((0)) FOR [IsControl]
GO
ALTER TABLE [dbo].[AccountStatements] ADD  DEFAULT ((0)) FOR [Credit]
GO
ALTER TABLE [dbo].[AccountStatements] ADD  DEFAULT ((0)) FOR [Debit]
GO
ALTER TABLE [dbo].[AuditLog] ADD  DEFAULT (getdate()) FOR [ActionDate]
GO
ALTER TABLE [dbo].[BillingConstants] ADD  DEFAULT ((0)) FOR [DefaultServiceFees]
GO
ALTER TABLE [dbo].[BillingConstants] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[BillingConstants] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[BillingConstants] ADD  CONSTRAINT [DF_BillingConstants_ServiceFees]  DEFAULT ((0)) FOR [ServiceFees]
GO
ALTER TABLE [dbo].[CollectorDevices] ADD  DEFAULT ((1)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[CollectorDevices] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CollectorDevices] ADD  CONSTRAINT [DF_CollectorDevices_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[CollectorSubscribers] ADD  DEFAULT (getdate()) FOR [AssignedAt]
GO
ALTER TABLE [dbo].[CollectorSubscribers] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[DocumentSequences] ADD  DEFAULT ((1)) FOR [NextNumber]
GO
ALTER TABLE [dbo].[ExpenseCategories] ADD  CONSTRAINT [DF_ExpenseCategories_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Expenses] ADD  CONSTRAINT [DF_Expenses_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Expenses] ADD  CONSTRAINT [DF_Expenses_IsPosted]  DEFAULT ((0)) FOR [IsPosted]
GO
ALTER TABLE [dbo].[Expenses] ADD  CONSTRAINT [DF_Expenses_Status]  DEFAULT (N'Posted') FOR [Status]
GO
ALTER TABLE [dbo].[FiscalYears] ADD  DEFAULT ((0)) FOR [IsClosed]
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [ServiceFees]
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT ((0)) FOR [Arrears]
GO
ALTER TABLE [dbo].[Invoices] ADD  CONSTRAINT [DF_Invoices_Status_Arabic]  DEFAULT (N'غير مدفوعة') FOR [Status]
GO
ALTER TABLE [dbo].[JournalEntries] ADD  DEFAULT ((0)) FOR [Debit]
GO
ALTER TABLE [dbo].[JournalEntries] ADD  DEFAULT ((0)) FOR [Credit]
GO
ALTER TABLE [dbo].[Journals] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Journals] ADD  DEFAULT ((0)) FOR [IsPosted]
GO
ALTER TABLE [dbo].[MainMeterReadings] ADD  CONSTRAINT [DF_MainMeterReadings_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[MessageTemplateMap] ADD  DEFAULT ((1)) FOR [IsEnabled]
GO
ALTER TABLE [dbo].[MessageTemplateMap] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[MessageTemplates] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[MessageTemplates] ADD  DEFAULT ('AR') FOR [Language]
GO
ALTER TABLE [dbo].[MessageTemplates] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Meters] ADD  DEFAULT (N'Sub') FOR [MeterType]
GO
ALTER TABLE [dbo].[Meters] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Meters] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[MobileReceiptImportLines] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[MobileReceiptImports] ADD  DEFAULT (N'New') FOR [ImportStatus]
GO
ALTER TABLE [dbo].[MobileReceiptImports] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[MobileSyncBatches] ADD  DEFAULT (getdate()) FOR [SyncDate]
GO
ALTER TABLE [dbo].[MobileSyncBatches] ADD  DEFAULT ((0)) FOR [RecordsCount]
GO
ALTER TABLE [dbo].[MobileSyncBatches] ADD  DEFAULT (N'Pending') FOR [SyncStatus]
GO
ALTER TABLE [dbo].[MobileSyncErrors] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Payments] ADD  CONSTRAINT [DF_Payments_PaymentType]  DEFAULT (N'Cash') FOR [PaymentType]
GO
ALTER TABLE [dbo].[Payments] ADD  CONSTRAINT [DF_Payments_PaymentCategory]  DEFAULT (N'NormalPayment') FOR [PaymentCategory]
GO
ALTER TABLE [dbo].[Permissions] ADD  CONSTRAINT [DF_Permissions_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ReceiptApplications] ADD  CONSTRAINT [DF_ReceiptApplications_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Receipts] ADD  CONSTRAINT [DF_Receipts_Status]  DEFAULT (N'Posted') FOR [Status]
GO
ALTER TABLE [dbo].[Receipts] ADD  CONSTRAINT [DF_Receipts_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReportPresets] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReportTemplateColumns] ADD  DEFAULT ((1)) FOR [IsVisible]
GO
ALTER TABLE [dbo].[ReportTemplates] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReportTemplates] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[RolePermissions] ADD  CONSTRAINT [DF_RolePermissions_IsAllowed]  DEFAULT ((1)) FOR [IsAllowed]
GO
ALTER TABLE [dbo].[SmsLogs] ADD  DEFAULT (getdate()) FOR [SentDate]
GO
ALTER TABLE [dbo].[SmsLogs] ADD  DEFAULT ((0)) FOR [RetryCount]
GO
ALTER TABLE [dbo].[SmsLogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SubscriberMeters] ADD  DEFAULT ((1)) FOR [IsPrimary]
GO
ALTER TABLE [dbo].[SubscriberMeters] ADD  DEFAULT (getdate()) FOR [LinkedAt]
GO
ALTER TABLE [dbo].[Subscribers] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Subscribers] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SystemAccounts2] ADD  DEFAULT ((1)) FOR [ID]
GO
ALTER TABLE [dbo].[TariffPlans] ADD  DEFAULT (N'Fixed') FOR [PricingModel]
GO
ALTER TABLE [dbo].[TariffPlans] ADD  DEFAULT ((0)) FOR [DefaultServiceFees]
GO
ALTER TABLE [dbo].[TariffPlans] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TariffPlans] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[AccountingPeriods]  WITH CHECK ADD FOREIGN KEY([FiscalYearID])
REFERENCES [dbo].[FiscalYears] ([FiscalYearID])
GO
ALTER TABLE [dbo].[AccountStatements]  WITH CHECK ADD  CONSTRAINT [FK_AccountStatements_Journal] FOREIGN KEY([JournalID])
REFERENCES [dbo].[Journals] ([JournalID])
GO
ALTER TABLE [dbo].[AccountStatements] CHECK CONSTRAINT [FK_AccountStatements_Journal]
GO
ALTER TABLE [dbo].[AccountStatements]  WITH CHECK ADD  CONSTRAINT [FK_AccountStatements_Meter] FOREIGN KEY([MeterID])
REFERENCES [dbo].[Meters] ([MeterID])
GO
ALTER TABLE [dbo].[AccountStatements] CHECK CONSTRAINT [FK_AccountStatements_Meter]
GO
ALTER TABLE [dbo].[AccountStatements]  WITH CHECK ADD  CONSTRAINT [FK_AccountStatements_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[AccountStatements] CHECK CONSTRAINT [FK_AccountStatements_Subscriber]
GO
ALTER TABLE [dbo].[CollectorDevices]  WITH CHECK ADD  CONSTRAINT [FK_CollectorDevices_Collector] FOREIGN KEY([CollectorID])
REFERENCES [dbo].[Collectors] ([CollectorID])
GO
ALTER TABLE [dbo].[CollectorDevices] CHECK CONSTRAINT [FK_CollectorDevices_Collector]
GO
ALTER TABLE [dbo].[Collectors]  WITH CHECK ADD  CONSTRAINT [FK_Collectors_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Collectors] CHECK CONSTRAINT [FK_Collectors_User]
GO
ALTER TABLE [dbo].[CollectorSubscribers]  WITH CHECK ADD  CONSTRAINT [FK_CollectorSubscribers_Collector] FOREIGN KEY([CollectorID])
REFERENCES [dbo].[Collectors] ([CollectorID])
GO
ALTER TABLE [dbo].[CollectorSubscribers] CHECK CONSTRAINT [FK_CollectorSubscribers_Collector]
GO
ALTER TABLE [dbo].[CollectorSubscribers]  WITH CHECK ADD  CONSTRAINT [FK_CollectorSubscribers_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[CollectorSubscribers] CHECK CONSTRAINT [FK_CollectorSubscribers_Subscriber]
GO
ALTER TABLE [dbo].[ExpenseCategories]  WITH CHECK ADD  CONSTRAINT [FK_ExpenseCategories_Account] FOREIGN KEY([DefaultAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ExpenseCategories] CHECK CONSTRAINT [FK_ExpenseCategories_Account]
GO
ALTER TABLE [dbo].[ExpenseLines]  WITH CHECK ADD  CONSTRAINT [FK_ExpenseLines_Expense] FOREIGN KEY([ExpenseID])
REFERENCES [dbo].[Expenses] ([ExpenseID])
GO
ALTER TABLE [dbo].[ExpenseLines] CHECK CONSTRAINT [FK_ExpenseLines_Expense]
GO
ALTER TABLE [dbo].[ExpenseLines]  WITH CHECK ADD  CONSTRAINT [FK_ExpenseLines_TargetAccount] FOREIGN KEY([TargetAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ExpenseLines] CHECK CONSTRAINT [FK_ExpenseLines_TargetAccount]
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD  CONSTRAINT [FK_Expenses_CashAccount] FOREIGN KEY([CashAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Expenses] CHECK CONSTRAINT [FK_Expenses_CashAccount]
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD  CONSTRAINT [FK_Expenses_Category] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[ExpenseCategories] ([CategoryID])
GO
ALTER TABLE [dbo].[Expenses] CHECK CONSTRAINT [FK_Expenses_Category]
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD  CONSTRAINT [FK_Expenses_CounterAccount] FOREIGN KEY([CounterAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Expenses] CHECK CONSTRAINT [FK_Expenses_CounterAccount]
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD  CONSTRAINT [FK_Expenses_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Expenses] CHECK CONSTRAINT [FK_Expenses_CreatedBy]
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD  CONSTRAINT [FK_Expenses_Journal] FOREIGN KEY([JournalID])
REFERENCES [dbo].[Journals] ([JournalID])
GO
ALTER TABLE [dbo].[Expenses] CHECK CONSTRAINT [FK_Expenses_Journal]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Meter] FOREIGN KEY([MeterID])
REFERENCES [dbo].[Meters] ([MeterID])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Meter]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Reading] FOREIGN KEY([ReadingID])
REFERENCES [dbo].[Readings] ([ReadingID])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Reading]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK_Invoices_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK_Invoices_Subscriber]
GO
ALTER TABLE [dbo].[JournalEntries]  WITH CHECK ADD  CONSTRAINT [FK_JournalEntries_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[JournalEntries] CHECK CONSTRAINT [FK_JournalEntries_Account]
GO
ALTER TABLE [dbo].[JournalEntries]  WITH CHECK ADD  CONSTRAINT [FK_JournalEntries_Journal] FOREIGN KEY([JournalID])
REFERENCES [dbo].[Journals] ([JournalID])
GO
ALTER TABLE [dbo].[JournalEntries] CHECK CONSTRAINT [FK_JournalEntries_Journal]
GO
ALTER TABLE [dbo].[MainMeterReadings]  WITH CHECK ADD  CONSTRAINT [FK_MainMeterReadings_Meter] FOREIGN KEY([MeterID])
REFERENCES [dbo].[Meters] ([MeterID])
GO
ALTER TABLE [dbo].[MainMeterReadings] CHECK CONSTRAINT [FK_MainMeterReadings_Meter]
GO
ALTER TABLE [dbo].[MeterReports]  WITH CHECK ADD  CONSTRAINT [FK_MeterReports_MainMeter] FOREIGN KEY([MainMeterID])
REFERENCES [dbo].[Meters] ([MeterID])
GO
ALTER TABLE [dbo].[MeterReports] CHECK CONSTRAINT [FK_MeterReports_MainMeter]
GO
ALTER TABLE [dbo].[MobileReceiptImportLines]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImportLines_Import] FOREIGN KEY([ImportID])
REFERENCES [dbo].[MobileReceiptImports] ([ImportID])
GO
ALTER TABLE [dbo].[MobileReceiptImportLines] CHECK CONSTRAINT [FK_MobileReceiptImportLines_Import]
GO
ALTER TABLE [dbo].[MobileReceiptImportLines]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImportLines_Invoice] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[Invoices] ([InvoiceID])
GO
ALTER TABLE [dbo].[MobileReceiptImportLines] CHECK CONSTRAINT [FK_MobileReceiptImportLines_Invoice]
GO
ALTER TABLE [dbo].[MobileReceiptImports]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImports_ApprovedByUser] FOREIGN KEY([ApprovedByUserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[MobileReceiptImports] CHECK CONSTRAINT [FK_MobileReceiptImports_ApprovedByUser]
GO
ALTER TABLE [dbo].[MobileReceiptImports]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImports_ApprovedReceipt] FOREIGN KEY([ApprovedReceiptID])
REFERENCES [dbo].[Receipts] ([ReceiptID])
GO
ALTER TABLE [dbo].[MobileReceiptImports] CHECK CONSTRAINT [FK_MobileReceiptImports_ApprovedReceipt]
GO
ALTER TABLE [dbo].[MobileReceiptImports]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImports_Batch] FOREIGN KEY([SyncBatchID])
REFERENCES [dbo].[MobileSyncBatches] ([SyncBatchID])
GO
ALTER TABLE [dbo].[MobileReceiptImports] CHECK CONSTRAINT [FK_MobileReceiptImports_Batch]
GO
ALTER TABLE [dbo].[MobileReceiptImports]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImports_Collector] FOREIGN KEY([CollectorID])
REFERENCES [dbo].[Collectors] ([CollectorID])
GO
ALTER TABLE [dbo].[MobileReceiptImports] CHECK CONSTRAINT [FK_MobileReceiptImports_Collector]
GO
ALTER TABLE [dbo].[MobileReceiptImports]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImports_Device] FOREIGN KEY([DeviceID])
REFERENCES [dbo].[CollectorDevices] ([DeviceID])
GO
ALTER TABLE [dbo].[MobileReceiptImports] CHECK CONSTRAINT [FK_MobileReceiptImports_Device]
GO
ALTER TABLE [dbo].[MobileReceiptImports]  WITH CHECK ADD  CONSTRAINT [FK_MobileReceiptImports_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[MobileReceiptImports] CHECK CONSTRAINT [FK_MobileReceiptImports_Subscriber]
GO
ALTER TABLE [dbo].[MobileSyncBatches]  WITH CHECK ADD  CONSTRAINT [FK_MobileSyncBatches_Collector] FOREIGN KEY([CollectorID])
REFERENCES [dbo].[Collectors] ([CollectorID])
GO
ALTER TABLE [dbo].[MobileSyncBatches] CHECK CONSTRAINT [FK_MobileSyncBatches_Collector]
GO
ALTER TABLE [dbo].[MobileSyncBatches]  WITH CHECK ADD  CONSTRAINT [FK_MobileSyncBatches_Device] FOREIGN KEY([DeviceID])
REFERENCES [dbo].[CollectorDevices] ([DeviceID])
GO
ALTER TABLE [dbo].[MobileSyncBatches] CHECK CONSTRAINT [FK_MobileSyncBatches_Device]
GO
ALTER TABLE [dbo].[MobileSyncErrors]  WITH CHECK ADD  CONSTRAINT [FK_MobileSyncErrors_Batch] FOREIGN KEY([SyncBatchID])
REFERENCES [dbo].[MobileSyncBatches] ([SyncBatchID])
GO
ALTER TABLE [dbo].[MobileSyncErrors] CHECK CONSTRAINT [FK_MobileSyncErrors_Batch]
GO
ALTER TABLE [dbo].[MobileSyncErrors]  WITH CHECK ADD  CONSTRAINT [FK_MobileSyncErrors_Import] FOREIGN KEY([ImportID])
REFERENCES [dbo].[MobileReceiptImports] ([ImportID])
GO
ALTER TABLE [dbo].[MobileSyncErrors] CHECK CONSTRAINT [FK_MobileSyncErrors_Import]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Collector] FOREIGN KEY([CollectorID])
REFERENCES [dbo].[Collectors] ([CollectorID])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Collector]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Invoice] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[Invoices] ([InvoiceID])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Invoice]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Receipt] FOREIGN KEY([ReceiptID])
REFERENCES [dbo].[Receipts] ([ReceiptID])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Receipt]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Subscriber]
GO
ALTER TABLE [dbo].[Readings]  WITH CHECK ADD  CONSTRAINT [FK_Readings_Meter] FOREIGN KEY([MeterID])
REFERENCES [dbo].[Meters] ([MeterID])
GO
ALTER TABLE [dbo].[Readings] CHECK CONSTRAINT [FK_Readings_Meter]
GO
ALTER TABLE [dbo].[Readings]  WITH CHECK ADD  CONSTRAINT [FK_Readings_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[Readings] CHECK CONSTRAINT [FK_Readings_Subscriber]
GO
ALTER TABLE [dbo].[ReceiptApplications]  WITH CHECK ADD  CONSTRAINT [FK_ReceiptApplications_Invoice] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[Invoices] ([InvoiceID])
GO
ALTER TABLE [dbo].[ReceiptApplications] CHECK CONSTRAINT [FK_ReceiptApplications_Invoice]
GO
ALTER TABLE [dbo].[ReceiptApplications]  WITH CHECK ADD  CONSTRAINT [FK_ReceiptApplications_Payment] FOREIGN KEY([PaymentID])
REFERENCES [dbo].[Payments] ([PaymentID])
GO
ALTER TABLE [dbo].[ReceiptApplications] CHECK CONSTRAINT [FK_ReceiptApplications_Payment]
GO
ALTER TABLE [dbo].[ReceiptApplications]  WITH CHECK ADD  CONSTRAINT [FK_ReceiptApplications_Receipt] FOREIGN KEY([ReceiptID])
REFERENCES [dbo].[Receipts] ([ReceiptID])
GO
ALTER TABLE [dbo].[ReceiptApplications] CHECK CONSTRAINT [FK_ReceiptApplications_Receipt]
GO
ALTER TABLE [dbo].[Receipts]  WITH CHECK ADD  CONSTRAINT [FK_Receipts_Collector] FOREIGN KEY([CollectorID])
REFERENCES [dbo].[Collectors] ([CollectorID])
GO
ALTER TABLE [dbo].[Receipts] CHECK CONSTRAINT [FK_Receipts_Collector]
GO
ALTER TABLE [dbo].[Receipts]  WITH CHECK ADD  CONSTRAINT [FK_Receipts_Journal] FOREIGN KEY([JournalID])
REFERENCES [dbo].[Journals] ([JournalID])
GO
ALTER TABLE [dbo].[Receipts] CHECK CONSTRAINT [FK_Receipts_Journal]
GO
ALTER TABLE [dbo].[Receipts]  WITH CHECK ADD  CONSTRAINT [FK_Receipts_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[Receipts] CHECK CONSTRAINT [FK_Receipts_Subscriber]
GO
ALTER TABLE [dbo].[ReportTemplateColumns]  WITH CHECK ADD  CONSTRAINT [FK_ReportTemplateColumns_ReportTemplates] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[ReportTemplates] ([TemplateID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReportTemplateColumns] CHECK CONSTRAINT [FK_ReportTemplateColumns_ReportTemplates]
GO
ALTER TABLE [dbo].[ReportTemplateFilters]  WITH CHECK ADD  CONSTRAINT [FK_ReportTemplateFilters_ReportTemplates] FOREIGN KEY([TemplateID])
REFERENCES [dbo].[ReportTemplates] ([TemplateID])
GO
ALTER TABLE [dbo].[ReportTemplateFilters] CHECK CONSTRAINT [FK_ReportTemplateFilters_ReportTemplates]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Permission] FOREIGN KEY([PermissionID])
REFERENCES [dbo].[Permissions] ([PermissionID])
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Permission]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Role]
GO
ALTER TABLE [dbo].[SmsLogs]  WITH CHECK ADD  CONSTRAINT [FK_SmsLogs_Collectors] FOREIGN KEY([CollectorID])
REFERENCES [dbo].[Collectors] ([CollectorID])
GO
ALTER TABLE [dbo].[SmsLogs] CHECK CONSTRAINT [FK_SmsLogs_Collectors]
GO
ALTER TABLE [dbo].[SmsLogs]  WITH CHECK ADD  CONSTRAINT [FK_SmsLogs_Receipt] FOREIGN KEY([ReceiptID])
REFERENCES [dbo].[Receipts] ([ReceiptID])
GO
ALTER TABLE [dbo].[SmsLogs] CHECK CONSTRAINT [FK_SmsLogs_Receipt]
GO
ALTER TABLE [dbo].[SubscriberCredits]  WITH CHECK ADD  CONSTRAINT [FK_SubscriberCredits_Meter] FOREIGN KEY([MeterID])
REFERENCES [dbo].[Meters] ([MeterID])
GO
ALTER TABLE [dbo].[SubscriberCredits] CHECK CONSTRAINT [FK_SubscriberCredits_Meter]
GO
ALTER TABLE [dbo].[SubscriberCredits]  WITH CHECK ADD  CONSTRAINT [FK_SubscriberCredits_Payment] FOREIGN KEY([PaymentID])
REFERENCES [dbo].[Payments] ([PaymentID])
GO
ALTER TABLE [dbo].[SubscriberCredits] CHECK CONSTRAINT [FK_SubscriberCredits_Payment]
GO
ALTER TABLE [dbo].[SubscriberCredits]  WITH CHECK ADD  CONSTRAINT [FK_SubscriberCredits_Receipt] FOREIGN KEY([ReceiptID])
REFERENCES [dbo].[Receipts] ([ReceiptID])
GO
ALTER TABLE [dbo].[SubscriberCredits] CHECK CONSTRAINT [FK_SubscriberCredits_Receipt]
GO
ALTER TABLE [dbo].[SubscriberCredits]  WITH CHECK ADD  CONSTRAINT [FK_SubscriberCredits_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[SubscriberCredits] CHECK CONSTRAINT [FK_SubscriberCredits_Subscriber]
GO
ALTER TABLE [dbo].[SubscriberMeters]  WITH CHECK ADD  CONSTRAINT [FK_SubMeter_Meter] FOREIGN KEY([MeterID])
REFERENCES [dbo].[Meters] ([MeterID])
GO
ALTER TABLE [dbo].[SubscriberMeters] CHECK CONSTRAINT [FK_SubMeter_Meter]
GO
ALTER TABLE [dbo].[SubscriberMeters]  WITH CHECK ADD  CONSTRAINT [FK_SubMeter_Subscriber] FOREIGN KEY([SubscriberID])
REFERENCES [dbo].[Subscribers] ([SubscriberID])
GO
ALTER TABLE [dbo].[SubscriberMeters] CHECK CONSTRAINT [FK_SubMeter_Subscriber]
GO
ALTER TABLE [dbo].[Subscribers]  WITH CHECK ADD  CONSTRAINT [FK_Subscriber_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Subscribers] CHECK CONSTRAINT [FK_Subscriber_Account]
GO
ALTER TABLE [dbo].[Subscribers]  WITH CHECK ADD  CONSTRAINT [FK_Subscribers_TariffPlan] FOREIGN KEY([TariffPlanID])
REFERENCES [dbo].[TariffPlans] ([TariffPlanID])
GO
ALTER TABLE [dbo].[Subscribers] CHECK CONSTRAINT [FK_Subscribers_TariffPlan]
GO
ALTER TABLE [dbo].[SystemAccounts]  WITH CHECK ADD FOREIGN KEY([CashAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[SystemAccounts]  WITH CHECK ADD FOREIGN KEY([RevenueAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[SystemAccounts]  WITH CHECK ADD FOREIGN KEY([ServiceRevenueAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[TariffRates]  WITH CHECK ADD  CONSTRAINT [FK_TariffRates_Plans] FOREIGN KEY([TariffPlanID])
REFERENCES [dbo].[TariffPlans] ([TariffPlanID])
GO
ALTER TABLE [dbo].[TariffRates] CHECK CONSTRAINT [FK_TariffRates_Plans]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Role]
GO
ALTER TABLE [dbo].[ExpenseCategories]  WITH CHECK ADD  CONSTRAINT [CK_ExpenseCategories_Type] CHECK  (([CategoryType]=N'Loss' OR [CategoryType]=N'Purchase' OR [CategoryType]=N'Expense'))
GO
ALTER TABLE [dbo].[ExpenseCategories] CHECK CONSTRAINT [CK_ExpenseCategories_Type]
GO
ALTER TABLE [dbo].[ExpenseLines]  WITH CHECK ADD  CONSTRAINT [CK_ExpenseLines_Qty] CHECK  (([Qty]>(0)))
GO
ALTER TABLE [dbo].[ExpenseLines] CHECK CONSTRAINT [CK_ExpenseLines_Qty]
GO
ALTER TABLE [dbo].[ExpenseLines]  WITH CHECK ADD  CONSTRAINT [CK_ExpenseLines_UnitPrice] CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[ExpenseLines] CHECK CONSTRAINT [CK_ExpenseLines_UnitPrice]
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD  CONSTRAINT [CK_Expenses_Amount_Positive] CHECK  (([TotalAmount]>(0)))
GO
ALTER TABLE [dbo].[Expenses] CHECK CONSTRAINT [CK_Expenses_Amount_Positive]
GO
ALTER TABLE [dbo].[Expenses]  WITH CHECK ADD  CONSTRAINT [CK_Expenses_Method] CHECK  (([PaymentMethod]=N'Credit' OR [PaymentMethod]=N'Cheque' OR [PaymentMethod]=N'Transfer' OR [PaymentMethod]=N'Cash'))
GO
ALTER TABLE [dbo].[Expenses] CHECK CONSTRAINT [CK_Expenses_Method]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [CK_Invoices_ServiceFees] CHECK  (([ServiceFees]>=(0)))
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [CK_Invoices_ServiceFees]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [CK_Invoices_Status_Allowed] CHECK  (([Status]=N'ملغاة' OR [Status]=N'مدفوعة' OR [Status]=N'مدفوعة جزئياً' OR [Status]=N'غير مدفوعة'))
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [CK_Invoices_Status_Allowed]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [CK_Invoices_UnitPrice] CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [CK_Invoices_UnitPrice]
GO
ALTER TABLE [dbo].[JournalEntries]  WITH CHECK ADD  CONSTRAINT [CK_JournalEntries_Amount] CHECK  (([Debit]>=(0) AND [Credit]>=(0)))
GO
ALTER TABLE [dbo].[JournalEntries] CHECK CONSTRAINT [CK_JournalEntries_Amount]
GO
ALTER TABLE [dbo].[MainMeterReadings]  WITH CHECK ADD  CONSTRAINT [CK_MainMeterReadings_Order] CHECK  (([CurrentReading]>=[PreviousReading]))
GO
ALTER TABLE [dbo].[MainMeterReadings] CHECK CONSTRAINT [CK_MainMeterReadings_Order]
GO
ALTER TABLE [dbo].[MainMeterReadings]  WITH CHECK ADD  CONSTRAINT [CK_MainMeterReadings_Positive] CHECK  (([PreviousReading]>=(0) AND [CurrentReading]>=(0) AND [Consumption]>=(0)))
GO
ALTER TABLE [dbo].[MainMeterReadings] CHECK CONSTRAINT [CK_MainMeterReadings_Positive]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [CK_Payments_Amount_Positive] CHECK  (([Amount]>(0)))
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [CK_Payments_Amount_Positive]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [CK_Payments_Category_Allowed] CHECK  (([PaymentCategory]=N'CreditSettlement' OR [PaymentCategory]=N'AdvanceCredit' OR [PaymentCategory]=N'NormalPayment'))
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [CK_Payments_Category_Allowed]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [CK_Payments_Type_Allowed] CHECK  (([PaymentType]=N'Other' OR [PaymentType]=N'Cheque' OR [PaymentType]=N'Transfer' OR [PaymentType]=N'Cash'))
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [CK_Payments_Type_Allowed]
GO
ALTER TABLE [dbo].[ReceiptApplications]  WITH CHECK ADD  CONSTRAINT [CK_ReceiptApplications_Amount_Positive] CHECK  (([AppliedAmount]>(0)))
GO
ALTER TABLE [dbo].[ReceiptApplications] CHECK CONSTRAINT [CK_ReceiptApplications_Amount_Positive]
GO
ALTER TABLE [dbo].[ReceiptApplications]  WITH CHECK ADD  CONSTRAINT [CK_ReceiptApplications_Type_Allowed] CHECK  (([ApplicationType]=N'AdvanceCredit' OR [ApplicationType]=N'CreditSettlement' OR [ApplicationType]=N'InvoicePayment'))
GO
ALTER TABLE [dbo].[ReceiptApplications] CHECK CONSTRAINT [CK_ReceiptApplications_Type_Allowed]
GO
ALTER TABLE [dbo].[Receipts]  WITH CHECK ADD  CONSTRAINT [CK_Receipts_Amount_Positive] CHECK  (([TotalAmount]>(0)))
GO
ALTER TABLE [dbo].[Receipts] CHECK CONSTRAINT [CK_Receipts_Amount_Positive]
GO
ALTER TABLE [dbo].[Receipts]  WITH CHECK ADD  CONSTRAINT [CK_Receipts_Method_Allowed] CHECK  (([PaymentMethod]=N'Other' OR [PaymentMethod]=N'Cheque' OR [PaymentMethod]=N'Transfer' OR [PaymentMethod]=N'Cash'))
GO
ALTER TABLE [dbo].[Receipts] CHECK CONSTRAINT [CK_Receipts_Method_Allowed]
GO
ALTER TABLE [dbo].[SmsLogs]  WITH CHECK ADD  CONSTRAINT [CK_SmsLogs_Status] CHECK  (([Status]='Failed' OR [Status]='Sent' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[SmsLogs] CHECK CONSTRAINT [CK_SmsLogs_Status]
GO
ALTER TABLE [dbo].[SubscriberCredits]  WITH CHECK ADD  CONSTRAINT [CK_SubscriberCredits_Amounts] CHECK  (([AmountTotal]>=(0) AND [AmountRemaining]>=(0) AND [AmountRemaining]<=[AmountTotal]))
GO
ALTER TABLE [dbo].[SubscriberCredits] CHECK CONSTRAINT [CK_SubscriberCredits_Amounts]
GO
ALTER TABLE [dbo].[SystemAccounts2]  WITH CHECK ADD  CONSTRAINT [CK_SystemAccounts2_OnlyOneRow] CHECK  (([ID]=(1)))
GO
ALTER TABLE [dbo].[SystemAccounts2] CHECK CONSTRAINT [CK_SystemAccounts2_OnlyOneRow]
GO
/****** Object:  StoredProcedure [dbo].[AddManualJournal]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   0) Helper: قراءة حسابات النظام (SystemAccounts) بأمان
   ========================================================= */
-- لا يوجد إجراء مستقل هنا، سنقرأ TOP 1 داخل كل إجراء.

/* =========================================================
   1) AddManualJournal
   ========================================================= */
CREATE   PROCEDURE [dbo].[AddManualJournal]
    @JournalDate DATE,
    @Description NVARCHAR(200),
    @DebitAccount INT,
    @CreditAccount INT,
    @Amount DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;

    IF dbo.fn_IsPeriodClosed(@JournalDate) = 1
        THROW 50010, N'الفترة المحاسبية مغلقة', 1;

    IF @Amount <= 0
        THROW 50001, N'المبلغ غير صحيح', 1;

    BEGIN TRY
        BEGIN TRAN;

        INSERT INTO Journals (JournalDate, Description, Source, SourceID, IsPosted, CreatedAt)
        VALUES (@JournalDate, @Description, 'Manual', NULL, 0, GETDATE());

        DECLARE @JID INT = SCOPE_IDENTITY();

        INSERT INTO JournalEntries (JournalID, AccountID, Debit, Credit) VALUES
        (@JID, @DebitAccount, @Amount, 0),
        (@JID, @CreditAccount, 0, @Amount);

        -- تحقق التوازن
        IF ROUND((SELECT SUM(Debit) - SUM(Credit) FROM JournalEntries WHERE JournalID = @JID),2) <> 0
            THROW 50002, N'القيد غير متوازن', 1;

        UPDATE Journals SET IsPosted = 1, PostedAt = GETDATE()
        WHERE JournalID = @JID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[AddMeterToSubscriber]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[AddMeterToSubscriber]
    @SubscriberID INT,
    @MeterNumber NVARCHAR(50),
    @Location NVARCHAR(150) = NULL,
    @IsPrimary BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MeterID INT;

    -- إن كان العداد موجود
    SELECT @MeterID = MeterID FROM dbo.Meters WHERE MeterNumber = @MeterNumber;

    IF @MeterID IS NULL
    BEGIN
        INSERT INTO dbo.Meters (MeterNumber, MeterType, Location, IsActive, CreatedAt)
        VALUES (@MeterNumber, N'Sub', @Location, 1, GETDATE());

        SET @MeterID = SCOPE_IDENTITY();
    END

    -- منع ربط العداد لمشترك آخر
    IF EXISTS (SELECT 1 FROM dbo.SubscriberMeters WHERE MeterID = @MeterID AND SubscriberID <> @SubscriberID)
        THROW 50020, N'هذا العداد مرتبط بمشترك آخر', 1;

    -- ربط إذا غير موجود
    IF NOT EXISTS (SELECT 1 FROM dbo.SubscriberMeters WHERE SubscriberID=@SubscriberID AND MeterID=@MeterID)
    BEGIN
        INSERT INTO dbo.SubscriberMeters (SubscriberID, MeterID, IsPrimary, LinkedAt)
        VALUES (@SubscriberID, @MeterID, @IsPrimary, GETDATE());
    END

    -- إذا أساسي: اجعل البقية غير أساسي
    IF @IsPrimary = 1
    BEGIN
        UPDATE dbo.SubscriberMeters
        SET IsPrimary = CASE WHEN MeterID = @MeterID THEN 1 ELSE 0 END
        WHERE SubscriberID = @SubscriberID;
    END

    SELECT @MeterID AS MeterID;
END
GO
/****** Object:  StoredProcedure [dbo].[AddReadingAndGenerateInvoice]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddReadingAndGenerateInvoice]
    @SubscriberID INT,
    @MeterID INT,
    @ReadingDate DATE,
    @CurrentReading DECIMAL(18,2),
    @UnitPrice DECIMAL(18,2) = NULL,
    @ServiceFees DECIMAL(18,2) = NULL,
    @Notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @PreviousReading DECIMAL(18,2) = 0,
        @LastReadingDate DATE = NULL,
        @Consumption DECIMAL(18,2),
        @ConsumptionAmount DECIMAL(18,2),
        @ServiceFeesAmount DECIMAL(18,2),
        @TotalAmount DECIMAL(18,2),
        @Arrears DECIMAL(18,2) = 0,
        @GrandTotal DECIMAL(18,2) = 0,

        @InvoiceID INT = NULL,
        @ReadingID INT = NULL,
        @JournalID INT = NULL,
        @SubscriberAccountID INT = NULL,
        @RevenueAccountID INT = NULL,
        @ServiceRevenueAccountID INT = NULL,
        @InvoiceNumber NVARCHAR(30) = NULL,

        @DefaultUnitPrice DECIMAL(18,2) = 0,
        @DefaultServiceFees DECIMAL(18,2) = 0,
        @TariffPlanID INT = NULL,
        @PricingModel NVARCHAR(20) = NULL,
        @FixedUnitPrice DECIMAL(18,2) = NULL,
        @PlanServiceFees DECIMAL(18,2) = NULL,

        @Need DECIMAL(18,2) = 0,
        @CreditID INT = NULL,
        @CreditAvail DECIMAL(18,2) = 0,
        @UseAmount DECIMAL(18,2) = 0,
        @PaidNow DECIMAL(18,2) = 0,

        @ExpectedDebit DECIMAL(18,2),
        @ExpectedCredit DECIMAL(18,2),
        @BalanceDiff DECIMAL(18,2),

        @OldInvoiceID INT = NULL,
        @OldInvoiceMeterID INT = NULL,
        @OldInvoiceNeed DECIMAL(18,2) = 0,
        @OldInvoiceGrandTotal DECIMAL(18,2) = 0,
        @OldInvoicePaid DECIMAL(18,2) = 0,
        @OldInvoiceStatus NVARCHAR(50) = NULL,

        @LatestPrevInvoiceID INT = NULL,
        @LatestPrevGrandTotal DECIMAL(18,2) = 0,
        @LatestPrevPaid DECIMAL(18,2) = 0,

        @CreditPaymentID INT = NULL;

    /* -----------------------------------------
       0) تحققات أولية
       ----------------------------------------- */
    IF dbo.fn_IsPeriodClosed(@ReadingDate) = 1
        THROW 50010, N'الفترة المحاسبية مغلقة', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.SubscriberMeters
        WHERE SubscriberID = @SubscriberID
          AND MeterID = @MeterID
    )
        THROW 50021, N'العداد غير مرتبط بهذا المشترك', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Readings
        WHERE SubscriberID = @SubscriberID
          AND MeterID = @MeterID
          AND ReadingDate = @ReadingDate
    )
        THROW 50022, N'توجد قراءة لهذا العداد في نفس التاريخ', 1;

    /* -----------------------------------------
       1) آخر قراءة مسجلة
       ----------------------------------------- */
    SELECT TOP (1)
        @LastReadingDate = ReadingDate,
        @PreviousReading = CurrentReading
    FROM dbo.Readings
    WHERE SubscriberID = @SubscriberID
      AND MeterID = @MeterID
    ORDER BY ReadingDate DESC, ReadingID DESC;

    SET @PreviousReading = ISNULL(@PreviousReading, 0);

    IF @LastReadingDate IS NOT NULL AND @ReadingDate <= @LastReadingDate
        THROW 50024, N'تاريخ القراءة يجب أن يكون بعد آخر قراءة مسجلة', 1;

    IF @CurrentReading < @PreviousReading
        THROW 50013, N'القراءة الحالية أقل من القراءة السابقة', 1;

    SET @Consumption = ROUND(@CurrentReading - @PreviousReading, 2);

    /* -----------------------------------------
       2) تحميل التعرفة
       ----------------------------------------- */
    SELECT TOP (1)
        @DefaultUnitPrice = ISNULL(UnitPrice, 0),
        @DefaultServiceFees = ISNULL(ServiceFees, 0)
    FROM dbo.BillingConstants
    WHERE IsActive = 1
      AND EffectiveFrom <= @ReadingDate
    ORDER BY EffectiveFrom DESC, ConstantID DESC;

    SELECT @TariffPlanID = TariffPlanID
    FROM dbo.Subscribers
    WHERE SubscriberID = @SubscriberID;

    IF @TariffPlanID IS NOT NULL
    BEGIN
        SELECT
            @PricingModel = PricingModel,
            @FixedUnitPrice = FixedUnitPrice,
            @PlanServiceFees = DefaultServiceFees
        FROM dbo.TariffPlans
        WHERE TariffPlanID = @TariffPlanID
          AND IsActive = 1;
    END

    IF @ServiceFees IS NULL
        SET @ServiceFees = COALESCE(@PlanServiceFees, @DefaultServiceFees, 0);

    IF @UnitPrice IS NULL
    BEGIN
        IF @TariffPlanID IS NULL OR ISNULL(@PricingModel, N'Fixed') = N'Fixed'
        BEGIN
            SET @UnitPrice =
                CASE
                    WHEN ISNULL(@FixedUnitPrice, 0) > 0 THEN @FixedUnitPrice
                    ELSE @DefaultUnitPrice
                END;
        END
        ELSE
        BEGIN
            THROW 50023, N'الخطة الشرائحية تتطلب تمرير سعر الوحدة المحسوب من التطبيق أو إجراء مخصص', 1;
        END
    END

    IF @UnitPrice IS NULL OR @UnitPrice < 0
        THROW 50011, N'سعر الوحدة غير صحيح', 1;

    IF @ServiceFees < 0
        THROW 50012, N'رسوم الخدمة غير صحيحة', 1;

    SET @ConsumptionAmount = ROUND(@Consumption * @UnitPrice, 2);
    SET @ServiceFeesAmount = ROUND(ISNULL(@ServiceFees, 0), 2);
    SET @TotalAmount = ROUND(@ConsumptionAmount + @ServiceFeesAmount, 2);

    BEGIN TRY
        BEGIN TRAN;

        /* -----------------------------------------
           3) حساب المشترك المحاسبي
           ----------------------------------------- */
        SELECT @SubscriberAccountID = AccountID
        FROM dbo.Subscribers
        WHERE SubscriberID = @SubscriberID;

        IF @SubscriberAccountID IS NULL
            THROW 50001, N'المشترك لا يمتلك حسابًا محاسبيًا', 1;

        /* -----------------------------------------
           4) حسابات الإيراد
           ----------------------------------------- */
        SELECT TOP (1)
            @RevenueAccountID = WaterRevenueAccountID,
            @ServiceRevenueAccountID = ServiceRevenueAccountID
        FROM dbo.vw_SystemAccountsUnified;

        IF @RevenueAccountID IS NULL
        BEGIN
            SELECT @RevenueAccountID = AccountID
            FROM dbo.Accounts
            WHERE AccountCode = N'4100';

            IF @RevenueAccountID IS NULL
            BEGIN
                INSERT INTO dbo.Accounts
                (
                    AccountCode, AccountName, AccountType, IsControl, ParentAccountID
                )
                VALUES
                (
                    N'4100', N'إيرادات المياه', N'Revenue', 0, NULL
                );

                SET @RevenueAccountID = SCOPE_IDENTITY();
            END
        END

        IF @ServiceFeesAmount > 0 AND @ServiceRevenueAccountID IS NULL
        BEGIN
            SELECT @ServiceRevenueAccountID = AccountID
            FROM dbo.Accounts
            WHERE AccountCode = N'4110';

            IF @ServiceRevenueAccountID IS NULL
            BEGIN
                INSERT INTO dbo.Accounts
                (
                    AccountCode, AccountName, AccountType, IsControl, ParentAccountID
                )
                VALUES
                (
                    N'4110', N'إيرادات رسوم الخدمة', N'Revenue', 0, NULL
                );

                SET @ServiceRevenueAccountID = SCOPE_IDENTITY();
            END
        END

        /* -----------------------------------------
           5) تسوية الرصيد المقدم القديم على الفواتير السابقة أولًا
           حتى لا تنتقل متأخرات غير صحيحة إلى الفاتورة الجديدة
           ----------------------------------------- */
        WHILE 1 = 1
        BEGIN
            SET @OldInvoiceID = NULL;
            SET @OldInvoiceMeterID = NULL;
            SET @OldInvoiceNeed = 0;
            SET @CreditID = NULL;
            SET @CreditAvail = 0;
            SET @UseAmount = 0;

            SELECT TOP (1)
                @OldInvoiceID = I.InvoiceID,
                @OldInvoiceMeterID = I.MeterID,
                @OldInvoiceNeed =
                    CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                    - CAST(ISNULL(PA.Paid,0) AS DECIMAL(18,2))
            FROM dbo.Invoices I WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            OUTER APPLY
            (
                SELECT SUM(P.Amount) AS Paid
                FROM dbo.Payments P
                WHERE P.InvoiceID = I.InvoiceID
            ) PA
            WHERE I.SubscriberID = @SubscriberID
              AND I.InvoiceDate < @ReadingDate
              AND ISNULL(I.Status, N'') <> N'ملغاة'
              AND CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                    > CAST(ISNULL(PA.Paid,0) AS DECIMAL(18,2))
            ORDER BY I.InvoiceDate, I.InvoiceID;

            IF @OldInvoiceID IS NULL
                BREAK;

            SELECT TOP (1)
                @CreditID = C.CreditID,
                @CreditAvail = CAST(C.AmountRemaining AS DECIMAL(18,2))
            FROM dbo.SubscriberCredits C WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            WHERE C.SubscriberID = @SubscriberID
              AND C.AmountRemaining > 0
              AND C.CreditDate <= @ReadingDate
              AND (C.MeterID = @OldInvoiceMeterID OR C.MeterID IS NULL)
            ORDER BY
                CASE WHEN C.MeterID = @OldInvoiceMeterID THEN 0 ELSE 1 END,
                C.CreditDate,
                C.CreditID;

            IF @CreditID IS NULL OR @CreditAvail <= 0
                BREAK;

            SET @UseAmount =
                CASE
                    WHEN @CreditAvail >= @OldInvoiceNeed THEN @OldInvoiceNeed
                    ELSE @CreditAvail
                END;

            UPDATE dbo.SubscriberCredits
            SET AmountRemaining = CAST(AmountRemaining - @UseAmount AS DECIMAL(18,2))
            WHERE CreditID = @CreditID;

            INSERT INTO dbo.Payments
            (
                InvoiceID, SubscriberID, CollectorID, PaymentDate, Amount,
                PaymentType, PaymentCategory, Notes, ReceiptNumber, ReceiptID
            )
            VALUES
            (
                @OldInvoiceID, @SubscriberID, NULL, @ReadingDate, @UseAmount,
                N'Other', N'CreditSettlement',
                N'تسوية من رصيد مقدم قبل إصدار فاتورة جديدة',
                NULL, NULL
            );

            SET @CreditPaymentID = SCOPE_IDENTITY();

            INSERT INTO dbo.AccountStatements
            (
                SubscriberID, InvoiceID, PaymentID, MeterID, [Date], Details,
                DocumentType, DocumentNumber, Debit, Credit, JournalID
            )
            VALUES
            (
                @SubscriberID, @OldInvoiceID, @CreditPaymentID, @OldInvoiceMeterID, @ReadingDate,
                N'تسوية من رصيد مقدم',
                N'CreditSettlement', CAST(@CreditID AS NVARCHAR(50)),
                0, @UseAmount, NULL
            );

            SELECT
                @OldInvoiceGrandTotal = CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
            FROM dbo.Invoices I
            WHERE I.InvoiceID = @OldInvoiceID;

            SELECT
                @OldInvoicePaid = CAST(ISNULL(SUM(P.Amount),0) AS DECIMAL(18,2))
            FROM dbo.Payments P
            WHERE P.InvoiceID = @OldInvoiceID;

            SET @OldInvoiceStatus =
                CASE
                    WHEN @OldInvoicePaid >= @OldInvoiceGrandTotal THEN N'مدفوعة'
                    WHEN @OldInvoicePaid > 0 THEN N'مدفوعة جزئياً'
                    ELSE N'غير مدفوعة'
                END;

            UPDATE dbo.Invoices
            SET Status = @OldInvoiceStatus
            WHERE InvoiceID = @OldInvoiceID;
        END

        /* -----------------------------------------
           6) حساب المتأخرات من آخر فاتورة سابقة فقط
           لأنها تحمل المتبقي المرحّل كله
           ----------------------------------------- */
        SELECT TOP (1)
            @LatestPrevInvoiceID = I.InvoiceID,
            @LatestPrevGrandTotal = CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
        FROM dbo.Invoices I
        WHERE I.SubscriberID = @SubscriberID
          AND I.InvoiceDate < @ReadingDate
          AND ISNULL(I.Status, N'') <> N'ملغاة'
        ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC;

        IF @LatestPrevInvoiceID IS NOT NULL
        BEGIN
            SELECT
                @LatestPrevPaid = CAST(ISNULL(SUM(P.Amount),0) AS DECIMAL(18,2))
            FROM dbo.Payments P
            WHERE P.InvoiceID = @LatestPrevInvoiceID;

            SET @Arrears = ROUND(@LatestPrevGrandTotal - ISNULL(@LatestPrevPaid,0), 2);
            IF @Arrears < 0 SET @Arrears = 0;
        END
        ELSE
        BEGIN
            SET @Arrears = 0;
        END

        /* -----------------------------------------
           7) حفظ القراءة
           ----------------------------------------- */
        INSERT INTO dbo.Readings
        (
            SubscriberID, MeterID, ReadingDate,
            PreviousReading, CurrentReading, Consumption, Notes
        )
        VALUES
        (
            @SubscriberID, @MeterID, @ReadingDate,
            @PreviousReading, @CurrentReading, @Consumption, @Notes
        );

        SET @ReadingID = SCOPE_IDENTITY();

        /* -----------------------------------------
           8) توليد رقم الفاتورة
           ----------------------------------------- */
        EXEC dbo.GetNextDocumentNumber
            @DocType = N'Invoice',
            @DocDate = @ReadingDate,
            @NewNumber = @InvoiceNumber OUTPUT;

        /* -----------------------------------------
           9) حفظ الفاتورة
           ----------------------------------------- */
        INSERT INTO dbo.Invoices
        (
            SubscriberID, InvoiceDate, Consumption, UnitPrice,
            ServiceFees, Arrears, TotalAmount, Status, Notes,
            ReadingID, InvoiceNumber, MeterID
        )
        VALUES
        (
            @SubscriberID, @ReadingDate, @Consumption, @UnitPrice,
            @ServiceFeesAmount, @Arrears, @TotalAmount, N'غير مدفوعة',
            @Notes, @ReadingID, @InvoiceNumber, @MeterID
        );

        SET @InvoiceID = SCOPE_IDENTITY();
        SET @GrandTotal = ROUND(@TotalAmount + @Arrears, 2);

        /* -----------------------------------------
           10) القيد المحاسبي
           القيد على قيمة الاستهلاك الجديد فقط
           وليس على المتأخرات المرحّلة
           ----------------------------------------- */
        SET @ExpectedDebit = ROUND(@TotalAmount, 2);
        SET @ExpectedCredit = ROUND(@ConsumptionAmount + ISNULL(@ServiceFeesAmount, 0), 2);

        IF @SubscriberAccountID IS NULL
            THROW 50031, N'حساب المشترك غير موجود', 1;

        IF @RevenueAccountID IS NULL
            THROW 50032, N'حساب إيراد المياه غير موجود', 1;

        IF @ServiceFeesAmount > 0 AND @ServiceRevenueAccountID IS NULL
            THROW 50033, N'حساب إيراد رسوم الخدمة غير موجود', 1;

        IF ROUND(@ExpectedDebit - @ExpectedCredit, 2) <> 0
            THROW 50034, N'القيم المحاسبية غير متوازنة قبل إدراج القيد', 1;

        INSERT INTO dbo.Journals
        (
            JournalDate, Description, Source, SourceID, IsPosted, CreatedAt
        )
        VALUES
        (
            @ReadingDate,
            N'فاتورة مياه رقم ' + @InvoiceNumber,
            N'Invoice',
            @InvoiceID,
            0,
            GETDATE()
        );

        SET @JournalID = SCOPE_IDENTITY();

        IF @ServiceFeesAmount > 0
        BEGIN
            INSERT INTO dbo.JournalEntries (JournalID, AccountID, Debit, Credit)
            SELECT @JournalID, @SubscriberAccountID, @ExpectedDebit, 0
            UNION ALL
            SELECT @JournalID, @RevenueAccountID, 0, ROUND(@ConsumptionAmount, 2)
            UNION ALL
            SELECT @JournalID, @ServiceRevenueAccountID, 0, ROUND(@ServiceFeesAmount, 2);
        END
        ELSE
        BEGIN
            INSERT INTO dbo.JournalEntries (JournalID, AccountID, Debit, Credit)
            SELECT @JournalID, @SubscriberAccountID, @ExpectedDebit, 0
            UNION ALL
            SELECT @JournalID, @RevenueAccountID, 0, ROUND(@ConsumptionAmount, 2);
        END

        SELECT
            @BalanceDiff = ROUND(SUM(ISNULL(Debit, 0)) - SUM(ISNULL(Credit, 0)), 2)
        FROM dbo.JournalEntries
        WHERE JournalID = @JournalID;

        IF ISNULL(@BalanceDiff, 0) <> 0
            THROW 50002, N'القيد غير متوازن بعد الإدراج', 1;

        UPDATE dbo.Journals
        SET IsPosted = 1, PostedAt = GETDATE()
        WHERE JournalID = @JournalID;

        /* -----------------------------------------
           11) كشف الحساب
           يسجل فقط قيمة الفترة الجديدة
           ----------------------------------------- */
        INSERT INTO dbo.AccountStatements
        (
            SubscriberID, InvoiceID, MeterID, [Date], Details,
            DocumentType, DocumentNumber, Debit, Credit, JournalID
        )
        VALUES
        (
            @SubscriberID, @InvoiceID, @MeterID, @ReadingDate,
            N'فاتورة مياه',
            N'Invoice', @InvoiceNumber,
            @TotalAmount, 0, @JournalID
        );

        /* -----------------------------------------
           12) تطبيق الرصيد المقدم المتبقي على الفاتورة الجديدة
           هنا الحاجة = إجمالي الفاتورة + المتأخرات
           ----------------------------------------- */
        SET @Need = CAST(@GrandTotal AS DECIMAL(18,2));

        WHILE @Need > 0
        BEGIN
            SET @CreditID = NULL;
            SET @CreditAvail = NULL;
            SET @UseAmount = NULL;

            SELECT TOP (1)
                @CreditID = C.CreditID,
                @CreditAvail = CAST(C.AmountRemaining AS DECIMAL(18,2))
            FROM dbo.SubscriberCredits C WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            WHERE C.SubscriberID = @SubscriberID
              AND C.AmountRemaining > 0
              AND C.CreditDate <= @ReadingDate
              AND (C.MeterID = @MeterID OR C.MeterID IS NULL)
            ORDER BY
                CASE WHEN C.MeterID = @MeterID THEN 0 ELSE 1 END,
                C.CreditDate,
                C.CreditID;

            IF @CreditID IS NULL OR @CreditAvail <= 0
                BREAK;

            SET @UseAmount =
                CASE
                    WHEN @CreditAvail >= @Need THEN @Need
                    ELSE @CreditAvail
                END;

            UPDATE dbo.SubscriberCredits
            SET AmountRemaining = CAST(AmountRemaining - @UseAmount AS DECIMAL(18,2))
            WHERE CreditID = @CreditID;

            INSERT INTO dbo.Payments
            (
                InvoiceID, SubscriberID, CollectorID, PaymentDate, Amount,
                PaymentType, PaymentCategory, Notes, ReceiptNumber, ReceiptID
            )
            VALUES
            (
                @InvoiceID, @SubscriberID, NULL, @ReadingDate, @UseAmount,
                N'Other', N'CreditSettlement', N'تسوية من رصيد مقدم',
                NULL, NULL
            );

            SET @CreditPaymentID = SCOPE_IDENTITY();

            INSERT INTO dbo.AccountStatements
            (
                SubscriberID, InvoiceID, PaymentID, MeterID, [Date], Details,
                DocumentType, DocumentNumber, Debit, Credit, JournalID
            )
            VALUES
            (
                @SubscriberID, @InvoiceID, @CreditPaymentID, @MeterID, @ReadingDate,
                N'تسوية من رصيد مقدم',
                N'CreditSettlement', CAST(@CreditID AS NVARCHAR(50)),
                0, @UseAmount, NULL
            );

            SET @Need = CAST(@Need - @UseAmount AS DECIMAL(18,2));
        END

        /* -----------------------------------------
           13) تحديث حالة الفاتورة الجديدة
           الحالة على أساس GrandTotal
           ----------------------------------------- */
        SELECT @PaidNow = CAST(ISNULL(SUM(Amount), 0) AS DECIMAL(18,2))
        FROM dbo.Payments
        WHERE InvoiceID = @InvoiceID;

        UPDATE dbo.Invoices
        SET Status =
            CASE
                WHEN @PaidNow >= @GrandTotal THEN N'مدفوعة'
                WHEN @PaidNow > 0 THEN N'مدفوعة جزئياً'
                ELSE N'غير مدفوعة'
            END
        WHERE InvoiceID = @InvoiceID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH;

    /* -----------------------------------------
       14) إرسال الرسالة بعد نجاح الحفظ
       ----------------------------------------- */
    BEGIN TRY
        EXEC dbo.SendSMSDynamic
            @EntityType = N'Invoice',
            @EntityID = @InvoiceID,
            @CustomMessage = NULL,
            @Language = N'AR';
    END TRY
    BEGIN CATCH
        -- لا نفشل العملية بسبب الرسالة
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[ApplyReceiptAsAdvanceCredit]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =========================================================
   G) ApplyReceiptAsAdvanceCredit - نسخة مصححة
   ========================================================= */
CREATE   PROCEDURE [dbo].[ApplyReceiptAsAdvanceCredit]
    @ReceiptID INT,
    @SubscriberID INT,
    @MeterID INT = NULL,
    @AppliedAmount DECIMAL(12,2),
    @PaymentID INT = NULL,
    @Notes NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @AppliedAmount IS NULL OR @AppliedAmount <= 0
        THROW 50001, N'المبلغ غير صحيح', 1;

    DECLARE
        @ReceiptTotal DECIMAL(12,2),
        @ReceiptSubscriberID INT,
        @AlreadyApplied DECIMAL(12,2),
        @RemainingReceipt DECIMAL(12,2),
        @ReceiptDate DATE;

    SELECT
        @ReceiptTotal = CAST(TotalAmount AS DECIMAL(12,2)),
        @ReceiptSubscriberID = SubscriberID,
        @ReceiptDate = PaymentDate
    FROM dbo.Receipts
    WHERE ReceiptID = @ReceiptID;

    IF @ReceiptSubscriberID IS NULL
        THROW 50002, N'الإيصال غير موجود', 1;

    IF @ReceiptSubscriberID <> @SubscriberID
        THROW 50003, N'الإيصال لا يخص هذا المشترك', 1;

    SELECT @AlreadyApplied =
        CAST(ISNULL(SUM(AppliedAmount),0) AS DECIMAL(12,2))
    FROM dbo.ReceiptApplications
    WHERE ReceiptID = @ReceiptID;

    SET @RemainingReceipt = @ReceiptTotal - ISNULL(@AlreadyApplied,0);

    IF @AppliedAmount > @RemainingReceipt
        THROW 50004, N'المبلغ المطلوب أكبر من المتبقي القابل للتطبيق من الإيصال', 1;

    BEGIN TRAN;

    INSERT INTO dbo.ReceiptApplications
    (
        ReceiptID, InvoiceID, CreditID, AppliedAmount, ApplicationType, PaymentID
    )
    VALUES
    (
        @ReceiptID, NULL, NULL, @AppliedAmount, N'AdvanceCredit', @PaymentID
    );

    INSERT INTO dbo.SubscriberCredits
    (
        SubscriberID, PaymentID, ReceiptID, MeterID, CreditDate,
        AmountTotal, AmountRemaining, Notes
    )
    VALUES
    (
        @SubscriberID, @PaymentID, @ReceiptID, @MeterID, @ReceiptDate,
        @AppliedAmount, @AppliedAmount, @Notes
    );

    COMMIT;
END
GO
/****** Object:  StoredProcedure [dbo].[ApplyReceiptToInvoice]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =========================================================
   H) ApplyReceiptToInvoice - نسخة مصححة
   ========================================================= */
CREATE   PROCEDURE [dbo].[ApplyReceiptToInvoice]
    @ReceiptID INT,
    @InvoiceID INT,
    @AppliedAmount DECIMAL(12,2),
    @PaymentID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @AppliedAmount IS NULL OR @AppliedAmount <= 0
        THROW 50001, N'المبلغ غير صحيح', 1;

    DECLARE
        @ReceiptTotal DECIMAL(12,2),
        @ReceiptSubscriberID INT,
        @AlreadyApplied DECIMAL(12,2),
        @RemainingReceipt DECIMAL(12,2),

        @InvoiceSubscriberID INT,
        @InvoiceBalance DECIMAL(12,2);

    SELECT
        @ReceiptTotal = CAST(TotalAmount AS DECIMAL(12,2)),
        @ReceiptSubscriberID = SubscriberID
    FROM dbo.Receipts
    WHERE ReceiptID = @ReceiptID;

    IF @ReceiptSubscriberID IS NULL
        THROW 50002, N'الإيصال غير موجود', 1;

    SELECT
        @InvoiceSubscriberID = I.SubscriberID,
        @InvoiceBalance =
            CAST(I.TotalAmount AS DECIMAL(12,2))
            - CAST(ISNULL(PA.Paid,0) AS DECIMAL(12,2))
    FROM dbo.Invoices I
    OUTER APPLY
    (
        SELECT SUM(P.Amount) AS Paid
        FROM dbo.Payments P
        WHERE P.InvoiceID = I.InvoiceID
    ) PA
    WHERE I.InvoiceID = @InvoiceID
      AND ISNULL(I.Status, N'') <> N'ملغاة';

    IF @InvoiceSubscriberID IS NULL
        THROW 50003, N'الفاتورة غير موجودة أو ملغاة', 1;

    IF @InvoiceSubscriberID <> @ReceiptSubscriberID
        THROW 50004, N'الإيصال والفاتورة لا يخصان نفس المشترك', 1;

    SELECT @AlreadyApplied =
        CAST(ISNULL(SUM(AppliedAmount),0) AS DECIMAL(12,2))
    FROM dbo.ReceiptApplications
    WHERE ReceiptID = @ReceiptID;

    SET @RemainingReceipt = @ReceiptTotal - ISNULL(@AlreadyApplied,0);

    IF @AppliedAmount > @RemainingReceipt
        THROW 50005, N'المبلغ المطلوب أكبر من المتبقي القابل للتطبيق من الإيصال', 1;

    IF @AppliedAmount > @InvoiceBalance
        THROW 50006, N'المبلغ المطلوب أكبر من رصيد الفاتورة', 1;

    INSERT INTO dbo.ReceiptApplications
    (
        ReceiptID, InvoiceID, CreditID, AppliedAmount, ApplicationType, PaymentID
    )
    VALUES
    (
        @ReceiptID, @InvoiceID, NULL, @AppliedAmount, N'InvoicePayment', @PaymentID
    );
END
GO
/****** Object:  StoredProcedure [dbo].[CollectorDevices_Activate]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[CollectorDevices_Activate]
    @DeviceID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CollectorDevices
    SET IsActive = 1,
        IsApproved = 1
    WHERE DeviceID = @DeviceID;

    IF @@ROWCOUNT = 0
        THROW 51006, N'الجهاز غير موجود', 1;
END
GO
/****** Object:  StoredProcedure [dbo].[CollectorDevices_Deactivate]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[CollectorDevices_Deactivate]
    @DeviceID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CollectorDevices
    SET IsActive = 0
    WHERE DeviceID = @DeviceID;

    IF @@ROWCOUNT = 0
        THROW 51005, N'الجهاز غير موجود', 1;
END
GO
/****** Object:  StoredProcedure [dbo].[CollectorDevices_GetByCollector]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[CollectorDevices_GetByCollector]
    @CollectorID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        D.DeviceID,
        D.CollectorID,
        C.Name AS CollectorName,
        D.DeviceName,
        D.PhoneNumber,
        D.DeviceCode,
        D.DeviceModel,
        D.AppVersion,
        D.IsApproved,
        D.IsActive,
        D.LastSyncAt,
        D.CreatedAt
    FROM dbo.CollectorDevices D
    INNER JOIN dbo.Collectors C
        ON C.CollectorID = D.CollectorID
    WHERE D.CollectorID = @CollectorID
    ORDER BY D.CreatedAt DESC, D.DeviceID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[CollectorDevices_Insert]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[CollectorDevices_Insert]
    @CollectorID INT,
    @DeviceName NVARCHAR(100) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @DeviceCode NVARCHAR(100),
    @DeviceModel NVARCHAR(100) = NULL,
    @AppVersion NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Collectors
        WHERE CollectorID = @CollectorID
    )
        THROW 51001, N'المحصل غير موجود', 1;

    SET @DeviceCode = LTRIM(RTRIM(ISNULL(@DeviceCode, N'')));

    IF @DeviceCode = N''
        THROW 51002, N'كود الجهاز مطلوب', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.CollectorDevices
        WHERE DeviceCode = @DeviceCode
    )
        THROW 51003, N'هذا الجهاز مسجل مسبقًا', 1;

    INSERT INTO dbo.CollectorDevices
    (
        CollectorID,
        DeviceCode,
        DeviceName,
        PhoneNumber,
        DeviceModel,
        AppVersion,
        IsApproved,
        IsActive,
        CreatedAt
    )
    VALUES
    (
        @CollectorID,
        @DeviceCode,
        NULLIF(LTRIM(RTRIM(ISNULL(@DeviceName, N''))), N''),
        NULLIF(LTRIM(RTRIM(ISNULL(@PhoneNumber, N''))), N''),
        NULLIF(LTRIM(RTRIM(ISNULL(@DeviceModel, N''))), N''),
        NULLIF(LTRIM(RTRIM(ISNULL(@AppVersion, N''))), N''),
        1,
        1,
        GETDATE()
    );

    SELECT SCOPE_IDENTITY() AS DeviceID;
END
GO
/****** Object:  StoredProcedure [dbo].[CollectorDevices_Update]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[CollectorDevices_Update]
    @DeviceID INT,
    @DeviceName NVARCHAR(100) = NULL,
    @PhoneNumber NVARCHAR(20) = NULL,
    @DeviceModel NVARCHAR(100) = NULL,
    @AppVersion NVARCHAR(30) = NULL,
    @IsApproved BIT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CollectorDevices
    SET
        DeviceName = NULLIF(LTRIM(RTRIM(ISNULL(@DeviceName, N''))), N''),
        PhoneNumber = NULLIF(LTRIM(RTRIM(ISNULL(@PhoneNumber, N''))), N''),
        DeviceModel = NULLIF(LTRIM(RTRIM(ISNULL(@DeviceModel, N''))), N''),
        AppVersion = NULLIF(LTRIM(RTRIM(ISNULL(@AppVersion, N''))), N''),
        IsApproved = @IsApproved,
        IsActive = @IsActive
    WHERE DeviceID = @DeviceID;

    IF @@ROWCOUNT = 0
        THROW 51004, N'الجهاز غير موجود', 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Collectors_Delete]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete (مع حماية: لا تحذف إذا مرتبط بمدفوعات)
CREATE   PROCEDURE [dbo].[Collectors_Delete]
    @CollectorID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(SELECT 1 FROM dbo.Payments WHERE CollectorID = @CollectorID)
        THROW 50003, N'لا يمكن حذف المتحصل لأنه مرتبط بمدفوعات', 1;

    DELETE FROM dbo.Collectors WHERE CollectorID = @CollectorID;

    IF @@ROWCOUNT = 0
        THROW 50002, N'المتحصل غير موجود', 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Collectors_GetAll]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Get All
CREATE   PROCEDURE [dbo].[Collectors_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CollectorID, Name, Phone
    FROM dbo.Collectors
    ORDER BY Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Collectors_Insert]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Insert
CREATE   PROCEDURE [dbo].[Collectors_Insert]
    @Name NVARCHAR(100),
    @Phone NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Name = LTRIM(RTRIM(@Name));
    IF @Name IS NULL OR @Name = ''
        THROW 50001, N'اسم المتحصل مطلوب', 1;

    INSERT INTO dbo.Collectors(Name, Phone)
    VALUES(@Name, NULLIF(LTRIM(RTRIM(@Phone)), ''));

    SELECT SCOPE_IDENTITY() AS NewID;
END
GO
/****** Object:  StoredProcedure [dbo].[Collectors_Search]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Search
CREATE   PROCEDURE [dbo].[Collectors_Search]
    @q NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET @q = LTRIM(RTRIM(ISNULL(@q,'')));

    SELECT CollectorID, Name, Phone
    FROM dbo.Collectors
    WHERE @q = ''
       OR Name LIKE N'%' + @q + N'%'
       OR Phone LIKE N'%' + @q + N'%'
    ORDER BY Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Collectors_Update]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Update
CREATE   PROCEDURE [dbo].[Collectors_Update]
    @CollectorID INT,
    @Name NVARCHAR(100),
    @Phone NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Name = LTRIM(RTRIM(@Name));
    IF @Name IS NULL OR @Name = ''
        THROW 50001, N'اسم المتحصل مطلوب', 1;

    UPDATE dbo.Collectors
    SET Name = @Name,
        Phone = NULLIF(LTRIM(RTRIM(@Phone)), '')
    WHERE CollectorID = @CollectorID;

    IF @@ROWCOUNT = 0
        THROW 50002, N'المتحصل غير موجود', 1;
END
GO
/****** Object:  StoredProcedure [dbo].[CreateReceipt]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CreateReceipt]
    @SubscriberID INT,
    @CollectorID INT = NULL,
    @PaymentDate DATE,
    @TotalAmount DECIMAL(12,2),
    @PaymentMethod NVARCHAR(30),
    @Notes NVARCHAR(200) = NULL,
    @ReceiptID INT OUTPUT,
    @ReceiptNumber NVARCHAR(30) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF dbo.fn_IsPeriodClosed(@PaymentDate) = 1
        THROW 50010, N'الفترة المحاسبية مغلقة', 1;

    IF @TotalAmount IS NULL OR @TotalAmount <= 0
        THROW 50001, N'المبلغ غير صحيح', 1;

    IF @PaymentMethod NOT IN (N'Cash', N'Transfer', N'Cheque', N'Other')
        THROW 50002, N'طريقة الدفع غير صحيحة', 1;

    BEGIN TRY
        BEGIN TRAN;

        EXEC dbo.GetNextDocumentNumber
            @DocType = N'Receipt',
            @DocDate = @PaymentDate,
            @NewNumber = @ReceiptNumber OUTPUT;

        INSERT INTO dbo.Receipts
        (
            ReceiptNumber, SubscriberID, CollectorID, PaymentDate,
            TotalAmount, PaymentMethod, Notes
        )
        VALUES
        (
            @ReceiptNumber, @SubscriberID, @CollectorID, @PaymentDate,
            @TotalAmount, @PaymentMethod, @Notes
        );

        SET @ReceiptID = SCOPE_IDENTITY();

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[CreateSubscriberAccount]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[CreateSubscriberAccount]
    @SubscriberID INT,
    @AccountID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExistingAccountID INT;

    -- إذا المشترك معه حساب مسبقًا
    SELECT @ExistingAccountID = AccountID
    FROM Subscribers
    WHERE SubscriberID = @SubscriberID;

    IF @ExistingAccountID IS NOT NULL
    BEGIN
        SET @AccountID = @ExistingAccountID;
        RETURN;
    END

    DECLARE @SubscriberName NVARCHAR(100);
    SELECT @SubscriberName = Name
    FROM Subscribers
    WHERE SubscriberID = @SubscriberID;

    IF @SubscriberName IS NULL
        THROW 50001, N'المشترك غير موجود', 1;

    -- كود الحساب: 1200 + SubscriberID (مع Padding)
    DECLARE @Code NVARCHAR(20) = '1200' + RIGHT('000000' + CAST(@SubscriberID AS NVARCHAR(10)), 6);

    -- إذا كود موجود لأي سبب، غيّره بإضافة -ID
    IF EXISTS (SELECT 1 FROM Accounts WHERE AccountCode = @Code)
        SET @Code = @Code + '-' + CAST(@SubscriberID AS NVARCHAR(10));

    -- إنشاء حساب ذمة
    INSERT INTO Accounts (AccountCode, AccountName, AccountType, IsControl, ParentAccountID)
    VALUES (@Code, N'ذمة مشترك: ' + @SubscriberName, 'Asset', 0, NULL);

    SET @AccountID = SCOPE_IDENTITY();

    -- ربطه بالمشترك
    UPDATE Subscribers
    SET AccountID = @AccountID
    WHERE SubscriberID = @SubscriberID;
END
GO
/****** Object:  StoredProcedure [dbo].[Dashboard_GetProductionSnapshot]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Dashboard_GetProductionSnapshot]
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ToDateExclusive DATE = DATEADD(DAY, 1, @ToDate);

    SELECT
        ActiveSubscribers =
            (SELECT COUNT(*) FROM dbo.Subscribers WHERE IsActive = 1),

        ActiveMeters =
            (SELECT COUNT(*) FROM dbo.Meters WHERE IsActive = 1),

        InvoiceCount =
            (SELECT COUNT(*)
             FROM dbo.Invoices
             WHERE InvoiceDate BETWEEN @FromDate AND @ToDate
               AND ISNULL(Status, N'') <> N'ملغاة'),

        BilledAmount =
            CAST(ISNULL((
                SELECT SUM(CAST(ISNULL(TotalAmount, 0) + ISNULL(Arrears, 0) AS DECIMAL(18,2)))
                FROM dbo.Invoices
                WHERE InvoiceDate BETWEEN @FromDate AND @ToDate
                  AND ISNULL(Status, N'') <> N'ملغاة'
            ), 0) AS DECIMAL(18,2)),

        ReceiptCount =
            (SELECT COUNT(*)
             FROM dbo.Receipts
             WHERE PaymentDate BETWEEN @FromDate AND @ToDate
               AND ISNULL(Status, N'Posted') <> N'Cancelled'),

        CollectedAmount =
            CAST(ISNULL((
                SELECT SUM(CAST(TotalAmount AS DECIMAL(18,2)))
                FROM dbo.Receipts
                WHERE PaymentDate BETWEEN @FromDate AND @ToDate
                  AND ISNULL(Status, N'Posted') <> N'Cancelled'
            ), 0) AS DECIMAL(18,2)),

        DebtorsCount =
            (SELECT COUNT(*)
             FROM dbo.vw_SubscriberBalance
             WHERE Balance > 0),

        OutstandingAmount =
            CAST(ISNULL((
                SELECT SUM(CASE WHEN Balance > 0 THEN Balance ELSE 0 END)
                FROM dbo.vw_SubscriberBalance
            ), 0) AS DECIMAL(18,2)),

        AdvanceCreditAmount =
            CAST(ISNULL((
                SELECT SUM(CAST(AmountRemaining AS DECIMAL(18,2)))
                FROM dbo.SubscriberCredits
                WHERE AmountRemaining > 0
            ), 0) AS DECIMAL(18,2)),

        ReadingsCount =
            (SELECT COUNT(*)
             FROM dbo.Readings
             WHERE ReadingDate BETWEEN @FromDate AND @ToDate),

        TotalConsumption =
            CAST(ISNULL((
                SELECT SUM(CAST(ISNULL(Consumption, 0) AS DECIMAL(18,2)))
                FROM dbo.Readings
                WHERE ReadingDate BETWEEN @FromDate AND @ToDate
            ), 0) AS DECIMAL(18,2)),

        SmsTotal =
            (SELECT COUNT(*)
             FROM dbo.SmsLogs
             WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive),

        SmsSent =
            (SELECT COUNT(*)
             FROM dbo.SmsLogs
             WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive
               AND Status = 'Sent'),

        SmsFailed =
            (SELECT COUNT(*)
             FROM dbo.SmsLogs
             WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive
               AND Status = 'Failed'),

        SmsPending =
            (SELECT COUNT(*)
             FROM dbo.SmsLogs
             WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive
               AND Status = 'Pending'),

        MobilePending =
            (SELECT COUNT(*)
             FROM dbo.MobileReceiptImports
             WHERE CreatedAt >= @FromDate AND CreatedAt < @ToDateExclusive
               AND ImportStatus IN (N'New', N'Pending')),

        ExpenseCount =
            (SELECT COUNT(*)
             FROM dbo.Expenses
             WHERE ExpenseDate BETWEEN @FromDate AND @ToDate
               AND ISNULL(Status, N'') <> N'Cancelled'),

        ExpenseAmount =
            CAST(ISNULL((
                SELECT SUM(CAST(TotalAmount AS DECIMAL(18,2)))
                FROM dbo.Expenses
                WHERE ExpenseDate BETWEEN @FromDate AND @ToDate
                  AND ISNULL(Status, N'') <> N'Cancelled'
            ), 0) AS DECIMAL(18,2)),

        LastWaterLoss =
            CAST(ISNULL((
                SELECT TOP (1) WaterLoss
                FROM dbo.MeterReports
                ORDER BY ReportDate DESC, ReportID DESC
            ), 0) AS DECIMAL(18,2));

    SELECT TOP (100)
        I.InvoiceID AS [رقم داخلي],
        ISNULL(I.InvoiceNumber, CAST(I.InvoiceID AS NVARCHAR(30))) AS [رقم الفاتورة],
        I.InvoiceDate AS [تاريخ الفاتورة],
        S.Name AS [المشترك],
        ISNULL(M.MeterNumber, N'') AS [رقم العداد],
        CAST(I.Consumption AS DECIMAL(18,2)) AS [الاستهلاك],
        CAST(I.TotalAmount AS DECIMAL(18,2)) AS [مبلغ الفترة],
        CAST(ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS [متأخرات],
        CAST(ISNULL(PA.Paid,0) AS DECIMAL(18,2)) AS [مدفوع],
        CAST((ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0)) - ISNULL(PA.Paid,0) AS DECIMAL(18,2)) AS [المتبقي],
        ISNULL(I.Status,N'') AS [الحالة]
    FROM dbo.Invoices I
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
    LEFT JOIN dbo.Meters M ON M.MeterID = I.MeterID
    OUTER APPLY
    (
        SELECT SUM(P.Amount) AS Paid
        FROM dbo.Payments P
        WHERE P.InvoiceID = I.InvoiceID
    ) PA
    WHERE I.InvoiceDate BETWEEN @FromDate AND @ToDate
    ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC;

    SELECT TOP (100)
        PaymentID AS [رقم العملية],
        [التاريخ],
        [المشترك],
        [رقم العداد],
        [رقم الفاتورة],
        CAST([المبلغ] AS DECIMAL(18,2)) AS [المبلغ],
        [النوع],
        [الطريقة],
        [المحصل],
        [رقم الإيصال],
        [ملاحظات]
    FROM dbo.vw_PaymentsDisplay
    WHERE [التاريخ] BETWEEN @FromDate AND @ToDate
    ORDER BY [التاريخ] DESC, PaymentID DESC;

    SELECT TOP (100)
        S.SubscriberID AS [رقم المشترك],
        S.Name AS [المشترك],
        ISNULL(S.PhoneNumber,N'') AS [الهاتف],
        ISNULL(MX.MeterNumber,N'') AS [رقم العداد],
        CAST(B.Balance AS DECIMAL(18,2)) AS [الرصيد المستحق]
    FROM dbo.vw_SubscriberBalance B
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = B.SubscriberID
    OUTER APPLY
    (
        SELECT TOP (1) M.MeterNumber
        FROM dbo.SubscriberMeters SM
        INNER JOIN dbo.Meters M ON M.MeterID = SM.MeterID
        WHERE SM.SubscriberID = S.SubscriberID
        ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC
    ) MX
    WHERE B.Balance > 0
    ORDER BY B.Balance DESC;

    SELECT TOP (100)
        R.ReadingID AS [رقم القراءة],
        R.ReadingDate AS [تاريخ القراءة],
        S.Name AS [المشترك],
        ISNULL(M.MeterNumber,N'') AS [رقم العداد],
        CAST(ISNULL(R.PreviousReading,0) AS DECIMAL(18,2)) AS [القراءة السابقة],
        CAST(ISNULL(R.CurrentReading,0) AS DECIMAL(18,2)) AS [القراءة الحالية],
        CAST(ISNULL(R.Consumption,0) AS DECIMAL(18,2)) AS [الاستهلاك],
        ISNULL(R.Notes,N'') AS [ملاحظات]
    FROM dbo.Readings R
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
    LEFT JOIN dbo.Meters M ON M.MeterID = R.MeterID
    WHERE R.ReadingDate BETWEEN @FromDate AND @ToDate
    ORDER BY R.ReadingDate DESC, R.ReadingID DESC;

    SELECT TOP (100)
        L.SmsID AS [رقم الرسالة],
        L.CreatedAt AS [تاريخ الإنشاء],
        ISNULL(S.Name,N'') AS [المشترك],
        ISNULL(L.PhoneNumber,N'') AS [الهاتف],
        ISNULL(L.MessageType,N'') AS [النوع],
        ISNULL(L.Status,N'') AS [الحالة],
        ISNULL(L.Reason,N'') AS [السبب],
        ISNULL(L.Message,N'') AS [نص الرسالة]
    FROM dbo.SmsLogs L
    LEFT JOIN dbo.Subscribers S ON S.SubscriberID = L.SubscriberID
    WHERE L.CreatedAt >= @FromDate AND L.CreatedAt < @ToDateExclusive
    ORDER BY L.CreatedAt DESC, L.SmsID DESC;

    SELECT TOP (100)
        I.ImportID AS [رقم الاستيراد],
        I.CreatedAt AS [تاريخ الإنشاء],
        C.Name AS [المحصل],
        S.Name AS [المشترك],
        CAST(I.TotalReceived AS DECIMAL(18,2)) AS [المبلغ],
        I.PaymentMethod AS [طريقة الدفع],
        I.ImportStatus AS [الحالة],
        ISNULL(I.RejectedReason,N'') AS [سبب الرفض]
    FROM dbo.MobileReceiptImports I
    INNER JOIN dbo.Collectors C ON C.CollectorID = I.CollectorID
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
    WHERE I.CreatedAt >= @FromDate AND I.CreatedAt < @ToDateExclusive
    ORDER BY I.CreatedAt DESC, I.ImportID DESC;

    SELECT TOP (100)
        E.ExpenseID AS [رقم المصروف],
        E.ExpenseNumber AS [رقم السند],
        E.ExpenseDate AS [تاريخ المصروف],
        C.CategoryName AS [التصنيف],
        C.CategoryType AS [النوع],
        ISNULL(E.SupplierName,N'') AS [المورد],
        CAST(E.TotalAmount AS DECIMAL(18,2)) AS [المبلغ],
        E.PaymentMethod AS [طريقة الدفع],
        E.Status AS [الحالة],
        ISNULL(E.Description,N'') AS [الوصف]
    FROM dbo.Expenses E
    INNER JOIN dbo.ExpenseCategories C ON C.CategoryID = E.CategoryID
    WHERE E.ExpenseDate BETWEEN @FromDate AND @ToDate
    ORDER BY E.ExpenseDate DESC, E.ExpenseID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteBillingConstant]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[DeleteBillingConstant]
    @ConstantID INT
AS
BEGIN
    DELETE FROM dbo.BillingConstants WHERE ConstantID=@ConstantID;
END
GO
/****** Object:  StoredProcedure [dbo].[GenerateLateSmsJobs]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GenerateLateSmsJobs]
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM MessageSettings WHERE SettingName='EnableSMS' AND SettingValue='1')
        RETURN;

    IF NOT EXISTS (SELECT 1 FROM MessageSettings WHERE SettingName='SendLateSMS' AND SettingValue='1')
        RETURN;

    DECLARE @LateDays INT = 0;
    SELECT @LateDays = TRY_CAST(SettingValue AS INT)
    FROM MessageSettings
    WHERE SettingName='LateDays';

    IF @LateDays IS NULL OR @LateDays <= 0 SET @LateDays = 7;

    ;WITH inv AS
    (
        SELECT
            I.InvoiceID,
            I.SubscriberID,
            I.InvoiceDate,
            CAST(I.TotalAmount AS DECIMAL(18,2)) AS TotalAmount,
            CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)) AS PaidAmount,
            CAST((CAST(I.TotalAmount AS DECIMAL(18,2)) - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2))) AS DECIMAL(18,2)) AS Remaining
        FROM Invoices I
        OUTER APPLY
        (
            SELECT SUM(Amount) AS PaidAmount
            FROM Payments P
            WHERE P.InvoiceID = I.InvoiceID
        ) PA
        WHERE I.InvoiceDate <= DATEADD(DAY, -@LateDays, CAST(GETDATE() AS DATE))
          AND (CAST(I.TotalAmount AS DECIMAL(18,2)) - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2))) > 0
          AND ISNULL(I.Status,N'') <> N'ملغاة'
    )
    SELECT * INTO #inv FROM inv;

    DECLARE @InvoiceID INT;

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT InvoiceID FROM #inv;

    OPEN cur;
    FETCH NEXT FROM cur INTO @InvoiceID;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- امنع التكرار: لا ترسل نفس Late لنفس الفاتورة خلال 24 ساعة
        IF NOT EXISTS (
            SELECT 1 FROM SmsLogs
            WHERE InvoiceID = @InvoiceID
              AND Status IN ('Pending','Sent')
              AND CreatedAt >= DATEADD(HOUR, -24, GETDATE())
              AND Message LIKE N'%متأخرات%'
        )
        BEGIN
            EXEC dbo.SendSMSDynamic
                @EntityType='Late',
                @EntityID=@InvoiceID,
                @CustomMessage=NULL,
                @Language='AR';
        END

        FETCH NEXT FROM cur INTO @InvoiceID;
    END

    CLOSE cur;
    DEALLOCATE cur;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetActiveBillingConstant]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetActiveBillingConstant]
    @AsOfDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        EffectiveFrom,
        UnitPrice,
        ServiceFees
    FROM BillingConstants
    WHERE IsActive = 1
      AND EffectiveFrom <= @AsOfDate
    ORDER BY EffectiveFrom DESC, ConstantID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetActiveTariff]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetActiveTariff]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        ConstantID AS TariffID,
        EffectiveFrom,
        UnitPrice,
        ServiceFees
    FROM dbo.BillingConstants
    WHERE IsActive = 1
      AND EffectiveFrom <= CAST(GETDATE() AS DATE)
    ORDER BY EffectiveFrom DESC, ConstantID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetBillingConstants]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetBillingConstants]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ConstantID, EffectiveFrom, UnitPrice, ServiceFees, IsActive, Notes, CreatedAt
    FROM dbo.BillingConstants
    ORDER BY EffectiveFrom DESC, ConstantID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetInvoiceDetails]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetInvoiceDetails]
    @InvoiceID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @InvoiceDate DATE,
        @SubscriberID INT,
        @TotalAmount DECIMAL(12,2),
        @StoredArrears DECIMAL(12,2),
        @Paid DECIMAL(12,2);

    SELECT 
        @InvoiceDate = InvoiceDate,
        @SubscriberID = SubscriberID,
        @TotalAmount = CAST(TotalAmount AS DECIMAL(12,2)),
        @StoredArrears = CAST(ISNULL(Arrears,0) AS DECIMAL(12,2))
    FROM dbo.Invoices 
    WHERE InvoiceID = @InvoiceID;

    SELECT 
        @Paid = CAST(ISNULL(SUM(Amount),0) AS DECIMAL(12,2))
    FROM dbo.Payments 
    WHERE InvoiceID = @InvoiceID;

    DECLARE @CurrentDue DECIMAL(12,2) = CAST(@TotalAmount - @Paid AS DECIMAL(12,2));
    IF @CurrentDue < 0 SET @CurrentDue = 0;

    DECLARE @TrueArrears DECIMAL(12,2) =
    (
        SELECT CAST(ISNULL(SUM(CAST(inv.TotalAmount AS DECIMAL(12,2)) - CAST(ISNULL(p.Paid,0) AS DECIMAL(12,2))), 0) AS DECIMAL(12,2))
        FROM dbo.Invoices inv
        LEFT JOIN (
            SELECT InvoiceID, SUM(Amount) AS Paid
            FROM dbo.Payments
            GROUP BY InvoiceID
        ) p ON inv.InvoiceID = p.InvoiceID
        WHERE inv.SubscriberID = @SubscriberID
          AND inv.InvoiceDate < @InvoiceDate
          AND (CAST(inv.TotalAmount AS DECIMAL(12,2)) - CAST(ISNULL(p.Paid,0) AS DECIMAL(12,2))) > 0
          AND ISNULL(inv.Status,N'') <> N'ملغاة'
    );

    DECLARE @InvoiceTotal DECIMAL(12,2) = CAST(@TotalAmount + @StoredArrears AS DECIMAL(12,2));
    DECLARE @TotalDueLive DECIMAL(12,2) = CAST(@CurrentDue + ISNULL(@TrueArrears,0) AS DECIMAL(12,2));

    SELECT 
        I.InvoiceID,
        I.SubscriberID,
        I.InvoiceDate,
        I.Consumption,
        I.UnitPrice,
        I.ServiceFees,
        @StoredArrears AS StoredArrears,
        ISNULL(@TrueArrears,0) AS Arrears,
        I.TotalAmount,
        @InvoiceTotal AS InvoiceTotal,
        @CurrentDue AS CurrentDue,
        @TotalDueLive AS TotalDue,
        @Paid AS Paid,
        I.Status,
        I.Notes,
        S.Name AS SubscriberName,
        S.Address,
        S.PhoneNumber,
        M.MeterNumber,
        R.PreviousReading,
        R.CurrentReading,
        R.ReadingDate
    FROM dbo.Invoices I
    INNER JOIN dbo.Subscribers S ON I.SubscriberID = S.SubscriberID
    LEFT JOIN dbo.Readings R ON I.ReadingID = R.ReadingID
    LEFT JOIN dbo.Meters M ON I.MeterID = M.MeterID
    WHERE I.InvoiceID = @InvoiceID;
END
GO
/****** Object:  StoredProcedure [dbo].[GetLastInvoice]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetLastInvoice]
    @SubscriberID INT,
    @MeterID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM dbo.Invoices
    WHERE SubscriberID = @SubscriberID
      AND (@MeterID IS NULL OR MeterID = @MeterID)
    ORDER BY InvoiceDate DESC, InvoiceID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetLastReading]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetLastReading]
    @SubscriberID INT,
    @MeterID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM dbo.Readings
    WHERE SubscriberID = @SubscriberID
      AND (@MeterID IS NULL OR MeterID = @MeterID)
    ORDER BY ReadingDate DESC, ReadingID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetNextDocumentNumber]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =========================================================
   D) GetNextDocumentNumber
   ========================================================= */
CREATE   PROCEDURE [dbo].[GetNextDocumentNumber]
    @DocType NVARCHAR(20),
    @DocDate DATE,
    @NewNumber NVARCHAR(30) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Y INT = YEAR(@DocDate);
    DECLARE @Next INT;
    DECLARE @Prefix NVARCHAR(10);

    BEGIN TRAN;

    UPDATE dbo.DocumentSequences WITH (UPDLOCK, HOLDLOCK)
    SET
        @Next = NextNumber,
        NextNumber = NextNumber + 1
    WHERE DocType = @DocType
      AND [Year] = @Y;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.DocumentSequences (DocType, [Year], NextNumber)
        VALUES (@DocType, @Y, 2);

        SET @Next = 1;
    END

    COMMIT;

    SET @Prefix =
        CASE
            WHEN @DocType = N'Invoice' THEN N'INV'
            WHEN @DocType = N'Receipt' THEN N'REC'
            ELSE UPPER(LEFT(@DocType, 3))
        END;

    SET @NewNumber =
        @Prefix + N'-' + CAST(@Y AS NVARCHAR(4))
        + N'-' + RIGHT(N'000000' + CAST(@Next AS NVARCHAR(10)), 6);
END
GO
/****** Object:  StoredProcedure [dbo].[GetSubscriberMeters]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetSubscriberMeters]
    @SubscriberID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sm.SubscriberMeterID, m.MeterID, m.MeterNumber, m.Location, sm.IsPrimary, sm.LinkedAt, m.IsActive
    FROM dbo.SubscriberMeters sm
    INNER JOIN dbo.Meters m ON sm.MeterID = m.MeterID
    WHERE sm.SubscriberID = @SubscriberID
    ORDER BY sm.IsPrimary DESC, m.MeterNumber;
END
GO
/****** Object:  StoredProcedure [dbo].[GetTariffForSubscriber]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetTariffForSubscriber]
    @SubscriberID INT,
    @AsOfDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @DefaultUnitPrice DECIMAL(18,2) = 0,
        @DefaultServiceFees DECIMAL(18,2) = 0,
        @TariffPlanID INT = NULL,
        @PricingModel NVARCHAR(20) = NULL,
        @FixedUnitPrice DECIMAL(18,2) = NULL,
        @PlanServiceFees DECIMAL(18,2) = NULL;

    -- 1) التعرفة العامة حسب التاريخ
    SELECT TOP 1
        @DefaultUnitPrice = ISNULL(UnitPrice, 0),
        @DefaultServiceFees = ISNULL(ServiceFees, 0)
    FROM BillingConstants
    WHERE IsActive = 1
      AND EffectiveFrom <= @AsOfDate
    ORDER BY EffectiveFrom DESC, ConstantID DESC;

    -- 2) خطة المشترك
    SELECT @TariffPlanID = TariffPlanID
    FROM Subscribers
    WHERE SubscriberID = @SubscriberID;

    IF @TariffPlanID IS NULL
    BEGIN
        SELECT
            CAST(NULL AS INT) AS TariffPlanID,
            N'DEFAULT' AS PricingModel,
            @DefaultUnitPrice AS UnitPrice,
            @DefaultServiceFees AS ServiceFees;
        RETURN;
    END

    SELECT
        @PricingModel = PricingModel,
        @FixedUnitPrice = FixedUnitPrice,
        @PlanServiceFees = DefaultServiceFees
    FROM TariffPlans
    WHERE TariffPlanID = @TariffPlanID
      AND IsActive = 1;

    -- الخطة غير موجودة أو غير فعالة
    IF @PricingModel IS NULL
    BEGIN
        SELECT
            CAST(NULL AS INT) AS TariffPlanID,
            N'DEFAULT' AS PricingModel,
            @DefaultUnitPrice AS UnitPrice,
            @DefaultServiceFees AS ServiceFees;
        RETURN;
    END

    -- الخطة الثابتة
    IF @PricingModel = N'Fixed'
    BEGIN
        SELECT
            @TariffPlanID AS TariffPlanID,
            @PricingModel AS PricingModel,
            CASE
                WHEN ISNULL(@FixedUnitPrice, 0) > 0 THEN @FixedUnitPrice
                ELSE @DefaultUnitPrice
            END AS UnitPrice,
            COALESCE(@PlanServiceFees, @DefaultServiceFees, 0) AS ServiceFees;
        RETURN;
    END

    -- Tiered
    SELECT
        @TariffPlanID AS TariffPlanID,
        @PricingModel AS PricingModel,
        @DefaultUnitPrice AS UnitPrice, -- fallback فقط
        COALESCE(@PlanServiceFees, @DefaultServiceFees, 0) AS ServiceFees;
END
GO
/****** Object:  StoredProcedure [dbo].[PayOldestInvoice]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PayOldestInvoice]
    @SubscriberID INT,
    @CollectorID INT,
    @PaymentDate DATE,
    @Amount DECIMAL(12,2),
    @PaymentType NVARCHAR(30),   -- Cash / Transfer / Cheque / Other
    @Notes NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    /* ============================================
       0) التحقق المبدئي
    ============================================ */
    IF dbo.fn_IsPeriodClosed(@PaymentDate) = 1
        THROW 50010, N'الفترة المحاسبية مغلقة', 1;

    IF @Amount IS NULL OR @Amount <= 0
        THROW 50001, N'المبلغ غير صحيح', 1;

    IF @PaymentType NOT IN (N'Cash', N'Transfer', N'Cheque', N'Other')
        THROW 50002, N'طريقة الدفع غير صحيحة', 1;

    DECLARE
        @ReceiptID INT = NULL,
        @ReceiptNumber NVARCHAR(30) = NULL,
        @JournalID INT = NULL,
        @SubscriberAccountID INT = NULL,
        @CashAccountID INT = NULL,

        @CashRemain DECIMAL(12,2) = 0,

        @InvoiceID INT = NULL,
        @InvoiceBalance DECIMAL(12,2) = NULL,
        @ApplyAmount DECIMAL(12,2) = NULL,
        @MeterID INT = NULL,
        @PaymentID INT = NULL,
        @LastPaymentID INT = NULL,

        @CreditID INT = NULL,
        @CreditRemain DECIMAL(12,2) = NULL,

        @NewAdvanceCredit DECIMAL(12,2) = 0,

        @InvoiceTotal DECIMAL(12,2) = 0,
        @PaidAfter DECIMAL(12,2) = 0;

    BEGIN TRY
        BEGIN TRAN;

        SET @CashRemain = @Amount;

        /* ============================================
           1) الحسابات الأساسية
        ============================================ */
        SELECT @SubscriberAccountID = S.AccountID
        FROM dbo.Subscribers S
        WHERE S.SubscriberID = @SubscriberID;

        IF @SubscriberAccountID IS NULL
            THROW 50003, N'المشترك لا يملك حسابًا محاسبيًا', 1;

        SELECT TOP (1)
            @CashAccountID = V.CashAccountID
        FROM dbo.vw_SystemAccountsUnified V;

        IF @CashAccountID IS NULL
        BEGIN
            SELECT @CashAccountID = A.AccountID
            FROM dbo.Accounts A
            WHERE A.AccountCode = N'1100';
        END

        IF @CashAccountID IS NULL
            THROW 50004, N'حساب الصندوق غير موجود', 1;

        /* ============================================
           2) إنشاء الإيصال الرئيسي
           الإيصال يمثل فقط النقد الداخل الآن
        ============================================ */
        EXEC dbo.CreateReceipt
            @SubscriberID = @SubscriberID,
            @CollectorID = @CollectorID,
            @PaymentDate = @PaymentDate,
            @TotalAmount = @Amount,
            @PaymentMethod = @PaymentType,
            @Notes = @Notes,
            @ReceiptID = @ReceiptID OUTPUT,
            @ReceiptNumber = @ReceiptNumber OUTPUT;

        /* ============================================
           3) قيد مالي واحد للإيصال كله
           مدين: الصندوق
           دائن: ذمة المشترك
        ============================================ */
        INSERT INTO dbo.Journals
        (
            JournalDate,
            Description,
            Source,
            SourceID,
            IsPosted,
            CreatedAt
        )
        VALUES
        (
            @PaymentDate,
            N'إيصال قبض رقم ' + @ReceiptNumber,
            N'Receipt',
            @ReceiptID,
            0,
            GETDATE()
        );

        SET @JournalID = SCOPE_IDENTITY();

        INSERT INTO dbo.JournalEntries
        (
            JournalID,
            AccountID,
            Debit,
            Credit
        )
        VALUES
            (@JournalID, @CashAccountID, @Amount, 0),
            (@JournalID, @SubscriberAccountID, 0, @Amount);

        IF ROUND(
            (
                SELECT SUM(ISNULL(E.Debit,0)) - SUM(ISNULL(E.Credit,0))
                FROM dbo.JournalEntries E
                WHERE E.JournalID = @JournalID
            ), 2
        ) <> 0
            THROW 50005, N'القيد غير متوازن', 1;

        UPDATE dbo.Journals
        SET IsPosted = 1,
            PostedAt = GETDATE()
        WHERE JournalID = @JournalID;

        UPDATE dbo.Receipts
        SET JournalID = @JournalID
        WHERE ReceiptID = @ReceiptID;

        /* ============================================
           4) استهلاك الرصيد المقدم القديم أولاً
           - لا ReceiptID
           - لا ReceiptApplications
           - لا ربط بالإيصال الجديد
           - لا قيد مالي جديد
        ============================================ */
        WHILE 1 = 1
        BEGIN
            SET @InvoiceID = NULL;
            SET @InvoiceBalance = NULL;
            SET @MeterID = NULL;
            SET @CreditID = NULL;
            SET @CreditRemain = NULL;
            SET @ApplyAmount = NULL;

            SELECT TOP (1)
                @InvoiceID = I.InvoiceID,
                @MeterID = I.MeterID,
                @InvoiceBalance =
                    CAST(ISNULL(I.TotalAmount, 0) + ISNULL(I.Arrears, 0) AS DECIMAL(12,2))
                    - CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
            FROM dbo.Invoices I WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            OUTER APPLY
            (
                SELECT SUM(P.Amount) AS Paid
                FROM dbo.Payments P
                WHERE P.InvoiceID = I.InvoiceID
            ) PA
            WHERE I.SubscriberID = @SubscriberID
              AND I.InvoiceDate <= @PaymentDate
              AND ISNULL(I.Status, N'') <> N'ملغاة'
              AND CAST(ISNULL(I.TotalAmount, 0) + ISNULL(I.Arrears, 0) AS DECIMAL(12,2))
                    > CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
            ORDER BY I.InvoiceDate, I.InvoiceID;

            IF @InvoiceID IS NULL
                BREAK;

            SELECT TOP (1)
                @CreditID = C.CreditID,
                @CreditRemain = CAST(C.AmountRemaining AS DECIMAL(12,2))
            FROM dbo.SubscriberCredits C WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            WHERE C.SubscriberID = @SubscriberID
              AND C.AmountRemaining > 0
              AND C.CreditDate <= @PaymentDate
              AND (C.MeterID = @MeterID OR C.MeterID IS NULL)
            ORDER BY
                CASE WHEN C.MeterID = @MeterID THEN 0 ELSE 1 END,
                C.CreditDate,
                C.CreditID;

            IF @CreditID IS NULL OR @CreditRemain <= 0
                BREAK;

            SET @ApplyAmount =
                CASE
                    WHEN @CreditRemain >= @InvoiceBalance THEN @InvoiceBalance
                    ELSE @CreditRemain
                END;

            IF @MeterID IS NULL
            BEGIN
                SELECT TOP (1) @MeterID = SM.MeterID
                FROM dbo.SubscriberMeters SM
                WHERE SM.SubscriberID = @SubscriberID
                ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC;
            END

            IF @MeterID IS NULL
                THROW 50006, N'لا يوجد عداد مرتبط بالمشترك', 1;

            INSERT INTO dbo.Payments
            (
                InvoiceID,
                SubscriberID,
                CollectorID,
                PaymentDate,
                Amount,
                PaymentType,
                PaymentCategory,
                Notes,
                ReceiptNumber,
                ReceiptID
            )
            VALUES
            (
                @InvoiceID,
                @SubscriberID,
                NULL,
                @PaymentDate,
                @ApplyAmount,
                N'Other',
                N'CreditSettlement',
                ISNULL(@Notes, N'') + N' (تسوية من رصيد مقدم)',
                NULL,
                NULL
            );

            SET @PaymentID = SCOPE_IDENTITY();

            UPDATE dbo.SubscriberCredits
            SET AmountRemaining = CAST(AmountRemaining - @ApplyAmount AS DECIMAL(18,2))
            WHERE CreditID = @CreditID;

            INSERT INTO dbo.AccountStatements
            (
                SubscriberID,
                InvoiceID,
                PaymentID,
                MeterID,
                [Date],
                Details,
                Debit,
                Credit,
                JournalID,
                DocumentType,
                DocumentNumber
            )
            VALUES
            (
                @SubscriberID,
                @InvoiceID,
                @PaymentID,
                @MeterID,
                @PaymentDate,
                N'تسوية من رصيد مقدم',
                0,
                @ApplyAmount,
                NULL,
                N'CreditSettlement',
                CAST(@CreditID AS NVARCHAR(50))
            );

            SELECT
                @InvoiceTotal = CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(12,2))
            FROM dbo.Invoices I
            WHERE I.InvoiceID = @InvoiceID;

            SELECT
                @PaidAfter = CAST(ISNULL(SUM(P.Amount), 0) AS DECIMAL(12,2))
            FROM dbo.Payments P
            WHERE P.InvoiceID = @InvoiceID;

            UPDATE dbo.Invoices
            SET Status =
                CASE
                    WHEN @PaidAfter >= @InvoiceTotal THEN N'مدفوعة'
                    WHEN @PaidAfter > 0 THEN N'مدفوعة جزئياً'
                    ELSE N'غير مدفوعة'
                END
            WHERE InvoiceID = @InvoiceID;
        END

        /* ============================================
           5) توزيع النقد الحالي على الفواتير الأقدم
           فقط هذا الجزء يرتبط بالإيصال
        ============================================ */
        WHILE (@CashRemain > 0)
        BEGIN
            SET @InvoiceID = NULL;
            SET @InvoiceBalance = NULL;
            SET @MeterID = NULL;
            SET @ApplyAmount = NULL;

            SELECT TOP (1)
                @InvoiceID = I.InvoiceID,
                @MeterID = I.MeterID,
                @InvoiceBalance =
                    CAST(ISNULL(I.TotalAmount, 0) + ISNULL(I.Arrears, 0) AS DECIMAL(12,2))
                    - CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
            FROM dbo.Invoices I WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            OUTER APPLY
            (
                SELECT SUM(P.Amount) AS Paid
                FROM dbo.Payments P
                WHERE P.InvoiceID = I.InvoiceID
            ) PA
            WHERE I.SubscriberID = @SubscriberID
              AND I.InvoiceDate <= @PaymentDate
              AND ISNULL(I.Status, N'') <> N'ملغاة'
              AND CAST(ISNULL(I.TotalAmount, 0) + ISNULL(I.Arrears, 0) AS DECIMAL(12,2))
                    > CAST(ISNULL(PA.Paid, 0) AS DECIMAL(12,2))
            ORDER BY I.InvoiceDate, I.InvoiceID;

            IF @InvoiceID IS NULL
                BREAK;

            SET @ApplyAmount =
                CASE
                    WHEN @CashRemain > @InvoiceBalance THEN @InvoiceBalance
                    ELSE @CashRemain
                END;

            IF @MeterID IS NULL
            BEGIN
                SELECT TOP (1) @MeterID = SM.MeterID
                FROM dbo.SubscriberMeters SM
                WHERE SM.SubscriberID = @SubscriberID
                ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC;
            END

            IF @MeterID IS NULL
                THROW 50006, N'لا يوجد عداد مرتبط بالمشترك', 1;

            INSERT INTO dbo.Payments
            (
                InvoiceID,
                SubscriberID,
                CollectorID,
                PaymentDate,
                Amount,
                PaymentType,
                PaymentCategory,
                Notes,
                ReceiptNumber,
                ReceiptID
            )
            VALUES
            (
                @InvoiceID,
                @SubscriberID,
                @CollectorID,
                @PaymentDate,
                @ApplyAmount,
                @PaymentType,
                N'NormalPayment',
                @Notes,
                NULL,
                @ReceiptID
            );

            SET @PaymentID = SCOPE_IDENTITY();
            SET @LastPaymentID = @PaymentID;

            INSERT INTO dbo.ReceiptApplications
            (
                ReceiptID,
                InvoiceID,
                CreditID,
                AppliedAmount,
                ApplicationType,
                PaymentID
            )
            VALUES
            (
                @ReceiptID,
                @InvoiceID,
                NULL,
                @ApplyAmount,
                N'InvoicePayment',
                @PaymentID
            );

            INSERT INTO dbo.AccountStatements
            (
                SubscriberID,
                InvoiceID,
                PaymentID,
                MeterID,
                [Date],
                Details,
                Debit,
                Credit,
                JournalID,
                DocumentType,
                DocumentNumber
            )
            VALUES
            (
                @SubscriberID,
                @InvoiceID,
                @PaymentID,
                @MeterID,
                @PaymentDate,
                N'سداد فاتورة',
                0,
                @ApplyAmount,
                @JournalID,
                N'Receipt',
                @ReceiptNumber
            );

            SELECT
                @InvoiceTotal = CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(12,2))
            FROM dbo.Invoices I
            WHERE I.InvoiceID = @InvoiceID;

            SELECT
                @PaidAfter = CAST(ISNULL(SUM(P.Amount), 0) AS DECIMAL(12,2))
            FROM dbo.Payments P
            WHERE P.InvoiceID = @InvoiceID;

            UPDATE dbo.Invoices
            SET Status =
                CASE
                    WHEN @PaidAfter >= @InvoiceTotal THEN N'مدفوعة'
                    WHEN @PaidAfter > 0 THEN N'مدفوعة جزئياً'
                    ELSE N'غير مدفوعة'
                END
            WHERE InvoiceID = @InvoiceID;

            SET @CashRemain = CAST(@CashRemain - @ApplyAmount AS DECIMAL(12,2));
        END

        /* ============================================
           6) المتبقي = رصيد مقدم جديد
           هذا جزء من الإيصال الحالي
        ============================================ */
        IF @CashRemain > 0
        BEGIN
            SET @NewAdvanceCredit = @CashRemain;

            SELECT TOP (1) @MeterID = SM.MeterID
            FROM dbo.SubscriberMeters SM
            WHERE SM.SubscriberID = @SubscriberID
            ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC;

            IF @MeterID IS NULL
                THROW 50006, N'لا يوجد عداد مرتبط بالمشترك', 1;

            INSERT INTO dbo.Payments
            (
                InvoiceID,
                SubscriberID,
                CollectorID,
                PaymentDate,
                Amount,
                PaymentType,
                PaymentCategory,
                Notes,
                ReceiptNumber,
                ReceiptID
            )
            VALUES
            (
                NULL,
                @SubscriberID,
                @CollectorID,
                @PaymentDate,
                @CashRemain,
                @PaymentType,
                N'AdvanceCredit',
                ISNULL(@Notes, N'') + N' (رصيد مقدم)',
                NULL,
                @ReceiptID
            );

            SET @PaymentID = SCOPE_IDENTITY();
            SET @LastPaymentID = @PaymentID;

            INSERT INTO dbo.ReceiptApplications
            (
                ReceiptID,
                InvoiceID,
                CreditID,
                AppliedAmount,
                ApplicationType,
                PaymentID
            )
            VALUES
            (
                @ReceiptID,
                NULL,
                NULL,
                @CashRemain,
                N'AdvanceCredit',
                @PaymentID
            );

            INSERT INTO dbo.SubscriberCredits
            (
                SubscriberID,
                PaymentID,
                ReceiptID,
                MeterID,
                CreditDate,
                AmountTotal,
                AmountRemaining,
                Notes
            )
            VALUES
            (
                @SubscriberID,
                @PaymentID,
                @ReceiptID,
                @MeterID,
                @PaymentDate,
                @CashRemain,
                @CashRemain,
                ISNULL(@Notes, N'') + N' (رصيد مقدم)'
            );

            INSERT INTO dbo.AccountStatements
            (
                SubscriberID,
                InvoiceID,
                PaymentID,
                MeterID,
                [Date],
                Details,
                Debit,
                Credit,
                JournalID,
                DocumentType,
                DocumentNumber
            )
            VALUES
            (
                @SubscriberID,
                NULL,
                @PaymentID,
                @MeterID,
                @PaymentDate,
                N'رصيد مقدم',
                0,
                @CashRemain,
                @JournalID,
                N'Receipt',
                @ReceiptNumber
            );
        END

        COMMIT;

        /* ============================================
           7) إرسال الرسالة حسب القالب الذي صممه المستخدم
        ============================================ */
        IF @LastPaymentID IS NOT NULL
        BEGIN
            EXEC dbo.SendSMSDynamic
                @EntityType = N'Payment',
                @EntityID = @LastPaymentID,
                @CustomMessage = NULL,
                @Language = N'AR',
                @ForceTemplateID = NULL;
        END
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[PostInvoiceJournal]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =========================================================
   K) PostInvoiceJournal - منع الترحيل المزدوج
   ========================================================= */
CREATE   PROCEDURE [dbo].[PostInvoiceJournal]
    @InvoiceID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Journals
        WHERE Source = N'Invoice'
          AND SourceID = @InvoiceID
    )
        THROW 50030, N'الفاتورة مرحّلة مسبقًا', 1;

    DECLARE
        @SubscriberID INT,
        @InvoiceDate DATE,
        @ConsumptionAmount DECIMAL(18,2),
        @ServiceFeesAmount DECIMAL(18,2),
        @TotalAmount DECIMAL(18,2),
        @JournalID INT,
        @SubscriberAccountID INT,
        @RevenueAccountID INT,
        @ServiceRevenueAccountID INT;

    SELECT
        @SubscriberID = SubscriberID,
        @InvoiceDate = InvoiceDate,
        @ConsumptionAmount = ROUND(Consumption * UnitPrice, 2),
        @ServiceFeesAmount = ROUND(ISNULL(ServiceFees, 0), 2)
    FROM dbo.Invoices
    WHERE InvoiceID = @InvoiceID;

    IF @SubscriberID IS NULL
        THROW 50031, N'الفاتورة غير موجودة', 1;

    IF dbo.fn_IsPeriodClosed(@InvoiceDate) = 1
        THROW 50010, N'الفترة المحاسبية مغلقة', 1;

    SET @TotalAmount = ROUND(@ConsumptionAmount + @ServiceFeesAmount, 2);

    SELECT @SubscriberAccountID = AccountID
    FROM dbo.Subscribers
    WHERE SubscriberID = @SubscriberID;

    IF @SubscriberAccountID IS NULL
        THROW 50001, N'المشترك لا يملك حسابًا محاسبيًا', 1;

    SELECT TOP (1)
        @RevenueAccountID = WaterRevenueAccountID,
        @ServiceRevenueAccountID = ServiceRevenueAccountID
    FROM dbo.vw_SystemAccountsUnified;

    IF @RevenueAccountID IS NULL
        THROW 50002, N'حساب الإيراد غير موجود', 1;

    IF @ServiceFeesAmount > 0 AND @ServiceRevenueAccountID IS NULL
        THROW 50003, N'حساب إيراد رسوم الخدمة غير موجود', 1;

    BEGIN TRY
        BEGIN TRAN;

        INSERT INTO dbo.Journals
        (
            JournalDate, Description, Source, SourceID, IsPosted, CreatedAt
        )
        VALUES
        (
            @InvoiceDate, N'إصدار فاتورة', N'Invoice', @InvoiceID, 0, GETDATE()
        );

        SET @JournalID = SCOPE_IDENTITY();

        INSERT INTO dbo.JournalEntries (JournalID, AccountID, Debit, Credit)
        SELECT @JournalID, @SubscriberAccountID, @TotalAmount, 0
        UNION ALL
        SELECT @JournalID, @RevenueAccountID, 0, @ConsumptionAmount
        UNION ALL
        SELECT @JournalID, @ServiceRevenueAccountID, 0, @ServiceFeesAmount
        WHERE @ServiceFeesAmount > 0;

        IF ROUND(
            (
                SELECT SUM(ISNULL(Debit,0)) - SUM(ISNULL(Credit,0))
                FROM dbo.JournalEntries
                WHERE JournalID = @JournalID
            ), 2
        ) <> 0
            THROW 50004, N'القيد غير متوازن', 1;

        UPDATE dbo.Journals
        SET IsPosted = 1, PostedAt = GETDATE()
        WHERE JournalID = @JournalID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[PostPaymentJournal]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =========================================================
   L) PostPaymentJournal - منع الترحيل المزدوج
   ========================================================= */
CREATE   PROCEDURE [dbo].[PostPaymentJournal]
    @PaymentID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @SubscriberID INT,
        @PaymentDate DATE,
        @Amount DECIMAL(18,2),
        @SubscriberAccountID INT,
        @CashAccountID INT,
        @JournalID INT,
        @ReceiptID INT;

    SELECT
        @SubscriberID = SubscriberID,
        @PaymentDate = PaymentDate,
        @Amount = Amount,
        @ReceiptID = ReceiptID
    FROM dbo.Payments
    WHERE PaymentID = @PaymentID;

    IF @SubscriberID IS NULL
        THROW 50040, N'الدفعة غير موجودة', 1;

    IF @ReceiptID IS NOT NULL
        THROW 50041, N'هذه الدفعة مرتبطة بإيصال وتم ترحيلها ضمن قيد الإيصال', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.Journals
        WHERE Source = N'Payment'
          AND SourceID = @PaymentID
    )
        THROW 50042, N'الدفعة مرحّلة مسبقًا', 1;

    IF dbo.fn_IsPeriodClosed(@PaymentDate) = 1
        THROW 50010, N'الفترة المحاسبية مغلقة', 1;

    SELECT @SubscriberAccountID = AccountID
    FROM dbo.Subscribers
    WHERE SubscriberID = @SubscriberID;

    IF @SubscriberAccountID IS NULL
        THROW 50001, N'المشترك لا يملك حسابًا محاسبيًا', 1;

    SELECT TOP (1) @CashAccountID = CashAccountID
    FROM dbo.vw_SystemAccountsUnified;

    IF @CashAccountID IS NULL
        THROW 50002, N'حساب الصندوق غير موجود', 1;

    BEGIN TRY
        BEGIN TRAN;

        INSERT INTO dbo.Journals
        (
            JournalDate, Description, Source, SourceID, IsPosted, CreatedAt
        )
        VALUES
        (
            @PaymentDate, N'تحصيل من مشترك', N'Payment', @PaymentID, 0, GETDATE()
        );

        SET @JournalID = SCOPE_IDENTITY();

        INSERT INTO dbo.JournalEntries (JournalID, AccountID, Debit, Credit)
        VALUES
            (@JournalID, @CashAccountID, @Amount, 0),
            (@JournalID, @SubscriberAccountID, 0, @Amount);

        IF ROUND(
            (
                SELECT SUM(ISNULL(Debit,0)) - SUM(ISNULL(Credit,0))
                FROM dbo.JournalEntries
                WHERE JournalID = @JournalID
            ), 2
        ) <> 0
            THROW 50003, N'القيد غير متوازن', 1;

        UPDATE dbo.Journals
        SET IsPosted = 1, PostedAt = GETDATE()
        WHERE JournalID = @JournalID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[PreparePaymentSms]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[PreparePaymentSms]
    @PaymentID INT,
    @Language NVARCHAR(10) = 'AR'
AS
BEGIN
    SET NOCOUNT ON;

    -- SMS enabled?
    IF NOT EXISTS (SELECT 1 FROM dbo.MessageSettings WHERE SettingName='EnableSMS' AND SettingValue='1')
        RETURN;

    IF NOT EXISTS (SELECT 1 FROM dbo.MessageSettings WHERE SettingName='SendPaymentSMS' AND SettingValue='1')
        RETURN;

    DECLARE
        @SubscriberID INT,
        @InvoiceID INT = NULL,
        @CollectorID INT = NULL,
        @PhoneNumber NVARCHAR(20),
        @SubscriberName NVARCHAR(150),
        @Amount DECIMAL(12,2),
        @PaymentDate DATE,
        @TemplateID INT,
        @TemplateText NVARCHAR(MAX),
        @Message NVARCHAR(MAX),
        @RemainingBalance DECIMAL(12,2) = NULL,
        @SubscriberBalance DECIMAL(12,2) = NULL;

    -- load payment + subscriber
    SELECT
        @SubscriberID = P.SubscriberID,
        @InvoiceID    = P.InvoiceID,
        @CollectorID  = P.CollectorID,
        @Amount       = CAST(P.Amount AS DECIMAL(12,2)),
        @PaymentDate  = P.PaymentDate
    FROM dbo.Payments P
    WHERE P.PaymentID = @PaymentID;

    IF @SubscriberID IS NULL
        RETURN;

    SELECT
        @SubscriberName = S.Name,              -- ✅ عندك Name وليس FullName
        @PhoneNumber = S.PhoneNumber
    FROM dbo.Subscribers S
    WHERE S.SubscriberID = @SubscriberID;

    IF ISNULL(@PhoneNumber,'') = ''
        RETURN;

    -- get active template (Payment)
    SELECT TOP 1
        @TemplateID = TemplateID,
        @TemplateText = TemplateText
    FROM dbo.MessageTemplates
    WHERE TemplateType = 'Payment'
      AND IsActive = 1
      AND Language = @Language
    ORDER BY TemplateID DESC;

    IF @TemplateText IS NULL
        RETURN;

    -- Remaining of invoice after all payments (if payment linked to invoice)
    IF @InvoiceID IS NOT NULL
    BEGIN
        SELECT
            @RemainingBalance =
                CAST(ISNULL(I.TotalAmount,0) AS DECIMAL(12,2))
                - CAST(ISNULL(PA.Paid,0) AS DECIMAL(12,2))
        FROM dbo.Invoices I
        OUTER APPLY (
            SELECT SUM(Amount) AS Paid
            FROM dbo.Payments
            WHERE InvoiceID = I.InvoiceID
        ) PA
        WHERE I.InvoiceID = @InvoiceID;

        IF @RemainingBalance < 0 SET @RemainingBalance = 0;
    END

    -- Subscriber overall balance (arrears) = invoices - payments (optional but useful)
    SELECT @SubscriberBalance =
        CAST(
            ISNULL((
                SELECT SUM(CAST(I.TotalAmount AS DECIMAL(12,2)))
                FROM dbo.Invoices I
                WHERE I.SubscriberID = @SubscriberID
                  AND ISNULL(I.Status,N'') <> N'ملغاة'
            ),0)
            -
            ISNULL((
                SELECT SUM(CAST(P.Amount AS DECIMAL(12,2)))
                FROM dbo.Payments P
                WHERE P.SubscriberID = @SubscriberID
            ),0)
        AS DECIMAL(12,2));

    IF @SubscriberBalance < 0 SET @SubscriberBalance = 0;

    -- build message by replacing tokens
    SET @Message = @TemplateText;

    SET @Message = REPLACE(@Message, '{SubscriberName}', ISNULL(@SubscriberName,''));
    SET @Message = REPLACE(@Message, '{PhoneNumber}', ISNULL(@PhoneNumber,''));
    SET @Message = REPLACE(@Message, '{InvoiceID}', ISNULL(CAST(@InvoiceID AS NVARCHAR(30)),''));
    SET @Message = REPLACE(@Message, '{PaymentID}', CAST(@PaymentID AS NVARCHAR(30)));
    SET @Message = REPLACE(@Message, '{Amount}', CAST(@Amount AS NVARCHAR(30)));
    SET @Message = REPLACE(@Message, '{PaymentDate}', CONVERT(NVARCHAR(10), @PaymentDate, 23));
    SET @Message = REPLACE(@Message, '{RemainingBalance}', ISNULL(CAST(@RemainingBalance AS NVARCHAR(30)),''));
    SET @Message = REPLACE(@Message, '{SubscriberBalance}', ISNULL(CAST(@SubscriberBalance AS NVARCHAR(30)),''));
    SET @Message = REPLACE(@Message, '{Today}', CONVERT(NVARCHAR(10), GETDATE(), 23));

    -- insert into SmsLogs and return SmsID + phone + message
    INSERT INTO dbo.SmsLogs
    (
        InvoiceID, SubscriberID, PhoneNumber, Message,
        Status, Reason, SentDate, CollectorID, TemplateID, RetryCount, CreatedAt
    )
    VALUES
    (
        @InvoiceID, @SubscriberID, @PhoneNumber, @Message,
        'Pending', NULL, GETDATE(), @CollectorID, @TemplateID, 0, GETDATE()
    );

    DECLARE @SmsID INT = SCOPE_IDENTITY();

    SELECT
        @SmsID AS SmsID,
        @PhoneNumber AS PhoneNumber,
        @Message AS Message,
        @SubscriberID AS SubscriberID,
        @InvoiceID AS InvoiceID;
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_AccountStatement_Generic]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROC [dbo].[rpt_AccountStatement_Generic]
    @SubscriberID INT,
    @FromDate DATE,
    @ToDate   DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST([Date] AS date)       AS Dt1,
        -- نصوص عامة
        CONVERT(nvarchar(50), [Date], 23) AS Col1, -- تاريخ كنص
        ISNULL([Details], N'')      AS Col2,       -- البيان
        ISNULL([DocumentType], N'') AS Col3,       -- نوع المستند
        ISNULL([DocumentNumber], N'') AS Col4,     -- رقم المستند
        ISNULL([Item], N'')         AS Col5,       -- بند/عنصر

        -- أرقام عامة
        CAST(ISNULL([Debit],0)  AS decimal(12,2)) AS Num1, -- مدين
        CAST(ISNULL([Credit],0) AS decimal(12,2)) AS Num2, -- دائن
        CAST(ISNULL([BalanceAfter],0) AS decimal(12,2)) AS Num3, -- رصيد بعد

        -- مفاتيح مساعدة
        [StatementID] AS RowId,
        [SubscriberID] AS RefId1,
        [MeterID] AS RefId2
    FROM dbo.AccountStatements
    WHERE SubscriberID = @SubscriberID
      AND [Date] BETWEEN @FromDate AND @ToDate
    ORDER BY [Date], StatementID;
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_AccountStatement_ShortByReceipt]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_AccountStatement_ShortByReceipt]
    @SubscriberID INT,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.PaymentDate AS Dt1,
        R.ReceiptNumber AS Col1,
        N'دفعة مستلمة' AS Col2,
        R.PaymentMethod AS Col3,
        ISNULL(R.Notes, N'') AS Col4,
        CAST(R.TotalAmount AS DECIMAL(12,2)) AS Num1,
        R.ReceiptID AS RowId,
        R.SubscriberID AS RefId1,
        NULL AS RefId2
    FROM dbo.Receipts R
    WHERE R.SubscriberID = @SubscriberID
      AND R.PaymentDate BETWEEN @FromDate AND @ToDate

    UNION ALL

    SELECT
        I.InvoiceDate AS Dt1,
        ISNULL(I.InvoiceNumber, CAST(I.InvoiceID AS NVARCHAR(30))) AS Col1,
        N'فاتورة' AS Col2,
        ISNULL(I.Status, N'') AS Col3,
        ISNULL(I.Notes, N'') AS Col4,
        CAST(I.TotalAmount AS DECIMAL(12,2)) AS Num1,
        I.InvoiceID AS RowId,
        I.SubscriberID AS RefId1,
        I.MeterID AS RefId2
    FROM dbo.Invoices I
    WHERE I.SubscriberID = @SubscriberID
      AND I.InvoiceDate BETWEEN @FromDate AND @ToDate

    ORDER BY Dt1, RowId;
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_AgingReceivables]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* =========================================================
   10) rpt_AgingReceivables
   ========================================================= */
CREATE   PROCEDURE [dbo].[rpt_AgingReceivables]
    @AsOfDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        S.SubscriberID,
        S.Name,
        SUM(CASE WHEN DATEDIFF(DAY, I.InvoiceDate, @AsOfDate) <= 30 
            THEN I.TotalAmount - ISNULL(P.Paid,0) ELSE 0 END) AS [0-30],
        SUM(CASE WHEN DATEDIFF(DAY, I.InvoiceDate, @AsOfDate) BETWEEN 31 AND 60 
            THEN I.TotalAmount - ISNULL(P.Paid,0) ELSE 0 END) AS [31-60],
        SUM(CASE WHEN DATEDIFF(DAY, I.InvoiceDate, @AsOfDate) BETWEEN 61 AND 90 
            THEN I.TotalAmount - ISNULL(P.Paid,0) ELSE 0 END) AS [61-90],
        SUM(CASE WHEN DATEDIFF(DAY, I.InvoiceDate, @AsOfDate) > 90 
            THEN I.TotalAmount - ISNULL(P.Paid,0) ELSE 0 END) AS [90+]
    FROM Invoices I
    JOIN Subscribers S ON I.SubscriberID = S.SubscriberID
    LEFT JOIN (
        SELECT InvoiceID, SUM(Amount) Paid
        FROM Payments
        GROUP BY InvoiceID
    ) P ON I.InvoiceID = P.InvoiceID
    WHERE I.InvoiceDate <= @AsOfDate
      AND (I.TotalAmount - ISNULL(P.Paid,0)) > 0
    GROUP BY S.SubscriberID, S.Name;
END;
GO
/****** Object:  StoredProcedure [dbo].[rpt_Expenses_Generic]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   13) إجراءات التقارير
   ========================================================= */

CREATE   PROCEDURE [dbo].[rpt_Expenses_Generic]
    @FromDate DATE,
    @ToDate DATE,
    @CategoryType NVARCHAR(20) = NULL,
    @CategoryID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.ExpenseDate AS Dt1,
        E.ExpenseNumber AS Col1,
        C.CategoryName AS Col2,
        C.CategoryType AS Col3,
        ISNULL(E.SupplierName, N'') AS Col4,
        ISNULL(E.Description, N'') AS Col5,

        CAST(E.TotalAmount AS DECIMAL(18,2)) AS Num1,
        E.ExpenseID AS RowId,
        E.CategoryID AS RefId1,
        E.JournalID AS RefId2
    FROM dbo.Expenses E
    INNER JOIN dbo.ExpenseCategories C ON E.CategoryID = C.CategoryID
    WHERE E.ExpenseDate BETWEEN @FromDate AND @ToDate
      AND (@CategoryType IS NULL OR C.CategoryType = @CategoryType)
      AND (@CategoryID IS NULL OR E.CategoryID = @CategoryID)
    ORDER BY E.ExpenseDate, E.ExpenseID;
END

GO
/****** Object:  StoredProcedure [dbo].[rpt_ExpenseSummary_ByCategory]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[rpt_ExpenseSummary_ByCategory]
    @FromDate DATE,
    @ToDate DATE,
    @CategoryType NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.CategoryName AS Col1,
        C.CategoryType AS Col2,
        COUNT(E.ExpenseID) AS Num1,
        CAST(ISNULL(SUM(E.TotalAmount),0) AS DECIMAL(18,2)) AS Num2,
        C.CategoryID AS RowId
    FROM dbo.ExpenseCategories C
    LEFT JOIN dbo.Expenses E
        ON E.CategoryID = C.CategoryID
       AND E.ExpenseDate BETWEEN @FromDate AND @ToDate
    WHERE (@CategoryType IS NULL OR C.CategoryType = @CategoryType)
    GROUP BY C.CategoryID, C.CategoryName, C.CategoryType
    ORDER BY C.CategoryType, C.CategoryName;
END

GO
/****** Object:  StoredProcedure [dbo].[rpt_ExpenseVoucher]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[rpt_ExpenseVoucher]
    @ExpenseID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.ExpenseID,
        E.ExpenseNumber,
        E.ExpenseDate,
        C.CategoryName,
        C.CategoryType,
        ISNULL(E.SupplierName, N'') AS SupplierName,
        ISNULL(E.Description, N'') AS Description,
        ISNULL(E.Notes, N'') AS Notes,
        E.TotalAmount,
        E.PaymentMethod,
        E.IsPosted,
        E.Status,
        ISNULL(U.FullName, N'') AS CreatedByName
    FROM dbo.Expenses E
    INNER JOIN dbo.ExpenseCategories C ON E.CategoryID = C.CategoryID
    LEFT JOIN dbo.Users U ON E.CreatedBy = U.UserID
    WHERE E.ExpenseID = @ExpenseID;

    SELECT
        L.ExpenseLineID,
        L.ItemName,
        L.Qty,
        L.UnitPrice,
        L.LineTotal,
        ISNULL(A.AccountName, N'') AS TargetAccountName,
        ISNULL(L.Notes, N'') AS Notes
    FROM dbo.ExpenseLines L
    LEFT JOIN dbo.Accounts A ON L.TargetAccountID = A.AccountID
    WHERE L.ExpenseID = @ExpenseID
    ORDER BY L.ExpenseLineID;
END

GO
/****** Object:  StoredProcedure [dbo].[rpt_GeneralJournal]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* =========================================================
   11) rpt_GeneralJournal
   ========================================================= */
CREATE   PROCEDURE [dbo].[rpt_GeneralJournal]
    @FromDate DATE,
    @ToDate DATE,
    @Source NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        J.JournalDate,
        J.JournalID,
        J.Description,
        J.Source,
        A.AccountName,
        E.Debit,
        E.Credit
    FROM Journals J
    JOIN JournalEntries E ON J.JournalID = E.JournalID
    JOIN Accounts A ON E.AccountID = A.AccountID
    WHERE J.JournalDate BETWEEN @FromDate AND @ToDate
      AND (@Source IS NULL OR J.Source = @Source)
    ORDER BY J.JournalDate, J.JournalID, E.EntryID;
END;
GO
/****** Object:  StoredProcedure [dbo].[rpt_GeneralLedger]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* =========================================================
   12) rpt_GeneralLedger
   ========================================================= */
CREATE   PROCEDURE [dbo].[rpt_GeneralLedger]
    @AccountID INT,
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Opening Balance
    SELECT 
        SUM(Debit - Credit) AS OpeningBalance
    FROM vw_GeneralLedger
    WHERE AccountID = @AccountID
      AND JournalDate < @FromDate;

    -- Ledger Movements
    SELECT *
    FROM vw_GeneralLedger
    WHERE AccountID = @AccountID
      AND JournalDate BETWEEN @FromDate AND @ToDate
    ORDER BY JournalDate;
END;
GO
/****** Object:  StoredProcedure [dbo].[rpt_Invoices_Generic]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROC [dbo].[rpt_Invoices_Generic]
    @FromDate DATE,
    @ToDate   DATE,
    @SubscriberID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.InvoiceDate AS Dt1,
        i.InvoiceNumber AS Col1,
        s.Name AS Col2,
        m.MeterNumber AS Col3,
        ISNULL(i.Status,N'') AS Col4,
        ISNULL(i.Notes,N'') AS Col5,

        CAST(i.Consumption AS decimal(12,2)) AS Num1,
        CAST(i.UnitPrice AS decimal(12,2))   AS Num2,
        CAST(ISNULL(i.ServiceFees,0) AS decimal(12,2)) AS Num3,
        CAST(ISNULL(i.Arrears,0) AS decimal(12,2))     AS Num4,
        CAST(i.TotalAmount AS decimal(12,2)) AS Num5,

        i.InvoiceID AS RowId,
        i.SubscriberID AS RefId1,
        i.MeterID AS RefId2
    FROM dbo.Invoices i
    JOIN dbo.Subscribers s ON s.SubscriberID = i.SubscriberID
    LEFT JOIN dbo.Meters m ON m.MeterID = i.MeterID
    WHERE i.InvoiceDate BETWEEN @FromDate AND @ToDate
      AND (@SubscriberID IS NULL OR i.SubscriberID = @SubscriberID)
    ORDER BY i.InvoiceDate, i.InvoiceID;
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_Payments_Generic]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_Payments_Generic]
    @FromDate DATE,
    @ToDate   DATE,
    @SubscriberID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.PaymentDate AS Dt1,
        ISNULL(p.ReceiptNumber, r.ReceiptNumber) AS Col1,
        s.Name AS Col2,
        ISNULL(c.Name,N'') AS Col3,
        CASE
            WHEN p.PaymentCategory = N'AdvanceCredit' THEN N'رصيد مقدم'
            WHEN p.PaymentCategory = N'CreditSettlement' THEN N'تسوية من رصيد مقدم'
            ELSE N'سداد'
        END AS Col4,
        ISNULL(p.Notes,N'') AS Col5,
        CAST(p.Amount AS decimal(12,2)) AS Num1,
        p.PaymentID AS RowId,
        p.SubscriberID AS RefId1,
        p.InvoiceID AS RefId2
    FROM dbo.Payments p
    JOIN dbo.Subscribers s ON s.SubscriberID = p.SubscriberID
    LEFT JOIN dbo.Collectors c ON c.CollectorID = p.CollectorID
    LEFT JOIN dbo.Receipts r ON r.ReceiptID = p.ReceiptID
    WHERE p.PaymentDate BETWEEN @FromDate AND @ToDate
      AND (@SubscriberID IS NULL OR p.SubscriberID = @SubscriberID)
    ORDER BY p.PaymentDate, p.PaymentID;
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_Receipts_Generic]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[rpt_Receipts_Generic]
    @FromDate DATE,
    @ToDate DATE,
    @SubscriberID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.PaymentDate AS Dt1,
        R.ReceiptNumber AS Col1,
        S.Name AS Col2,
        ISNULL(C.Name, N'') AS Col3,
        R.PaymentMethod AS Col4,
        ISNULL(R.Notes, N'') AS Col5,
        CAST(R.TotalAmount AS DECIMAL(12,2)) AS Num1,
        R.ReceiptID AS RowId,
        R.SubscriberID AS RefId1,
        NULL AS RefId2
    FROM dbo.Receipts R
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
    LEFT JOIN dbo.Collectors C ON C.CollectorID = R.CollectorID
    WHERE R.PaymentDate BETWEEN @FromDate AND @ToDate
      AND (@SubscriberID IS NULL OR R.SubscriberID = @SubscriberID)
    ORDER BY R.PaymentDate, R.ReceiptID;
END
GO
/****** Object:  StoredProcedure [dbo].[rpt_TrialBalance]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[rpt_TrialBalance]
    @AsOfDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        A.AccountCode,
        A.AccountName,
        CAST(ISNULL(SUM(ISNULL(E.Debit, 0)), 0) AS DECIMAL(18,2)) AS TotalDebit,
        CAST(ISNULL(SUM(ISNULL(E.Credit, 0)), 0) AS DECIMAL(18,2)) AS TotalCredit,
        CAST(ISNULL(SUM(ISNULL(E.Debit, 0) - ISNULL(E.Credit, 0)), 0) AS DECIMAL(18,2)) AS Balance
    FROM dbo.Accounts A
    LEFT JOIN dbo.JournalEntries E
        ON A.AccountID = E.AccountID
    LEFT JOIN dbo.Journals J
        ON E.JournalID = J.JournalID
       AND ISNULL(J.IsPosted, 0) = 1
       AND J.JournalDate <= @AsOfDate
    GROUP BY
        A.AccountCode,
        A.AccountName
    ORDER BY
        A.AccountCode;
END
GO
/****** Object:  StoredProcedure [dbo].[SaveBillingConstant]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[SaveBillingConstant]
    @ConstantID INT = NULL,
    @EffectiveFrom DATE,
    @UnitPrice DECIMAL(18,2),
    @ServiceFees DECIMAL(18,2),
    @IsActive BIT,
    @Notes NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @UnitPrice < 0 OR @ServiceFees < 0
        THROW 50011, N'القيم غير صحيحة', 1;

    IF @ConstantID IS NULL OR @ConstantID = 0
    BEGIN
        INSERT INTO dbo.BillingConstants (EffectiveFrom, UnitPrice, ServiceFees, IsActive, Notes)
        VALUES (@EffectiveFrom, @UnitPrice, @ServiceFees, @IsActive, @Notes);

        SELECT SCOPE_IDENTITY() AS NewID;
        RETURN;
    END

    UPDATE dbo.BillingConstants
    SET EffectiveFrom=@EffectiveFrom,
        UnitPrice=@UnitPrice,
        ServiceFees=@ServiceFees,
        IsActive=@IsActive,
        Notes=@Notes
    WHERE ConstantID=@ConstantID;

    SELECT @ConstantID AS NewID;
END
GO
/****** Object:  StoredProcedure [dbo].[SendInvoiceSMS]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SendInvoiceSMS]
    @InvoiceID INT
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.SendSMSDynamic
        @EntityType = N'Invoice',
        @EntityID = @InvoiceID,
        @CustomMessage = NULL,
        @Language = N'AR',
        @ForceTemplateID = NULL;
END
GO
/****** Object:  StoredProcedure [dbo].[SendSMSDynamic]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SendSMSDynamic]
    @EntityType NVARCHAR(50),     -- Invoice / Payment / Late
    @EntityID INT,
    @CustomMessage NVARCHAR(MAX) = NULL,
    @Language NVARCHAR(10) = 'AR',
    @ForceTemplateID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- التحقق من التفعيل العام
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.MessageSettings
        WHERE SettingName = N'EnableSMS'
          AND SettingValue = N'1'
    )
        RETURN;

    DECLARE
        @SubscriberID INT = NULL,
        @PhoneNumber NVARCHAR(20) = NULL,
        @Message NVARCHAR(MAX) = NULL,
        @TemplateText NVARCHAR(MAX) = NULL,
        @TemplateID INT = NULL,

        @InvoiceID INT = NULL,
        @PaymentID INT = NULL,
        @ReceiptID INT = NULL,
        @CollectorID INT = NULL,

        @InvoiceNumber NVARCHAR(30) = NULL,
        @ReceiptNumber NVARCHAR(30) = NULL,
        @PaymentType NVARCHAR(30) = NULL,
        @PaymentCategory NVARCHAR(30) = NULL,
        @Notes NVARCHAR(200) = NULL,

        @TotalAmount DECIMAL(18,2) = 0,
        @Amount DECIMAL(18,2) = 0,            -- مبلغ سجل الدفع نفسه
        @PaidTotal DECIMAL(18,2) = 0,
        @Remaining DECIMAL(18,2) = 0,
        @TotalReceived DECIMAL(18,2) = 0,     -- إجمالي الإيصال
        @PaidToInvoices DECIMAL(18,2) = 0,    -- ما تم تطبيقه على الفواتير من هذا الإيصال
        @CreditAmount DECIMAL(18,2) = 0,      -- رصيد مقدم جديد من هذا الإيصال
        @RemainingBalance DECIMAL(18,2) = 0,  -- الرصيد الكلي للمشترك
        @RemainingBalanceAbs DECIMAL(18,2) = 0,
        @CurrentDue DECIMAL(18,2) = 0,
        @CurrentCredit DECIMAL(18,2) = 0,
        @BalanceDirection NVARCHAR(20) = N'متوازن',

        @SubscriberName NVARCHAR(150) = NULL,
        @InvoiceDate DATE = NULL,
        @PaymentDate DATE = NULL,
        @LateDays INT = 0,

        @Consumption DECIMAL(18,2) = 0,
        @Arrears DECIMAL(18,2) = 0,
        @CurrentReading DECIMAL(18,2) = 0,
        @PreviousReading DECIMAL(18,2) = 0,
        @MeterID INT = NULL,
        @MeterNumber NVARCHAR(50) = NULL,
        @GrandTotal DECIMAL(18,2) = 0,

        @InvoicesCount INT = 0,
        @InvoiceList NVARCHAR(MAX) = N'';

    /* ============================================
       1) اختيار القالب
    ============================================ */
    IF @CustomMessage IS NOT NULL
    BEGIN
        SET @Message = @CustomMessage;
    END
    ELSE
    BEGIN
        -- إذا لم يُفرض قالب صراحةً، حاول أولاً من MessageTemplateMap
        IF @ForceTemplateID IS NULL
        BEGIN
            SELECT TOP (1)
                @ForceTemplateID = MTM.TemplateID
            FROM dbo.MessageTemplateMap MTM
            WHERE MTM.EventKey = @EntityType
              AND MTM.IsEnabled = 1
              AND MTM.TemplateID IS NOT NULL
            ORDER BY MTM.UpdatedAt DESC, MTM.MapID DESC;
        END

        IF @ForceTemplateID IS NOT NULL
        BEGIN
            SELECT
                @TemplateID = MT.TemplateID,
                @TemplateText = MT.TemplateText
            FROM dbo.MessageTemplates MT
            WHERE MT.TemplateID = @ForceTemplateID
              AND MT.IsActive = 1
              AND MT.Language = @Language;
        END
        ELSE
        BEGIN
            -- fallback: آخر قالب فعال من نفس النوع
            SELECT TOP (1)
                @TemplateID = MT.TemplateID,
                @TemplateText = MT.TemplateText
            FROM dbo.MessageTemplates MT
            WHERE MT.TemplateType = @EntityType
              AND MT.IsActive = 1
              AND MT.Language = @Language
            ORDER BY MT.TemplateID DESC;
        END

        IF @TemplateText IS NULL
            RETURN;

        SET @Message = @TemplateText;
    END

    /* ============================================
       2) تحميل البيانات
    ============================================ */

    IF @EntityType = N'Invoice'
    BEGIN
        SELECT
            @SubscriberID    = I.SubscriberID,
            @InvoiceID       = I.InvoiceID,
            @InvoiceNumber   = I.InvoiceNumber,
            @TotalAmount     = ISNULL(I.TotalAmount, 0),
            @Consumption     = ISNULL(I.Consumption, 0),
            @Arrears         = ISNULL(I.Arrears, 0),
            @InvoiceDate     = I.InvoiceDate,
            @SubscriberName  = S.Name,
            @PhoneNumber     = S.PhoneNumber,
            @CurrentReading  = ISNULL(R.CurrentReading, 0),
            @PreviousReading = ISNULL(R.PreviousReading, 0),
            @MeterID         = I.MeterID,
            @MeterNumber     = M.MeterNumber,
            @Notes           = I.Notes
        FROM dbo.Invoices I
        INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
        LEFT JOIN dbo.Readings R ON R.ReadingID = I.ReadingID
        LEFT JOIN dbo.Meters M ON M.MeterID = I.MeterID
        WHERE I.InvoiceID = @EntityID;
    END
    ELSE IF @EntityType = N'Payment'
    BEGIN
        SELECT
            @PaymentID        = P.PaymentID,
            @SubscriberID     = P.SubscriberID,
            @InvoiceID        = P.InvoiceID,
            @ReceiptID        = P.ReceiptID,
            @CollectorID      = P.CollectorID,
            @Amount           = ISNULL(P.Amount, 0),
            @PaymentDate      = P.PaymentDate,
            @PaymentType      = P.PaymentType,
            @PaymentCategory  = P.PaymentCategory,
            @Notes            = P.Notes,
            @SubscriberName   = S.Name,
            @PhoneNumber      = S.PhoneNumber
        FROM dbo.Payments P
        INNER JOIN dbo.Subscribers S ON S.SubscriberID = P.SubscriberID
        WHERE P.PaymentID = @EntityID;

        IF @ReceiptID IS NOT NULL
        BEGIN
            SELECT
                @ReceiptNumber = R.ReceiptNumber,
                @TotalReceived = CAST(ISNULL(R.TotalAmount, 0) AS DECIMAL(18,2))
            FROM dbo.Receipts R
            WHERE R.ReceiptID = @ReceiptID;

            SELECT
                @PaidToInvoices = CAST(ISNULL(SUM(RA.AppliedAmount), 0) AS DECIMAL(18,2))
            FROM dbo.ReceiptApplications RA
            WHERE RA.ReceiptID = @ReceiptID
              AND RA.ApplicationType = N'InvoicePayment';

            SELECT
                @CreditAmount = CAST(ISNULL(SUM(RA.AppliedAmount), 0) AS DECIMAL(18,2))
            FROM dbo.ReceiptApplications RA
            WHERE RA.ReceiptID = @ReceiptID
              AND RA.ApplicationType = N'AdvanceCredit';

            SELECT
                @InvoicesCount = COUNT(DISTINCT RA.InvoiceID)
            FROM dbo.ReceiptApplications RA
            WHERE RA.ReceiptID = @ReceiptID
              AND RA.InvoiceID IS NOT NULL;

            SELECT
                @InvoiceList = ISNULL(STRING_AGG(CAST(X.InvoiceID AS NVARCHAR(20)), N','), N'')
            FROM
            (
                SELECT DISTINCT RA.InvoiceID
                FROM dbo.ReceiptApplications RA
                WHERE RA.ReceiptID = @ReceiptID
                  AND RA.InvoiceID IS NOT NULL
            ) X;
        END
        ELSE
        BEGIN
            SET @TotalReceived = @Amount;
        END

        IF @InvoiceID IS NOT NULL
        BEGIN
            SELECT
                @TotalAmount   = ISNULL(I.TotalAmount, 0),
                @Arrears       = ISNULL(I.Arrears, 0),
                @InvoiceDate   = I.InvoiceDate,
                @InvoiceNumber = I.InvoiceNumber,
                @MeterID       = I.MeterID,
                @MeterNumber   = M.MeterNumber
            FROM dbo.Invoices I
            LEFT JOIN dbo.Meters M ON M.MeterID = I.MeterID
            WHERE I.InvoiceID = @InvoiceID;
        END
    END
    ELSE IF @EntityType = N'Late'
    BEGIN
        SELECT
            @SubscriberID   = I.SubscriberID,
            @InvoiceID      = I.InvoiceID,
            @InvoiceNumber  = I.InvoiceNumber,
            @TotalAmount    = ISNULL(I.TotalAmount, 0),
            @Arrears        = ISNULL(I.Arrears, 0),
            @InvoiceDate    = I.InvoiceDate,
            @SubscriberName = S.Name,
            @PhoneNumber    = S.PhoneNumber,
            @MeterID        = I.MeterID,
            @MeterNumber    = M.MeterNumber
        FROM dbo.Invoices I
        INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
        LEFT JOIN dbo.Meters M ON M.MeterID = I.MeterID
        WHERE I.InvoiceID = @EntityID;
    END
    ELSE
        RETURN;

    IF ISNULL(@PhoneNumber, N'') = N''
        RETURN;

    /* ============================================
       3) الحسابات
    ============================================ */

    -- الإجمالي الخاص بالفاتورة الحالية = مبلغ الفترة + المتأخرات وقت الإصدار
    SET @GrandTotal = ISNULL(@TotalAmount, 0) + ISNULL(@Arrears, 0);

    -- المدفوع والمتبقي على نفس الفاتورة فقط
    IF @InvoiceID IS NOT NULL
    BEGIN
        SELECT
            @PaidTotal = ISNULL(SUM(P.Amount), 0)
        FROM dbo.Payments P
        WHERE P.InvoiceID = @InvoiceID;

        SET @Remaining = @GrandTotal - @PaidTotal;
        IF @Remaining < 0 SET @Remaining = 0;
    END

    IF @InvoiceDate IS NOT NULL
        SET @LateDays = DATEDIFF(DAY, @InvoiceDate, GETDATE());

    /* ============================================
       3.1) الرصيد الكلي الحالي للمشترك
       موجب = عليه
       سالب = له
    ============================================ */
    IF @SubscriberID IS NOT NULL
    BEGIN
        SELECT
            @RemainingBalance = CAST(ISNULL(B.Balance, 0) AS DECIMAL(18,2))
        FROM dbo.vw_SubscriberBalance B
        WHERE B.SubscriberID = @SubscriberID;

        SET @RemainingBalance = ISNULL(@RemainingBalance, 0);
        SET @RemainingBalanceAbs = ABS(@RemainingBalance);

        SET @BalanceDirection =
            CASE
                WHEN @RemainingBalance > 0 THEN N'عليه'
                WHEN @RemainingBalance < 0 THEN N'له'
                ELSE N'متوازن'
            END;

        SET @CurrentDue =
            CASE
                WHEN @RemainingBalance > 0 THEN @RemainingBalance
                ELSE 0
            END;

        SET @CurrentCredit =
            CASE
                WHEN @RemainingBalance < 0 THEN ABS(@RemainingBalance)
                ELSE 0
            END;
    END

    /* ============================================
       4) المتغيرات الديناميكية
    ============================================ */
    DECLARE @Vars TABLE
    (
        VarName NVARCHAR(100),
        VarValue NVARCHAR(MAX)
    );

    INSERT INTO @Vars (VarName, VarValue)
    VALUES
        (N'SubscriberName',   ISNULL(@SubscriberName, N'')),
        (N'PhoneNumber',      ISNULL(@PhoneNumber, N'')),

        (N'InvoiceID',        ISNULL(CAST(@InvoiceID AS NVARCHAR(30)), N'')),
        (N'InvoiceNumber',    ISNULL(@InvoiceNumber, N'')),

        (N'PaymentID',        ISNULL(CAST(@PaymentID AS NVARCHAR(30)), N'')),
        (N'ReceiptID',        ISNULL(CAST(@ReceiptID AS NVARCHAR(30)), N'')),
        (N'ReceiptNumber',    ISNULL(@ReceiptNumber, N'')),

        (N'TotalAmount',      CAST(ISNULL(@TotalAmount, 0) AS NVARCHAR(30))),
        (N'Arrears',          CAST(ISNULL(@Arrears, 0) AS NVARCHAR(30))),
        (N'GrandTotal',       CAST(ISNULL(@GrandTotal, 0) AS NVARCHAR(30))),

        (N'Amount',           CAST(ISNULL(@Amount, 0) AS NVARCHAR(30))),
        (N'TotalReceived',    CAST(ISNULL(@TotalReceived, 0) AS NVARCHAR(30))),
        (N'PaidToInvoices',   CAST(ISNULL(@PaidToInvoices, 0) AS NVARCHAR(30))),
        (N'CreditAmount',     CAST(ISNULL(@CreditAmount, 0) AS NVARCHAR(30))),

        (N'PaidTotal',        CAST(ISNULL(@PaidTotal, 0) AS NVARCHAR(30))),
        (N'Remaining',        CAST(ISNULL(@Remaining, 0) AS NVARCHAR(30))),

        (N'RemainingBalance',    CAST(ISNULL(@RemainingBalance, 0) AS NVARCHAR(30))),
        (N'RemainingBalanceAbs', CAST(ISNULL(@RemainingBalanceAbs, 0) AS NVARCHAR(30))),
        (N'CurrentDue',          CAST(ISNULL(@CurrentDue, 0) AS NVARCHAR(30))),
        (N'CurrentCredit',       CAST(ISNULL(@CurrentCredit, 0) AS NVARCHAR(30))),
        (N'BalanceDirection',    ISNULL(@BalanceDirection, N'')),

        (N'Consumption',      CAST(ISNULL(@Consumption, 0) AS NVARCHAR(30))),
        (N'CurrentReading',   CAST(ISNULL(@CurrentReading, 0) AS NVARCHAR(30))),
        (N'PreviousReading',  CAST(ISNULL(@PreviousReading, 0) AS NVARCHAR(30))),

        (N'MeterID',          ISNULL(CAST(@MeterID AS NVARCHAR(30)), N'')),
        (N'MeterNumber',      ISNULL(@MeterNumber, N'')),

        (N'InvoiceDate',      ISNULL(CONVERT(NVARCHAR(10), @InvoiceDate, 23), N'')),
        (N'PaymentDate',      ISNULL(CONVERT(NVARCHAR(10), @PaymentDate, 23), N'')),

        (N'PaymentType',      ISNULL(@PaymentType, N'')),
        (N'PaymentCategory',  ISNULL(@PaymentCategory, N'')),
        (N'Notes',            ISNULL(@Notes, N'')),
        (N'LateDays',         CAST(ISNULL(@LateDays, 0) AS NVARCHAR(30))),

        (N'InvoicesCount',    CAST(ISNULL(@InvoicesCount, 0) AS NVARCHAR(30))),
        (N'InvoiceList',      ISNULL(@InvoiceList, N'')),
        (N'Today',            CONVERT(NVARCHAR(10), GETDATE(), 23));

    DECLARE
        @VarName NVARCHAR(100),
        @VarValue NVARCHAR(MAX);

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT VarName, VarValue
        FROM @Vars;

    OPEN cur;
    FETCH NEXT FROM cur INTO @VarName, @VarValue;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @Message = REPLACE(@Message, N'{' + @VarName + N'}', ISNULL(@VarValue, N''));
        FETCH NEXT FROM cur INTO @VarName, @VarValue;
    END

    CLOSE cur;
    DEALLOCATE cur;

    /* ============================================
       5) تسجيل الرسالة Pending
    ============================================ */
    INSERT INTO dbo.SmsLogs
    (
        InvoiceID,
        SubscriberID,
        PhoneNumber,
        Message,
        Status,
        Reason,
        SentDate,
        CollectorID,
        TemplateID,
        RetryCount,
        CreatedAt,
        MessageType,
        PaymentID,
        ReceiptID
    )
    VALUES
    (
        @InvoiceID,
        @SubscriberID,
        @PhoneNumber,
        @Message,
        N'Pending',
        NULL,
        NULL,
        @CollectorID,
        @TemplateID,
        0,
        GETDATE(),
        @EntityType,
        @PaymentID,
        @ReceiptID
    );
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateSmsLogStatus]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[UpdateSmsLogStatus]
    @SmsID INT,
    @Status NVARCHAR(20),
    @Reason NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SmsLogs
    SET Status = @Status,
        Reason = @Reason,
        SentDate = GETDATE()
    WHERE SmsID = @SmsID;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Expense_Delete]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   10) حذف حركة
   ========================================================= */

CREATE   PROCEDURE [dbo].[usp_Expense_Delete]
    @ExpenseID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @ExpenseDate DATE,
        @JournalID INT;

    IF @ExpenseID IS NULL OR @ExpenseID <= 0
        THROW 50201, N'معرف الحركة غير صحيح.', 1;

    SELECT
        @ExpenseDate = ExpenseDate,
        @JournalID = JournalID
    FROM dbo.Expenses
    WHERE ExpenseID = @ExpenseID;

    IF @ExpenseDate IS NULL
        THROW 50202, N'الحركة غير موجودة.', 1;

    IF dbo.fn_IsPeriodClosed(@ExpenseDate) = 1
        THROW 50203, N'لا يمكن حذف الحركة لأن الفترة المحاسبية مغلقة.', 1;

    BEGIN TRY
        BEGIN TRAN;

        DELETE FROM dbo.ExpenseLines
        WHERE ExpenseID = @ExpenseID;

        IF @JournalID IS NOT NULL
        BEGIN
            DELETE FROM dbo.JournalEntries
            WHERE JournalID = @JournalID;

            DELETE FROM dbo.Journals
            WHERE JournalID = @JournalID;
        END

        DELETE FROM dbo.Expenses
        WHERE ExpenseID = @ExpenseID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH;
END

GO
/****** Object:  StoredProcedure [dbo].[usp_Expense_GetDetails]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_Expense_GetDetails]
    @ExpenseID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.ExpenseID,
        E.ExpenseNumber,
        E.ExpenseDate,
        E.CategoryID,
        C.CategoryName,
        C.CategoryType,
        E.SupplierName,
        E.Description,
        E.Notes,
        E.TotalAmount,
        E.PaymentMethod,
        E.CashAccountID,
        E.CounterAccountID,
        E.JournalID,
        E.IsPosted,
        E.Status
    FROM dbo.Expenses E
    INNER JOIN dbo.ExpenseCategories C ON E.CategoryID = C.CategoryID
    WHERE E.ExpenseID = @ExpenseID;

    SELECT
        L.ExpenseLineID,
        L.ItemName,
        L.Qty,
        L.UnitPrice,
        L.LineTotal,
        L.TargetAccountID,
        L.Notes
    FROM dbo.ExpenseLines L
    WHERE L.ExpenseID = @ExpenseID
    ORDER BY L.ExpenseLineID;
END

GO
/****** Object:  StoredProcedure [dbo].[usp_Expense_Save]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   4) إصلاح حفظ المصروفات
   ========================================================= */

CREATE   PROCEDURE [dbo].[usp_Expense_Save]
    @ExpenseID INT = NULL OUTPUT,
    @ExpenseDate DATE,
    @CategoryID INT,
    @SupplierName NVARCHAR(150) = NULL,
    @Description NVARCHAR(250) = NULL,
    @Notes NVARCHAR(250) = NULL,
    @PaymentMethod NVARCHAR(30),
    @CashAccountID INT = NULL,
    @CounterAccountID INT = NULL,
    @CreatedBy INT = NULL,
    @Lines dbo.ExpenseLineType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @ExpenseNumber NVARCHAR(30),
        @JournalID INT,
        @DefaultAccountID INT,
        @TotalAmount DECIMAL(18,2),
        @DebitTotal DECIMAL(18,2),
        @CreditTotal DECIMAL(18,2);

    IF dbo.fn_IsPeriodClosed(@ExpenseDate) = 1
        THROW 50010, N'الفترة المحاسبية مغلقة.', 1;

    IF @PaymentMethod NOT IN (N'Cash', N'Transfer', N'Cheque', N'Credit')
        THROW 50012, N'طريقة الدفع غير صحيحة.', 1;

    IF NOT EXISTS (SELECT 1 FROM @Lines)
        THROW 50011, N'يجب إدخال بند واحد على الأقل.', 1;

    IF @CashAccountID IS NOT NULL AND @CashAccountID <= 0
        SET @CashAccountID = NULL;

    IF @CounterAccountID IS NOT NULL AND @CounterAccountID <= 0
        SET @CounterAccountID = NULL;

    SELECT @DefaultAccountID = DefaultAccountID
    FROM dbo.ExpenseCategories
    WHERE CategoryID = @CategoryID
      AND ISNULL(IsActive, 1) = 1;

    IF @DefaultAccountID IS NULL AND @CounterAccountID IS NULL
        THROW 50013, N'لا يوجد حساب افتراضي للتصنيف، ولم يتم اختيار حساب مقابل.', 1;

    IF @CashAccountID IS NULL
    BEGIN
        IF @PaymentMethod = N'Cash'
            SELECT TOP 1 @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1100';

        ELSE IF @PaymentMethod IN (N'Transfer', N'Cheque')
            SELECT TOP 1 @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1000';

        ELSE IF @PaymentMethod = N'Credit'
            SELECT TOP 1 @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'2100';
    END;

    IF @CashAccountID IS NULL
        THROW 50016, N'حساب الدفع غير محدد.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountID = @CashAccountID)
        THROW 50020, N'حساب الدفع غير موجود في دليل الحسابات.', 1;

    DECLARE @CleanLines TABLE
    (
        ItemName NVARCHAR(150),
        Qty DECIMAL(18,2),
        UnitPrice DECIMAL(18,2),
        LineTotal DECIMAL(18,2),
        DebitAccountID INT,
        TargetAccountID INT NULL,
        Notes NVARCHAR(200)
    );

    INSERT INTO @CleanLines
    (
        ItemName,
        Qty,
        UnitPrice,
        LineTotal,
        DebitAccountID,
        TargetAccountID,
        Notes
    )
    SELECT
        ISNULL(NULLIF(LTRIM(RTRIM(L.ItemName)), N''), N'بند'),
        ISNULL(L.Qty, 0),
        ISNULL(L.UnitPrice, 0),
        CAST(ROUND(ISNULL(L.Qty, 0) * ISNULL(L.UnitPrice, 0), 2) AS DECIMAL(18,2)),
        ISNULL(NULLIF(L.TargetAccountID, 0), ISNULL(@CounterAccountID, @DefaultAccountID)),
        NULLIF(L.TargetAccountID, 0),
        L.Notes
    FROM @Lines L
    WHERE ISNULL(L.Qty, 0) > 0
      AND ISNULL(L.UnitPrice, 0) > 0;

    IF NOT EXISTS (SELECT 1 FROM @CleanLines)
        THROW 50021, N'لا توجد بنود صحيحة للحفظ.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @CleanLines L
        WHERE L.DebitAccountID IS NULL
           OR NOT EXISTS
           (
               SELECT 1
               FROM dbo.Accounts A
               WHERE A.AccountID = L.DebitAccountID
           )
    )
        THROW 50022, N'يوجد بند بدون حساب مدين صحيح.', 1;

    SELECT @TotalAmount = CAST(ROUND(SUM(LineTotal), 2) AS DECIMAL(18,2))
    FROM @CleanLines;

    IF @TotalAmount <= 0
        THROW 50014, N'إجمالي الحركة غير صحيح.', 1;

    BEGIN TRY
        BEGIN TRAN;

        EXEC dbo.GetNextDocumentNumber
            @DocType = N'Expense',
            @DocDate = @ExpenseDate,
            @NewNumber = @ExpenseNumber OUTPUT;

        INSERT INTO dbo.Expenses
        (
            ExpenseNumber,
            ExpenseDate,
            CategoryID,
            SupplierName,
            Description,
            Notes,
            TotalAmount,
            PaymentMethod,
            CashAccountID,
            CounterAccountID,
            CreatedBy,
            IsPosted,
            Status
        )
        VALUES
        (
            @ExpenseNumber,
            @ExpenseDate,
            @CategoryID,
            @SupplierName,
            @Description,
            @Notes,
            @TotalAmount,
            @PaymentMethod,
            @CashAccountID,
            @CounterAccountID,
            @CreatedBy,
            0,
            N'Posted'
        );

        SET @ExpenseID = SCOPE_IDENTITY();

        INSERT INTO dbo.ExpenseLines
        (
            ExpenseID,
            ItemName,
            Qty,
            UnitPrice,
            TargetAccountID,
            Notes
        )
        SELECT
            @ExpenseID,
            ItemName,
            Qty,
            UnitPrice,
            TargetAccountID,
            Notes
        FROM @CleanLines;

        INSERT INTO dbo.Journals
        (
            JournalDate,
            Description,
            Source,
            SourceID,
            IsPosted,
            CreatedAt
        )
        VALUES
        (
            @ExpenseDate,
            N'قيد حركة مصروف/شراء رقم ' + @ExpenseNumber,
            N'Expense',
            @ExpenseID,
            0,
            GETDATE()
        );

        SET @JournalID = SCOPE_IDENTITY();

        INSERT INTO dbo.JournalEntries
        (
            JournalID,
            AccountID,
            Debit,
            Credit
        )
        SELECT
            @JournalID,
            DebitAccountID,
            CAST(ROUND(SUM(LineTotal), 2) AS DECIMAL(12,2)),
            0
        FROM @CleanLines
        GROUP BY DebitAccountID;

        SELECT @DebitTotal = CAST(ROUND(SUM(ISNULL(Debit, 0)), 2) AS DECIMAL(18,2))
        FROM dbo.JournalEntries
        WHERE JournalID = @JournalID;

        INSERT INTO dbo.JournalEntries
        (
            JournalID,
            AccountID,
            Debit,
            Credit
        )
        VALUES
        (
            @JournalID,
            @CashAccountID,
            0,
            CAST(@DebitTotal AS DECIMAL(12,2))
        );

        SELECT
            @DebitTotal = CAST(ROUND(SUM(ISNULL(Debit, 0)), 2) AS DECIMAL(18,2)),
            @CreditTotal = CAST(ROUND(SUM(ISNULL(Credit, 0)), 2) AS DECIMAL(18,2))
        FROM dbo.JournalEntries
        WHERE JournalID = @JournalID;

        IF @DebitTotal <> @CreditTotal
            THROW 50015, N'القيد غير متوازن داخل إجراء حفظ المصروف.', 1;

        UPDATE dbo.Journals
        SET IsPosted = 1,
            PostedAt = GETDATE()
        WHERE JournalID = @JournalID;

        UPDATE dbo.Expenses
        SET JournalID = @JournalID,
            IsPosted = 1,
            TotalAmount = @DebitTotal
        WHERE ExpenseID = @ExpenseID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;

    SELECT
        @ExpenseID AS ExpenseID,
        @ExpenseNumber AS ExpenseNumber,
        @DebitTotal AS TotalAmount;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Expense_Update]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   5) إصلاح تعديل المصروفات
   ========================================================= */

CREATE   PROCEDURE [dbo].[usp_Expense_Update]
    @ExpenseID INT,
    @ExpenseDate DATE,
    @CategoryID INT,
    @SupplierName NVARCHAR(150) = NULL,
    @Description NVARCHAR(250) = NULL,
    @Notes NVARCHAR(250) = NULL,
    @PaymentMethod NVARCHAR(30),
    @CashAccountID INT = NULL,
    @CounterAccountID INT = NULL,
    @Lines dbo.ExpenseLineType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @ExpenseNumber NVARCHAR(30),
        @JournalID INT,
        @DefaultAccountID INT,
        @DebitTotal DECIMAL(18,2),
        @CreditTotal DECIMAL(18,2);

    IF @ExpenseID IS NULL OR @ExpenseID <= 0
        THROW 50101, N'معرف الحركة غير صحيح.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Expenses WHERE ExpenseID = @ExpenseID)
        THROW 50102, N'الحركة غير موجودة.', 1;

    IF dbo.fn_IsPeriodClosed(@ExpenseDate) = 1
        THROW 50106, N'لا يمكن التعديل إلى فترة محاسبية مغلقة.', 1;

    IF @PaymentMethod NOT IN (N'Cash', N'Transfer', N'Cheque', N'Credit')
        THROW 50103, N'طريقة الدفع غير صحيحة.', 1;

    IF NOT EXISTS (SELECT 1 FROM @Lines)
        THROW 50104, N'يجب إدخال بند واحد على الأقل.', 1;

    IF @CashAccountID IS NOT NULL AND @CashAccountID <= 0
        SET @CashAccountID = NULL;

    IF @CounterAccountID IS NOT NULL AND @CounterAccountID <= 0
        SET @CounterAccountID = NULL;

    SELECT
        @ExpenseNumber = ExpenseNumber,
        @JournalID = JournalID
    FROM dbo.Expenses
    WHERE ExpenseID = @ExpenseID;

    SELECT @DefaultAccountID = DefaultAccountID
    FROM dbo.ExpenseCategories
    WHERE CategoryID = @CategoryID
      AND ISNULL(IsActive, 1) = 1;

    IF @DefaultAccountID IS NULL AND @CounterAccountID IS NULL
        THROW 50107, N'لا يوجد حساب افتراضي للتصنيف، ولم يتم اختيار حساب مقابل.', 1;

    IF @CashAccountID IS NULL
    BEGIN
        IF @PaymentMethod = N'Cash'
            SELECT TOP 1 @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1100';

        ELSE IF @PaymentMethod IN (N'Transfer', N'Cheque')
            SELECT TOP 1 @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'1000';

        ELSE IF @PaymentMethod = N'Credit'
            SELECT TOP 1 @CashAccountID = AccountID FROM dbo.Accounts WHERE AccountCode = N'2100';
    END;

    IF @CashAccountID IS NULL
        THROW 50110, N'حساب الدفع غير محدد.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountID = @CashAccountID)
        THROW 50111, N'حساب الدفع غير موجود في دليل الحسابات.', 1;

    DECLARE @CleanLines TABLE
    (
        ItemName NVARCHAR(150),
        Qty DECIMAL(18,2),
        UnitPrice DECIMAL(18,2),
        LineTotal DECIMAL(18,2),
        DebitAccountID INT,
        TargetAccountID INT NULL,
        Notes NVARCHAR(200)
    );

    INSERT INTO @CleanLines
    (
        ItemName,
        Qty,
        UnitPrice,
        LineTotal,
        DebitAccountID,
        TargetAccountID,
        Notes
    )
    SELECT
        ISNULL(NULLIF(LTRIM(RTRIM(L.ItemName)), N''), N'بند'),
        ISNULL(L.Qty, 0),
        ISNULL(L.UnitPrice, 0),
        CAST(ROUND(ISNULL(L.Qty, 0) * ISNULL(L.UnitPrice, 0), 2) AS DECIMAL(18,2)),
        ISNULL(NULLIF(L.TargetAccountID, 0), ISNULL(@CounterAccountID, @DefaultAccountID)),
        NULLIF(L.TargetAccountID, 0),
        L.Notes
    FROM @Lines L
    WHERE ISNULL(L.Qty, 0) > 0
      AND ISNULL(L.UnitPrice, 0) > 0;

    IF NOT EXISTS (SELECT 1 FROM @CleanLines)
        THROW 50112, N'لا توجد بنود صحيحة للحفظ.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @CleanLines L
        WHERE L.DebitAccountID IS NULL
           OR NOT EXISTS
           (
               SELECT 1
               FROM dbo.Accounts A
               WHERE A.AccountID = L.DebitAccountID
           )
    )
        THROW 50113, N'يوجد بند بدون حساب مدين صحيح.', 1;

    BEGIN TRY
        BEGIN TRAN;

        UPDATE dbo.Expenses
        SET
            ExpenseDate = @ExpenseDate,
            CategoryID = @CategoryID,
            SupplierName = @SupplierName,
            Description = @Description,
            Notes = @Notes,
            PaymentMethod = @PaymentMethod,
            CashAccountID = @CashAccountID,
            CounterAccountID = @CounterAccountID,
            IsPosted = 0,
            Status = N'Posted'
        WHERE ExpenseID = @ExpenseID;

        DELETE FROM dbo.ExpenseLines
        WHERE ExpenseID = @ExpenseID;

        INSERT INTO dbo.ExpenseLines
        (
            ExpenseID,
            ItemName,
            Qty,
            UnitPrice,
            TargetAccountID,
            Notes
        )
        SELECT
            @ExpenseID,
            ItemName,
            Qty,
            UnitPrice,
            TargetAccountID,
            Notes
        FROM @CleanLines;

        IF @JournalID IS NULL
        BEGIN
            INSERT INTO dbo.Journals
            (
                JournalDate,
                Description,
                Source,
                SourceID,
                IsPosted,
                CreatedAt
            )
            VALUES
            (
                @ExpenseDate,
                N'قيد حركة مصروف/شراء رقم ' + ISNULL(@ExpenseNumber, N''),
                N'Expense',
                @ExpenseID,
                0,
                GETDATE()
            );

            SET @JournalID = SCOPE_IDENTITY();

            UPDATE dbo.Expenses
            SET JournalID = @JournalID
            WHERE ExpenseID = @ExpenseID;
        END
        ELSE
        BEGIN
            UPDATE dbo.Journals
            SET
                JournalDate = @ExpenseDate,
                Description = N'قيد حركة مصروف/شراء رقم ' + ISNULL(@ExpenseNumber, N''),
                Source = N'Expense',
                SourceID = @ExpenseID,
                IsPosted = 0,
                PostedAt = NULL
            WHERE JournalID = @JournalID;

            DELETE FROM dbo.JournalEntries
            WHERE JournalID = @JournalID;
        END;

        INSERT INTO dbo.JournalEntries
        (
            JournalID,
            AccountID,
            Debit,
            Credit
        )
        SELECT
            @JournalID,
            DebitAccountID,
            CAST(ROUND(SUM(LineTotal), 2) AS DECIMAL(12,2)),
            0
        FROM @CleanLines
        GROUP BY DebitAccountID;

        SELECT @DebitTotal = CAST(ROUND(SUM(ISNULL(Debit, 0)), 2) AS DECIMAL(18,2))
        FROM dbo.JournalEntries
        WHERE JournalID = @JournalID;

        INSERT INTO dbo.JournalEntries
        (
            JournalID,
            AccountID,
            Debit,
            Credit
        )
        VALUES
        (
            @JournalID,
            @CashAccountID,
            0,
            CAST(@DebitTotal AS DECIMAL(12,2))
        );

        SELECT
            @DebitTotal = CAST(ROUND(SUM(ISNULL(Debit, 0)), 2) AS DECIMAL(18,2)),
            @CreditTotal = CAST(ROUND(SUM(ISNULL(Credit, 0)), 2) AS DECIMAL(18,2))
        FROM dbo.JournalEntries
        WHERE JournalID = @JournalID;

        IF @DebitTotal <> @CreditTotal
            THROW 50114, N'القيد غير متوازن داخل إجراء تعديل المصروف.', 1;

        UPDATE dbo.Journals
        SET IsPosted = 1,
            PostedAt = GETDATE()
        WHERE JournalID = @JournalID;

        UPDATE dbo.Expenses
        SET
            JournalID = @JournalID,
            IsPosted = 1,
            TotalAmount = @DebitTotal
        WHERE ExpenseID = @ExpenseID;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;

    SELECT
        @ExpenseID AS ExpenseID,
        @ExpenseNumber AS ExpenseNumber,
        @DebitTotal AS TotalAmount;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_ExpenseCategories_GetAll]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_ExpenseCategories_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CategoryID,
        CategoryName,
        CategoryType,
        DefaultAccountID,
        Notes,
        IsActive
    FROM dbo.ExpenseCategories
    ORDER BY CategoryType, CategoryName;
END

GO
/****** Object:  StoredProcedure [dbo].[usp_ExpenseCategory_Delete]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_ExpenseCategory_Delete]
    @CategoryID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @CategoryID IS NULL OR @CategoryID <= 0
        THROW 50401, N'معرف التصنيف غير صحيح.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.ExpenseCategories WHERE CategoryID = @CategoryID)
        THROW 50402, N'التصنيف غير موجود.', 1;

    IF EXISTS (SELECT 1 FROM dbo.Expenses WHERE CategoryID = @CategoryID)
        THROW 50403, N'لا يمكن حذف التصنيف لأنه مستخدم في حركات مسجلة.', 1;

    DELETE FROM dbo.ExpenseCategories
    WHERE CategoryID = @CategoryID;
END

GO
/****** Object:  StoredProcedure [dbo].[usp_ExpenseCategory_Save]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   12) إدارة التصنيفات
   ========================================================= */

CREATE   PROCEDURE [dbo].[usp_ExpenseCategory_Save]
    @CategoryID INT = NULL,
    @CategoryName NVARCHAR(100),
    @CategoryType NVARCHAR(20),
    @DefaultAccountID INT = NULL,
    @Notes NVARCHAR(200) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SET @CategoryName = LTRIM(RTRIM(@CategoryName));

    IF @CategoryName IS NULL OR @CategoryName = N''
        THROW 50301, N'اسم التصنيف مطلوب.', 1;

    IF @CategoryType NOT IN (N'Expense', N'Purchase', N'Loss')
        THROW 50302, N'نوع التصنيف غير صحيح.', 1;

    IF @DefaultAccountID IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountID = @DefaultAccountID)
        THROW 50303, N'الحساب الافتراضي غير موجود.', 1;

    IF @CategoryID IS NULL OR @CategoryID = 0
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM dbo.ExpenseCategories
            WHERE CategoryName = @CategoryName
              AND CategoryType = @CategoryType
        )
            THROW 50304, N'يوجد تصنيف بنفس الاسم والنوع مسبقًا.', 1;

        INSERT INTO dbo.ExpenseCategories
        (
            CategoryName,
            CategoryType,
            DefaultAccountID,
            Notes,
            IsActive
        )
        VALUES
        (
            @CategoryName,
            @CategoryType,
            @DefaultAccountID,
            @Notes,
            @IsActive
        );

        SELECT SCOPE_IDENTITY() AS CategoryID;
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.ExpenseCategories WHERE CategoryID = @CategoryID)
        THROW 50305, N'التصنيف غير موجود.', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.ExpenseCategories
        WHERE CategoryName = @CategoryName
          AND CategoryType = @CategoryType
          AND CategoryID <> @CategoryID
    )
        THROW 50306, N'يوجد تصنيف آخر بنفس الاسم والنوع.', 1;

    UPDATE dbo.ExpenseCategories
    SET
        CategoryName = @CategoryName,
        CategoryType = @CategoryType,
        DefaultAccountID = @DefaultAccountID,
        Notes = @Notes,
        IsActive = @IsActive
    WHERE CategoryID = @CategoryID;

    SELECT @CategoryID AS CategoryID;
END

GO
/****** Object:  StoredProcedure [dbo].[usp_Expenses_GetAll]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   11) قراءة الحركات والتفاصيل
   ========================================================= */

CREATE   PROCEDURE [dbo].[usp_Expenses_GetAll]
    @FromDate DATE = NULL,
    @ToDate DATE = NULL,
    @CategoryID INT = NULL,
    @CategoryType NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.ExpenseID,
        E.ExpenseNumber,
        E.ExpenseDate,
        C.CategoryName,
        C.CategoryType,
        E.SupplierName,
        E.Description,
        E.TotalAmount,
        E.PaymentMethod,
        E.IsPosted,
        E.Status,
        E.CreatedAt
    FROM dbo.Expenses E
    INNER JOIN dbo.ExpenseCategories C ON E.CategoryID = C.CategoryID
    WHERE (@FromDate IS NULL OR E.ExpenseDate >= @FromDate)
      AND (@ToDate IS NULL OR E.ExpenseDate <= @ToDate)
      AND (@CategoryID IS NULL OR E.CategoryID = @CategoryID)
      AND (@CategoryType IS NULL OR C.CategoryType = @CategoryType)
    ORDER BY E.ExpenseDate DESC, E.ExpenseID DESC;
END

GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_Approve]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_MobileReceipt_Approve]
    @ImportID            INT,
    @ApprovedByUserID    INT = NULL,
    @SendSms             BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @SyncBatchID INT = NULL,
        @CollectorID INT = NULL,
        @DeviceID INT = NULL,
        @SubscriberID INT = NULL,
        @PaymentDate DATE = NULL,
        @TotalReceived DECIMAL(12,2) = 0,
        @PaymentMethod NVARCHAR(30) = NULL,
        @Notes NVARCHAR(200) = NULL,
        @ImportStatus NVARCHAR(20) = NULL,
        @ApprovedReceiptID INT = NULL,

        @ReceiptID INT = NULL,
        @ReceiptNumber NVARCHAR(30) = NULL,
        @JournalID INT = NULL,
        @SubscriberAccountID INT = NULL,
        @CashAccountID INT = NULL,

        @LinesTotal DECIMAL(12,2) = 0,
        @AdvanceCreditTotal DECIMAL(12,2) = 0,
        @PrimaryMeterID INT = NULL,

        @InvoiceID INT = NULL,
        @InvoiceMeterID INT = NULL,
        @InvoiceGrandTotal DECIMAL(12,2) = 0,
        @InvoicePaid DECIMAL(12,2) = 0,
        @InvoiceDue DECIMAL(12,2) = 0,
        @ApplyAmount DECIMAL(12,2) = 0,
        @PaymentID INT = NULL,
        @LastPaymentID INT = NULL,
        @PaidAfter DECIMAL(12,2) = 0,

        @SmsPaymentID INT = NULL,
        @SmsPhoneNumber NVARCHAR(50) = NULL,
        @SmsMessage NVARCHAR(MAX) = NULL,

        @ErrorMessage NVARCHAR(4000) = NULL;

    BEGIN TRY
        BEGIN TRAN;

        IF @ApprovedByUserID IS NOT NULL
           AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserID = @ApprovedByUserID)
            THROW 62001, N'المستخدم المعتمد غير موجود.', 1;

        SELECT
            @SyncBatchID = I.SyncBatchID,
            @CollectorID = I.CollectorID,
            @DeviceID = I.DeviceID,
            @SubscriberID = I.SubscriberID,
            @PaymentDate = I.PaymentDate,
            @TotalReceived = I.TotalReceived,
            @PaymentMethod = I.PaymentMethod,
            @Notes = I.Notes,
            @ImportStatus = I.ImportStatus,
            @ApprovedReceiptID = I.ApprovedReceiptID
        FROM dbo.MobileReceiptImports I WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
        WHERE I.ImportID = @ImportID;

        IF @SubscriberID IS NULL
            THROW 62002, N'سجل التحصيل المستورد غير موجود.', 1;

        IF @ImportStatus = N'Approved' AND @ApprovedReceiptID IS NOT NULL
        BEGIN
            SELECT
                I.ImportID,
                I.ImportStatus,
                I.ApprovedReceiptID AS ReceiptID,
                R.ReceiptNumber,
                R.TotalAmount,
                R.PaymentDate,
                N'AlreadyApproved' AS ResultStatus
            FROM dbo.MobileReceiptImports I
            INNER JOIN dbo.Receipts R ON R.ReceiptID = I.ApprovedReceiptID
            WHERE I.ImportID = @ImportID;

            COMMIT;
            RETURN;
        END

        IF @ImportStatus = N'Duplicate'
            THROW 62003, N'هذا السجل مصنف كمكرر ولا يمكن اعتماده.', 1;

        IF @ImportStatus = N'Rejected'
            THROW 62004, N'هذا السجل مرفوض مسبقًا. أعد ضبط حالته إلى New إن أردت إعادة المحاولة.', 1;

        IF @ImportStatus <> N'New'
            THROW 62005, N'حالة السجل الحالية لا تسمح بالاعتماد.', 1;

        IF dbo.fn_IsPeriodClosed(@PaymentDate) = 1
            THROW 62006, N'الفترة المحاسبية مغلقة.', 1;

        IF @TotalReceived IS NULL OR @TotalReceived <= 0
            THROW 62007, N'إجمالي المبلغ المستلم غير صحيح.', 1;

        IF @PaymentMethod NOT IN (N'Cash', N'Transfer', N'Cheque', N'Wallet', N'Card', N'Bank', N'Other')
            THROW 62008, N'طريقة الدفع غير صحيحة.', 1;

        IF NOT EXISTS (SELECT 1 FROM dbo.MobileReceiptImportLines WHERE ImportID = @ImportID)
            THROW 62009, N'لا توجد تفاصيل توزيع لهذا التحصيل المستورد.', 1;

        IF OBJECT_ID('tempdb..#AggLines') IS NOT NULL DROP TABLE #AggLines;

        SELECT
            InvoiceID,
            ApplicationType,
            SUM(AppliedAmount) AS AppliedAmount,
            MAX(Notes) AS Notes
        INTO #AggLines
        FROM dbo.MobileReceiptImportLines
        WHERE ImportID = @ImportID
        GROUP BY InvoiceID, ApplicationType;

        IF EXISTS
        (
            SELECT 1
            FROM #AggLines
            WHERE AppliedAmount <= 0
               OR ApplicationType NOT IN (N'InvoicePayment', N'AdvanceCredit')
        )
            THROW 62010, N'يوجد سطر توزيع غير صالح.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM #AggLines
            WHERE ApplicationType = N'InvoicePayment'
              AND InvoiceID IS NULL
        )
            THROW 62011, N'سطر InvoicePayment يجب أن يحتوي على InvoiceID.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM #AggLines
            WHERE ApplicationType = N'AdvanceCredit'
              AND InvoiceID IS NOT NULL
        )
            THROW 62012, N'سطر AdvanceCredit يجب أن يكون InvoiceID فيه فارغًا.', 1;

        SELECT @LinesTotal = ISNULL(SUM(AppliedAmount), 0)
        FROM #AggLines;

        IF ROUND(@LinesTotal - @TotalReceived, 2) <> 0
            THROW 62013, N'مجموع تفاصيل التوزيع لا يساوي إجمالي المبلغ المستلم.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM #AggLines L
            LEFT JOIN dbo.Invoices I ON I.InvoiceID = L.InvoiceID
            WHERE L.ApplicationType = N'InvoicePayment'
              AND (
                    I.InvoiceID IS NULL
                 OR I.SubscriberID <> @SubscriberID
                 OR ISNULL(I.Status, N'') = N'ملغاة'
              )
        )
            THROW 62014, N'هناك فاتورة غير موجودة أو لا تخص هذا المشترك أو ملغاة.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM #AggLines L
            CROSS APPLY
            (
                SELECT
                    CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(12,2))
                    - CAST(ISNULL(PA.Paid,0) AS DECIMAL(12,2)) AS CurrentDue
                FROM dbo.Invoices I
                OUTER APPLY
                (
                    SELECT SUM(P.Amount) AS Paid
                    FROM dbo.Payments P
                    WHERE P.InvoiceID = I.InvoiceID
                ) PA
                WHERE I.InvoiceID = L.InvoiceID
            ) X
            WHERE L.ApplicationType = N'InvoicePayment'
              AND L.AppliedAmount > X.CurrentDue
        )
            THROW 62015, N'أحد مبالغ التوزيع أكبر من الرصيد الحالي للفواتير عند الاعتماد.', 1;

        SELECT @SubscriberAccountID = S.AccountID
        FROM dbo.Subscribers S
        WHERE S.SubscriberID = @SubscriberID;

        IF @SubscriberAccountID IS NULL
            THROW 62016, N'المشترك لا يملك حسابًا محاسبيًا.', 1;

        SELECT TOP (1) @CashAccountID = V.CashAccountID
        FROM dbo.vw_SystemAccountsUnified V;

        IF @CashAccountID IS NULL
        BEGIN
            SELECT @CashAccountID = A.AccountID
            FROM dbo.Accounts A
            WHERE A.AccountCode = N'1100';
        END

        IF @CashAccountID IS NULL
            THROW 62017, N'حساب الصندوق غير موجود.', 1;

        EXEC dbo.CreateReceipt
            @SubscriberID = @SubscriberID,
            @CollectorID = @CollectorID,
            @PaymentDate = @PaymentDate,
            @TotalAmount = @TotalReceived,
            @PaymentMethod = @PaymentMethod,
            @Notes = @Notes,
            @ReceiptID = @ReceiptID OUTPUT,
            @ReceiptNumber = @ReceiptNumber OUTPUT;

        INSERT INTO dbo.Journals
        (
            JournalDate,
            Description,
            Source,
            SourceID,
            IsPosted,
            CreatedAt
        )
        VALUES
        (
            @PaymentDate,
            N'إيصال قبض جوال رقم ' + @ReceiptNumber,
            N'Receipt',
            @ReceiptID,
            0,
            GETDATE()
        );

        SET @JournalID = SCOPE_IDENTITY();

        INSERT INTO dbo.JournalEntries
        (
            JournalID,
            AccountID,
            Debit,
            Credit
        )
        VALUES
            (@JournalID, @CashAccountID, @TotalReceived, 0),
            (@JournalID, @SubscriberAccountID, 0, @TotalReceived);

        IF ROUND(
            (
                SELECT SUM(ISNULL(Debit,0)) - SUM(ISNULL(Credit,0))
                FROM dbo.JournalEntries
                WHERE JournalID = @JournalID
            ), 2
        ) <> 0
            THROW 62018, N'قيد الإيصال غير متوازن.', 1;

        UPDATE dbo.Journals
        SET IsPosted = 1,
            PostedAt = GETDATE()
        WHERE JournalID = @JournalID;

        UPDATE dbo.Receipts
        SET JournalID = @JournalID
        WHERE ReceiptID = @ReceiptID;

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT InvoiceID, AppliedAmount
            FROM #AggLines
            WHERE ApplicationType = N'InvoicePayment'
            ORDER BY InvoiceID;

        OPEN cur;
        FETCH NEXT FROM cur INTO @InvoiceID, @ApplyAmount;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SELECT
                @InvoiceMeterID = I.MeterID,
                @InvoiceGrandTotal = CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(12,2))
            FROM dbo.Invoices I WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
            WHERE I.InvoiceID = @InvoiceID;

            SELECT
                @InvoicePaid = CAST(ISNULL(SUM(P.Amount),0) AS DECIMAL(12,2))
            FROM dbo.Payments P
            WHERE P.InvoiceID = @InvoiceID;

            SET @InvoiceDue = @InvoiceGrandTotal - ISNULL(@InvoicePaid, 0);
            IF @InvoiceDue < 0 SET @InvoiceDue = 0;

            IF @ApplyAmount > @InvoiceDue
                THROW 62019, N'الرصيد الحالي لإحدى الفواتير تغير وأصبح أقل من المبلغ المطلوب تطبيقه.', 1;

            IF @InvoiceMeterID IS NULL
            BEGIN
                SELECT TOP (1) @InvoiceMeterID = SM.MeterID
                FROM dbo.SubscriberMeters SM
                WHERE SM.SubscriberID = @SubscriberID
                ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC;
            END

            IF @InvoiceMeterID IS NULL
                THROW 62020, N'لا يوجد عداد مرتبط بالمشترك.', 1;

            INSERT INTO dbo.Payments
            (
                InvoiceID,
                SubscriberID,
                CollectorID,
                PaymentDate,
                Amount,
                PaymentType,
                Notes,
                ReceiptNumber,
                PaymentCategory,
                ReceiptID
            )
            VALUES
            (
                @InvoiceID,
                @SubscriberID,
                @CollectorID,
                @PaymentDate,
                @ApplyAmount,
                @PaymentMethod,
                @Notes,
                @ReceiptNumber,
                N'NormalPayment',
                @ReceiptID
            );

            SET @PaymentID = SCOPE_IDENTITY();
            SET @LastPaymentID = @PaymentID;

            INSERT INTO dbo.ReceiptApplications
            (
                ReceiptID,
                InvoiceID,
                CreditID,
                AppliedAmount,
                ApplicationType,
                PaymentID,
                CreatedAt
            )
            VALUES
            (
                @ReceiptID,
                @InvoiceID,
                NULL,
                @ApplyAmount,
                N'InvoicePayment',
                @PaymentID,
                GETDATE()
            );

            INSERT INTO dbo.AccountStatements
            (
                SubscriberID,
                InvoiceID,
                PaymentID,
                MeterID,
                [Date],
                Details,
                Item,
                DocumentType,
                DocumentNumber,
                Debit,
                Credit,
                BalanceAfter,
                JournalID
            )
            VALUES
            (
                @SubscriberID,
                @InvoiceID,
                @PaymentID,
                @InvoiceMeterID,
                @PaymentDate,
                N'سداد من تحصيل جوال',
                NULL,
                N'Receipt',
                @ReceiptNumber,
                0,
                @ApplyAmount,
                NULL,
                @JournalID
            );

            SELECT
                @PaidAfter = CAST(ISNULL(SUM(P.Amount),0) AS DECIMAL(12,2))
            FROM dbo.Payments P
            WHERE P.InvoiceID = @InvoiceID;

            UPDATE dbo.Invoices
            SET Status =
                CASE
                    WHEN @PaidAfter >= @InvoiceGrandTotal THEN N'مدفوعة'
                    WHEN @PaidAfter > 0 THEN N'مدفوعة جزئياً'
                    ELSE N'غير مدفوعة'
                END
            WHERE InvoiceID = @InvoiceID;

            FETCH NEXT FROM cur INTO @InvoiceID, @ApplyAmount;
        END

        CLOSE cur;
        DEALLOCATE cur;

        SELECT
            @AdvanceCreditTotal = ISNULL(SUM(AppliedAmount), 0)
        FROM #AggLines
        WHERE ApplicationType = N'AdvanceCredit';

        IF @AdvanceCreditTotal > 0
        BEGIN
            SELECT TOP (1) @PrimaryMeterID = SM.MeterID
            FROM dbo.SubscriberMeters SM
            WHERE SM.SubscriberID = @SubscriberID
            ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC;

            IF @PrimaryMeterID IS NULL
                THROW 62021, N'لا يوجد عداد أساسي أو مرتبط بالمشترك لتسجيل الرصيد المقدم.', 1;

            INSERT INTO dbo.Payments
            (
                InvoiceID,
                SubscriberID,
                CollectorID,
                PaymentDate,
                Amount,
                PaymentType,
                Notes,
                ReceiptNumber,
                PaymentCategory,
                ReceiptID
            )
            VALUES
            (
                NULL,
                @SubscriberID,
                @CollectorID,
                @PaymentDate,
                @AdvanceCreditTotal,
                @PaymentMethod,
                ISNULL(@Notes, N'') + N' (رصيد مقدم من تحصيل جوال)',
                @ReceiptNumber,
                N'AdvanceCredit',
                @ReceiptID
            );

            SET @PaymentID = SCOPE_IDENTITY();
            SET @LastPaymentID = @PaymentID;

            INSERT INTO dbo.ReceiptApplications
            (
                ReceiptID,
                InvoiceID,
                CreditID,
                AppliedAmount,
                ApplicationType,
                PaymentID,
                CreatedAt
            )
            VALUES
            (
                @ReceiptID,
                NULL,
                NULL,
                @AdvanceCreditTotal,
                N'AdvanceCredit',
                @PaymentID,
                GETDATE()
            );

            INSERT INTO dbo.SubscriberCredits
            (
                SubscriberID,
                PaymentID,
                MeterID,
                CreditDate,
                AmountTotal,
                AmountRemaining,
                Notes,
                ReceiptID
            )
            VALUES
            (
                @SubscriberID,
                @PaymentID,
                @PrimaryMeterID,
                @PaymentDate,
                @AdvanceCreditTotal,
                @AdvanceCreditTotal,
                ISNULL(@Notes, N'') + N' (رصيد مقدم من تحصيل جوال)',
                @ReceiptID
            );

            INSERT INTO dbo.AccountStatements
            (
                SubscriberID,
                InvoiceID,
                PaymentID,
                MeterID,
                [Date],
                Details,
                Item,
                DocumentType,
                DocumentNumber,
                Debit,
                Credit,
                BalanceAfter,
                JournalID
            )
            VALUES
            (
                @SubscriberID,
                NULL,
                @PaymentID,
                @PrimaryMeterID,
                @PaymentDate,
                N'رصيد مقدم من تحصيل جوال',
                NULL,
                N'Receipt',
                @ReceiptNumber,
                0,
                @AdvanceCreditTotal,
                NULL,
                @JournalID
            );
        END

        UPDATE dbo.MobileReceiptImports
        SET
            ImportStatus = N'Approved',
            ApprovedReceiptID = @ReceiptID,
            ApprovedByUserID = @ApprovedByUserID,
            ApprovedAt = GETDATE(),
            RejectedReason = NULL
        WHERE ImportID = @ImportID;

        COMMIT;

        /* =====================================================
           تجهيز سجل SmsLogs حتى يظهر السند في InvoicePrintPreviewForm
           ===================================================== */
        BEGIN TRY
            IF ISNULL(@SendSms, 0) = 1
            BEGIN
                SELECT TOP (1)
                    @SmsPaymentID = P.PaymentID
                FROM dbo.Payments P
                WHERE P.ReceiptID = @ReceiptID
                ORDER BY P.PaymentID DESC;

                SET @SmsPaymentID = ISNULL(@LastPaymentID, @SmsPaymentID);

                IF @SmsPaymentID IS NOT NULL
                BEGIN
                    BEGIN TRY
                        EXEC dbo.SendSMSDynamic
                            @EntityType = N'Payment',
                            @EntityID = @SmsPaymentID,
                            @CustomMessage = NULL,
                            @Language = N'AR',
                            @ForceTemplateID = NULL;
                    END TRY
                    BEGIN CATCH
                    END CATCH;
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.SmsLogs L
                    WHERE L.MessageType = N'Payment'
                      AND (
                            L.ReceiptID = @ReceiptID
                         OR L.PaymentID = @SmsPaymentID
                      )
                )
                BEGIN
                    SELECT
                        @SmsPhoneNumber = S.PhoneNumber
                    FROM dbo.Receipts R
                    INNER JOIN dbo.Subscribers S
                        ON S.SubscriberID = R.SubscriberID
                    WHERE R.ReceiptID = @ReceiptID;

                    SET @SmsMessage =
                        N'تم استلام مبلغ ' +
                        CONVERT(NVARCHAR(30), CAST(@TotalReceived AS DECIMAL(12,2))) +
                        N' ريال. رقم السند: ' +
                        ISNULL(@ReceiptNumber, N'');

                    INSERT INTO dbo.SmsLogs
                    (
                        InvoiceID,
                        SubscriberID,
                        PhoneNumber,
                        Message,
                        Status,
                        Reason,
                        SentDate,
                        CollectorID,
                        TemplateID,
                        RetryCount,
                        CreatedAt,
                        MessageType,
                        PaymentID,
                        ReceiptID
                    )
                    VALUES
                    (
                        NULL,
                        @SubscriberID,
                        @SmsPhoneNumber,
                        @SmsMessage,
                        CASE
                            WHEN NULLIF(LTRIM(RTRIM(ISNULL(@SmsPhoneNumber, N''))), N'') IS NULL
                                THEN N'Failed'
                            ELSE N'Pending'
                        END,
                        CASE
                            WHEN NULLIF(LTRIM(RTRIM(ISNULL(@SmsPhoneNumber, N''))), N'') IS NULL
                                THEN N'لا يوجد رقم هاتف للمشترك'
                            ELSE NULL
                        END,
                        NULL,
                        @CollectorID,
                        NULL,
                        0,
                        GETDATE(),
                        N'Payment',
                        @SmsPaymentID,
                        @ReceiptID
                    );
                END
            END
        END TRY
        BEGIN CATCH
            -- لا نجعل فشل تجهيز سجل SmsLogs يلغي اعتماد السند
        END CATCH;

        SELECT
            @ImportID AS ImportID,
            @ReceiptID AS ReceiptID,
            @ReceiptNumber AS ReceiptNumber,
            @TotalReceived AS TotalReceived,
            @LastPaymentID AS LastPaymentID,
            N'Approved' AS ResultStatus;
    END TRY
    BEGIN CATCH
        SET @ErrorMessage = ERROR_MESSAGE();

        IF CURSOR_STATUS('local', 'cur') >= -1
        BEGIN
            BEGIN TRY
                CLOSE cur;
                DEALLOCATE cur;
            END TRY
            BEGIN CATCH
            END CATCH
        END

        IF @@TRANCOUNT > 0
            ROLLBACK;

        UPDATE dbo.MobileReceiptImports
        SET
            ImportStatus = CASE WHEN ApprovedReceiptID IS NULL THEN N'Rejected' ELSE ImportStatus END,
            RejectedReason = LEFT(@ErrorMessage, 200)
        WHERE ImportID = @ImportID
          AND ISNULL(ImportStatus, N'') <> N'Approved';

        INSERT INTO dbo.MobileSyncErrors
        (
            SyncBatchID,
            ImportID,
            ErrorMessage,
            ErrorSource,
            CreatedAt
        )
        VALUES
        (
            @SyncBatchID,
            @ImportID,
            LEFT(@ErrorMessage, 500),
            N'usp_MobileReceipt_Approve',
            GETDATE()
        );

        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_ApproveBatch]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileReceipt_ApproveBatch]
    @SyncBatchID        INT = NULL,          -- لو تريد اعتماد دفعة مزامنة محددة
    @CollectorID        INT = NULL,          -- أو حسب محصل
    @DateFrom           DATE = NULL,         -- أو حسب فترة
    @DateTo             DATE = NULL,
    @LimitCount         INT = NULL,          -- حد أقصى لعدد السجلات في التشغيل الواحد
    @ApprovedByUserID   INT = NULL,
    @SendSms            BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT OFF; -- مهم لأننا نريد الاستمرار إذا فشل سجل واحد

    DECLARE
        @CurrentImportID     INT,
        @CurrentBatchID      INT,
        @ReceiptID           INT,
        @ReceiptNumber       NVARCHAR(30),
        @ImportStatus        NVARCHAR(20),
        @ErrMsg              NVARCHAR(4000),

        @TotalSelected       INT = 0,
        @ApprovedCount       INT = 0,
        @FailedCount         INT = 0,
        @AlreadyApprovedCount INT = 0;

    IF @ApprovedByUserID IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserID = @ApprovedByUserID)
        THROW 65001, N'المستخدم المعتمد غير موجود.', 1;

    IF @LimitCount IS NOT NULL AND @LimitCount <= 0
        THROW 65002, N'قيمة @LimitCount يجب أن تكون أكبر من صفر.', 1;

    IF OBJECT_ID('tempdb..#ToProcess') IS NOT NULL DROP TABLE #ToProcess;
    IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results;
    IF OBJECT_ID('tempdb..#TouchedBatches') IS NOT NULL DROP TABLE #TouchedBatches;

    CREATE TABLE #ToProcess
    (
        RowNo        INT IDENTITY(1,1) PRIMARY KEY,
        ImportID     INT NOT NULL,
        SyncBatchID  INT NOT NULL
    );

    CREATE TABLE #Results
    (
        ResultRowID      INT IDENTITY(1,1) PRIMARY KEY,
        ImportID         INT NOT NULL,
        SyncBatchID      INT NOT NULL,
        ResultStatus     NVARCHAR(30) NOT NULL,  -- Approved / Failed / AlreadyApproved
        ReceiptID        INT NULL,
        ReceiptNumber    NVARCHAR(30) NULL,
        ErrorMessage     NVARCHAR(4000) NULL,
        ProcessedAt      DATETIME NOT NULL DEFAULT GETDATE()
    );

    CREATE TABLE #TouchedBatches
    (
        SyncBatchID INT PRIMARY KEY
    );

    INSERT INTO #ToProcess (ImportID, SyncBatchID)
    SELECT TOP (ISNULL(@LimitCount, 2147483647))
        I.ImportID,
        I.SyncBatchID
    FROM dbo.MobileReceiptImports I
    WHERE I.ImportStatus = N'New'
      AND (@SyncBatchID IS NULL OR I.SyncBatchID = @SyncBatchID)
      AND (@CollectorID IS NULL OR I.CollectorID = @CollectorID)
      AND (@DateFrom IS NULL OR I.PaymentDate >= @DateFrom)
      AND (@DateTo IS NULL OR I.PaymentDate <= @DateTo)
    ORDER BY I.PaymentDate, I.ImportID;

    SELECT @TotalSelected = COUNT(*) FROM #ToProcess;

    IF @TotalSelected = 0
    BEGIN
        SELECT
            0 AS TotalSelected,
            0 AS ApprovedCount,
            0 AS FailedCount,
            0 AS AlreadyApprovedCount,
            N'NoData' AS BatchResultStatus;

        SELECT
            CAST(NULL AS INT) AS ImportID,
            CAST(NULL AS INT) AS SyncBatchID,
            CAST(NULL AS NVARCHAR(30)) AS ResultStatus,
            CAST(NULL AS INT) AS ReceiptID,
            CAST(NULL AS NVARCHAR(30)) AS ReceiptNumber,
            CAST(NULL AS NVARCHAR(4000)) AS ErrorMessage,
            CAST(NULL AS DATETIME) AS ProcessedAt
        WHERE 1 = 0;

        RETURN;
    END

    INSERT INTO #TouchedBatches (SyncBatchID)
    SELECT DISTINCT SyncBatchID
    FROM #ToProcess;

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT ImportID, SyncBatchID
        FROM #ToProcess
        ORDER BY RowNo;

    OPEN cur;
    FETCH NEXT FROM cur INTO @CurrentImportID, @CurrentBatchID;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            -- استدعاء الإجراء السابق بدون إظهار result set داخلي
            EXEC dbo.usp_MobileReceipt_Approve
                @ImportID = @CurrentImportID,
                @ApprovedByUserID = @ApprovedByUserID,
                @SendSms = @SendSms
            WITH RESULT SETS NONE;

            SELECT
                @ImportStatus = I.ImportStatus,
                @ReceiptID = I.ApprovedReceiptID
            FROM dbo.MobileReceiptImports I
            WHERE I.ImportID = @CurrentImportID;

            SELECT
                @ReceiptNumber = R.ReceiptNumber
            FROM dbo.Receipts R
            WHERE R.ReceiptID = @ReceiptID;

            IF @ImportStatus = N'Approved' AND @ReceiptID IS NOT NULL
            BEGIN
                INSERT INTO #Results
                (
                    ImportID, SyncBatchID, ResultStatus, ReceiptID, ReceiptNumber, ErrorMessage
                )
                VALUES
                (
                    @CurrentImportID, @CurrentBatchID, N'Approved', @ReceiptID, @ReceiptNumber, NULL
                );

                SET @ApprovedCount += 1;
            END
            ELSE
            BEGIN
                INSERT INTO #Results
                (
                    ImportID, SyncBatchID, ResultStatus, ReceiptID, ReceiptNumber, ErrorMessage
                )
                VALUES
                (
                    @CurrentImportID, @CurrentBatchID, N'Failed', NULL, NULL,
                    N'لم يتم اعتماد السجل رغم عدم ظهور خطأ صريح.'
                );

                SET @FailedCount += 1;
            END
        END TRY
        BEGIN CATCH
            SET @ErrMsg = ERROR_MESSAGE();

            -- لو كان الإجراء رجّع "AlreadyApproved" مستقبلًا أو حصل تكرار منطقي
            IF EXISTS
            (
                SELECT 1
                FROM dbo.MobileReceiptImports I
                WHERE I.ImportID = @CurrentImportID
                  AND I.ImportStatus = N'Approved'
                  AND I.ApprovedReceiptID IS NOT NULL
            )
            BEGIN
                SELECT
                    @ReceiptID = I.ApprovedReceiptID
                FROM dbo.MobileReceiptImports I
                WHERE I.ImportID = @CurrentImportID;

                SELECT
                    @ReceiptNumber = R.ReceiptNumber
                FROM dbo.Receipts R
                WHERE R.ReceiptID = @ReceiptID;

                INSERT INTO #Results
                (
                    ImportID, SyncBatchID, ResultStatus, ReceiptID, ReceiptNumber, ErrorMessage
                )
                VALUES
                (
                    @CurrentImportID, @CurrentBatchID, N'AlreadyApproved', @ReceiptID, @ReceiptNumber, NULL
                );

                SET @AlreadyApprovedCount += 1;
            END
            ELSE
            BEGIN
                INSERT INTO #Results
                (
                    ImportID, SyncBatchID, ResultStatus, ReceiptID, ReceiptNumber, ErrorMessage
                )
                VALUES
                (
                    @CurrentImportID, @CurrentBatchID, N'Failed', NULL, NULL, @ErrMsg
                );

                SET @FailedCount += 1;
            END
        END CATCH;

        FETCH NEXT FROM cur INTO @CurrentImportID, @CurrentBatchID;
    END

    CLOSE cur;
    DEALLOCATE cur;

    /* =========================================================
       تحديث حالة دفعات المزامنة التي شملها التشغيل
       ========================================================= */
    ;WITH BatchAgg AS
    (
        SELECT
            I.SyncBatchID,
            SUM(CASE WHEN I.ImportStatus = N'New' THEN 1 ELSE 0 END) AS NewCount,
            SUM(CASE WHEN I.ImportStatus = N'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
            SUM(CASE WHEN I.ImportStatus = N'Rejected' THEN 1 ELSE 0 END) AS RejectedCount
        FROM dbo.MobileReceiptImports I
        INNER JOIN #TouchedBatches B ON B.SyncBatchID = I.SyncBatchID
        GROUP BY I.SyncBatchID
    )
    UPDATE B
    SET
        SyncStatus =
            CASE
                WHEN A.NewCount > 0 THEN N'Pending'
                WHEN A.ApprovedCount > 0 AND A.RejectedCount = 0 THEN N'Imported'
                WHEN A.ApprovedCount = 0 AND A.RejectedCount > 0 THEN N'Rejected'
                WHEN A.ApprovedCount > 0 AND A.RejectedCount > 0 THEN N'Partial'
                ELSE B.SyncStatus
            END,
        FinishedAt = GETDATE(),
        Notes =
            N'Approved=' + CAST(A.ApprovedCount AS NVARCHAR(20))
            + N', Rejected=' + CAST(A.RejectedCount AS NVARCHAR(20))
            + N', New=' + CAST(A.NewCount AS NVARCHAR(20))
    FROM dbo.MobileSyncBatches B
    INNER JOIN BatchAgg A ON A.SyncBatchID = B.SyncBatchID;

    /* =========================================================
       النتيجة النهائية
       ========================================================= */
    SELECT
        @TotalSelected AS TotalSelected,
        @ApprovedCount AS ApprovedCount,
        @FailedCount AS FailedCount,
        @AlreadyApprovedCount AS AlreadyApprovedCount,
        CASE
            WHEN @ApprovedCount > 0 AND @FailedCount = 0 THEN N'Completed'
            WHEN @ApprovedCount > 0 AND @FailedCount > 0 THEN N'Partial'
            WHEN @ApprovedCount = 0 AND @FailedCount > 0 THEN N'Failed'
            ELSE N'Unknown'
        END AS BatchResultStatus;

    SELECT
        ImportID,
        SyncBatchID,
        ResultStatus,
        ReceiptID,
        ReceiptNumber,
        ErrorMessage,
        ProcessedAt
    FROM #Results
    ORDER BY ResultRowID;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_GetDetails]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileReceipt_GetDetails]
    @ImportID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Header
    SELECT
        I.ImportID,
        I.SyncBatchID,
        I.ImportStatus,
        I.LocalReceiptNo,
        I.LocalPaymentGuid,
        I.PaymentDate,
        I.TotalReceived,
        I.PaymentMethod,
        I.Notes,
        I.RejectedReason,
        I.CreatedAt,

        I.SubscriberID,
        S.Name AS SubscriberName,
        S.PhoneNumber,
        S.Address,

        I.CollectorID,
        C.Name AS CollectorName,

        I.DeviceID,
        D.DeviceCode,
        D.DeviceName,
        D.DeviceModel,

        I.ApprovedReceiptID,
        R.ReceiptNumber,
        I.ApprovedAt,
        I.ApprovedByUserID,
        U.FullName AS ApprovedByUserName
    FROM dbo.MobileReceiptImports I
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
    INNER JOIN dbo.Collectors C ON C.CollectorID = I.CollectorID
    INNER JOIN dbo.CollectorDevices D ON D.DeviceID = I.DeviceID
    LEFT JOIN dbo.Receipts R ON R.ReceiptID = I.ApprovedReceiptID
    LEFT JOIN dbo.Users U ON U.UserID = I.ApprovedByUserID
    WHERE I.ImportID = @ImportID;

    -- Lines
    SELECT
        L.ImportLineID,
        L.ImportID,
        L.InvoiceID,
        ISNULL(I.InvoiceNumber, CAST(L.InvoiceID AS NVARCHAR(30))) AS InvoiceNumber,
        I.InvoiceDate,
        I.TotalAmount,
        I.Arrears,
        I.Status AS InvoiceStatus,
        L.AppliedAmount,
        L.ApplicationType,
        L.Notes,
        L.CreatedAt
    FROM dbo.MobileReceiptImportLines L
    LEFT JOIN dbo.Invoices I ON I.InvoiceID = L.InvoiceID
    WHERE L.ImportID = @ImportID
    ORDER BY L.ImportLineID;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_GetQueue]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileReceipt_GetQueue]
    @ImportStatus   NVARCHAR(20) = NULL,   -- New / Approved / Rejected
    @CollectorID    INT = NULL,
    @SubscriberID   INT = NULL,
    @DateFrom       DATE = NULL,
    @DateTo         DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        I.ImportID,
        I.SyncBatchID,
        I.ImportStatus,
        I.LocalReceiptNo,
        I.LocalPaymentGuid,
        I.PaymentDate,
        I.TotalReceived,
        I.PaymentMethod,
        I.Notes,

        I.SubscriberID,
        S.Name AS SubscriberName,
        S.PhoneNumber,
        S.Address,

        I.CollectorID,
        C.Name AS CollectorName,

        I.DeviceID,
        D.DeviceCode,
        D.DeviceName,
        D.DeviceModel,

        I.ApprovedReceiptID,
        R.ReceiptNumber,
        I.ApprovedAt,
        I.ApprovedByUserID,
        U.FullName AS ApprovedByUserName,

        I.RejectedReason,
        I.CreatedAt
    FROM dbo.MobileReceiptImports I
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
    INNER JOIN dbo.Collectors C ON C.CollectorID = I.CollectorID
    INNER JOIN dbo.CollectorDevices D ON D.DeviceID = I.DeviceID
    LEFT JOIN dbo.Receipts R ON R.ReceiptID = I.ApprovedReceiptID
    LEFT JOIN dbo.Users U ON U.UserID = I.ApprovedByUserID
    WHERE
        (@ImportStatus IS NULL OR I.ImportStatus = @ImportStatus)
        AND (@CollectorID IS NULL OR I.CollectorID = @CollectorID)
        AND (@SubscriberID IS NULL OR I.SubscriberID = @SubscriberID)
        AND (@DateFrom IS NULL OR I.PaymentDate >= @DateFrom)
        AND (@DateTo IS NULL OR I.PaymentDate <= @DateTo)
    ORDER BY
        CASE WHEN I.ImportStatus = N'New' THEN 0 ELSE 1 END,
        I.PaymentDate DESC,
        I.ImportID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_GetSummaryCards]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileReceipt_GetSummaryCards]
    @CollectorID    INT = NULL,
    @SyncBatchID    INT = NULL,
    @DateFrom       DATE = NULL,
    @DateTo         DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH F AS
    (
        SELECT
            I.ImportID,
            I.SyncBatchID,
            I.ImportStatus,
            I.TotalReceived,
            I.CollectorID,
            I.PaymentDate
        FROM dbo.MobileReceiptImports I
        WHERE
            (@CollectorID IS NULL OR I.CollectorID = @CollectorID)
            AND (@SyncBatchID IS NULL OR I.SyncBatchID = @SyncBatchID)
            AND (@DateFrom IS NULL OR I.PaymentDate >= @DateFrom)
            AND (@DateTo IS NULL OR I.PaymentDate <= @DateTo)
    ),
    E AS
    (
        SELECT
            COUNT(*) AS ErrorCount
        FROM dbo.MobileSyncErrors X
        WHERE
            (@SyncBatchID IS NULL OR X.SyncBatchID = @SyncBatchID)
            AND (
                @DateFrom IS NULL OR CAST(X.CreatedAt AS DATE) >= @DateFrom
            )
            AND (
                @DateTo IS NULL OR CAST(X.CreatedAt AS DATE) <= @DateTo
            )
    )
    SELECT
        COUNT(*) AS TotalImports,
        COUNT(DISTINCT SyncBatchID) AS TotalBatches,

        SUM(CASE WHEN ImportStatus = N'New' THEN 1 ELSE 0 END) AS NewCount,
        SUM(CASE WHEN ImportStatus = N'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
        SUM(CASE WHEN ImportStatus = N'Rejected' THEN 1 ELSE 0 END) AS RejectedCount,

        CAST(ISNULL(SUM(TotalReceived), 0) AS DECIMAL(18,2)) AS TotalReceivedAmount,
        CAST(ISNULL(SUM(CASE WHEN ImportStatus = N'Approved' THEN TotalReceived ELSE 0 END), 0) AS DECIMAL(18,2)) AS ApprovedAmount,
        CAST(ISNULL(SUM(CASE WHEN ImportStatus = N'Rejected' THEN TotalReceived ELSE 0 END), 0) AS DECIMAL(18,2)) AS RejectedAmount,
        CAST(ISNULL(SUM(CASE WHEN ImportStatus = N'New' THEN TotalReceived ELSE 0 END), 0) AS DECIMAL(18,2)) AS PendingAmount,

        ISNULL((SELECT ErrorCount FROM E), 0) AS ErrorCount
    FROM F;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_Reject]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileReceipt_Reject]
    @ImportID            INT,
    @RejectedReason      NVARCHAR(200),
    @RejectedByUserID    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @SyncBatchID        INT = NULL,
        @ImportStatus       NVARCHAR(20) = NULL,
        @ApprovedReceiptID  INT = NULL,
        @FinalReason        NVARCHAR(200) = NULL;

    SET @FinalReason = NULLIF(LTRIM(RTRIM(@RejectedReason)), N'');

    IF @FinalReason IS NULL
        THROW 63001, N'سبب الرفض مطلوب.', 1;

    IF @RejectedByUserID IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserID = @RejectedByUserID)
        THROW 63002, N'المستخدم المحدد غير موجود.', 1;

    BEGIN TRY
        BEGIN TRAN;

        SELECT
            @SyncBatchID = I.SyncBatchID,
            @ImportStatus = I.ImportStatus,
            @ApprovedReceiptID = I.ApprovedReceiptID
        FROM dbo.MobileReceiptImports I WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
        WHERE I.ImportID = @ImportID;

        IF @ImportStatus IS NULL
            THROW 63003, N'سجل التحصيل المستورد غير موجود.', 1;

        -- لا تسمح برفض سجل تم اعتماده فعليًا
        IF @ImportStatus = N'Approved' OR @ApprovedReceiptID IS NOT NULL
            THROW 63004, N'لا يمكن رفض سجل تم اعتماده نهائيًا.', 1;

        -- إذا كان مرفوضًا مسبقًا، أعد النتيجة الحالية بدون كسر
        IF @ImportStatus = N'Rejected'
        BEGIN
            COMMIT;

            SELECT
                I.ImportID,
                I.ImportStatus,
                I.RejectedReason,
                CAST(NULL AS INT) AS ReceiptID,
                N'AlreadyRejected' AS ResultStatus
            FROM dbo.MobileReceiptImports I
            WHERE I.ImportID = @ImportID;

            RETURN;
        END

        UPDATE dbo.MobileReceiptImports
        SET
            ImportStatus = N'Rejected',
            RejectedReason = @FinalReason
        WHERE ImportID = @ImportID;

        INSERT INTO dbo.MobileSyncErrors
        (
            SyncBatchID,
            ImportID,
            ErrorMessage,
            ErrorSource,
            CreatedAt
        )
        VALUES
        (
            @SyncBatchID,
            @ImportID,
            N'تم رفض سجل التحصيل: ' + @FinalReason
            + CASE
                WHEN @RejectedByUserID IS NOT NULL
                    THEN N' | بواسطة المستخدم: ' + CAST(@RejectedByUserID AS NVARCHAR(20))
                ELSE N''
              END,
            N'usp_MobileReceipt_Reject',
            GETDATE()
        );

        COMMIT;

        SELECT
            I.ImportID,
            I.ImportStatus,
            I.RejectedReason,
            CAST(NULL AS INT) AS ReceiptID,
            N'Rejected' AS ResultStatus
        FROM dbo.MobileReceiptImports I
        WHERE I.ImportID = @ImportID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_ResetToNew]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileReceipt_ResetToNew]
    @ImportID         INT,
    @ResetByUserID    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @ImportStatus      NVARCHAR(20) = NULL,
        @ApprovedReceiptID INT = NULL;

    IF @ResetByUserID IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserID = @ResetByUserID)
        THROW 64001, N'المستخدم المحدد غير موجود.', 1;

    BEGIN TRY
        BEGIN TRAN;

        SELECT
            @ImportStatus = I.ImportStatus,
            @ApprovedReceiptID = I.ApprovedReceiptID
        FROM dbo.MobileReceiptImports I WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
        WHERE I.ImportID = @ImportID;

        IF @ImportStatus IS NULL
            THROW 64002, N'سجل التحصيل المستورد غير موجود.', 1;

        IF @ImportStatus = N'Approved' OR @ApprovedReceiptID IS NOT NULL
            THROW 64003, N'لا يمكن إعادة سجل معتمد إلى New.', 1;

        IF @ImportStatus = N'New'
        BEGIN
            COMMIT;

            SELECT
                I.ImportID,
                I.ImportStatus,
                I.RejectedReason,
                N'AlreadyNew' AS ResultStatus
            FROM dbo.MobileReceiptImports I
            WHERE I.ImportID = @ImportID;

            RETURN;
        END

        UPDATE dbo.MobileReceiptImports
        SET
            ImportStatus = N'New',
            RejectedReason = NULL,
            ApprovedByUserID = NULL,
            ApprovedAt = NULL,
            ApprovedReceiptID = NULL
        WHERE ImportID = @ImportID;

        INSERT INTO dbo.AuditLog
        (
            UserID,
            Action,
            TableName,
            RecordID,
            ActionDate
        )
        VALUES
        (
            @ResetByUserID,
            N'Reset Mobile Receipt Import To New',
            N'MobileReceiptImports',
            @ImportID,
            GETDATE()
        );

        COMMIT;

        SELECT
            I.ImportID,
            I.ImportStatus,
            I.RejectedReason,
            N'ResetToNew' AS ResultStatus
        FROM dbo.MobileReceiptImports I
        WHERE I.ImportID = @ImportID;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileReceipt_Search]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileReceipt_Search]
    @SearchText     NVARCHAR(100) = NULL,
    @ImportStatus   NVARCHAR(20) = NULL,   -- New / Approved / Rejected
    @CollectorID    INT = NULL,
    @SubscriberID   INT = NULL,
    @SyncBatchID    INT = NULL,
    @DateFrom       DATE = NULL,
    @DateTo         DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Q NVARCHAR(100) = NULLIF(LTRIM(RTRIM(@SearchText)), N'');

    SELECT
        I.ImportID,
        I.SyncBatchID,
        I.ImportStatus,
        I.LocalReceiptNo,
        I.LocalPaymentGuid,
        I.PaymentDate,
        I.TotalReceived,
        I.PaymentMethod,
        I.Notes,
        I.RejectedReason,
        I.CreatedAt,

        I.SubscriberID,
        S.Name AS SubscriberName,
        S.PhoneNumber,
        S.Address,

        I.CollectorID,
        C.Name AS CollectorName,

        I.DeviceID,
        D.DeviceCode,
        D.DeviceName,

        I.ApprovedReceiptID,
        R.ReceiptNumber,
        I.ApprovedAt,
        I.ApprovedByUserID,
        U.FullName AS ApprovedByUserName,

        ISNULL(LA.LineCount, 0) AS LineCount,
        ISNULL(LA.AppliedTotal, 0) AS AppliedTotal,
        ISNULL(LA.InvoiceList, N'') AS InvoiceList
    FROM dbo.MobileReceiptImports I
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
    INNER JOIN dbo.Collectors C ON C.CollectorID = I.CollectorID
    INNER JOIN dbo.CollectorDevices D ON D.DeviceID = I.DeviceID
    LEFT JOIN dbo.Receipts R ON R.ReceiptID = I.ApprovedReceiptID
    LEFT JOIN dbo.Users U ON U.UserID = I.ApprovedByUserID
    OUTER APPLY
    (
        SELECT
            COUNT(*) AS LineCount,
            CAST(ISNULL(SUM(L.AppliedAmount), 0) AS DECIMAL(18,2)) AS AppliedTotal,
            ISNULL(
                STRING_AGG(
                    CASE
                        WHEN L.InvoiceID IS NULL THEN N'AdvanceCredit'
                        ELSE CAST(L.InvoiceID AS NVARCHAR(20))
                    END,
                    N','
                ),
                N''
            ) AS InvoiceList
        FROM dbo.MobileReceiptImportLines L
        WHERE L.ImportID = I.ImportID
    ) LA
    WHERE
        (@ImportStatus IS NULL OR I.ImportStatus = @ImportStatus)
        AND (@CollectorID IS NULL OR I.CollectorID = @CollectorID)
        AND (@SubscriberID IS NULL OR I.SubscriberID = @SubscriberID)
        AND (@SyncBatchID IS NULL OR I.SyncBatchID = @SyncBatchID)
        AND (@DateFrom IS NULL OR I.PaymentDate >= @DateFrom)
        AND (@DateTo IS NULL OR I.PaymentDate <= @DateTo)
        AND
        (
            @Q IS NULL
            OR S.Name LIKE N'%' + @Q + N'%'
            OR S.PhoneNumber LIKE N'%' + @Q + N'%'
            OR I.LocalReceiptNo LIKE N'%' + @Q + N'%'
            OR I.LocalPaymentGuid LIKE N'%' + @Q + N'%'
            OR C.Name LIKE N'%' + @Q + N'%'
            OR ISNULL(R.ReceiptNumber, N'') LIKE N'%' + @Q + N'%'
            OR EXISTS
            (
                SELECT 1
                FROM dbo.MobileReceiptImportLines L
                LEFT JOIN dbo.Invoices IV ON IV.InvoiceID = L.InvoiceID
                WHERE L.ImportID = I.ImportID
                  AND
                  (
                      CAST(ISNULL(L.InvoiceID, 0) AS NVARCHAR(20)) LIKE N'%' + @Q + N'%'
                      OR ISNULL(IV.InvoiceNumber, N'') LIKE N'%' + @Q + N'%'
                  )
            )
        )
    ORDER BY
        CASE
            WHEN I.ImportStatus = N'New' THEN 0
            WHEN I.ImportStatus = N'Rejected' THEN 1
            WHEN I.ImportStatus = N'Approved' THEN 2
            ELSE 3
        END,
        I.PaymentDate DESC,
        I.ImportID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileSync_ExportReceivables]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileSync_ExportReceivables]
    @CollectorID INT,
    @AsOfDate DATE = NULL,
    @OnlyAssignedSubscribers BIT = 1,
    @IncludeAllIfNoAssignments BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @AsOfDate IS NULL
        SET @AsOfDate = CAST(GETDATE() AS DATE);

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Collectors
        WHERE CollectorID = @CollectorID
    )
        THROW 67001, N'المحصّل غير موجود.', 1;

    IF OBJECT_ID('tempdb..#TargetSubscribers') IS NOT NULL DROP TABLE #TargetSubscribers;

    CREATE TABLE #TargetSubscribers
    (
        SubscriberID INT PRIMARY KEY
    );

    /* =========================================================
       1) تحديد المشتركين المخصصين للمحصّل
       ========================================================= */
    IF ISNULL(@OnlyAssignedSubscribers, 1) = 1
    BEGIN
        INSERT INTO #TargetSubscribers (SubscriberID)
        SELECT CS.SubscriberID
        FROM dbo.CollectorSubscribers CS
        INNER JOIN dbo.Subscribers S ON S.SubscriberID = CS.SubscriberID
        WHERE CS.CollectorID = @CollectorID
          AND CS.IsActive = 1
          AND S.IsActive = 1;

        IF NOT EXISTS (SELECT 1 FROM #TargetSubscribers)
           AND ISNULL(@IncludeAllIfNoAssignments, 1) = 1
        BEGIN
            INSERT INTO #TargetSubscribers (SubscriberID)
            SELECT S.SubscriberID
            FROM dbo.Subscribers S
            WHERE S.IsActive = 1;
        END
    END
    ELSE
    BEGIN
        INSERT INTO #TargetSubscribers (SubscriberID)
        SELECT S.SubscriberID
        FROM dbo.Subscribers S
        WHERE S.IsActive = 1;
    END

    /* =========================================================
       2) Result Set 1: ملخص التصدير
       ========================================================= */
    SELECT
        @CollectorID AS CollectorID,
        @AsOfDate AS AsOfDate,
        GETDATE() AS ExportedAt,
        (SELECT COUNT(*) FROM #TargetSubscribers) AS SubscribersCount,
        (
            SELECT COUNT(*)
            FROM dbo.Invoices I
            INNER JOIN #TargetSubscribers T ON T.SubscriberID = I.SubscriberID
            OUTER APPLY
            (
                SELECT SUM(P.Amount) AS PaidAmount
                FROM dbo.Payments P
                WHERE P.InvoiceID = I.InvoiceID
                  AND P.PaymentDate <= @AsOfDate
            ) PA
            WHERE I.InvoiceDate <= @AsOfDate
              AND ISNULL(I.Status, N'') <> N'ملغاة'
              AND CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                    > CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2))
        ) AS OpenInvoicesCount,
        (
            SELECT COUNT(*)
            FROM dbo.SubscriberCredits C
            INNER JOIN #TargetSubscribers T ON T.SubscriberID = C.SubscriberID
            WHERE C.CreditDate <= @AsOfDate
              AND C.AmountRemaining > 0
        ) AS OpenCreditsCount;

    /* =========================================================
       3) Result Set 2: المشتركون + الأرصدة الحالية
       موجب = عليه
       سالب = له (رصيد مقدم)
       ========================================================= */
    ;WITH InvoiceDue AS
    (
        SELECT
            I.SubscriberID,
            CAST(SUM(
                CASE
                    WHEN (CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                          - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2))) > 0
                    THEN (CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                          - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)))
                    ELSE 0
                END
            ) AS DECIMAL(18,2)) AS CurrentDue
        FROM dbo.Invoices I
        INNER JOIN #TargetSubscribers T ON T.SubscriberID = I.SubscriberID
        OUTER APPLY
        (
            SELECT SUM(P.Amount) AS PaidAmount
            FROM dbo.Payments P
            WHERE P.InvoiceID = I.InvoiceID
              AND P.PaymentDate <= @AsOfDate
        ) PA
        WHERE I.InvoiceDate <= @AsOfDate
          AND ISNULL(I.Status, N'') <> N'ملغاة'
        GROUP BY I.SubscriberID
    ),
    CreditDue AS
    (
        SELECT
            C.SubscriberID,
            CAST(ISNULL(SUM(C.AmountRemaining), 0) AS DECIMAL(18,2)) AS CurrentCredit
        FROM dbo.SubscriberCredits C
        INNER JOIN #TargetSubscribers T ON T.SubscriberID = C.SubscriberID
        WHERE C.CreditDate <= @AsOfDate
          AND C.AmountRemaining > 0
        GROUP BY C.SubscriberID
    ),
    LastInvoice AS
    (
        SELECT
            X.SubscriberID,
            X.InvoiceID,
            X.InvoiceNumber,
            X.InvoiceDate,
            X.InvoiceTotal,
            X.Remaining,
            ROW_NUMBER() OVER (PARTITION BY X.SubscriberID ORDER BY X.InvoiceDate DESC, X.InvoiceID DESC) AS RN
        FROM
        (
            SELECT
                I.SubscriberID,
                I.InvoiceID,
                I.InvoiceNumber,
                I.InvoiceDate,
                CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS InvoiceTotal,
                CAST(
                    CASE
                        WHEN (CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                              - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2))) > 0
                        THEN (CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                              - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)))
                        ELSE 0
                    END
                AS DECIMAL(18,2)) AS Remaining
            FROM dbo.Invoices I
            INNER JOIN #TargetSubscribers T ON T.SubscriberID = I.SubscriberID
            OUTER APPLY
            (
                SELECT SUM(P.Amount) AS PaidAmount
                FROM dbo.Payments P
                WHERE P.InvoiceID = I.InvoiceID
                  AND P.PaymentDate <= @AsOfDate
            ) PA
            WHERE I.InvoiceDate <= @AsOfDate
              AND ISNULL(I.Status, N'') <> N'ملغاة'
        ) X
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

        CAST(ISNULL(ID.CurrentDue, 0) AS DECIMAL(18,2)) AS CurrentDue,
        CAST(ISNULL(CD.CurrentCredit, 0) AS DECIMAL(18,2)) AS CurrentCredit,
        CAST(ISNULL(ID.CurrentDue, 0) - ISNULL(CD.CurrentCredit, 0) AS DECIMAL(18,2)) AS CurrentBalance,

        LI.InvoiceID AS LastInvoiceID,
        LI.InvoiceNumber AS LastInvoiceNumber,
        LI.InvoiceDate AS LastInvoiceDate,
        LI.InvoiceTotal AS LastInvoiceTotal,
        LI.Remaining AS LastInvoiceRemaining
    FROM dbo.Subscribers S
    INNER JOIN #TargetSubscribers T ON T.SubscriberID = S.SubscriberID
    OUTER APPLY
    (
        SELECT TOP (1)
            M.MeterID,
            M.MeterNumber,
            M.Location
        FROM dbo.SubscriberMeters SM
        INNER JOIN dbo.Meters M ON M.MeterID = SM.MeterID
        WHERE SM.SubscriberID = S.SubscriberID
        ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC
    ) PM
    LEFT JOIN InvoiceDue ID ON ID.SubscriberID = S.SubscriberID
    LEFT JOIN CreditDue CD ON CD.SubscriberID = S.SubscriberID
    LEFT JOIN LastInvoice LI ON LI.SubscriberID = S.SubscriberID AND LI.RN = 1
    ORDER BY S.Name;

    /* =========================================================
       4) Result Set 3: العدادات المرتبطة بالمشتركين
       ========================================================= */
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
    INNER JOIN #TargetSubscribers T ON T.SubscriberID = SM.SubscriberID
    ORDER BY SM.SubscriberID, SM.IsPrimary DESC, M.MeterNumber;

    /* =========================================================
       5) Result Set 4: الفواتير المفتوحة فقط
       ========================================================= */
    SELECT
        I.InvoiceID,
        I.SubscriberID,
        I.MeterID,
        I.InvoiceNumber,
        I.InvoiceDate,
        I.Consumption,
        I.UnitPrice,
        I.ServiceFees,
        I.Arrears,
        I.TotalAmount,
        CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS GrandTotal,
        CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)) AS PaidTotal,
        CAST(
            CASE
                WHEN (CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                      - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2))) > 0
                THEN (CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
                      - CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)))
                ELSE 0
            END
        AS DECIMAL(18,2)) AS Remaining,
        I.Status,
        I.Notes
    FROM dbo.Invoices I
    INNER JOIN #TargetSubscribers T ON T.SubscriberID = I.SubscriberID
    OUTER APPLY
    (
        SELECT SUM(P.Amount) AS PaidAmount
        FROM dbo.Payments P
        WHERE P.InvoiceID = I.InvoiceID
          AND P.PaymentDate <= @AsOfDate
    ) PA
    WHERE I.InvoiceDate <= @AsOfDate
      AND ISNULL(I.Status, N'') <> N'ملغاة'
      AND CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(18,2))
            > CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2))
    ORDER BY I.SubscriberID, I.InvoiceDate, I.InvoiceID;

    /* =========================================================
       6) Result Set 5: الرصيد المقدم المتبقي
       ========================================================= */
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
    INNER JOIN #TargetSubscribers T ON T.SubscriberID = C.SubscriberID
    WHERE C.CreditDate <= @AsOfDate
      AND C.AmountRemaining > 0
    ORDER BY C.SubscriberID, C.CreditDate, C.CreditID;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileSync_GetBatchDashboard]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileSync_GetBatchDashboard]
    @SyncStatus   NVARCHAR(20) = NULL,   -- Pending / Imported / Partial / Rejected
    @CollectorID  INT = NULL,
    @DeviceID     INT = NULL,
    @DateFrom     DATE = NULL,
    @DateTo       DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ImportAgg AS
    (
        SELECT
            I.SyncBatchID,
            COUNT(*) AS ImportedRows,
            SUM(CASE WHEN I.ImportStatus = N'New' THEN 1 ELSE 0 END) AS NewCount,
            SUM(CASE WHEN I.ImportStatus = N'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
            SUM(CASE WHEN I.ImportStatus = N'Rejected' THEN 1 ELSE 0 END) AS RejectedCount,

            CAST(ISNULL(SUM(I.TotalReceived), 0) AS DECIMAL(18,2)) AS TotalReceivedSum,
            CAST(ISNULL(SUM(CASE WHEN I.ImportStatus = N'Approved' THEN I.TotalReceived ELSE 0 END), 0) AS DECIMAL(18,2)) AS ApprovedAmount,
            CAST(ISNULL(SUM(CASE WHEN I.ImportStatus = N'Rejected' THEN I.TotalReceived ELSE 0 END), 0) AS DECIMAL(18,2)) AS RejectedAmount,
            CAST(ISNULL(SUM(CASE WHEN I.ImportStatus = N'New' THEN I.TotalReceived ELSE 0 END), 0) AS DECIMAL(18,2)) AS PendingAmount
        FROM dbo.MobileReceiptImports I
        GROUP BY I.SyncBatchID
    ),
    ErrorAgg AS
    (
        SELECT
            E.SyncBatchID,
            COUNT(*) AS ErrorCount
        FROM dbo.MobileSyncErrors E
        GROUP BY E.SyncBatchID
    )
    SELECT
        B.SyncBatchID,
        B.SyncDate,
        B.SyncStatus,
        B.StartedAt,
        B.FinishedAt,
        B.RecordsCount AS DeclaredRows,
        B.Notes,

        B.CollectorID,
        C.Name AS CollectorName,
        C.Phone AS CollectorPhone,

        B.DeviceID,
        D.DeviceCode,
        D.DeviceName,
        D.DeviceModel,
        D.AppVersion,

        ISNULL(IA.ImportedRows, 0) AS ImportedRows,
        ISNULL(IA.NewCount, 0) AS NewCount,
        ISNULL(IA.ApprovedCount, 0) AS ApprovedCount,
        ISNULL(IA.RejectedCount, 0) AS RejectedCount,
        ISNULL(EA.ErrorCount, 0) AS ErrorCount,

        ISNULL(IA.TotalReceivedSum, 0) AS TotalReceivedSum,
        ISNULL(IA.ApprovedAmount, 0) AS ApprovedAmount,
        ISNULL(IA.RejectedAmount, 0) AS RejectedAmount,
        ISNULL(IA.PendingAmount, 0) AS PendingAmount,

        DATEDIFF(SECOND, B.StartedAt, B.FinishedAt) AS DurationSeconds
    FROM dbo.MobileSyncBatches B
    INNER JOIN dbo.Collectors C ON C.CollectorID = B.CollectorID
    INNER JOIN dbo.CollectorDevices D ON D.DeviceID = B.DeviceID
    LEFT JOIN ImportAgg IA ON IA.SyncBatchID = B.SyncBatchID
    LEFT JOIN ErrorAgg EA ON EA.SyncBatchID = B.SyncBatchID
    WHERE
        (@SyncStatus IS NULL OR B.SyncStatus = @SyncStatus)
        AND (@CollectorID IS NULL OR B.CollectorID = @CollectorID)
        AND (@DeviceID IS NULL OR B.DeviceID = @DeviceID)
        AND (@DateFrom IS NULL OR CAST(B.SyncDate AS DATE) >= @DateFrom)
        AND (@DateTo IS NULL OR CAST(B.SyncDate AS DATE) <= @DateTo)
    ORDER BY B.SyncDate DESC, B.SyncBatchID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileSync_GetBatchDetails]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileSync_GetBatchDetails]
    @SyncBatchID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.MobileSyncBatches
        WHERE SyncBatchID = @SyncBatchID
    )
        THROW 66001, N'دفعة المزامنة غير موجودة.', 1;

    /* =========================================================
       1) Header / Summary
       ========================================================= */
    ;WITH ImportAgg AS
    (
        SELECT
            I.SyncBatchID,
            COUNT(*) AS ImportedRows,
            SUM(CASE WHEN I.ImportStatus = N'New' THEN 1 ELSE 0 END) AS NewCount,
            SUM(CASE WHEN I.ImportStatus = N'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
            SUM(CASE WHEN I.ImportStatus = N'Rejected' THEN 1 ELSE 0 END) AS RejectedCount,
            CAST(ISNULL(SUM(I.TotalReceived), 0) AS DECIMAL(18,2)) AS TotalReceivedSum
        FROM dbo.MobileReceiptImports I
        WHERE I.SyncBatchID = @SyncBatchID
        GROUP BY I.SyncBatchID
    ),
    ErrorAgg AS
    (
        SELECT
            E.SyncBatchID,
            COUNT(*) AS ErrorCount
        FROM dbo.MobileSyncErrors E
        WHERE E.SyncBatchID = @SyncBatchID
        GROUP BY E.SyncBatchID
    )
    SELECT
        B.SyncBatchID,
        B.SyncDate,
        B.SyncStatus,
        B.StartedAt,
        B.FinishedAt,
        B.RecordsCount AS DeclaredRows,
        B.Notes,

        B.CollectorID,
        C.Name AS CollectorName,
        C.Phone AS CollectorPhone,

        B.DeviceID,
        D.DeviceCode,
        D.DeviceName,
        D.DeviceModel,
        D.AppVersion,

        ISNULL(IA.ImportedRows, 0) AS ImportedRows,
        ISNULL(IA.NewCount, 0) AS NewCount,
        ISNULL(IA.ApprovedCount, 0) AS ApprovedCount,
        ISNULL(IA.RejectedCount, 0) AS RejectedCount,
        ISNULL(EA.ErrorCount, 0) AS ErrorCount,
        ISNULL(IA.TotalReceivedSum, 0) AS TotalReceivedSum,

        DATEDIFF(SECOND, B.StartedAt, B.FinishedAt) AS DurationSeconds
    FROM dbo.MobileSyncBatches B
    INNER JOIN dbo.Collectors C ON C.CollectorID = B.CollectorID
    INNER JOIN dbo.CollectorDevices D ON D.DeviceID = B.DeviceID
    LEFT JOIN ImportAgg IA ON IA.SyncBatchID = B.SyncBatchID
    LEFT JOIN ErrorAgg EA ON EA.SyncBatchID = B.SyncBatchID
    WHERE B.SyncBatchID = @SyncBatchID;

    /* =========================================================
       2) Imports داخل الدفعة
       ========================================================= */
    SELECT
        I.ImportID,
        I.ImportStatus,
        I.LocalReceiptNo,
        I.LocalPaymentGuid,
        I.PaymentDate,
        I.TotalReceived,
        I.PaymentMethod,
        I.Notes,
        I.RejectedReason,
        I.CreatedAt,

        I.SubscriberID,
        S.Name AS SubscriberName,
        S.PhoneNumber,
        S.Address,

        I.ApprovedReceiptID,
        R.ReceiptNumber,
        I.ApprovedAt,
        I.ApprovedByUserID,
        U.FullName AS ApprovedByUserName,

        ISNULL(LA.LineCount, 0) AS LineCount,
        ISNULL(LA.AppliedTotal, 0) AS AppliedTotal
    FROM dbo.MobileReceiptImports I
    INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
    LEFT JOIN dbo.Receipts R ON R.ReceiptID = I.ApprovedReceiptID
    LEFT JOIN dbo.Users U ON U.UserID = I.ApprovedByUserID
    OUTER APPLY
    (
        SELECT
            COUNT(*) AS LineCount,
            CAST(ISNULL(SUM(L.AppliedAmount),0) AS DECIMAL(18,2)) AS AppliedTotal
        FROM dbo.MobileReceiptImportLines L
        WHERE L.ImportID = I.ImportID
    ) LA
    WHERE I.SyncBatchID = @SyncBatchID
    ORDER BY
        CASE
            WHEN I.ImportStatus = N'New' THEN 0
            WHEN I.ImportStatus = N'Rejected' THEN 1
            WHEN I.ImportStatus = N'Approved' THEN 2
            ELSE 3
        END,
        I.PaymentDate DESC,
        I.ImportID DESC;

    /* =========================================================
       3) Errors داخل الدفعة
       ========================================================= */
    SELECT
        E.SyncErrorID,
        E.SyncBatchID,
        E.ImportID,
        E.ErrorMessage,
        E.ErrorSource,
        E.CreatedAt
    FROM dbo.MobileSyncErrors E
    WHERE E.SyncBatchID = @SyncBatchID
    ORDER BY E.SyncErrorID DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileSync_GetImportDecisions]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_MobileSync_GetImportDecisions]
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
    BEGIN
        SELECT
            CAST(NULL AS INT) AS ImportID,
            CAST(NULL AS INT) AS SyncBatchID,
            CAST(NULL AS NVARCHAR(64)) AS LocalPaymentGuid,
            CAST(NULL AS NVARCHAR(50)) AS LocalReceiptNo,
            CAST(NULL AS NVARCHAR(30)) AS ImportStatus,
            CAST(NULL AS INT) AS ApprovedReceiptID,
            CAST(NULL AS NVARCHAR(50)) AS ReceiptNumber,
            CAST(NULL AS DATETIME) AS ApprovedAt,
            CAST(NULL AS INT) AS ApprovedByUserID,
            CAST(NULL AS NVARCHAR(200)) AS ApprovedByUserName,
            CAST(NULL AS NVARCHAR(500)) AS RejectedReason,
            CAST(NULL AS DATETIME) AS CreatedAt,
            CAST(NULL AS DATETIME) AS ChangedAt
        WHERE 1 = 0;

        RETURN;
    END;

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
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_MobileSync_SaveBatch]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   1) usp_MobileSync_SaveBatch
   ========================================================= */
CREATE   PROCEDURE [dbo].[usp_MobileSync_SaveBatch]
    @CollectorID        INT,
    @DeviceCode         NVARCHAR(100),
    @DeviceName         NVARCHAR(100) = NULL,
    @DeviceModel        NVARCHAR(100) = NULL,
    @AppVersion         NVARCHAR(30) = NULL,
    @Receipts           dbo.MobileReceiptImportType READONLY,
    @Lines              dbo.MobileReceiptImportLineType READONLY,
    @SyncBatchID        INT OUTPUT,
    @DeviceID           INT OUTPUT,
    @AutoCreateDevice   BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @ExistingCollectorID INT = NULL,
        @IsApproved BIT = 0,
        @IsActive BIT = 0,
        @TotalRows INT = 0,
        @InsertedCount INT = 0,
        @DuplicateCount INT = 0,
        @BatchStatus NVARCHAR(20) = N'Pending';

    IF NOT EXISTS (SELECT 1 FROM dbo.Collectors WHERE CollectorID = @CollectorID)
        THROW 61001, N'المحصّل غير موجود.', 1;

    IF NULLIF(LTRIM(RTRIM(@DeviceCode)), N'') IS NULL
        THROW 61002, N'معرف الجهاز DeviceCode مطلوب.', 1;

    SELECT @TotalRows = COUNT(*) FROM @Receipts;
    IF @TotalRows = 0
        THROW 61003, N'لا توجد بيانات تحصيل مرسلة من الهاتف.', 1;

    IF EXISTS
    (
        SELECT RowNo
        FROM @Receipts
        GROUP BY RowNo
        HAVING COUNT(*) > 1
    )
        THROW 61004, N'يوجد تكرار في RowNo داخل الدفعة المرسلة.', 1;

    IF EXISTS
    (
        SELECT LocalPaymentGuid
        FROM @Receipts
        GROUP BY LocalPaymentGuid
        HAVING COUNT(*) > 1
    )
        THROW 61005, N'يوجد تكرار في LocalPaymentGuid داخل الدفعة المرسلة.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @Receipts
        WHERE TotalReceived <= 0
    )
        THROW 61006, N'يوجد مبلغ مستلم غير صحيح داخل البيانات المرسلة.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @Receipts
        WHERE PaymentMethod NOT IN (N'Cash', N'Transfer', N'Cheque', N'Other')
    )
        THROW 61007, N'يوجد PaymentMethod غير صحيح داخل البيانات المرسلة.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @Lines
        WHERE AppliedAmount <= 0
           OR ApplicationType NOT IN (N'InvoicePayment', N'AdvanceCredit')
    )
        THROW 61008, N'يوجد سطر توزيع غير صحيح داخل البيانات المرسلة.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM @Lines L
        LEFT JOIN @Receipts R ON R.RowNo = L.ReceiptRowNo
        WHERE R.RowNo IS NULL
    )
        THROW 61009, N'يوجد سطر توزيع لا يطابق أي إيصال داخل نفس الدفعة.', 1;

    BEGIN TRY
        BEGIN TRAN;

        SELECT
            @DeviceID = D.DeviceID,
            @ExistingCollectorID = D.CollectorID,
            @IsApproved = D.IsApproved,
            @IsActive = D.IsActive
        FROM dbo.CollectorDevices D WITH (UPDLOCK, HOLDLOCK)
        WHERE D.DeviceCode = @DeviceCode;

        IF @DeviceID IS NULL
        BEGIN
            IF ISNULL(@AutoCreateDevice, 0) = 0
                THROW 61010, N'الجهاز غير مسجل ولا يمكن إنشاؤه تلقائيًا.', 1;

            INSERT INTO dbo.CollectorDevices
            (
                CollectorID,
                DeviceCode,
                DeviceName,
                DeviceModel,
                AppVersion,
                IsApproved,
                IsActive,
                CreatedAt
            )
            VALUES
            (
                @CollectorID,
                @DeviceCode,
                NULLIF(LTRIM(RTRIM(@DeviceName)), N''),
                NULLIF(LTRIM(RTRIM(@DeviceModel)), N''),
                NULLIF(LTRIM(RTRIM(@AppVersion)), N''),
                1,
                1,
                GETDATE()
            );

            SET @DeviceID = SCOPE_IDENTITY();
            SET @IsApproved = 1;
            SET @IsActive = 1;
            SET @ExistingCollectorID = @CollectorID;
        END
        ELSE
        BEGIN
            IF @ExistingCollectorID <> @CollectorID
                THROW 61011, N'هذا الجهاز مرتبط بمحصّل آخر.', 1;

            IF ISNULL(@IsApproved, 0) = 0
                THROW 61012, N'هذا الجهاز غير معتمد.', 1;

            IF ISNULL(@IsActive, 0) = 0
                THROW 61013, N'هذا الجهاز غير نشط.', 1;

            UPDATE dbo.CollectorDevices
            SET
                DeviceName = COALESCE(NULLIF(LTRIM(RTRIM(@DeviceName)), N''), DeviceName),
                DeviceModel = COALESCE(NULLIF(LTRIM(RTRIM(@DeviceModel)), N''), DeviceModel),
                AppVersion = COALESCE(NULLIF(LTRIM(RTRIM(@AppVersion)), N''), AppVersion)
            WHERE DeviceID = @DeviceID;
        END

        INSERT INTO dbo.MobileSyncBatches
        (
            CollectorID,
            DeviceID,
            SyncDate,
            RecordsCount,
            SyncStatus,
            StartedAt
        )
        VALUES
        (
            @CollectorID,
            @DeviceID,
            GETDATE(),
            @TotalRows,
            N'Pending',
            GETDATE()
        );

        SET @SyncBatchID = SCOPE_IDENTITY();

        IF OBJECT_ID('tempdb..#DupReceipts') IS NOT NULL DROP TABLE #DupReceipts;
        IF OBJECT_ID('tempdb..#NewReceipts') IS NOT NULL DROP TABLE #NewReceipts;
        IF OBJECT_ID('tempdb..#Map') IS NOT NULL DROP TABLE #Map;

        SELECT
            R.RowNo,
            R.LocalPaymentGuid,
            R.LocalReceiptNo,
            E.ImportID,
            E.ImportStatus
        INTO #DupReceipts
        FROM @Receipts R
        INNER JOIN dbo.MobileReceiptImports E
            ON E.DeviceID = @DeviceID
           AND E.LocalPaymentGuid = R.LocalPaymentGuid;

        SELECT @DuplicateCount = COUNT(*) FROM #DupReceipts;

        INSERT INTO dbo.MobileSyncErrors
        (
            SyncBatchID,
            ImportID,
            ErrorMessage,
            ErrorSource,
            CreatedAt
        )
        SELECT
            @SyncBatchID,
            D.ImportID,
            N'تم تجاهل العملية لأنها مكررة لنفس الجهاز. LocalPaymentGuid=' + D.LocalPaymentGuid,
            N'usp_MobileSync_SaveBatch',
            GETDATE()
        FROM #DupReceipts D;

        SELECT
            R.RowNo,
            R.LocalPaymentGuid,
            R.LocalReceiptNo,
            R.SubscriberID,
            R.PaymentDate,
            R.TotalReceived,
            R.PaymentMethod,
            R.Notes
        INTO #NewReceipts
        FROM @Receipts R
        LEFT JOIN #DupReceipts D
            ON D.RowNo = R.RowNo
        WHERE D.RowNo IS NULL;

        INSERT INTO dbo.MobileReceiptImports
        (
            SyncBatchID,
            CollectorID,
            DeviceID,
            LocalReceiptNo,
            LocalPaymentGuid,
            SubscriberID,
            PaymentDate,
            TotalReceived,
            PaymentMethod,
            Notes,
            ImportStatus,
            CreatedAt
        )
        SELECT
            @SyncBatchID,
            @CollectorID,
            @DeviceID,
            N.LocalReceiptNo,
            N.LocalPaymentGuid,
            N.SubscriberID,
            N.PaymentDate,
            N.TotalReceived,
            N.PaymentMethod,
            N.Notes,
            N'New',
            GETDATE()
        FROM #NewReceipts N;

        SELECT @InsertedCount = @@ROWCOUNT;

        SELECT
            N.RowNo,
            I.ImportID
        INTO #Map
        FROM #NewReceipts N
        INNER JOIN dbo.MobileReceiptImports I
            ON I.DeviceID = @DeviceID
           AND I.LocalPaymentGuid = N.LocalPaymentGuid;

        INSERT INTO dbo.MobileReceiptImportLines
        (
            ImportID,
            InvoiceID,
            AppliedAmount,
            ApplicationType,
            Notes,
            CreatedAt
        )
        SELECT
            M.ImportID,
            L.InvoiceID,
            L.AppliedAmount,
            L.ApplicationType,
            L.Notes,
            GETDATE()
        FROM @Lines L
        INNER JOIN #Map M
            ON M.RowNo = L.ReceiptRowNo;

        SET @BatchStatus =
            CASE
                WHEN @InsertedCount > 0 AND @DuplicateCount = 0 THEN N'Imported'
                WHEN @InsertedCount > 0 AND @DuplicateCount > 0 THEN N'Partial'
                WHEN @InsertedCount = 0 AND @DuplicateCount > 0 THEN N'Rejected'
                ELSE N'Pending'
            END;

        UPDATE dbo.MobileSyncBatches
        SET
            SyncStatus = @BatchStatus,
            FinishedAt = GETDATE(),
            Notes =
                N'Total=' + CAST(@TotalRows AS NVARCHAR(20))
                + N', Inserted=' + CAST(@InsertedCount AS NVARCHAR(20))
                + N', Duplicates=' + CAST(@DuplicateCount AS NVARCHAR(20))
        WHERE SyncBatchID = @SyncBatchID;

        UPDATE dbo.CollectorDevices
        SET LastSyncAt = GETDATE()
        WHERE DeviceID = @DeviceID;

        COMMIT;

        SELECT
            @SyncBatchID AS SyncBatchID,
            @DeviceID AS DeviceID,
            @TotalRows AS TotalRows,
            @InsertedCount AS InsertedCount,
            @DuplicateCount AS DuplicateCount,
            @BatchStatus AS BatchStatus;

        SELECT
            R.RowNo,
            R.LocalPaymentGuid,
            COALESCE(M.ImportID, D.ImportID) AS ImportID,
            CASE
                WHEN D.ImportID IS NOT NULL THEN N'Duplicate'
                ELSE N'Inserted'
            END AS SaveStatus
        FROM @Receipts R
        LEFT JOIN #Map M
            ON M.RowNo = R.RowNo
        LEFT JOIN #DupReceipts D
            ON D.RowNo = R.RowNo
        ORDER BY R.RowNo;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_SaveMainMeterReadingAndGenerateReport]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[usp_SaveMainMeterReadingAndGenerateReport]
    @MainMeterID INT,
    @ReportDate DATE,
    @CurrentReading DECIMAL(18,2),
    @Notes NVARCHAR(200) = NULL,
    @TotalFromQasem DECIMAL(12,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @PrevMainReading DECIMAL(18,2) = 0,
        @PrevMainDate DATE = NULL,
        @MainDiff DECIMAL(18,2) = 0,

        @TotalSubPrev DECIMAL(18,2) = 0,
        @TotalSubCurr DECIMAL(18,2) = 0,
        @TotalSubDiff DECIMAL(18,2) = 0,

        @WaterLoss DECIMAL(18,2) = 0,
        @WaterLossPercent DECIMAL(18,2) = 0,

        @TotalConsumptionAmount DECIMAL(12,2) = 0,
        @TotalServiceFees DECIMAL(12,2) = 0,
        @TotalDue DECIMAL(12,2) = 0,
        @Arrears DECIMAL(12,2) = 0,
        @RemainingWithQasem DECIMAL(12,2) = 0;

    IF @CurrentReading IS NULL OR @CurrentReading < 0
        THROW 50001, N'قراءة العداد الرئيسي غير صحيحة.', 1;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.Meters
        WHERE MeterID = @MainMeterID
          AND MeterType = N'Main'
    )
        THROW 50002, N'العداد المحدد ليس عدادًا رئيسيًا أو غير موجود.', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.MainMeterReadings
        WHERE MeterID = @MainMeterID
          AND ReadingDate = @ReportDate
    )
        THROW 50003, N'تم تسجيل قراءة لهذا العداد الرئيسي في نفس التاريخ مسبقًا.', 1;

    SELECT TOP (1)
        @PrevMainReading = CurrentReading,
        @PrevMainDate = ReadingDate
    FROM dbo.MainMeterReadings
    WHERE MeterID = @MainMeterID
    ORDER BY ReadingDate DESC, MainReadingID DESC;

    SET @PrevMainReading = ISNULL(@PrevMainReading, 0);

    IF @PrevMainDate IS NOT NULL AND @ReportDate <= @PrevMainDate
        THROW 50004, N'تاريخ القراءة يجب أن يكون بعد آخر قراءة رئيسية مسجلة.', 1;

    IF @CurrentReading < @PrevMainReading
        THROW 50005, N'القراءة الحالية أقل من السابقة.', 1;

    SET @MainDiff = ROUND(@CurrentReading - @PrevMainReading, 2);

    /* إجمالي العدادات الفرعية لنفس تاريخ الدورة */
    SELECT
        @TotalSubPrev = ISNULL(SUM(CAST(R.PreviousReading AS DECIMAL(18,2))), 0),
        @TotalSubCurr = ISNULL(SUM(CAST(R.CurrentReading AS DECIMAL(18,2))), 0),
        @TotalSubDiff = ISNULL(SUM(CAST(R.Consumption AS DECIMAL(18,2))), 0)
    FROM dbo.Readings R
    INNER JOIN dbo.Meters M ON M.MeterID = R.MeterID
    WHERE R.ReadingDate = @ReportDate
      AND ISNULL(M.MeterType, N'Sub') = N'Sub';

    SET @WaterLoss = ROUND(@MainDiff - @TotalSubDiff, 2);

    SET @WaterLossPercent =
        CASE
            WHEN @MainDiff > 0 THEN ROUND((@WaterLoss / @MainDiff) * 100.0, 2)
            ELSE 0
        END;

    /* مبالغ استهلاك اليوم */
    SELECT
        @TotalConsumptionAmount = ISNULL(SUM(CAST(I.Consumption * I.UnitPrice AS DECIMAL(12,2))), 0),
        @TotalServiceFees = ISNULL(SUM(CAST(ISNULL(I.ServiceFees,0) AS DECIMAL(12,2))), 0),
        @TotalDue = ISNULL(SUM(CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(12,2))), 0)
    FROM dbo.Invoices I
    WHERE I.InvoiceDate = @ReportDate
      AND ISNULL(I.Status, N'') <> N'ملغاة';

    /* إجمالي المتأخرات الحية حتى تاريخ التقرير */
    ;WITH Inv AS
    (
        SELECT
            I.InvoiceID,
            CAST(ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0) AS DECIMAL(12,2)) AS InvoiceTotal,
            CAST(ISNULL((
                SELECT SUM(P.Amount)
                FROM dbo.Payments P
                WHERE P.InvoiceID = I.InvoiceID
                  AND P.PaymentDate <= @ReportDate
            ),0) AS DECIMAL(12,2)) AS PaidUntilDate
        FROM dbo.Invoices I
        WHERE I.InvoiceDate <= @ReportDate
          AND ISNULL(I.Status, N'') <> N'ملغاة'
    )
    SELECT
        @Arrears = ISNULL(SUM(
            CASE
                WHEN InvoiceTotal > PaidUntilDate THEN InvoiceTotal - PaidUntilDate
                ELSE 0
            END
        ),0)
    FROM Inv;

    SET @RemainingWithQasem = ISNULL(@TotalFromQasem, 0) - ISNULL(@TotalDue, 0);

    BEGIN TRY
        BEGIN TRAN;

        INSERT INTO dbo.MainMeterReadings
        (
            MeterID, ReadingDate, PreviousReading, CurrentReading, Consumption, Notes
        )
        VALUES
        (
            @MainMeterID, @ReportDate, @PrevMainReading, @CurrentReading, @MainDiff, @Notes
        );

        MERGE dbo.MeterReports AS T
        USING
        (
            SELECT
                @MainMeterID AS MainMeterID,
                @ReportDate AS ReportDate
        ) AS S
        ON T.MainMeterID = S.MainMeterID
       AND T.ReportDate = S.ReportDate

        WHEN MATCHED THEN
            UPDATE SET
                MainMeterPrev = @PrevMainReading,
                MainMeterCurr = @CurrentReading,
                MainMeterDiff = @MainDiff,
                TotalSubMetersPrev = @TotalSubPrev,
                TotalSubMetersCurr = @TotalSubCurr,
                TotalSubMetersDiff = @TotalSubDiff,
                WaterLoss = @WaterLoss,
                TotalConsumptionAmount = @TotalConsumptionAmount,
                TotalServiceFees = @TotalServiceFees,
                TotalDue = @TotalDue,
                TotalFromQasem = ISNULL(@TotalFromQasem, 0),
                RemainingWithQasem = @RemainingWithQasem,
                Arrears = @Arrears

        WHEN NOT MATCHED THEN
            INSERT
            (
                MainMeterID,
                ReportDate,
                MainMeterPrev,
                MainMeterCurr,
                MainMeterDiff,
                TotalSubMetersPrev,
                TotalSubMetersCurr,
                TotalSubMetersDiff,
                WaterLoss,
                TotalConsumptionAmount,
                TotalServiceFees,
                TotalDue,
                TotalFromQasem,
                RemainingWithQasem,
                Arrears
            )
            VALUES
            (
                @MainMeterID,
                @ReportDate,
                @PrevMainReading,
                @CurrentReading,
                @MainDiff,
                @TotalSubPrev,
                @TotalSubCurr,
                @TotalSubDiff,
                @WaterLoss,
                @TotalConsumptionAmount,
                @TotalServiceFees,
                @TotalDue,
                ISNULL(@TotalFromQasem, 0),
                @RemainingWithQasem,
                @Arrears
            );

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;
        THROW;
    END CATCH;

    SELECT
        @MainMeterID AS MainMeterID,
        @ReportDate AS ReportDate,
        @PrevMainReading AS MainMeterPrev,
        @CurrentReading AS MainMeterCurr,
        @MainDiff AS MainMeterDiff,
        @TotalSubPrev AS TotalSubMetersPrev,
        @TotalSubCurr AS TotalSubMetersCurr,
        @TotalSubDiff AS TotalSubMetersDiff,
        @WaterLoss AS WaterLoss,
        @WaterLossPercent AS WaterLossPercent,
        @TotalConsumptionAmount AS TotalConsumptionAmount,
        @TotalServiceFees AS TotalServiceFees,
        @TotalDue AS TotalDue,
        @Arrears AS Arrears;
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_Subscriber_Import]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Subscriber_Import]
    @Rows dbo.SubscriberImportType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Result TABLE
    (
        RowNo        INT,
        IsSuccess    BIT,
        Message      NVARCHAR(4000),
        SubscriberID INT NULL,
        MeterID      INT NULL
    );

    DECLARE
        @RowNo               INT,
        @SubscriberName      NVARCHAR(100),
        @PhoneNumber         NVARCHAR(20),
        @Address             NVARCHAR(150),
        @MeterNumber         NVARCHAR(50),
        @MeterLocation       NVARCHAR(150),
        @IsPrimary           BIT,
        @AccountCode         NVARCHAR(50),
        @InitialReading      DECIMAL(18,3),
        @AccountID           INT,
        @SubscriberID        INT,
        @MeterID             INT,
        @EffectiveIsPrimary  BIT;

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT
            RowNo,
            SubscriberName,
            PhoneNumber,
            Address,
            MeterNumber,
            MeterLocation,
            IsPrimary,
            AccountCode,
            InitialReading
        FROM @Rows
        ORDER BY RowNo;

    OPEN cur;

    FETCH NEXT FROM cur INTO
        @RowNo, @SubscriberName, @PhoneNumber, @Address,
        @MeterNumber, @MeterLocation, @IsPrimary, @AccountCode, @InitialReading;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            SET @AccountID = NULL;
            SET @SubscriberID = NULL;
            SET @MeterID = NULL;
            SET @EffectiveIsPrimary = ISNULL(@IsPrimary, 0);

            IF NULLIF(LTRIM(RTRIM(@SubscriberName)), N'') IS NULL
                THROW 50101, N'اسم المشترك مطلوب.', 1;

            IF NULLIF(LTRIM(RTRIM(@MeterNumber)), N'') IS NULL
                THROW 50102, N'رقم العداد مطلوب.', 1;
IF @InitialReading IS NULL
    THROW 50103, N'القراءة الابتدائية مطلوبة.', 1;

IF @InitialReading < 0
    THROW 50104, N'القراءة الابتدائية لا يمكن أن تكون سالبة.', 1;

            -- محاولة جلب الحساب من كود الحساب القادم من ملف الاستيراد
            IF NULLIF(LTRIM(RTRIM(@AccountCode)), N'') IS NOT NULL
            BEGIN
                SELECT TOP (1) @AccountID = AccountID
                FROM dbo.Accounts
                WHERE AccountCode = LTRIM(RTRIM(@AccountCode));
            END

            -- لو لم يوجد حساب من AccountCode، استخدم أول حساب غير Revenue كافتراضي
            IF @AccountID IS NULL
            BEGIN
                SELECT TOP (1) @AccountID = AccountID
                FROM dbo.Accounts
                WHERE AccountType <> 'Revenue'
                ORDER BY AccountID;
            END

            IF @AccountID IS NULL
                THROW 50104, N'تعذر تحديد حساب ذمة صالح للمشترك.', 1;

            -- البحث عن مشترك قائم
            SELECT TOP (1) @SubscriberID = SubscriberID
            FROM dbo.Subscribers
            WHERE Name = LTRIM(RTRIM(@SubscriberName))
              AND ISNULL(PhoneNumber, N'') = ISNULL(LTRIM(RTRIM(@PhoneNumber)), N'')
              AND IsActive = 1
            ORDER BY SubscriberID DESC;

            -- إذا لم يوجد مشترك: ننشئ مشتركًا جديدًا مع عداده الأساسي
            IF @SubscriberID IS NULL
            BEGIN
                EXEC dbo.usp_Subscriber_Manage
                    @Action         = N'ADD_SUBSCRIBER',
                    @SubscriberID   = @SubscriberID OUTPUT,
                    @MeterID        = @MeterID OUTPUT,
                    @Name           = @SubscriberName,
                    @PhoneNumber    = @PhoneNumber,
                    @Address        = @Address,
                    @AccountID      = @AccountID,
                    @MeterNumber    = @MeterNumber,
                    @MeterLocation  = @MeterLocation,
                    @IsPrimary      = 1,
                    @InitialReading = @InitialReading;
            END
            ELSE
            BEGIN
                -- تحديث الحساب إذا كان المشترك الحالي لا يملك حسابًا
                UPDATE dbo.Subscribers
                SET AccountID = ISNULL(AccountID, @AccountID)
                WHERE SubscriberID = @SubscriberID;

                EXEC dbo.usp_Subscriber_Manage
                    @Action         = N'ADD_METER',
                    @SubscriberID   = @SubscriberID OUTPUT,
                    @MeterID        = @MeterID OUTPUT,
                    @MeterNumber    = @MeterNumber,
                    @MeterLocation  = @MeterLocation,
                    @IsPrimary      = @EffectiveIsPrimary,
                    @InitialReading = @InitialReading;
            END

            INSERT INTO @Result
            (
                RowNo,
                IsSuccess,
                Message,
                SubscriberID,
                MeterID
            )
            VALUES
            (
                @RowNo,
                1,
                N'تم الحفظ',
                @SubscriberID,
                @MeterID
            );
        END TRY
        BEGIN CATCH
            INSERT INTO @Result
            (
                RowNo,
                IsSuccess,
                Message,
                SubscriberID,
                MeterID
            )
            VALUES
            (
                @RowNo,
                0,
                ERROR_MESSAGE(),
                @SubscriberID,
                @MeterID
            );
        END CATCH;

        FETCH NEXT FROM cur INTO
            @RowNo, @SubscriberName, @PhoneNumber, @Address,
            @MeterNumber, @MeterLocation, @IsPrimary, @AccountCode, @InitialReading;
    END

    CLOSE cur;
    DEALLOCATE cur;

    SELECT *
    FROM @Result
    ORDER BY RowNo;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Subscriber_Manage]    Script Date: 5/8/2026 3:43:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_Subscriber_Manage]
    @Action         NVARCHAR(30),
    @SubscriberID   INT = NULL OUTPUT,
    @MeterID        INT = NULL OUTPUT,

    @Name           NVARCHAR(100) = NULL,
    @PhoneNumber    NVARCHAR(20) = NULL,
    @Address        NVARCHAR(150) = NULL,
    @AccountID      INT = NULL,

    @MeterNumber    NVARCHAR(50) = NULL,
    @MeterLocation  NVARCHAR(150) = NULL,
    @IsPrimary      BIT = NULL,
    @InitialReading DECIMAL(18, 3) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        IF @Action = N'ADD_SUBSCRIBER'
        BEGIN
            IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL
                THROW 50001, N'اسم المشترك مطلوب.', 1;

            IF NULLIF(LTRIM(RTRIM(@MeterNumber)), N'') IS NULL
                THROW 50002, N'رقم العداد مطلوب.', 1;

            IF @AccountID IS NULL
                THROW 50003, N'حساب الذمة مطلوب.', 1;

				IF @InitialReading IS NULL
    THROW 50030, N'القراءة الابتدائية مطلوبة.', 1;

IF @InitialReading < 0
    THROW 50031, N'القراءة الابتدائية لا يمكن أن تكون سالبة.', 1;
            INSERT INTO dbo.Subscribers
            (
                Name,
                MeterNumber,
                PhoneNumber,
                Address,
                CreatedDate,
                AccountID,
                IsActive,
                TariffPlanID
            )
            VALUES
            (
                LTRIM(RTRIM(@Name)),
                LTRIM(RTRIM(@MeterNumber)),
                NULLIF(LTRIM(RTRIM(@PhoneNumber)), N''),
                NULLIF(LTRIM(RTRIM(@Address)), N''),
                GETDATE(),
                @AccountID,
                1,
                NULL
            );

            SET @SubscriberID = CAST(SCOPE_IDENTITY() AS INT);

            SELECT @MeterID = MeterID
            FROM dbo.Meters WITH (UPDLOCK, HOLDLOCK)
            WHERE MeterNumber = LTRIM(RTRIM(@MeterNumber));

            IF @MeterID IS NULL
            BEGIN
                INSERT INTO dbo.Meters
                (
                    MeterNumber,
                    MeterType,
                    Location,
                    IsActive,
                    CreatedAt
                )
                VALUES
                (
                    LTRIM(RTRIM(@MeterNumber)),
                    N'Sub',
                    NULLIF(LTRIM(RTRIM(@MeterLocation)), N''),
                    1,
                    GETDATE()
                );

                SET @MeterID = CAST(SCOPE_IDENTITY() AS INT);
            END

            IF EXISTS
            (
                SELECT 1
                FROM dbo.SubscriberMeters
                WHERE MeterID = @MeterID
                  AND SubscriberID <> @SubscriberID
            )
                THROW 50004, N'هذا العداد مرتبط بمشترك آخر.', 1;

            INSERT INTO dbo.SubscriberMeters
            (
                SubscriberID,
                MeterID,
                IsPrimary,
                LinkedAt
            )
            VALUES
            (
                @SubscriberID,
                @MeterID,
                1,
                GETDATE()
            );

            IF @InitialReading IS NOT NULL
            BEGIN
                DECLARE @PrevReading DECIMAL(18,3) = 0;

                SELECT TOP (1) @PrevReading = CurrentReading
                FROM dbo.Readings
                WHERE MeterID = @MeterID
                ORDER BY ReadingDate DESC, ReadingID DESC;

                IF @InitialReading < @PrevReading
                    THROW 50005, N'القراءة الجديدة لا يمكن أن تكون أقل من القراءة السابقة.', 1;

                INSERT INTO dbo.Readings
                (
                    SubscriberID,
                    ReadingDate,
                    PreviousReading,
                    CurrentReading,
                    Consumption,
                    Notes,
                    MeterID
                )
                VALUES
                (
                    @SubscriberID,
                    GETDATE(),
                    @PrevReading,
                    @InitialReading,
                    @InitialReading - @PrevReading,
                    N'قراءة ابتدائية عند إضافة المشترك',
                    @MeterID
                );
            END
        END

        ELSE IF @Action = N'UPDATE_SUBSCRIBER'
        BEGIN
            IF @SubscriberID IS NULL OR @SubscriberID <= 0
                THROW 50006, N'معرف المشترك غير صحيح.', 1;

            IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL
                THROW 50007, N'اسم المشترك مطلوب.', 1;

            IF @AccountID IS NULL
                THROW 50008, N'حساب الذمة مطلوب.', 1;

            UPDATE dbo.Subscribers
            SET
                Name        = LTRIM(RTRIM(@Name)),
                PhoneNumber = NULLIF(LTRIM(RTRIM(@PhoneNumber)), N''),
                Address     = NULLIF(LTRIM(RTRIM(@Address)), N''),
                AccountID   = @AccountID
            WHERE SubscriberID = @SubscriberID;

            IF @@ROWCOUNT = 0
                THROW 50009, N'المشترك غير موجود.', 1;
        END

        ELSE IF @Action = N'ADD_METER'
        BEGIN
            DECLARE @IsNewLink BIT = 0;

            IF @SubscriberID IS NULL OR @SubscriberID <= 0
                THROW 50010, N'اختر مشتركًا أولًا.', 1;

            IF NOT EXISTS (
                SELECT 1
                FROM dbo.Subscribers
                WHERE SubscriberID = @SubscriberID
            )
                THROW 50019, N'المشترك غير موجود.', 1;

            IF NULLIF(LTRIM(RTRIM(@MeterNumber)), N'') IS NULL
                THROW 50011, N'رقم العداد مطلوب.', 1;
				IF @InitialReading IS NULL
    THROW 50032, N'القراءة الابتدائية مطلوبة عند إضافة عداد.', 1;

IF @InitialReading < 0
    THROW 50033, N'القراءة الابتدائية لا يمكن أن تكون سالبة.', 1;

            SELECT @MeterID = MeterID
            FROM dbo.Meters WITH (UPDLOCK, HOLDLOCK)
            WHERE MeterNumber = LTRIM(RTRIM(@MeterNumber));

            IF @MeterID IS NULL
            BEGIN
                INSERT INTO dbo.Meters
                (
                    MeterNumber,
                    MeterType,
                    Location,
                    IsActive,
                    CreatedAt
                )
                VALUES
                (
                    LTRIM(RTRIM(@MeterNumber)),
                    N'Sub',
                    NULLIF(LTRIM(RTRIM(@MeterLocation)), N''),
                    1,
                    GETDATE()
                );

                SET @MeterID = CAST(SCOPE_IDENTITY() AS INT);
            END
            ELSE
            BEGIN
                UPDATE dbo.Meters
                SET Location = COALESCE(NULLIF(LTRIM(RTRIM(@MeterLocation)), N''), Location)
                WHERE MeterID = @MeterID;
            END

            IF EXISTS (
                SELECT 1
                FROM dbo.SubscriberMeters
                WHERE MeterID = @MeterID
                  AND SubscriberID <> @SubscriberID
            )
                THROW 50012, N'هذا العداد مرتبط بمشترك آخر.', 1;

            IF NOT EXISTS (
                SELECT 1
                FROM dbo.SubscriberMeters
                WHERE SubscriberID = @SubscriberID
                  AND MeterID = @MeterID
            )
            BEGIN
                SET @IsNewLink = 1;

                IF @IsPrimary IS NULL
                    SET @IsPrimary = 0;

                IF NOT EXISTS (
                    SELECT 1
                    FROM dbo.SubscriberMeters
                    WHERE SubscriberID = @SubscriberID
                      AND IsPrimary = 1
                )
                    SET @IsPrimary = 1;

                INSERT INTO dbo.SubscriberMeters
                (
                    SubscriberID,
                    MeterID,
                    IsPrimary,
                    LinkedAt
                )
                VALUES
                (
                    @SubscriberID,
                    @MeterID,
                    @IsPrimary,
                    GETDATE()
                );
            END

            IF ISNULL(@IsPrimary, 0) = 1
            BEGIN
                UPDATE dbo.SubscriberMeters
                SET IsPrimary = CASE WHEN MeterID = @MeterID THEN 1 ELSE 0 END
                WHERE SubscriberID = @SubscriberID;

                UPDATE dbo.Subscribers
                SET MeterNumber = LTRIM(RTRIM(@MeterNumber))
                WHERE SubscriberID = @SubscriberID;
            END

            IF @InitialReading IS NOT NULL AND @IsNewLink = 1
            BEGIN
                DECLARE @PrevReading2 DECIMAL(18,3) = 0;

                SELECT TOP (1) @PrevReading2 = CurrentReading
                FROM dbo.Readings
                WHERE MeterID = @MeterID
                ORDER BY ReadingDate DESC, ReadingID DESC;

                IF @InitialReading < @PrevReading2
                    THROW 50013, N'القراءة الجديدة لا يمكن أن تكون أقل من القراءة السابقة.', 1;

                INSERT INTO dbo.Readings
                (
                    SubscriberID,
                    ReadingDate,
                    PreviousReading,
                    CurrentReading,
                    Consumption,
                    Notes,
                    MeterID
                )
                VALUES
                (
                    @SubscriberID,
                    GETDATE(),
                    @PrevReading2,
                    @InitialReading,
                    @InitialReading - @PrevReading2,
                    N'قراءة ابتدائية عند إضافة عداد',
                    @MeterID
                );
            END
        END

        ELSE IF @Action = N'SET_PRIMARY_METER'
        BEGIN
            IF @SubscriberID IS NULL OR @MeterID IS NULL
                THROW 50014, N'المشترك والعداد مطلوبان.', 1;

            IF NOT EXISTS (
                SELECT 1
                FROM dbo.SubscriberMeters
                WHERE SubscriberID = @SubscriberID
                  AND MeterID = @MeterID
            )
                THROW 50015, N'هذا العداد غير مربوط بهذا المشترك.', 1;

            UPDATE dbo.SubscriberMeters
            SET IsPrimary = CASE WHEN MeterID = @MeterID THEN 1 ELSE 0 END
            WHERE SubscriberID = @SubscriberID;

            UPDATE S
            SET S.MeterNumber = M.MeterNumber
            FROM dbo.Subscribers S
            INNER JOIN dbo.Meters M ON M.MeterID = @MeterID
            WHERE S.SubscriberID = @SubscriberID;
        END

        ELSE IF @Action = N'UNLINK_METER'
        BEGIN
            IF @SubscriberID IS NULL OR @MeterID IS NULL
                THROW 50016, N'المشترك والعداد مطلوبان.', 1;

            IF NOT EXISTS (
                SELECT 1
                FROM dbo.SubscriberMeters
                WHERE SubscriberID = @SubscriberID
                  AND MeterID = @MeterID
            )
                THROW 50020, N'هذا العداد غير مربوط بهذا المشترك.', 1;

            DECLARE @WasPrimary BIT = 0;

            SELECT @WasPrimary = ISNULL(IsPrimary, 0)
            FROM dbo.SubscriberMeters
            WHERE SubscriberID = @SubscriberID
              AND MeterID = @MeterID;

            DELETE FROM dbo.SubscriberMeters
            WHERE SubscriberID = @SubscriberID
              AND MeterID = @MeterID;

            IF @WasPrimary = 1
            BEGIN
                DECLARE @NewPrimaryMeterID INT = NULL;
                DECLARE @NewPrimaryMeterNumber NVARCHAR(50) = NULL;

                SELECT TOP (1) @NewPrimaryMeterID = MeterID
                FROM dbo.SubscriberMeters
                WHERE SubscriberID = @SubscriberID
                ORDER BY LinkedAt DESC, SubscriberMeterID DESC;

                IF @NewPrimaryMeterID IS NOT NULL
                BEGIN
                    UPDATE dbo.SubscriberMeters
                    SET IsPrimary = CASE WHEN MeterID = @NewPrimaryMeterID THEN 1 ELSE 0 END
                    WHERE SubscriberID = @SubscriberID;

                    SELECT @NewPrimaryMeterNumber = MeterNumber
                    FROM dbo.Meters
                    WHERE MeterID = @NewPrimaryMeterID;
                END

                UPDATE dbo.Subscribers
                SET MeterNumber = @NewPrimaryMeterNumber
                WHERE SubscriberID = @SubscriberID;
            END
        END

        ELSE IF @Action = N'DISABLE_SUBSCRIBER'
        BEGIN
            IF @SubscriberID IS NULL OR @SubscriberID <= 0
                THROW 50017, N'معرف المشترك غير صحيح.', 1;

            UPDATE dbo.Subscribers
            SET IsActive = 0
            WHERE SubscriberID = @SubscriberID;

            IF @@ROWCOUNT = 0
                THROW 50021, N'المشترك غير موجود.', 1;
        END

        ELSE
        BEGIN
            THROW 50018, N'قيمة @Action غير معروفة.', 1;
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH
END
GO
