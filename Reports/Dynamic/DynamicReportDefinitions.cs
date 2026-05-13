using System.Collections.Generic;
using System.Linq;

namespace water3.Reports.Dynamic
{
    public static class DynamicReportDefinitions
    {
        public static List<DynamicReportDefinition> GetAll()
        {
            return new List<DynamicReportDefinition>
            {
                GetInvoices(),
                GetPayments(),
                GetReceipts(),
                GetSubscribers(),
                GetMeters(),
                GetReadings(),
                GetExpenses(),
                GetAccountStatements(),
                GetSubscriberBalances()
            };
        }

        public static DynamicReportDefinition GetByKey(string reportKey)
        {
            DynamicReportDefinition def = GetAll()
                .FirstOrDefault(x => x.ReportKey == reportKey);

            return def ?? GetInvoices();
        }
        private static DynamicReportDefinition GetInvoices()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "Invoices";
            def.Title = "تقرير الفواتير";
            def.DateExpression = "i.InvoiceDate";
            def.SubscriberIdExpression = "i.SubscriberID";

            def.BaseSql = @"
FROM dbo.Invoices i
INNER JOIN dbo.Subscribers s ON s.SubscriberID = i.SubscriberID
LEFT JOIN dbo.Meters m ON m.MeterID = i.MeterID
LEFT JOIN dbo.Readings r ON r.ReadingID = i.ReadingID
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY i.InvoiceDate DESC, i.InvoiceID DESC";

            def.SearchExpressions.Add("ISNULL(i.InvoiceNumber, N'')");
            def.SearchExpressions.Add("CAST(i.InvoiceID AS NVARCHAR(50))");
            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
            def.SearchExpressions.Add("ISNULL(s.PhoneNumber, N'')");
            def.SearchExpressions.Add("ISNULL(s.Address, N'')");
            def.SearchExpressions.Add("ISNULL(m.MeterNumber, N'')");
            def.SearchExpressions.Add("ISNULL(i.Status, N'')");
            def.SearchExpressions.Add("ISNULL(i.Notes, N'')");

            // بيانات أساسية
            Add(def, "InvoiceID", "رقم داخلي", "i.InvoiceID", 1, false);

            Add(def, "InvoiceNumber", "رقم الفاتورة",
                "ISNULL(i.InvoiceNumber, N'')",
                2, true);

            Add(def, "InvoiceDate", "تاريخ الفاتورة",
                "i.InvoiceDate",
                3, true);

            // فترة الفاتورة
            // إذا لا يوجد عندك FromDate و ToDate داخل جدول Invoices، هذا يعطي فترة تقريبية:
            // من تاريخ = قبل تاريخ الفاتورة بشهر
            // إلى تاريخ = تاريخ الفاتورة
            Add(def, "FromDate", "من تاريخ",
                "DATEADD(MONTH, -1, CAST(i.InvoiceDate AS DATE))",
                4, false);

            Add(def, "ToDate", "إلى تاريخ",
                "CAST(i.InvoiceDate AS DATE)",
                5, false);

            // بيانات المشترك
            Add(def, "SubscriberName", "اسم المشترك",
                "s.Name",
                6, true);

            Add(def, "SubscriberPhone", "هاتف المشترك",
                "ISNULL(s.PhoneNumber, N'')",
                7, false);

            Add(def, "Address", "العنوان",
                "ISNULL(s.Address, N'')",
                8, false);

            // لا يوجد واضح في الكود الحالي عمود مربع، لذلك نجعله فارغًا حتى لا يعطي SQL Error
            Add(def, "Square", "المربع",
                "N''",
                9, false);

            Add(def, "MeterNumber", "رقم العداد",
                "ISNULL(m.MeterNumber, N'')",
                10, true);

            // القراءات
            Add(def, "PreviousReading", "السابقة",
                "ISNULL(r.PreviousReading, 0)",
                11, false);

            Add(def, "CurrentReading", "الحالية",
                "ISNULL(r.CurrentReading, 0)",
                12, false);

