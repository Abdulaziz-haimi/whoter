using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace water3
{
    public class ReadingForm : Form
    {
        private TextBox txtSubscriberSearch;
        private ListBox lstSubscriberSuggestions;

        private DateTimePicker dtpReadingDate;
        private TextBox txtPreviousReading, txtCurrentReading, txtConsumption, txtNotes;
        private Label lblLastReading, lblLastInvoice, lblMessage, lblTariff;
        private Button btnAdd;

         

        private int? SelectedSubscriberID = null;
        private int? SelectedMeterID = null;

        // Tariff state
        private int? CurrentTariffPlanID = null;
        private string CurrentPricingModel = "DEFAULT"; // DEFAULT / Fixed / Tiered
        private decimal CurrentUnitPrice = 0;
        private decimal CurrentServiceFees = 0;

        private bool _suppressSearch = false;

        public ReadingForm()
        {
            Text = "إدخال قراءة عداد";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18, 14, 18, 14),
                BackColor = Color.White
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 285));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(237, 242, 247),
                WrapContents = false,
                AutoScroll = true
            };

            lblTariff = new Label
            {
                Text = "التعرفة: ---",
                AutoSize = true,
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 80, 40),
                Margin = new Padding(28, 6, 0, 0)
            };

            lblLastInvoice = new Label
            {
                Text = "آخر فاتورة: ---",
                AutoSize = true,
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 60, 70),
                Margin = new Padding(28, 6, 0, 0)
            };

            lblLastReading = new Label
            {
                Text = "آخر قراءة: ---",
                AutoSize = true,
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 60, 70),
                Margin = new Padding(0, 6, 0, 0)
            };

            topPanel.Controls.Add(lblTariff);
            topPanel.Controls.Add(lblLastInvoice);
            topPanel.Controls.Add(lblLastReading);

            var group = new GroupBox
            {
                Text = "إضافة قراءة جديدة",
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 49, 89),
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(13, 10, 13, 7)
            };

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 4,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(12)
            };

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            txtSubscriberSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 11),
                RightToLeft = RightToLeft.Yes
            };

            dtpReadingDate = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Tahoma", 11)
            };

            txtPreviousReading = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Gainsboro,
                Font = new Font("Tahoma", 11),
                TextAlign = HorizontalAlignment.Right,
                Text = "0"
            };

            txtCurrentReading = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 11),
                TextAlign = HorizontalAlignment.Right
            };

            txtConsumption = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Gainsboro,
                Font = new Font("Tahoma", 11),
                TextAlign = HorizontalAlignment.Right
            };

            txtNotes = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                Height = 38,
                Font = new Font("Tahoma", 11),
                RightToLeft = RightToLeft.Yes,
                ScrollBars = ScrollBars.Vertical
            };

            btnAdd = new Button
            {
                Text = "إضافة",
                Dock = DockStyle.Fill,
                Height = 38,
                BackColor = Color.FromArgb(80, 199, 110),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnAdd.FlatAppearance.BorderSize = 0;

            lblMessage = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.DarkGreen,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            table.Controls.Add(new Label { Text = "المشترك/العداد:", Anchor = AnchorStyles.Right, AutoSize = true, Font = new Font("Tahoma", 11, FontStyle.Bold) }, 0, 0);
            table.Controls.Add(txtSubscriberSearch, 1, 0);

            table.Controls.Add(new Label { Text = "تاريخ القراءة:", Anchor = AnchorStyles.Right, AutoSize = true, Font = new Font("Tahoma", 11, FontStyle.Bold) }, 2, 0);
            table.Controls.Add(dtpReadingDate, 3, 0);

            table.Controls.Add(new Label { Text = "القراءة السابقة:", Anchor = AnchorStyles.Right, AutoSize = true, Font = new Font("Tahoma", 11) }, 0, 1);
            table.Controls.Add(txtPreviousReading, 1, 1);

            table.Controls.Add(new Label { Text = "القراءة الحالية:", Anchor = AnchorStyles.Right, AutoSize = true, Font = new Font("Tahoma", 11) }, 2, 1);
            table.Controls.Add(txtCurrentReading, 3, 1);

            table.Controls.Add(new Label { Text = "الاستهلاك:", Anchor = AnchorStyles.Right, AutoSize = true, Font = new Font("Tahoma", 11) }, 0, 2);
            table.Controls.Add(txtConsumption, 1, 2);

            table.Controls.Add(new Label { Text = "ملاحظات:", Anchor = AnchorStyles.Right, AutoSize = true, Font = new Font("Tahoma", 11) }, 2, 2);
            table.Controls.Add(txtNotes, 3, 2);

            table.Controls.Add(btnAdd, 1, 3);
            table.Controls.Add(lblMessage, 3, 3);

            group.Controls.Add(table);

            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(group, 0, 1);
            Controls.Add(mainLayout);

            // Suggestions list
            lstSubscriberSuggestions = new ListBox
            {
                Visible = false,
                Height = 170,
                Font = new Font("Tahoma", 11),
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false
            };
            Controls.Add(lstSubscriberSuggestions);
            lstSubscriberSuggestions.BringToFront();

            // Events
            txtSubscriberSearch.TextChanged += TxtSubscriberSearch_TextChanged;
            txtSubscriberSearch.KeyDown += TxtSubscriberSearch_KeyDown;
            txtSubscriberSearch.Leave += (s, e) =>
                BeginInvoke((MethodInvoker)(() => { if (!lstSubscriberSuggestions.Focused) lstSubscriberSuggestions.Visible = false; }));

            lstSubscriberSuggestions.MouseClick += (s, e) => SelectSubscriberFromList();
            lstSubscriberSuggestions.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { SelectSubscriberFromList(); e.Handled = true; }
                else if (e.KeyCode == Keys.Escape) { lstSubscriberSuggestions.Visible = false; txtSubscriberSearch.Focus(); }
            };

            txtCurrentReading.KeyPress += TxtCurrentReading_KeyPress;
            txtCurrentReading.TextChanged += TxtCurrentReading_TextChanged;

            btnAdd.Click += BtnAdd_Click;

            Shown += (s, e) =>
            {
                txtSubscriberSearch.Focus();
                LoadDefaultTariff();
            };

            Resize += (s, e) => PositionSuggestionList();
        }

        // ===================== Tariff =====================
        private void LoadDefaultTariff()
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.GetActiveBillingConstant", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            CurrentPricingModel = "DEFAULT";
                            CurrentTariffPlanID = null;
                            CurrentUnitPrice = Convert.ToDecimal(dr["UnitPrice"]);
                            CurrentServiceFees = Convert.ToDecimal(dr["ServiceFees"]);
                            lblTariff.Text = $"التعرفة: (عامة) سعر {CurrentUnitPrice:N2} + رسوم {CurrentServiceFees:N2}";
                        }
                        else
                        {
                            CurrentUnitPrice = 0;
                            CurrentServiceFees = 0;
                            lblTariff.Text = "التعرفة: لا توجد ثوابت نشطة";
                        }
                    }
                }
            }
            catch
            {
                CurrentUnitPrice = 0;
                CurrentServiceFees = 0;
                lblTariff.Text = "التعرفة: خطأ في التحميل";
            }
        }

        // ✅ الأفضل أن يكون التعرفة مرتبطة بالمشترك (أو بالعداد إن عندك)
        private void LoadTariffForSubscriber(int subscriberId, DateTime asOfDate)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.GetTariffForSubscriber", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                    cmd.Parameters.AddWithValue("@AsOfDate", asOfDate.Date);

                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            CurrentTariffPlanID = dr["TariffPlanID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["TariffPlanID"]);
                            CurrentPricingModel = dr["PricingModel"]?.ToString() ?? "DEFAULT";
                            CurrentUnitPrice = Convert.ToDecimal(dr["UnitPrice"]);
                            CurrentServiceFees = Convert.ToDecimal(dr["ServiceFees"]);

                            string planText = CurrentTariffPlanID.HasValue ? $"خطة:{CurrentPricingModel}" : "عامة";
                            lblTariff.Text = $"التعرفة: ({planText}) سعر {CurrentUnitPrice:N2} + رسوم {CurrentServiceFees:N2}";
                        }
                        else
                        {
                            LoadDefaultTariff();
                        }
                    }
                }
            }
            catch
            {
                LoadDefaultTariff();
            }
        }

        private decimal ResolveTieredUnitPrice(int tariffPlanId, decimal consumption)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 UnitPrice
                FROM TariffRates
                WHERE TariffPlanID = @Plan
                  AND @C >= FromQty
                  AND (ToQty IS NULL OR @C <= ToQty)
                ORDER BY FromQty DESC, TariffRateID DESC;", con))
            {
                cmd.Parameters.AddWithValue("@Plan", tariffPlanId);
                cmd.Parameters.AddWithValue("@C", consumption);

                con.Open();
                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return CurrentUnitPrice;
                return Convert.ToDecimal(o);
            }
        }

        // ===================== Search Subscribers+Meters =====================
        private void TxtSubscriberSearch_TextChanged(object sender, EventArgs e)
        {
            if (_suppressSearch) return;

            SelectedSubscriberID = null;
            SelectedMeterID = null;

            btnAdd.Enabled = false;
            lblMessage.Text = "";
            txtPreviousReading.Text = "0";
            txtConsumption.Text = "";
            lblLastReading.Text = "آخر قراءة: ---";
            lblLastInvoice.Text = "آخر فاتورة: ---";
            LoadDefaultTariff();

            string text = txtSubscriberSearch.Text.Trim();
            if (text.Length < 2)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            DataTable dt = SearchSubscribersAndMeters(text);
            if (dt.Rows.Count == 0)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            lstSubscriberSuggestions.DataSource = dt;
            lstSubscriberSuggestions.DisplayMember = "DisplayText";
            lstSubscriberSuggestions.ValueMember = "MeterID"; // ✅ نختار العداد

            PositionSuggestionList();
            lstSubscriberSuggestions.Visible = true;
        }

        private DataTable SearchSubscribersAndMeters(string keyword)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 30
            s.SubscriberID,
            m.MeterID,
            (CAST(s.SubscriberID AS NVARCHAR(20)) + ' - ' + s.Name
             + ' | عداد: ' + m.MeterNumber
             + CASE WHEN ISNULL(m.Location,'')<>'' THEN ' | موقع: ' + m.Location ELSE '' END
            ) AS DisplayText
        FROM Subscribers s
        JOIN SubscriberMeters sm ON sm.SubscriberID = s.SubscriberID
        JOIN Meters m ON m.MeterID = sm.MeterID
        WHERE s.IsActive = 1 AND m.IsActive = 1
          AND (
                s.Name LIKE @K
             OR m.MeterNumber LIKE @K
             OR CAST(s.SubscriberID AS NVARCHAR(20)) = @Exact
             OR CAST(m.MeterID AS NVARCHAR(20)) = @Exact
          )
        ORDER BY s.Name, m.MeterNumber;", con))
            {
                cmd.Parameters.AddWithValue("@K", "%" + keyword + "%");
                cmd.Parameters.AddWithValue("@Exact", keyword);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private void TxtSubscriberSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && lstSubscriberSuggestions.Visible)
            {
                lstSubscriberSuggestions.Focus();
                if (lstSubscriberSuggestions.Items.Count > 0)
                    lstSubscriberSuggestions.SelectedIndex = 0;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (lstSubscriberSuggestions.Visible && lstSubscriberSuggestions.Items.Count > 0)
                {
                    if (lstSubscriberSuggestions.SelectedIndex < 0)
                        lstSubscriberSuggestions.SelectedIndex = 0;

                    SelectSubscriberFromList();
                }
                e.Handled = true;
            }
        }

        private void PositionSuggestionList()
        {
            Point p = txtSubscriberSearch.PointToScreen(Point.Empty);
            Point f = PointToClient(p);

            lstSubscriberSuggestions.Width = txtSubscriberSearch.Width;
            lstSubscriberSuggestions.Left = f.X;
            lstSubscriberSuggestions.Top = f.Y + txtSubscriberSearch.Height + 2;
        }

        private void SelectSubscriberFromList()
        {
            if (lstSubscriberSuggestions.SelectedItem == null) return;

            var drv = (DataRowView)lstSubscriberSuggestions.SelectedItem;

            int subscriberId = Convert.ToInt32(drv["SubscriberID"]);
            int meterId = Convert.ToInt32(drv["MeterID"]);
            string display = drv["DisplayText"].ToString();

            _suppressSearch = true;
            try
            {
                SelectedSubscriberID = subscriberId;
                SelectedMeterID = meterId;

                txtSubscriberSearch.Text = display;
                txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
                lstSubscriberSuggestions.Visible = false;
            }
            finally { _suppressSearch = false; }

            LoadSelectionData(subscriberId, meterId);
            LoadTariffForSubscriber(subscriberId, dtpReadingDate.Value);

            txtCurrentReading.Focus();
            txtCurrentReading.SelectAll();
        }

        // ===================== Load Selected Meter Data =====================
        private void LoadSelectionData(int subscriberId, int meterId)
        {
            // اختر أحد الخيارين:

            // خيار A (Meter فقط)
           // LoadPreviousReading(meterId);

            // خيار B (مطابق للإجراء 100%)
             LoadPreviousReading(subscriberId, meterId);

            LoadLastReadingLabel(meterId);
            LoadLastInvoiceLabel(meterId);
            btnAdd.Enabled = CheckCurrentReadingValid();
        }


        private void LoadPreviousReading(int subscriberId, int meterId)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 1 CurrentReading
            FROM Readings
            WHERE SubscriberID=@SID AND MeterID=@MID
            ORDER BY ReadingDate DESC, ReadingID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@SID", subscriberId);
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    txtPreviousReading.Text = (result == null || result == DBNull.Value)
                        ? "0"
                        : Convert.ToDecimal(result).ToString("0.##");
                }

                txtCurrentReading.Text = "";
                txtConsumption.Text = "";
            }
            catch
            {
                txtPreviousReading.Text = "0";
                txtCurrentReading.Text = "";
                txtConsumption.Text = "";
            }
        }

        private void LoadLastReadingLabel(int meterId)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 1 ReadingDate, PreviousReading, CurrentReading
            FROM Readings
            WHERE MeterID=@MID
            ORDER BY ReadingDate DESC, ReadingID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var prev = dr["PreviousReading"]?.ToString();
                            var curr = dr["CurrentReading"]?.ToString();
                            var date = Convert.ToDateTime(dr["ReadingDate"]).ToString("yyyy-MM-dd");
                            lblLastReading.Text = $"آخر قراءة: {date} (السابق: {prev} / الحالي: {curr})";
                        }
                        else
                            lblLastReading.Text = "آخر قراءة: لا توجد";
                    }
                }
            }
            catch { lblLastReading.Text = "آخر قراءة: خطأ في التحميل"; }
        }

        private void LoadLastInvoiceLabel(int meterId)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 1 InvoiceDate, TotalAmount, Status
            FROM Invoices
            WHERE MeterID=@MID
            ORDER BY InvoiceDate DESC, InvoiceID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string date = Convert.ToDateTime(dr["InvoiceDate"]).ToString("yyyy-MM-dd");
                            lblLastInvoice.Text = $"آخر فاتورة: {date} | المبلغ: {dr["TotalAmount"]} | الحالة: {dr["Status"]}";
                        }
                        else
                            lblLastInvoice.Text = "آخر فاتورة: لا توجد";
                    }
                }
            }
            catch { lblLastInvoice.Text = "آخر فاتورة: خطأ في التحميل"; }
        }

        // ===================== Input Validation =====================
        private void TxtCurrentReading_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            char ch = NormalizeArabicDigit(e.KeyChar);

            if (char.IsDigit(ch))
            {
                e.KeyChar = ch;
                return;
            }

            if (ch == '.' || ch == ',')
            {
                if (!txtCurrentReading.Text.Contains("."))
                    e.KeyChar = '.';
                else
                    e.Handled = true;

                return;
            }

            e.Handled = true;
        }

        private void TxtCurrentReading_TextChanged(object sender, EventArgs e)
        {
            lblMessage.ForeColor = Color.Red;
            btnAdd.Enabled = false;

            string curText = NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NormalizeNumberString(txtPreviousReading.Text);

            if (!TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            if (TryParseDecimal(curText, out decimal current))
            {
                if (SelectedMeterID == null)
                {
                    lblMessage.Text = "⚠️ اختر مشترك/عداد أولاً";
                    txtConsumption.Text = "";
                    return;
                }

                if (current < previous)
                {
                    lblMessage.Text = "⚠️ القراءة الحالية أقل من السابقة";
                    txtConsumption.Text = "";
                    return;
                }

                decimal cons = current - previous;
                txtConsumption.Text = cons.ToString("N2");

                btnAdd.Enabled = SelectedMeterID.HasValue && (current >= previous);
                if (btnAdd.Enabled)
                {
                    lblMessage.ForeColor = Color.Green;
                    lblMessage.Text = "✓ جاهز للإضافة";
                }
            }
            else
            {
                txtConsumption.Text = "";
                if (!string.IsNullOrWhiteSpace(txtCurrentReading.Text))
                    lblMessage.Text = "❌ القيمة المدخلة غير صحيحة";
            }
        }

        private bool CheckCurrentReadingValid()
        {
            if (!SelectedMeterID.HasValue) return false;

            string curText = NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NormalizeNumberString(txtPreviousReading.Text);

            if (!TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            return TryParseDecimal(curText, out decimal current) && current >= previous;
        }

        private static bool TryParseDecimal(string s, out decimal value)
        {
            return decimal.TryParse(
                s,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out value);
        }

        private static string NormalizeNumberString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            var sb = new StringBuilder();
            bool dotAdded = false;

            foreach (char c0 in input.Trim())
            {
                char c = NormalizeArabicDigit(c0);

                if (char.IsDigit(c))
                    sb.Append(c);
                else if ((c == '.' || c == ',') && !dotAdded)
                {
                    sb.Append('.');
                    dotAdded = true;
                }
            }

            return sb.ToString();
        }

        private static char NormalizeArabicDigit(char c)
        {
            if (c >= '٠' && c <= '٩') return (char)('0' + (c - '٠'));
            if (c >= '۰' && c <= '۹') return (char)('0' + (c - '۰'));
            return c;
        }

        // ===================== Add Reading (Meter-based) =====================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ اختر مشترك/عداد صحيح.";
                return;
            }

            string curText = NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NormalizeNumberString(txtPreviousReading.Text);

            if (!TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            if (!TryParseDecimal(curText, out decimal current) || current < previous)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ القراءة الحالية غير صحيحة.";
                return;
            }

            decimal consumption = current - previous;

            decimal unitPriceToUse = CurrentUnitPrice;
            decimal serviceFeesToUse = CurrentServiceFees;

            if (CurrentTariffPlanID.HasValue &&
                string.Equals(CurrentPricingModel, "Tiered", StringComparison.OrdinalIgnoreCase))
            {
                unitPriceToUse = ResolveTieredUnitPrice(CurrentTariffPlanID.Value, consumption);
                lblTariff.Text = $"التعرفة: (Tiered) سعر الشريحة {unitPriceToUse:N2} + رسوم {serviceFeesToUse:N2}";
            }

            if (unitPriceToUse <= 0)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ لا توجد تعرفة صحيحة.";
                return;
            }

            try
            {
                // ✅ نفترض SP معدل ليستقبل MeterID
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.AddReadingAndGenerateInvoice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SubscriberID", SelectedSubscriberID.Value); // لو لازالت موجودة في SP
                    cmd.Parameters.AddWithValue("@MeterID", SelectedMeterID.Value);           // ✅ الجديد
                    cmd.Parameters.AddWithValue("@ReadingDate", dtpReadingDate.Value.Date);
                    cmd.Parameters.AddWithValue("@CurrentReading", current);
                    cmd.Parameters.AddWithValue("@UnitPrice", unitPriceToUse);
                    cmd.Parameters.AddWithValue("@ServiceFees", serviceFeesToUse);
                    cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim());

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadSelectionData(SelectedSubscriberID.Value, SelectedMeterID.Value);

                btnAdd.Enabled = false;
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "✅ تمت الإضافة وإنشاء الفاتورة بنجاح!";

                txtNotes.Text = "";
                txtCurrentReading.Text = "";
                txtSubscriberSearch.Focus();
                txtSubscriberSearch.SelectAll();
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ خطأ: " + ex.Message;
            }
        }
    }
}




