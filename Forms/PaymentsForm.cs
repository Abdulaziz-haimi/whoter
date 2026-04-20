using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using water3.Repositories;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
    public partial class PaymentsForm : Form
    {
        // ===== Dependencies =====
        private readonly SubscribersRepository _subsRepo = new SubscribersRepository();
        private readonly CollectorsRepository _collectorsRepo = new CollectorsRepository();
        private readonly PaymentsRepository _paymentsRepo = new PaymentsRepository();
        private readonly PaymentsService _paymentsService;

        // ===== State =====
        private const string SearchPlaceholder = "ابحث باسم المشترك...";
        private int? selectedSubscriberID = null;

        private Timer _msgTimer;
        private bool _isSaving;

        public PaymentsForm()
        {
            InitializeComponent();

            _paymentsService = new PaymentsService(_paymentsRepo);

            InitUiCore();
            ApplyTheme();
            WireEventsCore();
            InitSubscribersPopup();

            EnableDoubleBuffering(dgv);

            LoadCollectors();
            InitPaymentTypes();
            LoadPayments();

            Resize += (s, e) => RepositionSubscribersPopup();
            LocationChanged += (s, e) => RepositionSubscribersPopup();
            pnlSearch.Resize += (s, e) => RepositionSubscribersPopup();
            grpAdd.Resize += (s, e) => RepositionSubscribersPopup();
        }
        private void InitUiCore()
        {
            Text = "إدارة المدفوعات";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;

            txtSubscriberSearch.MaxLength = 200;
            txtSubscriberSearch.ShortcutsEnabled = true;

            dtpPaymentDate.MaxDate = DateTime.Today;
            dtpPaymentDate.Value = DateTime.Today;

            SetSearchPlaceholder();

            _msgTimer = new Timer { Interval = 2500 };
            _msgTimer.Tick += (s, e) =>
            {
                _msgTimer.Stop();
                pnlMessage.Visible = false;
                lblMessage.Text = "";
            };
        }
       
        private void WireEventsCore()
        {
            txtSubscriberSearch.GotFocus += SubscriberSearch_GotFocus;
            txtSubscriberSearch.LostFocus += SubscriberSearch_LostFocus;
            txtSubscriberSearch.TextChanged += TxtSubscriberSearch_TextChanged;
            txtSubscriberSearch.KeyDown += TxtSubscriberSearch_KeyDown;

            btnAdd.Click += BtnAdd_Click;

            btnRefresh.Click += (s, e) =>
            {
                LoadCollectors();
                LoadPayments();
                if (selectedSubscriberID.HasValue) LoadSubscriberBalance(dtpPaymentDate.Value.Date);
            };
            txtSubscriberSearch.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Tab) e.IsInputKey = true;
            };
            // حتى Tab يمر على KeyDown بدل ما يغيّر فوكس مباشرة
           
            btnClear.Click += (s, e) => ClearInputs();

            dgv.CellDoubleClick += Dgv_CellDoubleClick;

            dgv.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var row = dgv.Rows[e.RowIndex];
                var type = row.Cells["النوع"]?.Value?.ToString() ?? "";

                if (type == "رصيد مقدم")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                else if (type == "سداد + رصيد مقدم")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
                else if (type == "سداد")
                    row.DefaultCellStyle.BackColor = Color.White;
            };

            WireAmountInput();

            btnExport.Click += (s, e) =>
                MessageBox.Show("اربط التصدير هنا (Excel/CSV).", "تصدير",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadCollectors()
        {
            var list = _collectorsRepo.GetAll();
            ddlCollectors.DataSource = list;
            ddlCollectors.DisplayMember = "Name";
            ddlCollectors.ValueMember = "CollectorID";
        }

        private void LoadPayments()
        {
            DataTable dt = _paymentsRepo.GetPaymentsTable();
            dgv.DataSource = dt;

            if (dgv.Columns.Contains("ReceiptID"))
                dgv.Columns["ReceiptID"].Visible = false;

            if (dgv.Columns.Contains("التاريخ"))
            {
                dgv.Columns["التاريخ"].DefaultCellStyle.Format = "yyyy/MM/dd";
                dgv.Columns["التاريخ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (dgv.Columns.Contains("المبلغ"))
            {
                dgv.Columns["المبلغ"].DefaultCellStyle.Format = "N2";
                dgv.Columns["المبلغ"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["المبلغ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            if (dgv.Columns.Contains("مسدد على الفواتير"))
            {
                dgv.Columns["مسدد على الفواتير"].DefaultCellStyle.Format = "N2";
                dgv.Columns["مسدد على الفواتير"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["مسدد على الفواتير"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (dgv.Columns.Contains("رصيد مقدم"))
            {
                dgv.Columns["رصيد مقدم"].DefaultCellStyle.Format = "N2";
                dgv.Columns["رصيد مقدم"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["رصيد مقدم"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (dgv.Columns.Contains("النوع"))
                dgv.Columns["النوع"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgv.ClearSelection();
        }
        private void LoadSubscriberBalance(DateTime asOfDate)
{
    if (!selectedSubscriberID.HasValue) return;

   // var bal = _subsRepo.GetBalanceAsOf(selectedSubscriberID.Value, asOfDate);
    var bal = _subsRepo.GetBalanceAsOf(selectedSubscriberID.Value, dtpPaymentDate.Value.Date);
            if (bal > 0)
    {
        lblCurrentBalance.ForeColor = Color.FromArgb(220, 53, 69);
        lblCurrentBalance.Text = $"عليه متأخرات حتى {asOfDate:yyyy/MM/dd}: {bal:N2} ريال";

        // اقتراح المبلغ تلقائياً حسب التاريخ
        txtAmount.Text = bal.ToString("0.00");
    }
    else if (bal < 0)
    {
        lblCurrentBalance.ForeColor = Color.FromArgb(40, 167, 69);
        lblCurrentBalance.Text = $"له رصيد مقدم حتى {asOfDate:yyyy/MM/dd}: {Math.Abs(bal):N2} ريال";

        // لا يوجد مبلغ مستحق؛ افرغ الحقل
        txtAmount.Clear();
    }
    else
    {
        lblCurrentBalance.ForeColor = Color.FromArgb(33, 37, 41);
        lblCurrentBalance.Text = $"حتى {asOfDate:yyyy/MM/dd}: 0.00 ريال (متوازن)";

        txtAmount.Clear();
    }
}
      

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ClearErrors();
            if (_isSaving) return;

            pnlMessage.Visible = false;

            if (!selectedSubscriberID.HasValue)
            {
                SetError(txtSubscriberSearch, "اختر مشترك");
                ShowMsg("يرجى اختيار مشترك", true);
                return;
            }

            int collectorId;
            if (ddlCollectors.SelectedValue == null || !int.TryParse(ddlCollectors.SelectedValue.ToString(), out collectorId))
            {
                SetError(ddlCollectors, "اختر محصل");
                ShowMsg("يرجى اختيار محصل", true);
                return;
            }

            decimal amount;
            if (!MoneyParser.TryParse(txtAmount.Text, out amount) || amount <= 0)
            {
                SetError(txtAmount, "أدخل مبلغ صحيح");
                ShowMsg("مبلغ غير صحيح", true);
                return;
            }

            string paymentTypeDb = MapPaymentTypeToDb(ddlPaymentType.Text);

            try
            {
                _isSaving = true;
                btnAdd.Enabled = false;

                _paymentsService.AddPayment(
                    subscriberId: selectedSubscriberID.Value,
                    collectorId: collectorId,
                    paymentDate: dtpPaymentDate.Value.Date,
                    amount: amount,
                    paymentTypeDb: paymentTypeDb,
                    notes: (txtNotes.Text ?? "").Trim()
                );

                ShowMsg("تم تسجيل الدفعة بنجاح ✅", false);

                ClearAfterSuccess();
                LoadPayments();
                LoadSubscriberBalance(dtpPaymentDate.Value.Date);
            }
            catch (Exception ex)
            {
                var msg = ex.Message ?? "";

                if (msg.Contains("Cannot insert duplicate key row") && msg.Contains("dbo.Payments"))
                    ShowMsg("تعذر حفظ الدفعة بسبب تعارض في رقم الإيصال داخل سجل المدفوعات. تم تغيير التصميم ليكون رقم الإيصال في جدول الإيصالات الرئيسي فقط.", true);
                else
                    ShowMsg(msg, true);
            }
            finally
            {
                _isSaving = false;
                btnAdd.Enabled = true;
            }
        }

        private void InitPaymentTypes()
        {
            ddlPaymentType.Items.Clear();
            ddlPaymentType.Items.AddRange(new object[] { "نقداً", "تحويل", "شيك", "أخرى" });
            ddlPaymentType.SelectedIndex = ddlPaymentType.Items.Count > 0 ? 0 : -1;
        }

        private string MapPaymentTypeToDb(string uiText)
        {
            switch ((uiText ?? "").Trim())
            {
                case "نقداً": return "Cash";
                case "تحويل": return "Transfer";
                case "شيك": return "Cheque";
                case "أخرى": return "Other";
                default: return "Cash";
            }
        }
        private void Dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgv.Rows[e.RowIndex];
            var receiptId = row.Cells["ReceiptID"].Value;
            var receiptNumber = row.Cells["رقم الإيصال"].Value?.ToString();
            var totalAmount = row.Cells["المبلغ"].Value?.ToString();
            var date = row.Cells["التاريخ"].Value?.ToString();
            var subscriber = row.Cells["المشترك"].Value?.ToString();

            // جلب تفاصيل التوزيع
            var details = _paymentsRepo.GetReceiptDetails(Convert.ToInt32(receiptId));

            MessageBox.Show(
                $"📄 إيصال رقم: {receiptNumber}\n" +
                $"👤 المشترك: {subscriber}\n" +
                $"📅 التاريخ: {date}\n" +
                $"💰 المبلغ الإجمالي: {totalAmount} ريال\n\n" +
                $"📋 تفاصيل التوزيع:\n{details}",
                "تفاصيل الدفعة",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    
        private void EnableDoubleBuffering(Control c)
        {
            try
            {
                var prop = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                if (prop != null) prop.SetValue(c, true, null);
            }
            catch { }
        }
    }
}

/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class PaymentsForm : Form
    {
        ComboBox ddlSubscribers, ddlCollectors, ddlPaymentType;
        Label lblCurrentBalance, lblMessage;
        DateTimePicker dtpPaymentDate;
        TextBox txtAmount, txtNotes;
        Button btnAdd;
        DataGridView dgv;

         

        public PaymentsForm()
        {
            this.Text = "إدارة المدفوعات";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(15)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 270));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var group = new GroupBox
            {
                Text = "إضافة دفعة جديدة",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.MidnightBlue,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(18, 8, 18, 8)
            };
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 3
            };

            for (int i = 0; i < 6; i++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6f));

            grid.Controls.Add(new Label() { Text = "اسم المشترك:", Anchor = AnchorStyles.Right }, 0, 0);
            ddlSubscribers = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDown, // مهم
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.CustomSource
            };
            ddlSubscribers.SelectedIndexChanged += DdlSubscribers_SelectedIndexChanged;
            ddlSubscribers.TextChanged += DdlSubscribers_TextChanged;
            ddlSubscribers.SelectedIndexChanged += DdlSubscribers_SelectedIndexChanged;
            grid.Controls.Add(ddlSubscribers, 1, 0);

            grid.Controls.Add(new Label() { Text = "الرصيد الحالي:", Anchor = AnchorStyles.Right }, 2, 0);
            lblCurrentBalance = new Label { Dock = DockStyle.Fill, ForeColor = Color.DarkRed, TextAlign = ContentAlignment.MiddleCenter };
            grid.Controls.Add(lblCurrentBalance, 3, 0);

            grid.Controls.Add(new Label() { Text = "اسم المحصل:", Anchor = AnchorStyles.Right }, 4, 0);
            ddlCollectors = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            grid.Controls.Add(ddlCollectors, 5, 0);

            grid.Controls.Add(new Label() { Text = "تاريخ الدفع:", Anchor = AnchorStyles.Right }, 0, 1);
            dtpPaymentDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            grid.Controls.Add(dtpPaymentDate, 1, 1);

            grid.Controls.Add(new Label() { Text = "المبلغ:", Anchor = AnchorStyles.Right }, 2, 1);
            txtAmount = new TextBox { Dock = DockStyle.Fill };
            grid.Controls.Add(txtAmount, 3, 1);

            grid.Controls.Add(new Label() { Text = "طريقة الدفع:", Anchor = AnchorStyles.Right }, 4, 1);
            ddlPaymentType = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            ddlPaymentType.Items.AddRange(new object[] { "نقداً", "تحويل", "شيك", "أخرى" });
            ddlPaymentType.SelectedIndex = 0;
            grid.Controls.Add(ddlPaymentType, 5, 1);

            grid.Controls.Add(new Label() { Text = "ملاحظات:", Anchor = AnchorStyles.Right }, 0, 2);
            txtNotes = new TextBox { Dock = DockStyle.Fill, Multiline = true };
            grid.SetColumnSpan(txtNotes, 3);
            grid.Controls.Add(txtNotes, 1, 2);

            btnAdd = new Button
            {
                Text = "إضافة",
                Dock = DockStyle.Fill,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White
            };
            btnAdd.Click += BtnAdd_Click;
            grid.Controls.Add(btnAdd, 4, 2);

            lblMessage = new Label { Dock = DockStyle.Fill, ForeColor = Color.DarkGreen };
            grid.Controls.Add(lblMessage, 5, 2);

            group.Controls.Add(grid);
            mainLayout.Controls.Add(group, 0, 0);
          
            #region ===== جدول الدفعات =====
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.UserDeletingRow += Dgv_UserDeletingRow;
            mainLayout.Controls.Add(dgv, 0, 1);
            #endregion

            this.Controls.Add(mainLayout);

            LoadSubscribers();
            LoadCollectors();
            LoadPayments();
        }

        #region ===== تحميل البيانات =====
        private DataTable subscribersTable;

        private void LoadSubscribers()
        {
            ddlSubscribers.Items.Clear();
            subscribersTable = new DataTable();

            using (SqlConnection con = Db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT SubscriberID, Name FROM Subscribers ORDER BY Name", con);
                da.Fill(subscribersTable);
            }

            AutoCompleteStringCollection auto = new AutoCompleteStringCollection();

            foreach (DataRow r in subscribersTable.Rows)
            {
                var item = new ComboBoxItem(
                    r["Name"].ToString(),
                    r["SubscriberID"].ToString());

                ddlSubscribers.Items.Add(item);
                auto.Add(r["Name"].ToString());
            }

            ddlSubscribers.AutoCompleteCustomSource = auto;
        }
        private void DdlSubscribers_TextChanged(object sender, EventArgs e)
        {
            foreach (ComboBoxItem item in ddlSubscribers.Items)
            {
                if (item.Text.Equals(ddlSubscribers.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    ddlSubscribers.SelectedItem = item;
                    break;
                }
            }
        }

        private void LoadCollectors()
        {
            ddlCollectors.Items.Clear();
            using (SqlConnection con = Db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT CollectorID, Name FROM Collectors", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                    ddlCollectors.Items.Add(new ComboBoxItem(r["Name"].ToString(), r["CollectorID"].ToString()));
            }
            if (ddlCollectors.Items.Count > 0) ddlCollectors.SelectedIndex = 0;
        }

        private void LoadPayments()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT P.PaymentID, S.Name AS المشترك, P.InvoiceID AS الفاتورة,
                           P.PaymentDate AS التاريخ, P.Amount AS المبلغ,
                           P.PaymentType AS الطريقة, P.Notes AS ملاحظات
                    FROM Payments P
                    INNER JOIN Subscribers S ON P.SubscriberID = S.SubscriberID", con);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }
        #endregion

        #region ===== الرصيد =====
        private void DdlSubscribers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSubscribers.SelectedItem is ComboBoxItem item)
            {
                using (SqlConnection con = Db.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(SUM(Debit - Credit),0)
                        FROM AccountStatements
                        WHERE SubscriberID=@ID", con);
                    cmd.Parameters.AddWithValue("@ID", item.Value);
                    con.Open();
                    decimal balance = Convert.ToDecimal(cmd.ExecuteScalar());
                    lblCurrentBalance.Text = balance.ToString("N2") + " ريال";
                }
            }
        }
        #endregion

        #region ===== إضافة دفعة =====
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "مبلغ غير صحيح";
                return;
            }

            using (SqlConnection con = Db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("PayOldestInvoice", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SubscriberID", ((ComboBoxItem)ddlSubscribers.SelectedItem).Value);
                cmd.Parameters.AddWithValue("@CollectorID", ((ComboBoxItem)ddlCollectors.SelectedItem).Value);
                cmd.Parameters.AddWithValue("@PaymentDate", dtpPaymentDate.Value.Date);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@PaymentType", ddlPaymentType.Text);
                cmd.Parameters.AddWithValue("@Notes", txtNotes.Text);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.ForeColor = Color.Green;
            lblMessage.Text = "تم تسجيل الدفعة بنجاح";
            txtAmount.Clear();
            txtNotes.Clear();

            LoadPayments();
            DdlSubscribers_SelectedIndexChanged(null, null);
        }
        #endregion

        #region ===== عكس دفعة =====
        private void Dgv_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            int paymentID = Convert.ToInt32(e.Row.Cells["PaymentID"].Value);

            using (SqlConnection con = Db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("CancelPayment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PaymentID", paymentID);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public ComboBoxItem(string t, string v) { Text = t; Value = v; }
            public override string ToString() => Text;
        }
        private void PaymentsForm_Load(object sender, EventArgs e)
        {

        }
    }
}

*/