            Add(def, "Consumption", "الاستهلاك",
                "ISNULL(i.Consumption, 0)",
                13, true);

            Add(def, "UnitPrice", "سعر الوحدة",
                "ISNULL(i.UnitPrice, 0)",
                14, true);

            Add(def, "ConsumptionValue", "قيمة الاستهلاك",
                "ISNULL(i.Consumption, 0) * ISNULL(i.UnitPrice, 0)",
                15, false);

            // الرسوم والمتأخرات
            Add(def, "ServiceFees", "رسوم الخدمة",
                "ISNULL(i.ServiceFees, 0)",
                16, true);

            Add(def, "Arrears", "المتأخرات",
                "ISNULL(i.Arrears, 0)",
                17, true);

            // الإجمالي
            Add(def, "TotalAmount", "الإجمالي",
                "ISNULL(i.TotalAmount, 0)",
                18, true);

            // المدفوع من جدول Payments المرتبط بالفاتورة
            Add(def, "PaidAmount", "المدفوع",
                @"ISNULL((
            SELECT SUM(ISNULL(p.Amount, 0))
            FROM dbo.Payments p
            WHERE p.InvoiceID = i.InvoiceID
        ), 0)",
                19, false);

            // الرصيد = الإجمالي - المدفوع
            Add(def, "Balance", "الرصيد",
                @"ISNULL(i.TotalAmount, 0) -
        ISNULL((
            SELECT SUM(ISNULL(p.Amount, 0))
            FROM dbo.Payments p
            WHERE p.InvoiceID = i.InvoiceID
        ), 0)",
                20, false);

            Add(def, "Status", "الحالة",
                "ISNULL(i.Status, N'')",
                21, true);

            Add(def, "Notes", "ملاحظات",
                "ISNULL(i.Notes, N'')",
                22, false);

            return def;
        }

        //        private static DynamicReportDefinition GetInvoices()
        //        {
        //            DynamicReportDefinition def = new DynamicReportDefinition();

        //            def.ReportKey = "Invoices";
        //            def.Title = "تقرير الفواتير";
        //            def.DateExpression = "i.InvoiceDate";
        //            def.SubscriberIdExpression = "i.SubscriberID";

        //            def.BaseSql = @"
        //FROM dbo.Invoices i
        //INNER JOIN dbo.Subscribers s ON s.SubscriberID = i.SubscriberID
        //LEFT JOIN dbo.Meters m ON m.MeterID = i.MeterID
        //LEFT JOIN dbo.Readings r ON r.ReadingID = i.ReadingID
        //WHERE 1 = 1
        //";

        //            def.OrderBySql = "ORDER BY i.InvoiceDate DESC, i.InvoiceID DESC";

        //            def.SearchExpressions.Add("ISNULL(i.InvoiceNumber, N'')");
        //            def.SearchExpressions.Add("CAST(i.InvoiceID AS NVARCHAR(50))");
        //            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
        //            def.SearchExpressions.Add("ISNULL(m.MeterNumber, N'')");
        //            def.SearchExpressions.Add("ISNULL(i.Status, N'')");
        //            def.SearchExpressions.Add("ISNULL(i.Notes, N'')");

        //            Add(def, "InvoiceID", "رقم داخلي", "i.InvoiceID", 1, false);
        //            Add(def, "InvoiceNumber", "رقم الفاتورة", "ISNULL(i.InvoiceNumber, N'')", 2, true);
        //            Add(def, "InvoiceDate", "تاريخ الفاتورة", "i.InvoiceDate", 3, true);
        //            Add(def, "SubscriberName", "اسم المشترك", "s.Name", 4, true);
        //            Add(def, "MeterNumber", "رقم العداد", "ISNULL(m.MeterNumber, N'')", 5, true);
        //            Add(def, "PreviousReading", "القراءة السابقة", "ISNULL(r.PreviousReading, 0)", 6, false);
        //            Add(def, "CurrentReading", "القراءة الحالية", "ISNULL(r.CurrentReading, 0)", 7, false);
        //            Add(def, "Consumption", "الاستهلاك", "ISNULL(i.Consumption, 0)", 8, true);
        //            Add(def, "UnitPrice", "سعر الوحدة", "ISNULL(i.UnitPrice, 0)", 9, true);
        //            Add(def, "ServiceFees", "رسوم الخدمة", "ISNULL(i.ServiceFees, 0)", 10, true);
        //            Add(def, "Arrears", "المتأخرات", "ISNULL(i.Arrears, 0)", 11, true);
        //            Add(def, "TotalAmount", "الإجمالي", "ISNULL(i.TotalAmount, 0)", 12, true);
        //            Add(def, "Status", "الحالة", "ISNULL(i.Status, N'')", 13, true);
        //            Add(def, "Notes", "ملاحظات", "ISNULL(i.Notes, N'')", 14, false);

        //            return def;
        //        }

        private static DynamicReportDefinition GetPayments()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "Payments";
            def.Title = "تقرير السداد";
            def.DateExpression = "p.PaymentDate";
            def.SubscriberIdExpression = "p.SubscriberID";

            def.BaseSql = @"