/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace water3
{
    public class ReadingForm : Form
    {
        ComboBox ddlSubscribers;
        DateTimePicker dtpReadingDate;
        TextBox txtPreviousReading, txtCurrentReading, txtConsumption, txtNotes;
        Label lblLastReading, lblLastInvoice, lblMessage;
        Button btnAdd;
         
        //string connStr = "Data Source=.;Initial Catalog=Water2026;Integrated Security=True";
        private DataTable subscribersData = new DataTable();
        private bool suppressTextChanged = false;

        public ReadingForm()
        {
            this.Text = "إدخال قراءة عداد";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            // TableLayoutPanel رئيسي
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18, 14, 18, 14),
                BackColor = Color.White
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // ملخص أعلى
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 260)); // إدخال القراءة
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // فارغ (للمستقبل)

            // ملخص القراءة والفاتورة الأخيرة
            FlowLayoutPanel topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8, 8, 8, 8),
                AutoSize = true,
                BackColor = Color.FromArgb(237, 242, 247)
            };

            lblLastInvoice = new Label
            {
                Text = "آخر فاتورة: ---",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 60, 70),
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(28, 0, 0, 0)
            };
            lblLastReading = new Label
            {
                Text = "آخر قراءة: ---",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 60, 70),
                TextAlign = ContentAlignment.MiddleRight
            };
            topPanel.Controls.Add(lblLastInvoice);
            topPanel.Controls.Add(lblLastReading);

            // مجموعة إدخال القراءة
            GroupBox group = new GroupBox
            {
                Text = "إضافة قراءة جديدة",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 49, 89),
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(13, 10, 13, 7)
            };

            // TableLayoutPanel داخلي للحقول
            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 4,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(8)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));

            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // الحقول
            ddlSubscribers = new ComboBox()
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDown, // يسمح بالكتابة
                Font = new Font("Segoe UI", 11),
                AutoCompleteMode = AutoCompleteMode.None, // بدون أي إكمال تلقائي
                AutoCompleteSource = AutoCompleteSource.None
            };

            ddlSubscribers.SelectedIndexChanged += DdlSubscribers_SelectedIndexChanged;
            ddlSubscribers.KeyUp += DdlSubscribers_KeyUp;
            ddlSubscribers.TextChanged += DdlSubscribers_TextChanged;

            dtpReadingDate = new DateTimePicker() { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) };

            txtPreviousReading = new TextBox() { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.Gainsboro, Font = new Font("Segoe UI", 11) };
            txtCurrentReading = new TextBox() { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11) };
            txtCurrentReading.TextChanged += TxtCurrentReading_TextChanged;
            txtConsumption = new TextBox() { Dock = DockStyle.Fill, ReadOnly = true, BackColor = Color.Gainsboro, Font = new Font("Segoe UI", 11) };
            txtNotes = new TextBox() { Dock = DockStyle.Fill, Multiline = true, Height = 38, Font = new Font("Segoe UI", 11) };

            btnAdd = new Button()
            {
                Text = "إضافة",
                Dock = DockStyle.Fill,
                Height = 38,
                BackColor = Color.FromArgb(80, 199, 110),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false // يبدأ معطل
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.MouseEnter += (s, e) => btnAdd.BackColor = Color.FromArgb(56, 170, 90);
            btnAdd.MouseLeave += (s, e) => btnAdd.BackColor = Color.FromArgb(80, 199, 110);
            btnAdd.Click += BtnAdd_Click;

            lblMessage = new Label()
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ترتيب الحقول داخل الجدول
            table.Controls.Add(new Label() { Text = "اسم المشترك:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) }, 0, 0);
            table.Controls.Add(ddlSubscribers, 1, 0);
            table.Controls.Add(new Label() { Text = "تاريخ القراءة:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) }, 2, 0);
            table.Controls.Add(dtpReadingDate, 3, 0);

            table.Controls.Add(new Label() { Text = "القراءة السابقة:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, AutoSize = true, Font = new Font("Segoe UI", 11) }, 0, 1);
            table.Controls.Add(txtPreviousReading, 1, 1);
            table.Controls.Add(new Label() { Text = "القراءة الحالية:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, AutoSize = true, Font = new Font("Segoe UI", 11) }, 2, 1);
            table.Controls.Add(txtCurrentReading, 3, 1);

            table.Controls.Add(new Label() { Text = "الاستهلاك:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, AutoSize = true, Font = new Font("Segoe UI", 11) }, 0, 2);
            table.Controls.Add(txtConsumption, 1, 2);
            table.Controls.Add(new Label() { Text = "ملاحظات:", Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight, AutoSize = true, Font = new Font("Segoe UI", 11) }, 2, 2);
            table.Controls.Add(txtNotes, 3, 2);

            table.Controls.Add(btnAdd, 1, 3);
            table.Controls.Add(lblMessage, 3, 3);

            group.Controls.Add(table);

            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(group, 0, 1);

            this.Controls.Add(mainLayout);

            // تحميل قائمة المشتركين تلقائياً
            LoadSubscribers();
        }

        private void LoadSubscribers()
        {
            ddlSubscribers.Items.Clear();
            using (SqlConnection con = Db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT SubscriberID, Name FROM Subscribers", con);
                subscribersData = new DataTable();
                da.Fill(subscribersData);

                ddlSubscribers.Items.Clear();
                foreach (DataRow row in subscribersData.Rows)
                {
                    ddlSubscribers.Items.Add(new ComboBoxItem(row["Name"].ToString(), row["SubscriberID"].ToString()));
                }
            }
            if (ddlSubscribers.Items.Count > 0)
                ddlSubscribers.SelectedIndex = 0;
        }
        private bool isFiltering = false;

        private void DdlSubscribers_TextChanged(object sender, EventArgs e)
        {
            if (suppressTextChanged || isFiltering) return;
            suppressTextChanged = true;

            string searchText = ddlSubscribers.Text.ToLower();

            // ابدأ وضع التصفية، حتى لا يتداخل مع اختيار المستخدم
            isFiltering = true;

            if (searchText.Length >= 0)
            {
                ddlSubscribers.Items.Clear();

                var filteredItems = subscribersData.AsEnumerable()
                    .Select(row => row["Name"].ToString())
                    .Where(name => name.ToLower().Contains(searchText))
                    .Distinct()
                    .ToList();

                foreach (var name in filteredItems)
                {
                    var row = subscribersData.AsEnumerable().First(r => r["Name"].ToString() == name);
                    ddlSubscribers.Items.Add(new ComboBoxItem(name, row["SubscriberID"].ToString()));
                }

                ddlSubscribers.DroppedDown = filteredItems.Count > 0;
            }
            else
            {
                ddlSubscribers.Items.Clear();
                ddlSubscribers.DroppedDown = false;
            }

            ddlSubscribers.IntegralHeight = true;
            // **لا تعدل SelectedIndex هنا!**

            // تحقق من المطابقة التامة فقط لتفعيل زر الإضافة
            string matchedId = "";
            foreach (var it in ddlSubscribers.Items)
            {
                if (it.ToString().Trim().Equals(ddlSubscribers.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    matchedId = ((ComboBoxItem)it).Value;
            }
            btnAdd.Enabled = !string.IsNullOrEmpty(matchedId) && CheckCurrentReadingValid();
            lblMessage.Text = "";

            isFiltering = false;
            suppressTextChanged = false;
        }

        private void DdlSubscribers_KeyUp(object sender, KeyEventArgs e)
        {
            // عند الضغط على أسهم الاتجاهات
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (!ddlSubscribers.DroppedDown)
                {
                    ddlSubscribers.DroppedDown = true;
                }
            }
            // عند الضغط على Enter وتكون القائمة مفتوحة
            else if (e.KeyCode == Keys.Enter && ddlSubscribers.DroppedDown)
            {
                foreach (var item in ddlSubscribers.Items)
                {
                    if (item.ToString().Equals(ddlSubscribers.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        ddlSubscribers.SelectedItem = item;
                        break;
                    }
                }
                ddlSubscribers.DroppedDown = false;
                e.Handled = true;
            }
        }

        // عند تغيير الاختيار في القائمة (بالفأرة أو Enter فقط) هنا تحمل بيانات المشترك
        private void DdlSubscribers_SelectedIndexChanged(object sender, EventArgs e)
        {
            string subscriberId = GetSelectedSubscriberID();
            if (!string.IsNullOrEmpty(subscriberId))
            {
                LoadLastReading(subscriberId);
                LoadLastInvoice(subscriberId);
                LoadPreviousReading(subscriberId);
                btnAdd.Enabled = CheckCurrentReadingValid();
                lblMessage.Text = "";
            }
            else
            {
                btnAdd.Enabled = false;
            }
        }
        private string GetSelectedSubscriberID()
        {
            if (ddlSubscribers.SelectedItem is ComboBoxItem item)
                return item.Value;

            foreach (var it in ddlSubscribers.Items)
            {
                if (it.ToString().Trim().Equals(ddlSubscribers.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    return ((ComboBoxItem)it).Value;
            }
            return "";
        }

        private void LoadLastReading(string subscriberId)
        {
            using (SqlConnection con = Db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("GetLastReading", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();
                lblLastReading.Text = "آخر قراءة: " + (result != null ? result.ToString() : "لا توجد");
            }
        }

        private void LoadLastInvoice(string subscriberId)
        {
            using (SqlConnection con = Db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("GetLastInvoice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblLastInvoice.Text = "آخر فاتورة: " +
                        "المبلغ: " + dr["TotalAmount"].ToString() +
                        " - الحالة: " + dr["Status"].ToString();
                }
                else
                {
                    lblLastInvoice.Text = "آخر فاتورة: لا توجد";
                }
                con.Close();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ReadingForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ReadingForm";
            this.Load += new System.EventHandler(this.ReadingForm_Load);
            this.ResumeLayout(false);

        }

        private void ReadingForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadPreviousReading(string subscriberId)
        {
            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();

                // 1️⃣ جلب آخر قراءة
                string queryReading = @"SELECT TOP 1 CurrentReading 
                                FROM Readings 
                                WHERE SubscriberID = @SubscriberID 
                                ORDER BY ReadingDate DESC, ReadingID DESC";
                using (SqlCommand cmd = new SqlCommand(queryReading, con))
                {
                    cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                    object result = cmd.ExecuteScalar();
                    txtPreviousReading.Text = result != null ? result.ToString() : "0";
                }

                // 2️⃣ التحقق من وجود الحساب المحاسبي للمشترك
                string queryAccount = @"SELECT AccountID 
                                FROM Subscribers 
                                WHERE SubscriberID = @SubscriberID";
                using (SqlCommand cmd = new SqlCommand(queryAccount, con))
                {
                    cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                    object accountResult = cmd.ExecuteScalar();

                    if (accountResult == null || accountResult == DBNull.Value)
                    {
                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Text = "⚠️ هذا المشترك لا يمتلك حسابًا محاسبيًا! لا يمكن إضافة القراءة.";
                        btnAdd.Enabled = false;
                    }
                    else
                    {
                        lblMessage.Text = "";
                        // السماح بالإضافة فقط إذا كانت القراءة صحيحة
                        btnAdd.Enabled = CheckCurrentReadingValid();
                    }
                }

                con.Close();
            }

            // تفريغ الحقول الأخرى
            txtCurrentReading.Text = "";
            txtConsumption.Text = "";
        }


        private void TxtCurrentReading_TextChanged(object sender, EventArgs e)
        {
            lblMessage.ForeColor = Color.Red;
            btnAdd.Enabled = false;

            if (decimal.TryParse(txtCurrentReading.Text, out decimal current) &&
                decimal.TryParse(txtPreviousReading.Text, out decimal previous))
            {
                if (current < previous)
                {
                    lblMessage.Text = "⚠️ القراءة الحالية يجب أن تكون أكبر من القراءة السابقة!";
                    txtConsumption.Text = "";
                    return;
                }
                else if (current == previous)
                {
                    lblMessage.Text = "⚠️ القراءة الحالية تساوي القراءة السابقة (لا يوجد استهلاك)";
                    txtConsumption.Text = "0";
                    return;
                }
                else
                {
                    txtConsumption.Text = (current - previous).ToString();
                    lblMessage.Text = "";
                }
            }
            else
            {
                txtConsumption.Text = "";
                lblMessage.Text = "⚠️ أدخل أرقامًا صحيحة في الحقول";
                return;
            }

            // السماح بالإضافة فقط إذا كان مشترك صحيح والاستهلاك موجب
            string subscriberId = GetSelectedSubscriberID();
            btnAdd.Enabled = !string.IsNullOrEmpty(subscriberId) && CheckCurrentReadingValid();
            if (btnAdd.Enabled)
            {
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "";
            }
        }

        private bool CheckCurrentReadingValid()
        {
            if (decimal.TryParse(txtCurrentReading.Text, out decimal current) &&
                decimal.TryParse(txtPreviousReading.Text, out decimal previous))
            {
                return current > previous;
            }
            return false;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string subscriberId = GetSelectedSubscriberID();

            if (string.IsNullOrEmpty(subscriberId))
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ يرجى اختيار مشترك صحيح.";
                return;
            }

            if (!decimal.TryParse(txtCurrentReading.Text, out decimal current) ||
                !decimal.TryParse(txtPreviousReading.Text, out decimal previous) ||
                current <= previous)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ القراءة الحالية يجب أن تكون أكبر من السابقة.";
                return;
            }

            decimal consumption = current - previous;
            decimal unitPrice = 100;    // يمكن لاحقًا جعله من جدول الإعدادات
            decimal serviceFees = 0;

            try
            {
                using (SqlConnection con = Db.GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("AddReadingAndGenerateInvoice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                        cmd.Parameters.AddWithValue("@ReadingDate", dtpReadingDate.Value.Date);
                        cmd.Parameters.AddWithValue("@CurrentReading", current);
                        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        cmd.Parameters.AddWithValue("@ServiceFees", serviceFees);
                        cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                // تحديث واجهة المستخدم بعد الإدخال
                LoadPreviousReading(subscriberId);
                LoadLastReading(subscriberId);
                LoadLastInvoice(subscriberId);

                btnAdd.Enabled = false;
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "✅ تمت الإضافة وإنشاء الفاتورة والقيد المحاسبي بنجاح!";
            }
            catch (SqlException ex)
            {
                // عرض رسالة واضحة من SP أو SQL
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = $"❌ خطأ من قاعدة البيانات: {ex.Message}";
            }
            catch (Exception ex)
            {
                // أي خطأ آخر
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = $"❌ حدث خطأ غير متوقع: {ex.Message}";
            }
        }


        class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public ComboBoxItem(string text, string value)
            {
                Text = text; Value = value;
            }
            public override string ToString()
            {
                return Text;
            }
        }
    }
}
*/
