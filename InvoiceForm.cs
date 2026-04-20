
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace water3
{
    public partial class InvoiceForm : Form
    {
        // بيانات البحث
        private List<ComboItem> allSubscribers = new List<ComboItem>();
        private List<ComboItem> allInvoices = new List<ComboItem>();
        private bool suppressTextChanged = false;

        // مؤقتات لحساب الفترة
        private int _tempSubscriberID = 0;
        private DateTime _tempReadingDate = DateTime.MinValue;

        public InvoiceForm()
        {
            InitializeComponent();

            InitializeForm();
            ApplyTheme();
            SetupGrid();
            WireEvents();

            LoadSubscribersAndInvoices();
        }

        private void InitializeForm()
        {
            Text = "عرض الفواتير";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void ApplyTheme()
        {
            Font = new Font("Segoe UI", 10);

            lblSearchTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // أزرار
            StyleButton(btnLoadDetails, Color.SeaGreen, Color.White);
            StyleButton(btnReload, Color.DodgerBlue, Color.White);
            StyleButton(btnPrint, Color.DodgerBlue, Color.White);
            btnPrint.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            // Combo
            ddlSearch.Font = new Font("Segoe UI", 10);

            // زر الطباعة: توسيط بدون إحداثيات سلبية
            pnlBottom.Padding = new Padding(180, 7, 180, 7);
            btnPrint.Dock = DockStyle.Fill;
        }

        private void StyleButton(Button b, Color back, Color fore)
        {
            b.BackColor = back;
            b.ForeColor = fore;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Height = 32;
        }

        private void SetupGrid()
        {
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
        }

        private void WireEvents()
        {
            ddlSearch.TextChanged += DdlSearch_TextChanged;
            ddlSearch.KeyDown += DdlSearch_KeyDown;
            ddlSearch.SelectionChangeCommitted += DdlSearch_SelectionChangeCommitted;

            btnLoadDetails.Click += BtnLoadDetails_Click;
            btnReload.Click += (s, e) => LoadSubscribersAndInvoices();

            dgv.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgv.Rows[e.RowIndex].Cells[0].Value == null) return;

                string invoiceId = dgv.Rows[e.RowIndex].Cells[0].Value.ToString();
                LoadInvoiceDetails(invoiceId);
            };

            btnPrint.Click += (s, e) =>
            {
                try { browser.ShowPrintPreviewDialog(); }
                catch { browser.Print(); }
            };
        }

        // =========================
        // Load subscribers + invoices into ddlSearch
        // =========================
        private void LoadSubscribersAndInvoices()
        {
            allSubscribers.Clear();
            allInvoices.Clear();

            ddlSearch.Items.Clear();
            ddlSearch.AutoCompleteCustomSource.Clear();

            using (SqlConnection con = Db.GetConnection())
            {
                // Subscribers
                SqlDataAdapter daSub = new SqlDataAdapter("SELECT SubscriberID, Name FROM Subscribers ORDER BY Name", con);
                DataTable dtSub = new DataTable();
                daSub.Fill(dtSub);

                foreach (DataRow r in dtSub.Rows)
                {
                    var item = new ComboItem(r["Name"].ToString(), r["SubscriberID"].ToString());
                    allSubscribers.Add(item);
                    ddlSearch.AutoCompleteCustomSource.Add(item.Text);
                }

                // Invoices
                SqlDataAdapter daInv = new SqlDataAdapter("SELECT InvoiceID, SubscriberID FROM Invoices ORDER BY InvoiceID DESC", con);
                DataTable dtInv = new DataTable();
                daInv.Fill(dtInv);

                foreach (DataRow r in dtInv.Rows)
                {
                    string subId = r["SubscriberID"].ToString();
                    string subscriberName =
                        dtSub.AsEnumerable()
                             .FirstOrDefault(x => x["SubscriberID"].ToString() == subId)?["Name"].ToString() ?? "";

                    var item = new ComboItem("فاتورة #" + r["InvoiceID"] + " - " + subscriberName, r["InvoiceID"].ToString());
                    allInvoices.Add(item);
                    ddlSearch.AutoCompleteCustomSource.Add(item.Text);
                }

                foreach (var item in allSubscribers) ddlSearch.Items.Add(item);
                foreach (var item in allInvoices) ddlSearch.Items.Add(item);
            }
        }

        private void DdlSearch_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChanged) return;

            string search = ddlSearch.Text.Trim();

            if (string.IsNullOrEmpty(search))
            {
                ddlSearch.Items.Clear();
                foreach (var item in allSubscribers) ddlSearch.Items.Add(item);
                foreach (var item in allInvoices) ddlSearch.Items.Add(item);
                ddlSearch.DroppedDown = false;
                return;
            }

            suppressTextChanged = true;

            var filtered =
                allSubscribers.Where(x => x.Text.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                .Concat(allInvoices.Where(x => x.Text.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();

            ddlSearch.Items.Clear();
            foreach (var item in filtered) ddlSearch.Items.Add(item);

            ddlSearch.DroppedDown = filtered.Count > 0 && ddlSearch.Focused;

            suppressTextChanged = false;
        }

        private void DdlSearch_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ddlSearch.SelectedItem is ComboItem item)
            {
                LoadSelectedInvoiceOrSubscriber(item);
                ddlSearch.DroppedDown = false;
            }
        }

        private void DdlSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && ddlSearch.SelectedItem is ComboItem item)
            {
                LoadSelectedInvoiceOrSubscriber(item);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void LoadSelectedInvoiceOrSubscriber(ComboItem item)
        {
            // Invoice selected
            if (allInvoices.Contains(item))
            {
                LoadInvoiceDetails(item.Value);
                return;
            }

            // Subscriber selected => latest invoice
            using (SqlConnection con = Db.GetConnection())
            using (SqlDataAdapter da = new SqlDataAdapter(
                "SELECT TOP 1 InvoiceID FROM Invoices WHERE SubscriberID=@id ORDER BY InvoiceDate DESC, InvoiceID DESC", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@id", item.Value);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                    LoadInvoiceDetails(dt.Rows[0]["InvoiceID"].ToString());
                else
                    MessageBox.Show("لا توجد فواتير لهذا المشترك", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnLoadDetails_Click(object sender, EventArgs e)
        {
            if (ddlSearch.SelectedItem is ComboItem item)
            {
                string invoiceId = item.Value;

                // إذا مشترك: احصل على آخر فاتورة
                if (!allInvoices.Contains(item))
                {
                    invoiceId = GetLatestInvoiceIdForSubscriber(item.Value);
                    if (string.IsNullOrEmpty(invoiceId))
                    {
                        MessageBox.Show("لا توجد فواتير لهذا المشترك", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                // ✅ عرض RDLC
                ShowInvoiceRdlc(invoiceId);
            }
            else
            {
                MessageBox.Show("اختر مشترك أو فاتورة من القائمة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // =========================
        // RDLC
        // =========================
        private void ShowInvoiceRdlc(string invoiceID)
        {
            try
            {
                DataTable dt = BuildInvoiceDataTable();
                FillInvoiceRow(dt, invoiceID);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات لهذه الفاتورة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Form reportForm = new Form()
                {
                    Width = 1100,
                    Height = 800,
                    Text = "فاتورة استهلاك مياه",
                    StartPosition = FormStartPosition.CenterScreen
                };

                ReportViewer rv = new ReportViewer()
                {
                    Dock = DockStyle.Fill,
                    ProcessingMode = ProcessingMode.Local
                };
                reportForm.Controls.Add(rv);

                string rdlcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "Invoice_Water_2008_compatible.rdlc");
                if (!File.Exists(rdlcPath))
                {
                    MessageBox.Show("ملف RDLC غير موجود:\n" + rdlcPath, "RDLC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                rv.ReportError += (s, e) =>
                {
                    MessageBox.Show(e.Exception.ToString(), "ReportError", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                rv.Reset();
                rv.LocalReport.ReportPath = rdlcPath;
                rv.LocalReport.DataSources.Clear();
                rv.LocalReport.DataSources.Add(new ReportDataSource("dsInvoice", dt));

                rv.LocalReport.EnableExternalImages = true;

                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                string logoUri = File.Exists(logoPath) ? new Uri(logoPath).AbsoluteUri : "about:blank";

                ReportParameter[] ps = new ReportParameter[]
                {
                    new ReportParameter("pLogoPath", logoUri),
                    new ReportParameter("pAccentColor", "#0076D6")
                };
                rv.LocalReport.SetParameters(ps);

                rv.RefreshReport();
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // Data Table helpers
        // =========================
        private DataTable BuildInvoiceDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProjectNameEn", typeof(string));
            dt.Columns.Add("ProjectNameAr", typeof(string));
            dt.Columns.Add("AddressLine", typeof(string));

            dt.Columns.Add("InvoiceNo", typeof(string));
            dt.Columns.Add("InvoiceDate", typeof(DateTime));

            dt.Columns.Add("PeriodFrom", typeof(DateTime));
            dt.Columns.Add("PeriodTo", typeof(DateTime));

            dt.Columns.Add("SubscriberName", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("MeterNumber", typeof(string));
            dt.Columns.Add("MeterId", typeof(string));

            dt.Columns.Add("PrevReading", typeof(decimal));
            dt.Columns.Add("CurrReading", typeof(decimal));
            dt.Columns.Add("Consumption", typeof(decimal));

            dt.Columns.Add("UnitPrice", typeof(decimal));
            dt.Columns.Add("ConsumptionAmount", typeof(decimal));
            dt.Columns.Add("ServiceFees", typeof(decimal));

            // ✅ ترحيل المتأخرات
            dt.Columns.Add("Arrears", typeof(decimal));      // المتأخرات المخزنة في الفاتورة
            dt.Columns.Add("TotalDue", typeof(decimal));     // إجمالي الفاتورة = TotalAmount + StoredArrears

            dt.Columns.Add("PaidPrev", typeof(decimal));     // المدفوع لهذه الفاتورة
            dt.Columns.Add("CurrentBalance", typeof(decimal)); // المتبقي على الفاتورة

            dt.Columns.Add("TotalWords", typeof(string));
            dt.Columns.Add("Note1", typeof(string));
            dt.Columns.Add("Note2", typeof(string));
            return dt;
        }

        private void FillInvoiceRow(DataTable dt, string invoiceID)
        {
            int subscriberID = 0;

            string subscriberName = "";
            string meter = "";
            string address = "";
            string mobile = "";
            DateTime readingDate = DateTime.Today;

            decimal prevReading = 0m;
            decimal currReading = 0m;
            decimal consumption = 0m;
            decimal unitPrice = 0m;
            decimal serviceFees = 0m;

            // ✅ مبالغ الفاتورة الصحيحة
            decimal storedArrears = 0m;   // Arrears المخزنة وقت الإصدار
            decimal currentCharges = 0m;  // TotalAmount (مبلغ الفترة)
            decimal invoiceTotal = 0m;    // InvoiceTotal = currentCharges + storedArrears
            decimal paid = 0m;            // Paid لهذه الفاتورة
            decimal balance = 0m;         // المتبقي على الفاتورة

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("GetInvoiceDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceID", invoiceID);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return;

                    subscriberID = dr["SubscriberID"] != DBNull.Value ? Convert.ToInt32(dr["SubscriberID"]) : 0;

                    subscriberName = dr["SubscriberName"] != DBNull.Value ? dr["SubscriberName"].ToString() : "";
                    meter = dr["MeterNumber"] != DBNull.Value ? dr["MeterNumber"].ToString() : "";
                    address = dr["Address"] != DBNull.Value ? dr["Address"].ToString() : "";
                    mobile = dr["PhoneNumber"] != DBNull.Value ? dr["PhoneNumber"].ToString() : "";

                    readingDate = dr["ReadingDate"] != DBNull.Value ? Convert.ToDateTime(dr["ReadingDate"]) : DateTime.Today;

                    prevReading = dr["PreviousReading"] != DBNull.Value ? Convert.ToDecimal(dr["PreviousReading"]) : 0m;
                    currReading = dr["CurrentReading"] != DBNull.Value ? Convert.ToDecimal(dr["CurrentReading"]) : 0m;

                    consumption = dr["Consumption"] != DBNull.Value ? Convert.ToDecimal(dr["Consumption"]) : 0m;
                    unitPrice = dr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dr["UnitPrice"]) : 0m;
                    serviceFees = dr["ServiceFees"] != DBNull.Value ? Convert.ToDecimal(dr["ServiceFees"]) : 0m;

                    // ✅ القيم الجديدة من SP
                    storedArrears = dr["StoredArrears"] != DBNull.Value ? Convert.ToDecimal(dr["StoredArrears"]) : 0m;
                    currentCharges = dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : 0m;   // مبلغ الفترة
                    invoiceTotal = dr["InvoiceTotal"] != DBNull.Value ? Convert.ToDecimal(dr["InvoiceTotal"]) : (currentCharges + storedArrears);

                    paid = dr["Paid"] != DBNull.Value ? Convert.ToDecimal(dr["Paid"]) : 0m;

                    balance = Math.Round(invoiceTotal - paid, 2);
                    if (balance < 0) balance = 0;
                }

                DateTime prevDate = GetPreviousReadingDate(con, subscriberID, readingDate);
                decimal consValue = Math.Round(consumption * unitPrice, 2);

                DataRow r = dt.NewRow();

                r["ProjectNameEn"] = "Al-Madena Project\r\nfor Pure Water";
                r["ProjectNameAr"] = "مشروع المدينة\r\nللمياه النقية";
                r["AddressLine"] = address;

                r["InvoiceNo"] = invoiceID;
                r["InvoiceDate"] = readingDate;

                r["PeriodFrom"] = prevDate;
                r["PeriodTo"] = readingDate;

                r["SubscriberName"] = subscriberName;
                r["City"] = "—";
                r["MeterNumber"] = meter;
                r["MeterId"] = "1";

                r["PrevReading"] = prevReading;
                r["CurrReading"] = currReading;
                r["Consumption"] = consumption;

                r["UnitPrice"] = unitPrice;
                r["ConsumptionAmount"] = consValue;
                r["ServiceFees"] = serviceFees;

                // ✅ ترحيل المتأخرات + إجمالي الفاتورة
                r["Arrears"] = storedArrears;
                r["TotalDue"] = invoiceTotal;

                r["PaidPrev"] = paid;
                r["CurrentBalance"] = balance;

                r["TotalWords"] = NumberToWords(invoiceTotal);

                r["Note1"] = "نرجو سرعة تسديد الفاتورة خلال فترة أقصاها 3 أيام من إصدارها.";
                r["Note2"] = "في حالة التأخير سيتم قطع الخدمة ولن يتم إعادتها إلا بغرامة مالية.";

                dt.Rows.Add(r);
            }
        }

        private DateTime GetPreviousReadingDate(SqlConnection con, int subscriberID, DateTime currentReadingDate)
        {
            using (SqlCommand lastCmd = new SqlCommand(@"
SELECT TOP 1 ReadingDate
FROM Readings
WHERE SubscriberID=@SubscriberID AND ReadingDate < @CurrentReadingDate
ORDER BY ReadingDate DESC, ReadingID DESC;", con))
            {
                lastCmd.Parameters.AddWithValue("@SubscriberID", subscriberID);
                lastCmd.Parameters.AddWithValue("@CurrentReadingDate", currentReadingDate);

                object obj = lastCmd.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                    return Convert.ToDateTime(obj);
            }
            return currentReadingDate;
        }

        private string GetLatestInvoiceIdForSubscriber(string subscriberId)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TOP 1 InvoiceID FROM Invoices WHERE SubscriberID=@id ORDER BY InvoiceDate DESC, InvoiceID DESC", con))
            {
                cmd.Parameters.AddWithValue("@id", subscriberId);
                con.Open();
                object obj = cmd.ExecuteScalar();
                return (obj == null || obj == DBNull.Value) ? null : obj.ToString();
            }
        }

        // =========================
        // HTML preview + table
        // =========================
        private void LoadInvoiceDetails(string invoiceID)
        {
            try
            {
                string htmlPath = Path.Combine(Application.StartupPath, "invoice1.html");
                if (!File.Exists(htmlPath))
                    throw new FileNotFoundException("ملف invoice1.html غير موجود في مجلد التشغيل", htmlPath);

                string html = File.ReadAllText(htmlPath);

                // صور/شعار (URI)
                string logoPath = Path.Combine(Application.StartupPath, "logo.png");
                string logoUri = File.Exists(logoPath) ? new Uri(logoPath).AbsoluteUri : "about:blank";
                html = html.Replace("logo.png", logoUri);

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("GetInvoiceDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceID);

                    con.Open();
                    DateTime readingDate = DateTime.Today;
                    int subscriberID = 0;

                    // بيانات الفاتورة
                    string subscriberName = "", meter = "", address = "", mobile = "";
                    decimal prevReading = 0m, currReading = 0m, consumption = 0m, unitPrice = 0m, serviceFees = 0m;
                    decimal storedArrears = 0m, currentCharges = 0m, invoiceTotal = 0m, paid = 0m, balance = 0m;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                            throw new Exception("الفاتورة غير موجودة.");

                        subscriberID = dr["SubscriberID"] != DBNull.Value ? Convert.ToInt32(dr["SubscriberID"]) : 0;

                        subscriberName = dr["SubscriberName"]?.ToString() ?? "";
                        meter = dr["MeterNumber"]?.ToString() ?? "";
                        address = dr["Address"]?.ToString() ?? "";
                        mobile = dr["PhoneNumber"]?.ToString() ?? "";
                        readingDate = dr["ReadingDate"] != DBNull.Value ? Convert.ToDateTime(dr["ReadingDate"]) : DateTime.Today;

                        prevReading = dr["PreviousReading"] != DBNull.Value ? Convert.ToDecimal(dr["PreviousReading"]) : 0m;
                        currReading = dr["CurrentReading"] != DBNull.Value ? Convert.ToDecimal(dr["CurrentReading"]) : 0m;
                        consumption = dr["Consumption"] != DBNull.Value ? Convert.ToDecimal(dr["Consumption"]) : 0m;
                        unitPrice = dr["UnitPrice"] != DBNull.Value ? Convert.ToDecimal(dr["UnitPrice"]) : 0m;
                        serviceFees = dr["ServiceFees"] != DBNull.Value ? Convert.ToDecimal(dr["ServiceFees"]) : 0m;

                        decimal consValue = Math.Round(consumption * unitPrice, 2);

                        storedArrears = dr["StoredArrears"] != DBNull.Value ? Convert.ToDecimal(dr["StoredArrears"]) : 0m;
                        currentCharges = dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"]) : Math.Round(consValue + serviceFees, 2);
                        invoiceTotal = dr["InvoiceTotal"] != DBNull.Value ? Convert.ToDecimal(dr["InvoiceTotal"]) : Math.Round(currentCharges + storedArrears, 2);

                        paid = dr["Paid"] != DBNull.Value ? Convert.ToDecimal(dr["Paid"]) : 0m;

                        balance = Math.Round(invoiceTotal - paid, 2);
                        if (balance < 0) balance = 0;

                        string totalWords = NumberToWords(invoiceTotal);

                        // تاريخ الفترة
                        DateTime prevDate = GetPreviousReadingDate(con, subscriberID, readingDate);
                        string period = prevDate.ToString("yyyy/MM/dd") + " - " + readingDate.ToString("yyyy/MM/dd");

                        // تعبئة placeholders (نفس التصميم)
                        html = html.Replace("{{Name}}", subscriberName)
                                   .Replace("{{Meter}}", meter)
                                   .Replace("{{Address}}", address)
                                   .Replace("{{InvoiceNo}}", invoiceID)
                                   .Replace("{{Date}}", readingDate.ToString("yyyy/MM/dd"))
                                   .Replace("{{Prev}}", prevReading.ToString("N0"))
                                   .Replace("{{Curr}}", currReading.ToString("N0"))
                                   .Replace("{{Cons}}", consumption.ToString("N0"))
                                   .Replace("{{UnitPrice}}", unitPrice.ToString("N2"))
                                   .Replace("{{ServiceFees}}", serviceFees.ToString("N2"))
                                   .Replace("{{ConsValue}}", consValue.ToString("N2"))
                                   .Replace("{{Arrears}}", storedArrears.ToString("N2"))
                                   .Replace("{{Total}}", invoiceTotal.ToString("N2"))
                                   .Replace("{{Paid}}", paid.ToString("N2"))
                                   .Replace("{{Balance}}", balance.ToString("N2"))
                                   .Replace("{{Mobile}}", mobile)
                                   .Replace("{{Account}}", "—")
                                   .Replace("{{Square}}", "1")
                                   .Replace("{{TotalWords}}", totalWords)
                                   .Replace("{{FromDate}}", prevDate.ToString("yyyy/MM/dd"))
                                   .Replace("{{ToDate}}", readingDate.ToString("yyyy/MM/dd"))
                                   .Replace("{{Period}}", period);

                        // حفظ مؤقت
                        _tempSubscriberID = subscriberID;
                        _tempReadingDate = readingDate;
                    }
                }

                browser.DocumentText = html;
                LoadInvoicesTable(invoiceID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadInvoicesTable(string selectedInvoiceID = null)
        {
            try
            {
                int subscriberID = 0;

                if (!string.IsNullOrEmpty(selectedInvoiceID))
                {
                    using (SqlConnection con = Db.GetConnection())
                    using (SqlCommand cmd = new SqlCommand("SELECT SubscriberID FROM Invoices WHERE InvoiceID=@id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedInvoiceID);
                        con.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null && obj != DBNull.Value)
                            subscriberID = Convert.ToInt32(obj);
                    }
                }

                using (SqlConnection con = Db.GetConnection())
                {
                    // ✅ عرض: متأخرات + مبلغ الفترة + الإجمالي
                    string query = @"
SELECT 
    InvoiceID AS [رقم الفاتورة],
    InvoiceDate AS [التاريخ],
    Arrears AS [متأخرات],
    TotalAmount AS [مبلغ الفترة],
    (Arrears + TotalAmount) AS [الإجمالي],
    Status AS [الحالة]
FROM Invoices
WHERE (@sid = 0 OR SubscriberID=@sid)
ORDER BY InvoiceDate DESC, InvoiceID DESC;";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@sid", subscriberID);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;

                    if (dgv.Columns.Contains("التاريخ"))
                        dgv.Columns["التاريخ"].DefaultCellStyle.Format = "yyyy/MM/dd";

                    if (dgv.Columns.Contains("متأخرات") || dgv.Columns.Contains("مبلغ الفترة") || dgv.Columns.Contains("الإجمالي"))
                    {
                        if (dgv.Columns.Contains("متأخرات")) dgv.Columns["متأخرات"].DefaultCellStyle.Format = "N2";
                        if (dgv.Columns.Contains("مبلغ الفترة")) dgv.Columns["مبلغ الفترة"].DefaultCellStyle.Format = "N2";
                        if (dgv.Columns.Contains("الإجمالي")) dgv.Columns["الإجمالي"].DefaultCellStyle.Format = "N2";
                    }

                    if (!string.IsNullOrEmpty(selectedInvoiceID))
                    {
                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == selectedInvoiceID)
                            {
                                row.Selected = true;
                                dgv.FirstDisplayedScrollingRowIndex = row.Index;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string NumberToWords(decimal number)
        {
            return number.ToString("N2") + " ريال";
        }

        private class ComboItem
        {
            public string Text;
            public string Value;
            public ComboItem(string t, string v) { Text = t; Value = v; }
            public override string ToString() => Text;
        }
    }
}
/*  using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;


   namespace water3
   {

   public partial class InvoicesForm : Form
   {
        
       DataGridView dgv;
       DateTimePicker dtFrom, dtTo;
       Button btnFilter, btnPrint;
       Label lblMessage;

       public InvoicesForm()
       {
           this.Text = "إدارة الفواتير";
           this.Size = new Size(1200, 650);

           GroupBox gb = new GroupBox()
           {
               Text = "جدول الفواتير",
               Font = new Font("Tahoma", 11, FontStyle.Bold),
               ForeColor = Color.SeaGreen,
               Location = new Point(30, 15),
               Size = new Size(1120, 530),
               BackColor = Color.White
           };

           int y = 30;
           gb.Controls.Add(new Label() { Text = "من تاريخ:", Left = 30, Top = y + 4 });
           dtFrom = new DateTimePicker() { Left = 100, Top = y, Width = 140, Format = DateTimePickerFormat.Short };
           gb.Controls.Add(dtFrom);

           gb.Controls.Add(new Label() { Text = "إلى تاريخ:", Left = 270, Top = y + 4 });
           dtTo = new DateTimePicker() { Left = 340, Top = y, Width = 140, Format = DateTimePickerFormat.Short };
           gb.Controls.Add(dtTo);

           btnFilter = new Button() { Text = "عرض حسب التاريخ", Left = 500, Top = y, Width = 120, BackColor = Color.LightSkyBlue };
           btnFilter.Click += BtnFilter_Click;
           gb.Controls.Add(btnFilter);

           btnPrint = new Button() { Text = "طباعة النتائج", Left = 630, Top = y, Width = 120, BackColor = Color.LightGreen };
           btnPrint.Click += BtnPrint_Click;
           gb.Controls.Add(btnPrint);

           lblMessage = new Label() { Left = 780, Top = y + 5, Width = 280, ForeColor = Color.DarkRed, Font = new Font("Tahoma", 10, FontStyle.Bold) };
           gb.Controls.Add(lblMessage);

           // جدول البيانات
           dgv = new DataGridView()
           {
               Left = 15,
               Top = y + 40,
               Width = 1080,
               Height = 420,
               ReadOnly = true,
               AllowUserToAddRows = false,
               AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
           };
           dgv.CellContentClick += Dgv_CellContentClick;
           gb.Controls.Add(dgv);

           this.Controls.Add(gb);
           LoadInvoices();
       }

       private void BtnFilter_Click(object sender, EventArgs e)
       {
           LoadInvoices(dtFrom.Value.Date, dtTo.Value.Date);
       }

       private void BtnPrint_Click(object sender, EventArgs e)
       {
           // يمكنك الطباعة مباشرة أو فتح نافذة طباعة
           PrintDialog printDialog = new PrintDialog();
           printDialog.Document = new System.Drawing.Printing.PrintDocument();
           if (printDialog.ShowDialog() == DialogResult.OK)
           {
               // للطباعة البسيطة يمكنك تحويل الجدول لصورة أو تصديره لملف ثم طباعته.
               MessageBox.Show("تم إرسال الجدول للطابعة.");
           }
       }

       private void LoadInvoices(DateTime? from = null, DateTime? to = null)
       {
           using (SqlConnection con = Db.GetConnection())
           {
               string query = @"SELECT I.InvoiceID, S.Name AS SubscriberName, I.InvoiceDate, I.Consumption, I.TotalAmount, I.Status
                                FROM Invoices I
                                INNER JOIN Subscribers S ON I.SubscriberID = S.SubscriberID
                                WHERE 1=1";
               if (from != null)
                   query += " AND I.InvoiceDate >= @FromDate";
               if (to != null)
                   query += " AND I.InvoiceDate <= @ToDate";
               SqlCommand cmd = new SqlCommand(query, con);
               if (from != null)
                   cmd.Parameters.AddWithValue("@FromDate", from);
               if (to != null)
                   cmd.Parameters.AddWithValue("@ToDate", to);

               SqlDataAdapter da = new SqlDataAdapter(cmd);
               DataTable dt = new DataTable();
               da.Fill(dt);

               // زر عرض الفاتورة
               if (!dt.Columns.Contains("Show")) dt.Columns.Add("Show", typeof(string));
               foreach (DataRow row in dt.Rows) row["Show"] = "عرض";

               dgv.DataSource = dt;

               // تحويل عمود العرض إلى زر
               if (!dgv.Columns.Contains("ShowInvoice"))
               {
                   DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
                   btnCol.Name = "ShowInvoice";
                   btnCol.HeaderText = "عرض الفاتورة";
                   btnCol.Text = "عرض";
                   btnCol.UseColumnTextForButtonValue = true;
                   dgv.Columns.Add(btnCol);
               }
           }
       }

       // عرض تفاصيل الفاتورة في نافذة منبثقة
       private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
       {
           if (dgv.Columns[e.ColumnIndex].Name == "ShowInvoice" && e.RowIndex >= 0)
           {
               int invoiceID = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["InvoiceID"].Value);
               ShowInvoiceDetails(invoiceID);
           }
       }

       private void ShowInvoiceDetails(int invoiceID)
       {
           // جلب تفاصيل الفاتورة عبر الإجراء المخزني
           using (SqlConnection con = Db.GetConnection())
           {
               SqlCommand cmd = new SqlCommand("GetInvoiceDetails", con);
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.Parameters.AddWithValue("@InvoiceID", invoiceID);

               con.Open();
               SqlDataReader dr = cmd.ExecuteReader();
               if (dr.Read())
               {
                   string details = "";
                   details += "مشروع المدينة للمياه النقية\n";
                   details += "العنوان: صنعاء جولة مصعب ت/ 781600016\n";
                   details += "------------------------------------------\n";
                   details += $"رقم الفاتورة: {dr["InvoiceID"]}\t\tتاريخ: {Convert.ToDateTime(dr["InvoiceDate"]).ToString("yyyy-MM-dd")}\n";
                   details += $"اسم المشترك: {dr["SubscriberName"]}\n";
                   details += $"العنوان: {dr["Address"]}\n";
                   details += $"رقم العداد: {dr["MeterNumber"]}\n";
                   details += $"ق. السابقة: {dr["PreviousReading"] ?? "-"}\tق. الحالية: {dr["CurrentReading"] ?? "-"}\n";
                   details += $"الاستهلاك: {dr["Consumption"]}\tرسوم خدمات: {dr["ServiceFees"]}\n";
                   details += $"المتأخرات: {dr["Arrears"]}\tالإجمالي: {dr["TotalAmount"]}\n";
                   details += $"الحالة: {dr["Status"]}\n";
                   details += "------------------------------------------\n";

                   // جلب المدفوع والرصيد
                   decimal paid = 0, total = Convert.ToDecimal(dr["TotalAmount"]);
                   int subscriberId = Convert.ToInt32(dr["SubscriberID"]);
                   DateTime invoiceDate = Convert.ToDateTime(dr["InvoiceDate"]);
                   dr.Close();

                   SqlCommand cmd2 = new SqlCommand("SELECT ISNULL(SUM(Amount),0) FROM Payments WHERE SubscriberID=@SubscriberID AND PaymentDate <= @InvoiceDate", con);
                   cmd2.Parameters.AddWithValue("@SubscriberID", subscriberId);
                   cmd2.Parameters.AddWithValue("@InvoiceDate", invoiceDate);
                   paid = Convert.ToDecimal(cmd2.ExecuteScalar());

                   details += $"المدفوع: {paid}\tالرصيد الحالي: {total - paid}\n";
                   details += "------------------------------------------\n";
                   details += $"إجمالي المبلغ كتابةً: {ConvertAmountToWords(total - paid)} ريال\n";
                   details += "يرجى سداد قيمة الفاتورة خلال 5 أيام من إصدارها.\n";

                   // نافذة منبثقة
                   MessageBox.Show(details, "تفاصيل الفاتورة", MessageBoxButtons.OK, MessageBoxIcon.Information);
               }
               else
               {
                   MessageBox.Show("لا توجد بيانات!", "تفاصيل الفاتورة");
               }
               con.Close();
           }
       }

       // دالة تحويل الرقم إلى نص (مبسط)
       private string ConvertAmountToWords(decimal amount)
       {
           // بإمكانك دمج مكتبة متقدمة لاحقاً
           return amount.ToString("N0");
       }
   }
}

*/