FROM dbo.Payments p
INNER JOIN dbo.Subscribers s ON s.SubscriberID = p.SubscriberID
LEFT JOIN dbo.Collectors c ON c.CollectorID = p.CollectorID
LEFT JOIN dbo.Receipts r ON r.ReceiptID = p.ReceiptID
LEFT JOIN dbo.Invoices i ON i.InvoiceID = p.InvoiceID
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY p.PaymentDate DESC, p.PaymentID DESC";

            def.SearchExpressions.Add("CAST(p.PaymentID AS NVARCHAR(50))");
            def.SearchExpressions.Add("ISNULL(r.ReceiptNumber, ISNULL(p.ReceiptNumber, N''))");
            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
            def.SearchExpressions.Add("ISNULL(c.Name, N'')");
            def.SearchExpressions.Add("ISNULL(p.PaymentType, N'')");
            def.SearchExpressions.Add("ISNULL(p.PaymentCategory, N'')");
            def.SearchExpressions.Add("ISNULL(p.Notes, N'')");

            Add(def, "PaymentID", "رقم السداد", "p.PaymentID", 1, false);
            Add(def, "PaymentDate", "تاريخ السداد", "p.PaymentDate", 2, true);
            Add(def, "ReceiptNumber", "رقم الإيصال", "ISNULL(r.ReceiptNumber, ISNULL(p.ReceiptNumber, N''))", 3, true);
            Add(def, "SubscriberName", "اسم المشترك", "s.Name", 4, true);
            Add(def, "InvoiceNumber", "رقم الفاتورة", "ISNULL(i.InvoiceNumber, CAST(p.InvoiceID AS NVARCHAR(50)))", 5, false);
            Add(def, "CollectorName", "المحصل", "ISNULL(c.Name, N'')", 6, true);
            Add(def, "Amount", "المبلغ", "ISNULL(p.Amount, 0)", 7, true);
            Add(def, "PaymentType", "طريقة الدفع", "ISNULL(p.PaymentType, N'')", 8, true);
            Add(def, "PaymentCategory", "نوع السداد", "ISNULL(p.PaymentCategory, N'')", 9, true);
            Add(def, "Notes", "ملاحظات", "ISNULL(p.Notes, N'')", 10, false);

            return def;
        }

        private static DynamicReportDefinition GetReceipts()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "Receipts";
            def.Title = "تقرير الإيصالات";
            def.DateExpression = "r.PaymentDate";
            def.SubscriberIdExpression = "r.SubscriberID";

            def.BaseSql = @"
