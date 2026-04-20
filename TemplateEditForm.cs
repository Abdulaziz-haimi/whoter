using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using water3.Services;
using water3.Utils;

namespace water3
{
    public partial class TemplateEditForm : Form
    {
        public int TemplateID { get; private set; }
        public string TemplateName { get; private set; }
        public string TemplateText { get; private set; }
        public string TemplateType { get; private set; }
        public bool IsActive { get; private set; }
        public string Language { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly bool _isEdit;
        private readonly MessageTemplatePreviewService _previewService = new MessageTemplatePreviewService();

        public TemplateEditForm(DataGridViewRow row = null)
        {
            InitializeComponent();

            _isEdit = row != null;
            CreatedAt = DateTime.Now;

            InitializeForm();
            SetupCombos();
            WireEvents();
            ApplyMode(row);
            SetupVariablesForSelectedType();
            UpdateCounters();
        }

        private void InitializeForm()
        {
            Text = _isEdit ? "تعديل القالب" : "قالب جديد";
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Tahoma", 9F);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            txtName.MaxLength = 150;
            txtText.MaxLength = 2000;

            // لتحسين عرض قائمة المتغيرات
            cmbVars.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVars.DrawMode = DrawMode.OwnerDrawFixed;
            cmbVars.ItemHeight = 22;
            cmbVars.DrawItem -= CmbVars_DrawItem;
            cmbVars.DrawItem += CmbVars_DrawItem;
        }

        private void SetupCombos()
        {
            cmbType.Items.Clear();
            cmbType.Items.AddRange(new object[]
            {
                "Invoice",
                "Payment",
                "Late"
            });

            cmbLang.Items.Clear();
            cmbLang.Items.AddRange(new object[]
            {
                "AR",
                "EN"
            });

            if (cmbType.Items.Count > 0)
                cmbType.SelectedIndex = 0;

            if (cmbLang.Items.Count > 0)
                cmbLang.SelectedIndex = 0;
        }

        private void WireEvents()
        {
            btnSave.Click -= BtnSave_Click;
            btnSave.Click += BtnSave_Click;

            btnCancel.Click -= BtnCancel_Click;
            btnCancel.Click += BtnCancel_Click;

            btnInsertVar.Click -= BtnInsertVar_Click;
            btnInsertVar.Click += BtnInsertVar_Click;

            btnPreview.Click -= BtnPreview_Click;
            btnPreview.Click += BtnPreview_Click;

            txtText.TextChanged -= TxtAny_TextChanged;
            txtText.TextChanged += TxtAny_TextChanged;

            txtName.TextChanged -= TxtAny_TextChanged;
            txtName.TextChanged += TxtAny_TextChanged;

            cmbType.SelectedIndexChanged -= CmbType_SelectedIndexChanged;
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;

            cmbVars.DoubleClick -= CmbVars_DoubleClick;
            cmbVars.DoubleClick += CmbVars_DoubleClick;
        }

        private void ApplyMode(DataGridViewRow row)
        {
            if (!_isEdit || row == null)
            {
                chkActive.Checked = true;
                return;
            }

            TemplateID = ToInt(row.Cells["TemplateID"]?.Value);
            txtName.Text = ToStr(row.Cells["TemplateName"]?.Value);
            txtText.Text = ToStr(row.Cells["TemplateText"]?.Value);

            string type = ToStr(row.Cells["TemplateType"]?.Value);
            string lang = ToStr(row.Cells["Language"]?.Value);

            if (!string.IsNullOrWhiteSpace(type) && cmbType.Items.Contains(type))
                cmbType.SelectedItem = type;

            if (!string.IsNullOrWhiteSpace(lang) && cmbLang.Items.Contains(lang))
                cmbLang.SelectedItem = lang;

            chkActive.Checked = ToBool(row.Cells["IsActive"]?.Value, true);
            CreatedAt = ToDate(row.Cells["CreatedAt"]?.Value, DateTime.Now);
        }

        private void SetupVariablesForSelectedType()
        {
            string type = cmbType.SelectedItem?.ToString() ?? "Invoice";

            var vars = SmsTemplateVariables.GetByType(type)
                .OrderBy(v => v.Name)
                .ToList();

            cmbVars.BeginUpdate();
            cmbVars.DataSource = null;
            cmbVars.Items.Clear();

            // نربط العناصر ككائنات وليس كنصوص فقط
            cmbVars.DataSource = vars;
            cmbVars.DisplayMember = nameof(SmsTemplateVariableInfo.Name);
            cmbVars.ValueMember = nameof(SmsTemplateVariableInfo.Name);

            cmbVars.EndUpdate();

            if (cmbVars.Items.Count > 0)
                cmbVars.SelectedIndex = 0;
        }

        private void CmbVars_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0 || e.Index >= cmbVars.Items.Count)
                return;

