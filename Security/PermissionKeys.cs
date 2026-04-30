namespace water3.Security
{
    public static class PermissionKeys
    {
        // =========================
        // الرئيسية والتشغيل
        // =========================
        public const string DashboardView = "DASHBOARD_VIEW";
        public const string InvoicesView = "INVOICES_VIEW";
        public const string SubscribersView = "SUBSCRIBERS_VIEW";
        public const string PaymentsView = "PAYMENTS_VIEW";
        public const string ReadingsView = "READINGS_VIEW";
        public const string ReadingEntryView = "READING_ENTRY_VIEW";
        public const string UnpaidInvoicesView = "UNPAID_INVOICES_VIEW";
        public const string InvoicePrintView = "INVOICE_PRINT_VIEW";

        // =========================
        // المحصلون والمزامنة
        // =========================
        public const string CollectorsView = "COLLECTORS_VIEW";
        public const string CollectorsManage = "COLLECTORS_MANAGE";
        public const string CollectorsLinkUser = "COLLECTORS_LINK_USER";

        public const string CollectorDevicesView = "COLLECTOR_DEVICES_VIEW";
        public const string CollectorDevicesManage = "COLLECTOR_DEVICES_MANAGE";
        public const string CollectorDevicesApprove = "COLLECTOR_DEVICES_APPROVE";

        public const string MobileSyncView = "MOBILE_SYNC_VIEW";
        public const string MobileSyncToPhoneView = "MOBILE_SYNC_TO_PHONE_VIEW";

        // =========================
        // المستخدمون والصلاحيات
        // =========================
        public const string UsersView = "USERS_VIEW";
        public const string UsersManage = "USERS_MANAGE";
        public const string UsersCreate = "USERS_CREATE";
        public const string RolesManage = "ROLES_MANAGE";

        // =========================
        // السجلات والضبط
        // =========================
        public const string AuditLogView = "AUDITLOG_VIEW";
        public const string DbSettingsView = "DB_SETTINGS_VIEW";
        public const string BillingConstantsView = "BILLING_CONSTANTS_VIEW";

        // =========================
        // الرسائل
        // =========================
        public const string SmsLogsView = "SMS_LOGS_VIEW";
        public const string SmsReportView = "SMS_REPORT_VIEW";
        public const string MessagesManage = "MESSAGES_MANAGE";

        // =========================
        // التقارير الأساسية
        // =========================
        public const string ReportsCenterView = "REPORTS_CENTER_VIEW";

        public const string SubscribersReportView = "SUBSCRIBERS_REPORT_VIEW";
        public const string CollectorsReportView = "COLLECTORS_REPORT_VIEW";
        public const string AccountStatementView = "ACCOUNT_STATEMENT_VIEW";

        public const string ReportInvoicesView = "REPORT_INVOICES_VIEW";
        public const string ReportPaymentsView = "REPORT_PAYMENTS_VIEW";
        public const string ReportReceiptsView = "REPORT_RECEIPTS_VIEW";
        public const string ReportAccountStatementView = "REPORT_ACCOUNTSTATEMENT_VIEW";
        public const string ReportAgingView = "REPORT_AGING_VIEW";
        public const string ReportGeneralJournalView = "REPORT_GENERALJOURNAL_VIEW";
        public const string ReportTrialBalanceView = "REPORT_TRIALBALANCE_VIEW";

        public const string ReportExport = "REPORT_EXPORT";
        public const string ReportPrint = "REPORT_PRINT";

        // =========================
        // تقارير التشغيل والمزامنة
        // =========================
        public const string ReportCollectorCollectionsView = "REPORT_COLLECTOR_COLLECTIONS_VIEW";
        public const string ReportCollectorDevicesView = "REPORT_COLLECTOR_DEVICES_VIEW";
        public const string ReportMobileSyncBatchesView = "REPORT_MOBILE_SYNC_BATCHES_VIEW";
        public const string ReportMobileSyncErrorsView = "REPORT_MOBILE_SYNC_ERRORS_VIEW";
        public const string ReportCollectorSubscribersView = "REPORT_COLLECTOR_SUBSCRIBERS_VIEW";
        public const string ReportPhonePushView = "REPORT_PHONE_PUSH_VIEW";

        // =========================
        // العداد الرئيسي والفاقد
        // =========================
        public const string MainMeterReportView = "MAIN_METER_REPORT_VIEW";
        public const string MainMeterReportPrint = "MAIN_METER_REPORT_PRINT";

        // =========================
        // المصروفات والمشتريات والخسائر
        // =========================
        public const string ExpensesView = "EXPENSES_VIEW";
        public const string ExpensesManage = "EXPENSES_MANAGE";
        public const string ExpensesPrint = "EXPENSES_PRINT";

        public const string ExpenseCategoriesManage = "EXPENSE_CATEGORIES_MANAGE";

        public const string ExpenseReportsView = "EXPENSE_REPORTS_VIEW";
        public const string ExpenseReportsPrint = "EXPENSE_REPORTS_PRINT";

        // =========================
        // سندات وتقارير المصروفات
        // =========================
        public const string ExpenseVoucherPrint = "EXPENSE_VOUCHER_PRINT";
        public const string ExpenseSummaryView = "EXPENSE_SUMMARY_VIEW";
        public const string PurchaseReportView = "PURCHASE_REPORT_VIEW";
        public const string LossReportView = "LOSS_REPORT_VIEW";
    }
}