FROM dbo.Receipts r
INNER JOIN dbo.Subscribers s ON s.SubscriberID = r.SubscriberID
LEFT JOIN dbo.Collectors c ON c.CollectorID = r.CollectorID
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY r.PaymentDate DESC, r.ReceiptID DESC";

            def.SearchExpressions.Add("ISNULL(r.ReceiptNumber, N'')");
            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
            def.SearchExpressions.Add("ISNULL(c.Name, N'')");
            def.SearchExpressions.Add("ISNULL(r.PaymentMethod, N'')");
            def.SearchExpressions.Add("ISNULL(r.Status, N'')");
            def.SearchExpressions.Add("ISNULL(r.Notes, N'')");

            Add(def, "ReceiptID", "رقم داخلي", "r.ReceiptID", 1, false);
            Add(def, "ReceiptNumber", "رقم الإيصال", "r.ReceiptNumber", 2, true);
            Add(def, "PaymentDate", "تاريخ الإيصال", "r.PaymentDate", 3, true);
            Add(def, "SubscriberName", "اسم المشترك", "s.Name", 4, true);
            Add(def, "CollectorName", "المحصل", "ISNULL(c.Name, N'')", 5, true);
            Add(def, "TotalAmount", "المبلغ", "r.TotalAmount", 6, true);
            Add(def, "PaymentMethod", "طريقة الدفع", "r.PaymentMethod", 7, true);
            Add(def, "Status", "الحالة", "r.Status", 8, true);
            Add(def, "Notes", "ملاحظات", "ISNULL(r.Notes, N'')", 9, false);
            Add(def, "CreatedAt", "تاريخ الإنشاء", "r.CreatedAt", 10, false);

            return def;
        }

        private static DynamicReportDefinition GetSubscribers()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "Subscribers";
            def.Title = "تقرير المشتركين";
            def.DateExpression = "s.CreatedDate";
            def.SubscriberIdExpression = "s.SubscriberID";

            def.BaseSql = @"
FROM dbo.Subscribers s
LEFT JOIN dbo.Accounts a ON a.AccountID = s.AccountID
LEFT JOIN dbo.TariffPlans tp ON tp.TariffPlanID = s.TariffPlanID
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY s.Name";

            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
            def.SearchExpressions.Add("ISNULL(s.PhoneNumber, N'')");
            def.SearchExpressions.Add("ISNULL(s.Address, N'')");
            def.SearchExpressions.Add("ISNULL(a.AccountCode, N'')");

            Add(def, "SubscriberID", "رقم المشترك", "s.SubscriberID", 1, false);
            Add(def, "AccountCode", "رقم الحساب", "ISNULL(a.AccountCode, N'')", 2, true);
            Add(def, "SubscriberName", "اسم المشترك", "s.Name", 3, true);
            Add(def, "PhoneNumber", "الهاتف", "ISNULL(s.PhoneNumber, N'')", 4, true);
            Add(def, "Address", "العنوان", "ISNULL(s.Address, N'')", 5, true);
            Add(def, "CreatedDate", "تاريخ الإضافة", "s.CreatedDate", 6, false);
            Add(def, "TariffPlan", "خطة التعرفة", "ISNULL(tp.PlanName, N'')", 7, false);
            Add(def, "IsActive", "نشط", "CASE WHEN ISNULL(s.IsActive, 0) = 1 THEN N'نعم' ELSE N'لا' END", 8, true);

            return def;
        }

        private static DynamicReportDefinition GetMeters()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "Meters";
            def.Title = "تقرير العدادات";
            def.DateExpression = "m.CreatedAt";

            def.BaseSql = @"
FROM dbo.Meters m
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY m.MeterNumber";

            def.SearchExpressions.Add("ISNULL(m.MeterNumber, N'')");
            def.SearchExpressions.Add("ISNULL(m.MeterType, N'')");
            def.SearchExpressions.Add("ISNULL(m.Location, N'')");

            Add(def, "MeterID", "رقم داخلي", "m.MeterID", 1, false);
            Add(def, "MeterNumber", "رقم العداد", "m.MeterNumber", 2, true);
            Add(def, "MeterType", "نوع العداد", "m.MeterType", 3, true);
            Add(def, "Location", "الموقع", "ISNULL(m.Location, N'')", 4, true);
            Add(def, "IsActive", "نشط", "CASE WHEN ISNULL(m.IsActive, 0) = 1 THEN N'نعم' ELSE N'لا' END", 5, true);
            Add(def, "CreatedAt", "تاريخ الإضافة", "m.CreatedAt", 6, false);

            return def;
        }

        private static DynamicReportDefinition GetReadings()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "Readings";
            def.Title = "تقرير القراءات";
            def.DateExpression = "r.ReadingDate";
            def.SubscriberIdExpression = "r.SubscriberID";

            def.BaseSql = @"