            var item = cmbVars.Items[e.Index] as SmsTemplateVariableInfo;
            if (item == null)
                return;

            string text = "{" + item.Name + "} - " + item.Description;

            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(
                    text,
                    e.Font,
                    brush,
                    e.Bounds.Left + 4,
                    e.Bounds.Top + 2);
            }

            e.DrawFocusRectangle();
        }

        private void BtnInsertVar_Click(object sender, EventArgs e)
        {
            InsertSelectedVariable();
        }

        private void CmbVars_DoubleClick(object sender, EventArgs e)
        {
            InsertSelectedVariable();
        }

        private void InsertSelectedVariable()
        {
            if (!(cmbVars.SelectedItem is SmsTemplateVariableInfo item))
                return;

            string token = "{" + item.Name + "}";

            int start = txtText.SelectionStart;
            string current = txtText.Text ?? string.Empty;

            txtText.Text = current.Insert(start, token);
            txtText.SelectionStart = start + token.Length;
            txtText.SelectionLength = 0;
            txtText.Focus();

            UpdateCounters();
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            string type = cmbType.SelectedItem?.ToString() ?? "Invoice";
            string text = (txtText.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(
                    "نص القالب فارغ.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var invalid = _previewService.ValidateTemplate(type, text);
            if (invalid.Count > 0)
            {
                MessageBox.Show(
                    "القالب يحتوي على متغيرات غير مدعومة لهذا النوع:\n\n" +
                    string.Join("\n", invalid.Select(x => "{" + x + "}")),
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string preview = _previewService.RenderPreview(type, text);

            using (var p = new TemplatePreviewForm(txtName.Text.Trim(), preview))
                p.ShowDialog(this);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveAndClose();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupVariablesForSelectedType();
        }

        private void TxtAny_TextChanged(object sender, EventArgs e)
        {
            UpdateCounters();
        }

        private void UpdateCounters()
        {
            int len = (txtText.Text ?? string.Empty).Length;
            lblChars.Text = $"الأحرف: {len}";

            // تقدير تقريبي فقط
            if (len <= 160)
                lblSmsHint.Text = "SMS: رسالة واحدة غالبًا";
            else if (len <= 320)
                lblSmsHint.Text = "SMS: قد تصبح رسالتين";
            else
                lblSmsHint.Text = "SMS: قد تصبح عدة رسائل";
        }

        private void SaveAndClose()
        {
            string name = (txtName.Text ?? string.Empty).Trim();
            string text = (txtText.Text ?? string.Empty).Trim();
            string type = cmbType.SelectedItem?.ToString();
            string lang = cmbLang.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "يرجى تعبئة اسم القالب.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(
                    "يرجى تعبئة نص القالب.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtText.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(lang))
            {
                MessageBox.Show(
                    "يرجى اختيار النوع واللغة.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var invalid = _previewService.ValidateTemplate(type, text);
            if (invalid.Count > 0)
            {
                MessageBox.Show(
                    "لا يمكن حفظ القالب لأن فيه متغيرات غير مدعومة لهذا النوع:\n\n" +
                    string.Join("\n", invalid.Select(x => "{" + x + "}")),
                    "خطأ في القالب",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            TemplateName = name;
            TemplateText = text;
            TemplateType = type;
            Language = lang;
            IsActive = chkActive.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private static string ToStr(object v)
        {
            return v == null || v == DBNull.Value ? string.Empty : v.ToString();
        }

        private static int ToInt(object v)
        {
            int.TryParse(ToStr(v), out int x);
            return x;
        }

        private static bool ToBool(object v, bool def)
        {
            if (v == null || v == DBNull.Value)
                return def;

            if (v is bool b)
                return b;

            bool.TryParse(v.ToString(), out bool x);
            return x;
        }

        private static DateTime ToDate(object v, DateTime def)
        {
            if (v == null || v == DBNull.Value)
                return def;

            if (v is DateTime d)
                return d;

            DateTime.TryParse(v.ToString(), out DateTime x);
            return x == default ? def : x;
        }
    }
}
/*using System;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class TemplateEditForm : Form
    {
        public int TemplateID { get; private set; }
        public string TemplateName { get; private set; }
        public string TemplateText { get; private set; }
        public string TemplateType { get; private set; }
        public bool IsActive { get; private set; }
        public string Language { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly bool isEdit;

        public TemplateEditForm(DataGridViewRow row = null)
        {
            InitializeComponent();

            isEdit = row != null;
            CreatedAt = DateTime.Now;

            SetupCombos();
            WireEvents();

            ApplyMode(row);

            // مهم: بعد ApplyMode عشان يكون النوع معروف ونحمل المتغيرات المناسبة
            SetupVariablesForSelectedType();

            UpdateCounters();
        }

        private void SetupCombos()
        {
            cmbType.Items.Clear();
            cmbType.Items.AddRange(new object[]
            {
                "Invoice",
                "Payment",
                "Late"
            });

            cmbLang.Items.Clear();
            cmbLang.Items.AddRange(new object[] { "AR", "EN" });

            if (cmbType.Items.Count > 0) cmbType.SelectedIndex = 0;
            if (cmbLang.Items.Count > 0) cmbLang.SelectedIndex = 0;
        }

        /// <summary>
        /// تحميل المتغيرات بحسب نوع القالب ليتوافق مع SendSMSDynamic الجديد
        /// </summary>
        /// <summary>
        /// تحميل جميع المتغيرات المدعومة في SendSMSDynamic
        /// </summary>
        private void SetupVariablesForSelectedType()
        {
            string type = cmbType.SelectedItem?.ToString() ?? "Invoice";

            cmbVars.Items.Clear();

            // =========================
            // متغيرات مشتركة لكل الأنواع
            // =========================
            AddVar("{SubscriberName}");
            AddVar("{PhoneNumber}");
            AddVar("{Today}");

            AddVar("{InvoiceID}");
            AddVar("{PaymentID}");
            AddVar("{InvoiceDate}");

            AddVar("{TotalAmount}");
            AddVar("{Arrears}");
            AddVar("{GrandTotal}");

            AddVar("{Amount}");
            AddVar("{PaidTotal}");
            AddVar("{Remaining}");

            AddVar("{Consumption}");
            AddVar("{CurrentReading}");
            AddVar("{PreviousReading}");
            AddVar("{MeterID}");

            AddVar("{LateDays}");

            // =========================
            // تخصيص العرض حسب النوع
            // =========================
            // (فقط لتحسين تجربة المستخدم — ليس إلزامي)

            if (type == "Invoice")
            {
                HighlightRecommended(new[]
                {
            "{InvoiceID}",
            "{CurrentReading}",
            "{Consumption}",
            "{TotalAmount}",
            "{Arrears}",
            "{GrandTotal}",
            "{Remaining}"
        });
            }
            else if (type == "Payment")
            {
                HighlightRecommended(new[]
                {
            "{PaymentID}",
            "{InvoiceID}",
            "{Amount}",
            "{PaidTotal}",
            "{Remaining}"
        });
            }
            else if (type == "Late")
            {
                HighlightRecommended(new[]
                {
            "{InvoiceID}",
            "{GrandTotal}",
            "{Remaining}",
            "{LateDays}"
        });
            }

            if (cmbVars.Items.Count > 0)
                cmbVars.SelectedIndex = 0;

            btnInsertVar.Click -= BtnInsertVar_Click;
            btnInsertVar.Click += BtnInsertVar_Click;
        }
        private void HighlightRecommended(string[] tokens)
        {
            // فقط تلميح بصري: لا يغير المنطق
            for (int i = 0; i < cmbVars.Items.Count; i++)
            {
                string item = cmbVars.Items[i].ToString();
                if (Array.Exists(tokens, t => t == item))
                {
                    // يمكن لاحقًا إضافة أي تمييز بصري
                    // حالياً لا نغير شيء – فقط جاهز للتطوير
                }
            }
        }
        private void AddVar(string token)
        {
            if (!cmbVars.Items.Contains(token))
                cmbVars.Items.Add(token);
        }

        private void BtnInsertVar_Click(object sender, EventArgs e)
        {
            if (cmbVars.SelectedItem == null) return;

            string token = cmbVars.SelectedItem.ToString();
            int start = txtText.SelectionStart;

            txtText.Text = txtText.Text.Insert(start, token);
            txtText.SelectionStart = start + token.Length;
            txtText.Focus();

            UpdateCounters();
        }

        private void WireEvents()
        {
            btnSave.Click += (s, e) => SaveAndClose();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            txtText.TextChanged += (s, e) => UpdateCounters();
            txtName.TextChanged += (s, e) => UpdateCounters();

            // لما يغير نوع القالب: نغير المتغيرات المعروضة
            cmbType.SelectedIndexChanged += (s, e) => SetupVariablesForSelectedType();

            btnPreview.Click += (s, e) =>
            {
                using (var p = new TemplatePreviewForm(txtName.Text.Trim(), txtText.Text))
                    p.ShowDialog(this);
            };
        }

        private void ApplyMode(DataGridViewRow row)
        {
            Text = isEdit ? "تعديل القالب" : "قالب جديد";

            if (isEdit)
            {
                TemplateID = ToInt(row.Cells["TemplateID"]?.Value);
                txtName.Text = ToStr(row.Cells["TemplateName"]?.Value);
                txtText.Text = ToStr(row.Cells["TemplateText"]?.Value);

                var type = ToStr(row.Cells["TemplateType"]?.Value);
                var lang = ToStr(row.Cells["Language"]?.Value);

                if (!string.IsNullOrWhiteSpace(type) && cmbType.Items.Contains(type))
                    cmbType.SelectedItem = type;

                if (!string.IsNullOrWhiteSpace(lang) && cmbLang.Items.Contains(lang))
                    cmbLang.SelectedItem = lang;

                chkActive.Checked = ToBool(row.Cells["IsActive"]?.Value, true);
                CreatedAt = ToDate(row.Cells["CreatedAt"]?.Value, DateTime.Now);
            }
            else
            {
                chkActive.Checked = true;
            }
        }

        private void UpdateCounters()
        {
            int len = (txtText.Text ?? "").Length;
            lblChars.Text = $"الأحرف: {len}";

            // تنبيه SMS تقريبي
            lblSmsHint.Text = len <= 160 ? "SMS: رسالة واحدة غالبًا"
                        : len <= 320 ? "SMS: قد تصبح رسالتين"
                        : "SMS: قد تصبح عدة رسائل";
        }

        private void SaveAndClose()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtText.Text))
            {
                MessageBox.Show("يرجى تعبئة اسم القالب ونص القالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbType.SelectedItem == null || cmbLang.SelectedItem == null)
            {
                MessageBox.Show("يرجى اختيار النوع واللغة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TemplateName = txtName.Text.Trim();
            TemplateText = txtText.Text.Trim();
            TemplateType = cmbType.SelectedItem.ToString();
            Language = cmbLang.SelectedItem.ToString();
            IsActive = chkActive.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        // Helpers
        private static string ToStr(object v) => v == null || v == DBNull.Value ? "" : v.ToString();
        private static int ToInt(object v) { int.TryParse(ToStr(v), out int x); return x; }

        private static bool ToBool(object v, bool def)
        {
            if (v == null || v == DBNull.Value) return def;
            if (v is bool b) return b;
            bool.TryParse(v.ToString(), out bool x);
            return x;
        }

        private static DateTime ToDate(object v, DateTime def)
        {
            if (v == null || v == DBNull.Value) return def;
            if (v is DateTime d) return d;
            DateTime.TryParse(v.ToString(), out DateTime x);
            return x == default ? def : x;
        }
    }
}
*/