FROM dbo.Readings r
INNER JOIN dbo.Subscribers s ON s.SubscriberID = r.SubscriberID
LEFT JOIN dbo.Meters m ON m.MeterID = r.MeterID
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY r.ReadingDate DESC, r.ReadingID DESC";

            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
            def.SearchExpressions.Add("ISNULL(m.MeterNumber, N'')");
            def.SearchExpressions.Add("ISNULL(r.Notes, N'')");

            Add(def, "ReadingID", "رقم القراءة", "r.ReadingID", 1, false);
            Add(def, "ReadingDate", "تاريخ القراءة", "r.ReadingDate", 2, true);
            Add(def, "SubscriberName", "اسم المشترك", "s.Name", 3, true);
            Add(def, "MeterNumber", "رقم العداد", "ISNULL(m.MeterNumber, N'')", 4, true);
            Add(def, "PreviousReading", "القراءة السابقة", "ISNULL(r.PreviousReading, 0)", 5, true);
            Add(def, "CurrentReading", "القراءة الحالية", "ISNULL(r.CurrentReading, 0)", 6, true);
            Add(def, "Consumption", "الاستهلاك", "ISNULL(r.Consumption, 0)", 7, true);
            Add(def, "Notes", "ملاحظات", "ISNULL(r.Notes, N'')", 8, false);

            return def;
        }

        private static DynamicReportDefinition GetExpenses()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "Expenses";
            def.Title = "تقرير المصروفات والمشتريات والخسائر";
            def.DateExpression = "e.ExpenseDate";
            def.CategoryTypeExpression = "c.CategoryType";

            def.BaseSql = @"
FROM dbo.Expenses e
INNER JOIN dbo.ExpenseCategories c ON c.CategoryID = e.CategoryID
LEFT JOIN dbo.Users u ON u.UserID = e.CreatedBy
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY e.ExpenseDate DESC, e.ExpenseID DESC";

            def.SearchExpressions.Add("ISNULL(e.ExpenseNumber, N'')");
            def.SearchExpressions.Add("ISNULL(c.CategoryName, N'')");
            def.SearchExpressions.Add("ISNULL(e.SupplierName, N'')");
            def.SearchExpressions.Add("ISNULL(e.Description, N'')");
            def.SearchExpressions.Add("ISNULL(e.Notes, N'')");
            def.SearchExpressions.Add("ISNULL(e.PaymentMethod, N'')");
            def.SearchExpressions.Add("ISNULL(e.Status, N'')");

            Add(def, "ExpenseID", "رقم داخلي", "e.ExpenseID", 1, false);
            Add(def, "ExpenseNumber", "رقم السند", "e.ExpenseNumber", 2, true);
            Add(def, "ExpenseDate", "التاريخ", "e.ExpenseDate", 3, true);
            Add(def, "CategoryName", "التصنيف", "c.CategoryName", 4, true);
            Add(def, "CategoryType", "النوع", "c.CategoryType", 5, true);
            Add(def, "SupplierName", "المورد/الجهة", "ISNULL(e.SupplierName, N'')", 6, true);
            Add(def, "Description", "البيان", "ISNULL(e.Description, N'')", 7, true);
            Add(def, "TotalAmount", "المبلغ", "e.TotalAmount", 8, true);
            Add(def, "PaymentMethod", "طريقة الدفع", "e.PaymentMethod", 9, false);
            Add(def, "Status", "الحالة", "e.Status", 10, false);
            Add(def, "CreatedBy", "أنشئ بواسطة", "ISNULL(u.FullName, N'')", 11, false);
            Add(def, "Notes", "ملاحظات", "ISNULL(e.Notes, N'')", 12, false);

            return def;
        }

        private static DynamicReportDefinition GetAccountStatements()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "AccountStatements";
            def.Title = "كشف الحساب الديناميكي";
            def.DateExpression = "st.[Date]";
            def.SubscriberIdExpression = "st.SubscriberID";

            def.BaseSql = @"
FROM dbo.AccountStatements st
INNER JOIN dbo.Subscribers s ON s.SubscriberID = st.SubscriberID
LEFT JOIN dbo.Meters m ON m.MeterID = st.MeterID
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY st.[Date] DESC, st.StatementID DESC";

            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
            def.SearchExpressions.Add("ISNULL(m.MeterNumber, N'')");
            def.SearchExpressions.Add("ISNULL(st.Details, N'')");
            def.SearchExpressions.Add("ISNULL(st.DocumentType, N'')");
            def.SearchExpressions.Add("ISNULL(st.DocumentNumber, N'')");

            Add(def, "StatementID", "رقم الحركة", "st.StatementID", 1, false);
            Add(def, "Date", "التاريخ", "st.[Date]", 2, true);
            Add(def, "SubscriberName", "اسم المشترك", "s.Name", 3, true);
            Add(def, "MeterNumber", "رقم العداد", "ISNULL(m.MeterNumber, N'')", 4, true);
            Add(def, "Details", "البيان", "ISNULL(st.Details, N'')", 5, true);
            Add(def, "DocumentType", "نوع المستند", "ISNULL(st.DocumentType, N'')", 6, true);
            Add(def, "DocumentNumber", "رقم المستند", "ISNULL(st.DocumentNumber, N'')", 7, true);
            Add(def, "Debit", "مدين", "ISNULL(st.Debit, 0)", 8, true);
            Add(def, "Credit", "دائن", "ISNULL(st.Credit, 0)", 9, true);
            Add(def, "BalanceAfter", "الرصيد بعد", "ISNULL(st.BalanceAfter, 0)", 10, true);

            return def;
        }

        private static DynamicReportDefinition GetSubscriberBalances()
        {
            DynamicReportDefinition def = new DynamicReportDefinition();

            def.ReportKey = "SubscriberBalances";
            def.Title = "تقرير أرصدة المشتركين";
            def.SubscriberIdExpression = "s.SubscriberID";

            def.BaseSql = @"
FROM dbo.Subscribers s
LEFT JOIN dbo.vw_SubscriberBalance b ON b.SubscriberID = s.SubscriberID
WHERE 1 = 1
";

            def.OrderBySql = "ORDER BY s.Name";

            def.SearchExpressions.Add("ISNULL(s.Name, N'')");
            def.SearchExpressions.Add("ISNULL(s.PhoneNumber, N'')");
            def.SearchExpressions.Add("ISNULL(s.Address, N'')");

            Add(def, "SubscriberID", "رقم المشترك", "s.SubscriberID", 1, false);
            Add(def, "SubscriberName", "اسم المشترك", "s.Name", 2, true);
            Add(def, "PhoneNumber", "الهاتف", "ISNULL(s.PhoneNumber, N'')", 3, true);
            Add(def, "Address", "العنوان", "ISNULL(s.Address, N'')", 4, false);
            Add(def, "Balance", "الرصيد", "ISNULL(b.Balance, 0)", 5, true);
            Add(def, "BalanceStatus", "حالة الرصيد", "CASE WHEN ISNULL(b.Balance,0) > 0 THEN N'عليه' WHEN ISNULL(b.Balance,0) < 0 THEN N'له' ELSE N'متوازن' END", 6, true);

            return def;
        }

        private static void Add(
            DynamicReportDefinition def,
            string key,
            string title,
            string sql,
            int order,
            bool visible)
        {
            def.Columns.Add(new DynamicReportColumn
            {
                ColumnKey = key,
                ColumnTitle = title,
                SqlExpression = sql,
                DisplayOrder = order,
                IsDefaultVisible = visible
            });
        }
    }
}