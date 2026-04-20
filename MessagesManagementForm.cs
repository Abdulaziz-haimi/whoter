using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using water3.Services;

namespace water3
{
    public partial class MessagesManagementForm : Form
    {
        private TabControl tabControl;
        private readonly MessageTemplatePreviewService _previewService = new MessageTemplatePreviewService();
        private DataGridView dgvSettings;
        private DataGridView dgvTemplates;
        private DataGridView dgvDefaults;

        private DataTable settingsTable;
        private DataTable templatesTable;
        private DataTable defaultsTable;

        private bool _templateToggleWired = false;
        private bool _defaultsWired = false;

        public MessagesManagementForm()
        {
            InitializeComponent();

            InitializeForm();
            BuildUI();

            LoadSettingsData();
            LoadTemplatesData();

            EnsureDefaultsRows();
            LoadDefaultsData();
        }

        private void InitializeForm()
        {
            Text = "إدارة الرسائل";
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Tahoma", 9F);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;
        }

        private void BuildUI()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill };

            // ===== Tab 1: Settings =====
            dgvSettings = CreateGrid();
            var tabSettings = new TabPage("إعدادات الرسائل");
            tabSettings.Controls.Add(dgvSettings);
            tabSettings.Controls.Add(CreateToolbarSettings());

            // ===== Tab 2: Templates =====
            dgvTemplates = CreateGrid();
            dgvTemplates.DataError += (s, e) => e.ThrowException = false;

            var tabTemplates = new TabPage("قوالب الرسائل");
            tabTemplates.Controls.Add(dgvTemplates);
            tabTemplates.Controls.Add(CreateToolbarTemplates());

            // ===== Tab 3: Defaults =====
            dgvDefaults = CreateGrid();
            dgvDefaults.DataError += (s, e) => e.ThrowException = false;

            var tabDefaults = new TabPage("القالب الافتراضي (Invoice/Payment/Late)");
            tabDefaults.Controls.Add(dgvDefaults);
            tabDefaults.Controls.Add(CreateToolbarDefaults());

            tabControl.TabPages.Add(tabSettings);
            tabControl.TabPages.Add(tabTemplates);
            tabControl.TabPages.Add(tabDefaults);

            Controls.Add(tabControl);
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        // ===================== Toolbars =====================

        private Panel CreateToolbarSettings()
        {
            var p = CreateToolbarPanel(Color.FromArgb(52, 152, 219));
            p.Controls.Add(CreateBtn("إضافة", 10, AddSetting_Click));
            p.Controls.Add(CreateBtn("تعديل", 125, EditSetting_Click));
            p.Controls.Add(CreateBtn("حذف", 240, DeleteSetting_Click));
            p.Controls.Add(CreateBtn("تحديث", 355, (s, e) => LoadSettingsData()));
            return p;
        }

        private Panel CreateToolbarTemplates()
        {
            var p = CreateToolbarPanel(Color.FromArgb(155, 89, 182));
            p.Controls.Add(CreateBtn("قالب جديد", 10, AddTemplate_Click));
            p.Controls.Add(CreateBtn("تعديل", 125, EditTemplate_Click));
            p.Controls.Add(CreateBtn("معاينة", 240, PreviewTemplate_Click));
            p.Controls.Add(CreateBtn("تحديث", 355, (s, e) => LoadTemplatesData()));
            p.Controls.Add(CreateBtn("تعيين افتراضي", 470, SetDefaultByTemplateType_Click));
            return p;
        }

        private Panel CreateToolbarDefaults()
        {
            var p = CreateToolbarPanel(Color.FromArgb(46, 204, 113));
            p.Controls.Add(CreateBtn("تحديث", 10, (s, e) => LoadDefaultsData()));
            p.Controls.Add(CreateBtn("حفظ", 125, (s, e) => SaveDefaultsAll()));
            return p;
        }

        private Panel CreateToolbarPanel(Color back)
        {
            return new Panel { Dock = DockStyle.Top, Height = 45, BackColor = back };
        }

        private Button CreateBtn(string text, int x, EventHandler click)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, 7),
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += click;
            return b;
        }

        // ===================== Load Settings =====================

        private void LoadSettingsData()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                var da = new SqlDataAdapter(
                    "SELECT SettingID, SettingName, SettingValue, Description FROM MessageSettings ORDER BY SettingID DESC",
                    con);

                settingsTable = new DataTable();
                da.Fill(settingsTable);
                dgvSettings.DataSource = settingsTable;
            }

            if (dgvSettings.Columns.Contains("SettingID"))
            {
                dgvSettings.Columns["SettingID"].HeaderText = "الرقم";
                dgvSettings.Columns["SettingName"].HeaderText = "الاسم";
                dgvSettings.Columns["SettingValue"].HeaderText = "القيمة";
                dgvSettings.Columns["Description"].HeaderText = "الوصف";
                dgvSettings.Columns["SettingID"].FillWeight = 12;
            }
        }

        // ===================== Load Templates =====================

        private void LoadTemplatesData()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                var da = new SqlDataAdapter(
                    @"SELECT TemplateID, TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt
                      FROM MessageTemplates
                      ORDER BY TemplateID DESC",
                    con);

                templatesTable = new DataTable();
                da.Fill(templatesTable);
                dgvTemplates.DataSource = templatesTable;
            }

            SetupTemplateGrid();
            LoadDefaultsData();
        }

        private void SetupTemplateGrid()
        {
            if (!dgvTemplates.Columns.Contains("TemplateID")) return;

            dgvTemplates.Columns["TemplateID"].HeaderText = "الرقم";
            dgvTemplates.Columns["TemplateName"].HeaderText = "اسم القالب";
            dgvTemplates.Columns["TemplateType"].HeaderText = "النوع";
            dgvTemplates.Columns["Language"].HeaderText = "اللغة";
            dgvTemplates.Columns["CreatedAt"].HeaderText = "تاريخ الإنشاء";

            if (dgvTemplates.Columns.Contains("TemplateText"))
                dgvTemplates.Columns["TemplateText"].Visible = false;

            if (!dgvTemplates.Columns.Contains("IsActiveToggle"))
            {
                dgvTemplates.Columns["IsActive"].Visible = false;

                var chk = new DataGridViewCheckBoxColumn
                {
                    Name = "IsActiveToggle",
                    HeaderText = "نشط",
                    DataPropertyName = "IsActive",
                    Width = 60
                };

                int idx = dgvTemplates.Columns["TemplateType"].Index + 1;
                dgvTemplates.Columns.Insert(idx, chk);
            }

            if (!_templateToggleWired)
            {
                dgvTemplates.CurrentCellDirtyStateChanged += (s, e) =>
                {
                    if (dgvTemplates.IsCurrentCellDirty)
                        dgvTemplates.CommitEdit(DataGridViewDataErrorContexts.Commit);
                };

                dgvTemplates.CellValueChanged += (s, e) =>
                {
                    if (e.RowIndex < 0) return;
                    if (dgvTemplates.Columns[e.ColumnIndex].Name != "IsActiveToggle") return;

                    int id = Convert.ToInt32(dgvTemplates.Rows[e.RowIndex].Cells["TemplateID"].Value);
                    bool active = Convert.ToBoolean(dgvTemplates.Rows[e.RowIndex].Cells["IsActiveToggle"].Value);

                    using (SqlConnection con = Db.GetConnection())
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE MessageTemplates SET IsActive=@a WHERE TemplateID=@id", con))
                        {
                            cmd.Parameters.AddWithValue("@a", active);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    LoadDefaultsData();
                };

                _templateToggleWired = true;
            }

            foreach (DataGridViewColumn c in dgvTemplates.Columns)
                c.ReadOnly = c.Name != "IsActiveToggle";
        }

        // ===================== Defaults =====================

        private void EnsureDefaultsRows()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
IF OBJECT_ID('dbo.MessageTemplateMap','U') IS NULL
BEGIN
    CREATE TABLE dbo.MessageTemplateMap
    (
        MapID      INT IDENTITY(1,1) PRIMARY KEY,
        EventKey   NVARCHAR(50) NOT NULL UNIQUE,
        TemplateID INT NULL,
        IsEnabled  BIT NOT NULL DEFAULT(1),
        UpdatedAt  DATETIME NOT NULL DEFAULT(GETDATE())
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey='Invoice')
    INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled) VALUES ('Invoice', NULL, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey='Payment')
    INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled) VALUES ('Payment', NULL, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey='Late')
    INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled) VALUES ('Late', NULL, 1);
", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadDefaultsData()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                var da = new SqlDataAdapter(@"
SELECT MapID, EventKey, TemplateID, IsEnabled, UpdatedAt
FROM dbo.MessageTemplateMap
WHERE EventKey IN ('Invoice','Payment','Late')
ORDER BY EventKey;", con);

                defaultsTable = new DataTable();
                da.Fill(defaultsTable);
            }

            dgvDefaults.Columns.Clear();
            dgvDefaults.DataSource = defaultsTable;

            dgvDefaults.Columns["MapID"].HeaderText = "الرقم";
            dgvDefaults.Columns["EventKey"].HeaderText = "النوع";
            dgvDefaults.Columns["TemplateID"].Visible = false;
            dgvDefaults.Columns["IsEnabled"].HeaderText = "مفعل";
            dgvDefaults.Columns["UpdatedAt"].HeaderText = "آخر تحديث";

            var combo = new DataGridViewComboBoxColumn
            {
                Name = "TemplatePick",
                HeaderText = "القالب الافتراضي",
                FlatStyle = FlatStyle.Flat
            };

            dgvDefaults.Columns.Insert(2, combo);

            FillDefaultsTemplateChoices();

            foreach (DataGridViewColumn c in dgvDefaults.Columns)
                c.ReadOnly = !(c.Name == "TemplatePick" || c.Name == "IsEnabled");

            if (!_defaultsWired)
            {
                dgvDefaults.CurrentCellDirtyStateChanged += DgvDefaults_CurrentCellDirtyStateChanged;
                dgvDefaults.CellValueChanged += DgvDefaults_CellValueChanged;
                dgvDefaults.CellFormatting += DgvDefaults_CellFormatting;
                _defaultsWired = true;
            }
        }

        private void FillDefaultsTemplateChoices()
        {
            for (int i = 0; i < dgvDefaults.Rows.Count; i++)
            {
                var row = dgvDefaults.Rows[i];
                string eventKey = row.Cells["EventKey"].Value?.ToString() ?? "";
                int? currentTemplateId = null;

                if (row.Cells["TemplateID"].Value != null &&
                    row.Cells["TemplateID"].Value != DBNull.Value &&
                    int.TryParse(row.Cells["TemplateID"].Value.ToString(), out int tid))
                {
                    currentTemplateId = tid;
                }

                var items = GetActiveTemplatesByType(eventKey);

                var cell = new DataGridViewComboBoxCell
                {
                    DisplayMember = "Text",
                    ValueMember = "Value",
                    DataSource = items,
                    FlatStyle = FlatStyle.Flat
                };

                row.Cells["TemplatePick"] = cell;

                if (currentTemplateId.HasValue && items.Any(x => x.Value == currentTemplateId.Value))
                    row.Cells["TemplatePick"].Value = currentTemplateId.Value;
                else
                    row.Cells["TemplatePick"].Value = DBNull.Value;
            }
        }

        private List<TemplatePickItem> GetActiveTemplatesByType(string templateType)
        {
            var list = new List<TemplatePickItem>
            {
                new TemplatePickItem { Value = 0, Text = "(بدون قالب)" }
            };

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TemplateID, TemplateName
FROM dbo.MessageTemplates
WHERE IsActive = 1
  AND Language = 'AR'
  AND TemplateType = @t
ORDER BY TemplateName;", con))
                {
                    cmd.Parameters.AddWithValue("@t", templateType);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new TemplatePickItem
                            {
                                Value = Convert.ToInt32(r["TemplateID"]),
                                Text = r["TemplateName"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }

        private void DgvDefaults_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvDefaults.IsCurrentCellDirty)
                dgvDefaults.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvDefaults_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvDefaults.Columns[e.ColumnIndex].Name != "TemplatePick" &&
                dgvDefaults.Columns[e.ColumnIndex].Name != "IsEnabled")
                return;

            SaveDefaultsRow(e.RowIndex);
        }

        private void DgvDefaults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvDefaults.Columns[e.ColumnIndex].Name == "EventKey" && e.Value != null)
            {
                var v = e.Value.ToString();
                if (v == "Invoice") e.Value = "فاتورة (Invoice)";
                else if (v == "Payment") e.Value = "سداد (Payment)";
                else if (v == "Late") e.Value = "متأخرات (Late)";
            }
        }

        private bool IsTemplateTypeValidForEvent(int templateId, string eventKey)
        {
            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 TemplateType
FROM dbo.MessageTemplates
WHERE TemplateID = @id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", templateId);
                    var result = cmd.ExecuteScalar()?.ToString();

                    if (string.IsNullOrWhiteSpace(result))
                        return false;

                    return string.Equals(result.Trim(), eventKey?.Trim(), StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        private void SaveDefaultsRow(int rowIndex)
        {
            var row = dgvDefaults.Rows[rowIndex];
            if (row == null) return;

            if (!int.TryParse(row.Cells["MapID"].Value?.ToString(), out int mapId))
                return;

            string eventKey = row.Cells["EventKey"].Value?.ToString() ?? "";

            int? templateId = null;
            var tplVal = row.Cells["TemplatePick"].Value;
            if (tplVal != null && tplVal != DBNull.Value && int.TryParse(tplVal.ToString(), out int tid) && tid > 0)
                templateId = tid;

            bool enabled = true;
            var enVal = row.Cells["IsEnabled"].Value;
            if (enVal != null && enVal != DBNull.Value)
                enabled = Convert.ToBoolean(enVal);

            if (templateId.HasValue && !IsTemplateTypeValidForEvent(templateId.Value, eventKey))
            {
                MessageBox.Show(
                    $"القالب المختار لا يطابق النوع المطلوب.\nالحدث الحالي: {eventKey}",
                    "اختيار غير صحيح",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                LoadDefaultsData();
                return;
            }

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.MessageTemplateMap
SET TemplateID=@tid, IsEnabled=@en, UpdatedAt=GETDATE()
WHERE MapID=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@tid", (object)templateId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@en", enabled);
                    cmd.Parameters.AddWithValue("@id", mapId);
                    cmd.ExecuteNonQuery();
                }
            }

            row.Cells["UpdatedAt"].Value = DateTime.Now;
        }

        private void SaveDefaultsAll()
        {
            for (int i = 0; i < dgvDefaults.Rows.Count; i++)
                SaveDefaultsRow(i);

            MessageBox.Show("تم حفظ القوالب الافتراضية ✅");
        }

        // ===================== Settings CRUD =====================

        private void AddSetting_Click(object sender, EventArgs e)
        {
            using (var f = new SettingEditForm())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    using (SqlConnection con = Db.GetConnection())
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO MessageSettings (SettingName, SettingValue, Description)
VALUES (@n,@v,@d);
SELECT SCOPE_IDENTITY();", con))
                        {
                            cmd.Parameters.AddWithValue("@n", f.SettingName);
                            cmd.Parameters.AddWithValue("@v", f.SettingValue);
                            cmd.Parameters.AddWithValue("@d", (object)f.Description ?? DBNull.Value);

                            int newId = Convert.ToInt32(cmd.ExecuteScalar());

                            var r = settingsTable.NewRow();
                            r["SettingID"] = newId;
                            r["SettingName"] = f.SettingName;
                            r["SettingValue"] = f.SettingValue;
                            r["Description"] = (object)f.Description ?? DBNull.Value;
                            settingsTable.Rows.InsertAt(r, 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }

        private void EditSetting_Click(object sender, EventArgs e)
        {
            if (dgvSettings.SelectedRows.Count == 0) return;

            var row = dgvSettings.SelectedRows[0];
            using (var f = new SettingEditForm(row))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    using (SqlConnection con = Db.GetConnection())
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE MessageSettings
SET SettingName=@n, SettingValue=@v, Description=@d
WHERE SettingID=@id;", con))
                        {
                            cmd.Parameters.AddWithValue("@n", f.SettingName);
                            cmd.Parameters.AddWithValue("@v", f.SettingValue);
                            cmd.Parameters.AddWithValue("@d", (object)f.Description ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@id", row.Cells["SettingID"].Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    row.Cells["SettingName"].Value = f.SettingName;
                    row.Cells["SettingValue"].Value = f.SettingValue;
                    row.Cells["Description"].Value = (object)f.Description ?? DBNull.Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }

        private void DeleteSetting_Click(object sender, EventArgs e)
        {
            if (dgvSettings.SelectedRows.Count == 0) return;

            if (MessageBox.Show("هل أنت متأكد من الحذف؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();

                    foreach (DataGridViewRow r in dgvSettings.SelectedRows)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "DELETE FROM MessageSettings WHERE SettingID=@id", con))
                        {
                            cmd.Parameters.AddWithValue("@id", r.Cells["SettingID"].Value);
                            cmd.ExecuteNonQuery();
                        }

                        dgvSettings.Rows.Remove(r);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ: " + ex.Message);
            }
        }

        // ===================== Templates CRUD =====================

        private void AddTemplate_Click(object sender, EventArgs e)
        {
            using (var f = new TemplateEditForm())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO MessageTemplates(TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt)
VALUES(@n,@t,@ty,@a,@l,@c);", con))
                    {
                        cmd.Parameters.AddWithValue("@n", f.TemplateName);
                        cmd.Parameters.AddWithValue("@t", f.TemplateText);
                        cmd.Parameters.AddWithValue("@ty", f.TemplateType);
                        cmd.Parameters.AddWithValue("@a", f.IsActive);
                        cmd.Parameters.AddWithValue("@l", f.Language);
                        cmd.Parameters.AddWithValue("@c", f.CreatedAt);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            LoadTemplatesData();
        }

        private void EditTemplate_Click(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedRows.Count == 0) return;

            var row = dgvTemplates.SelectedRows[0];
            using (var f = new TemplateEditForm(row))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
UPDATE MessageTemplates
SET TemplateName=@n, TemplateText=@t, TemplateType=@ty, IsActive=@a, Language=@l
WHERE TemplateID=@id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", row.Cells["TemplateID"].Value);
                        cmd.Parameters.AddWithValue("@n", f.TemplateName);
                        cmd.Parameters.AddWithValue("@t", f.TemplateText);
                        cmd.Parameters.AddWithValue("@ty", f.TemplateType);
                        cmd.Parameters.AddWithValue("@a", f.IsActive);
                        cmd.Parameters.AddWithValue("@l", f.Language);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            LoadTemplatesData();
        }
        private void PreviewTemplate_Click(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedRows.Count == 0) return;

            var row = dgvTemplates.SelectedRows[0];

            string name = row.Cells["TemplateName"].Value?.ToString() ?? "";
            string text = row.Cells["TemplateText"].Value?.ToString() ?? "";
            string type = row.Cells["TemplateType"].Value?.ToString() ?? "";

            var invalid = _previewService.ValidateTemplate(type, text);
            if (invalid.Count > 0)
            {
                MessageBox.Show(
                    "القالب يحتوي على متغيرات غير مدعومة لهذا النوع:\n\n" +
                    string.Join("\n", invalid.ConvertAll(x => "{" + x + "}")),
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string preview = _previewService.RenderPreview(type, text);

            using (var p = new TemplatePreviewForm(name, preview))
                p.ShowDialog(this);
        }
        //private void PreviewTemplate_Click(object sender, EventArgs e)
        //{
        //    if (dgvTemplates.SelectedRows.Count == 0) return;

        //    var row = dgvTemplates.SelectedRows[0];
        //    string name = row.Cells["TemplateName"].Value?.ToString() ?? "";
        //    string text = row.Cells["TemplateText"].Value?.ToString() ?? "";

        //    using (var p = new TemplatePreviewForm(name, text))
        //        p.ShowDialog(this);
        //}

        private void SetDefaultByTemplateType_Click(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedRows.Count == 0) return;

            var row = dgvTemplates.SelectedRows[0];
            int templateId = Convert.ToInt32(row.Cells["TemplateID"].Value);
            string type = row.Cells["TemplateType"].Value?.ToString();

            if (type != "Invoice" && type != "Payment" && type != "Late")
            {
                MessageBox.Show("لا يمكن تعيين افتراضي إلا إذا كان TemplateType = Invoice أو Payment أو Late");
                return;
            }

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.MessageTemplateMap
SET TemplateID=@tid, IsEnabled=1, UpdatedAt=GETDATE()
WHERE EventKey=@k;", con))
                {
                    cmd.Parameters.AddWithValue("@tid", templateId);
                    cmd.Parameters.AddWithValue("@k", type);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadDefaultsData();
            MessageBox.Show($"تم تعيين القالب افتراضيًا لـ {type} ✅");
        }

        private class TemplatePickItem
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;
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
    public partial class MessagesManagementForm : Form
    {
        private TabControl tabControl;

        private DataGridView dgvSettings;
        private DataGridView dgvTemplates;
        private DataGridView dgvDefaults;

        private DataTable settingsTable;
        private DataTable templatesTable;
        private DataTable defaultsTable;

        public MessagesManagementForm()
        {
            InitializeComponent();

            InitializeForm();
            BuildUI();

            LoadSettingsData();
            LoadTemplatesData();

            EnsureDefaultsRows();
            LoadDefaultsData();
        }

        private void InitializeForm()
        {
            Text = "إدارة الرسائل";
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Tahoma", 9F);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
        }

        private void BuildUI()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill };

            // ===== Tab 1: Settings =====
            dgvSettings = CreateGrid();
            var tabSettings = new TabPage("إعدادات الرسائل");
            tabSettings.Controls.Add(dgvSettings);
            tabSettings.Controls.Add(CreateToolbarSettings());

            // ===== Tab 2: Templates =====
            dgvTemplates = CreateGrid();
            dgvTemplates.DataError += (s, e) => e.ThrowException = false;

            var tabTemplates = new TabPage("قوالب الرسائل");
            tabTemplates.Controls.Add(dgvTemplates);
            tabTemplates.Controls.Add(CreateToolbarTemplates());

            // ===== Tab 3: Defaults =====
            dgvDefaults = CreateGrid();
            dgvDefaults.DataError += (s, e) => e.ThrowException = false;

            var tabDefaults = new TabPage("القالب الافتراضي (Invoice/Payment/Late)");
            tabDefaults.Controls.Add(dgvDefaults);
            tabDefaults.Controls.Add(CreateToolbarDefaults());

            tabControl.TabPages.Add(tabSettings);
            tabControl.TabPages.Add(tabTemplates);
            tabControl.TabPages.Add(tabDefaults);

            Controls.Add(tabControl);
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        // ===================== Toolbars =====================

        private Panel CreateToolbarSettings()
        {
            var p = CreateToolbarPanel(Color.FromArgb(52, 152, 219));
            p.Controls.Add(CreateBtn("إضافة", 10, AddSetting_Click));
            p.Controls.Add(CreateBtn("تعديل", 125, EditSetting_Click));
            p.Controls.Add(CreateBtn("حذف", 240, DeleteSetting_Click));
            p.Controls.Add(CreateBtn("تحديث", 355, (s, e) => LoadSettingsData()));
            return p;
        }

        private Panel CreateToolbarTemplates()
        {
            var p = CreateToolbarPanel(Color.FromArgb(155, 89, 182));
            p.Controls.Add(CreateBtn("قالب جديد", 10, AddTemplate_Click));
            p.Controls.Add(CreateBtn("تعديل", 125, EditTemplate_Click));
            p.Controls.Add(CreateBtn("معاينة", 240, PreviewTemplate_Click));
            p.Controls.Add(CreateBtn("تحديث", 355, (s, e) => LoadTemplatesData()));
            p.Controls.Add(CreateBtn("تعيين افتراضي", 470, SetDefaultByTemplateType_Click));
            return p;
        }

        private Panel CreateToolbarDefaults()
        {
            var p = CreateToolbarPanel(Color.FromArgb(46, 204, 113));
            p.Controls.Add(CreateBtn("تحديث", 10, (s, e) => LoadDefaultsData()));
            p.Controls.Add(CreateBtn("حفظ", 125, (s, e) => SaveDefaultsAll()));
            return p;
        }

        private Panel CreateToolbarPanel(Color back)
        {
            return new Panel { Dock = DockStyle.Top, Height = 45, BackColor = back };
        }

        private Button CreateBtn(string text, int x, EventHandler click)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, 7),
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += click;
            return b;
        }

        // ===================== Load Settings =====================

        private void LoadSettingsData()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                var da = new SqlDataAdapter(
                    "SELECT SettingID, SettingName, SettingValue, Description FROM MessageSettings ORDER BY SettingID DESC",
                    con);

                settingsTable = new DataTable();
                da.Fill(settingsTable);
                dgvSettings.DataSource = settingsTable;
            }

            if (dgvSettings.Columns.Contains("SettingID"))
            {
                dgvSettings.Columns["SettingID"].HeaderText = "الرقم";
                dgvSettings.Columns["SettingName"].HeaderText = "الاسم";
                dgvSettings.Columns["SettingValue"].HeaderText = "القيمة";
                dgvSettings.Columns["Description"].HeaderText = "الوصف";
                dgvSettings.Columns["SettingID"].FillWeight = 12;
            }
        }

        // ===================== Load Templates =====================

        private void LoadTemplatesData()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                var da = new SqlDataAdapter(
                    "SELECT TemplateID, TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt FROM MessageTemplates ORDER BY TemplateID DESC",
                    con);

                templatesTable = new DataTable();
                da.Fill(templatesTable);
                dgvTemplates.DataSource = templatesTable;
            }

            SetupTemplateGrid();
            LoadDefaultsData(); // لتحديث قوائم الافتراضي بعد أي تعديل
        }

        private void SetupTemplateGrid()
        {
            if (!dgvTemplates.Columns.Contains("TemplateID")) return;

            dgvTemplates.Columns["TemplateID"].HeaderText = "الرقم";
            dgvTemplates.Columns["TemplateName"].HeaderText = "اسم القالب";
            dgvTemplates.Columns["TemplateType"].HeaderText = "النوع";
            dgvTemplates.Columns["Language"].HeaderText = "اللغة";
            dgvTemplates.Columns["CreatedAt"].HeaderText = "تاريخ الإنشاء";

            if (dgvTemplates.Columns.Contains("TemplateText"))
                dgvTemplates.Columns["TemplateText"].Visible = false;

            // Toggle Active
            if (!dgvTemplates.Columns.Contains("IsActiveToggle"))
            {
                dgvTemplates.Columns["IsActive"].Visible = false;

                var chk = new DataGridViewCheckBoxColumn
                {
                    Name = "IsActiveToggle",
                    HeaderText = "نشط",
                    DataPropertyName = "IsActive",
                    Width = 60
                };

                int idx = dgvTemplates.Columns["TemplateType"].Index + 1;
                dgvTemplates.Columns.Insert(idx, chk);

                dgvTemplates.CurrentCellDirtyStateChanged += (s, e) =>
                {
                    if (dgvTemplates.IsCurrentCellDirty)
                        dgvTemplates.CommitEdit(DataGridViewDataErrorContexts.Commit);
                };

                dgvTemplates.CellValueChanged += (s, e) =>
                {
                    if (e.RowIndex < 0) return;
                    if (dgvTemplates.Columns[e.ColumnIndex].Name != "IsActiveToggle") return;

                    int id = Convert.ToInt32(dgvTemplates.Rows[e.RowIndex].Cells["TemplateID"].Value);
                    bool active = Convert.ToBoolean(dgvTemplates.Rows[e.RowIndex].Cells["IsActiveToggle"].Value);

                    using (SqlConnection con = Db.GetConnection())
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE MessageTemplates SET IsActive=@a WHERE TemplateID=@id", con))
                        {
                            cmd.Parameters.AddWithValue("@a", active);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    LoadDefaultsData();
                };
            }

            // Readonly columns
            foreach (DataGridViewColumn c in dgvTemplates.Columns)
                c.ReadOnly = c.Name != "IsActiveToggle";
        }

        // ===================== Defaults (Map) =====================
        private bool IsTemplateTypeValidForEvent(int templateId, string eventKey)
        {
            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 TemplateType
FROM dbo.MessageTemplates
WHERE TemplateID = @id;", con))
                {
                    cmd.Parameters.AddWithValue("@id", templateId);
                    var result = cmd.ExecuteScalar()?.ToString();

                    if (string.IsNullOrWhiteSpace(result))
                        return false;

                    return string.Equals(result.Trim(), eventKey?.Trim(), StringComparison.OrdinalIgnoreCase);
                }
            }
        }
        private void EnsureDefaultsRows()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
IF OBJECT_ID('dbo.MessageTemplateMap','U') IS NULL
BEGIN
    CREATE TABLE dbo.MessageTemplateMap
    (
        MapID      INT IDENTITY(1,1) PRIMARY KEY,
        EventKey   NVARCHAR(50) NOT NULL UNIQUE,
        TemplateID INT NULL,
        IsEnabled  BIT NOT NULL DEFAULT(1),
        UpdatedAt  DATETIME NOT NULL DEFAULT(GETDATE())
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey='Invoice')
    INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled) VALUES ('Invoice', NULL, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey='Payment')
    INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled) VALUES ('Payment', NULL, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.MessageTemplateMap WHERE EventKey='Late')
    INSERT INTO dbo.MessageTemplateMap(EventKey, TemplateID, IsEnabled) VALUES ('Late', NULL, 1);
", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadDefaultsData()
        {
            // Load map
            using (SqlConnection con = Db.GetConnection())
            {
                var da = new SqlDataAdapter(@"
SELECT MapID, EventKey, TemplateID, IsEnabled, UpdatedAt
FROM dbo.MessageTemplateMap
WHERE EventKey IN ('Invoice','Payment','Late')
ORDER BY EventKey;", con);

                defaultsTable = new DataTable();
                da.Fill(defaultsTable);
            }

            // Load active templates to pick (AR only)
            DataTable activeTemplates;
            using (SqlConnection con = Db.GetConnection())
            {
                var daT = new SqlDataAdapter(@"
SELECT TemplateID, TemplateName, TemplateType
FROM dbo.MessageTemplates
WHERE IsActive=1 AND Language='AR' AND TemplateType IN ('Invoice','Payment','Late')
ORDER BY TemplateType, TemplateName;", con);

                activeTemplates = new DataTable();
                daT.Fill(activeTemplates);
            }

            dgvDefaults.Columns.Clear();
            dgvDefaults.DataSource = defaultsTable;

            dgvDefaults.Columns["MapID"].HeaderText = "الرقم";
            dgvDefaults.Columns["EventKey"].HeaderText = "النوع";
            dgvDefaults.Columns["IsEnabled"].HeaderText = "مفعل";
            dgvDefaults.Columns["UpdatedAt"].HeaderText = "آخر تحديث";

            dgvDefaults.Columns["TemplateID"].Visible = false;

            var combo = new DataGridViewComboBoxColumn
            {
                Name = "TemplatePick",
                HeaderText = "القالب الافتراضي",
                DataPropertyName = "TemplateID",
                DataSource = activeTemplates,
                DisplayMember = "TemplateName",
                ValueMember = "TemplateID",
                FlatStyle = FlatStyle.Flat
            };

            dgvDefaults.Columns.Insert(2, combo);

            foreach (DataGridViewColumn c in dgvDefaults.Columns)
                c.ReadOnly = !(c.Name == "TemplatePick" || c.Name == "IsEnabled");

            dgvDefaults.CurrentCellDirtyStateChanged -= DgvDefaults_CurrentCellDirtyStateChanged;
            dgvDefaults.CellValueChanged -= DgvDefaults_CellValueChanged;

            dgvDefaults.CurrentCellDirtyStateChanged += DgvDefaults_CurrentCellDirtyStateChanged;
            dgvDefaults.CellValueChanged += DgvDefaults_CellValueChanged;

            dgvDefaults.CellFormatting += (s, e) =>
            {
                if (dgvDefaults.Columns[e.ColumnIndex].Name == "EventKey" && e.Value != null)
                {
                    var v = e.Value.ToString();
                    if (v == "Invoice") e.Value = "فاتورة (Invoice) — تشمل القراءة";
                    else if (v == "Payment") e.Value = "سداد (Payment)";
                    else if (v == "Late") e.Value = "متأخرات (Late)";
                }
            };
        }

        private void DgvDefaults_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvDefaults.IsCurrentCellDirty)
                dgvDefaults.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvDefaults_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            SaveDefaultsRow(e.RowIndex);
        }
        private void SaveDefaultsRow(int rowIndex)
        {
            var row = dgvDefaults.Rows[rowIndex];
            if (row == null) return;

            if (!int.TryParse(row.Cells["MapID"].Value?.ToString(), out int mapId))
                return;

            string eventKey = row.Cells["EventKey"].Value?.ToString();

            int? templateId = null;
            var tplVal = row.Cells["TemplatePick"].Value;
            if (tplVal != null && int.TryParse(tplVal.ToString(), out int tid))
                templateId = tid;

            bool enabled = true;
            var enVal = row.Cells["IsEnabled"].Value;
            if (enVal != null) enabled = Convert.ToBoolean(enVal);

            if (templateId.HasValue && !IsTemplateTypeValidForEvent(templateId.Value, eventKey))
            {
                MessageBox.Show(
                    $"القالب المختار لا يطابق النوع المطلوب.\n" +
                    $"الحدث الحالي: {eventKey}\n" +
                    $"يجب اختيار قالب من نفس النوع فقط.",
                    "اختيار غير صحيح",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                LoadDefaultsData();
                return;
            }

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.MessageTemplateMap
SET TemplateID=@tid, IsEnabled=@en, UpdatedAt=GETDATE()
WHERE MapID=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@tid", (object)templateId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@en", enabled);
                    cmd.Parameters.AddWithValue("@id", mapId);
                    cmd.ExecuteNonQuery();
                }
            }

            row.Cells["UpdatedAt"].Value = DateTime.Now;
        }
  
        private void SaveDefaultsAll()
        {
            for (int i = 0; i < dgvDefaults.Rows.Count; i++)
                SaveDefaultsRow(i);

            MessageBox.Show("تم حفظ القوالب الافتراضية ✅");
        }

        // ===================== Settings CRUD =====================

        private void AddSetting_Click(object sender, EventArgs e)
        {
            using (var f = new SettingEditForm())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    using (SqlConnection con = Db.GetConnection())
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO MessageSettings (SettingName, SettingValue, Description)
VALUES (@n,@v,@d);
SELECT SCOPE_IDENTITY();", con))
                        {
                            cmd.Parameters.AddWithValue("@n", f.SettingName);
                            cmd.Parameters.AddWithValue("@v", f.SettingValue);
                            cmd.Parameters.AddWithValue("@d", (object)f.Description ?? DBNull.Value);

                            int newId = Convert.ToInt32(cmd.ExecuteScalar());
                            settingsTable.Rows.InsertAt(settingsTable.NewRow(), 0);
                            settingsTable.Rows[0]["SettingID"] = newId;
                            settingsTable.Rows[0]["SettingName"] = f.SettingName;
                            settingsTable.Rows[0]["SettingValue"] = f.SettingValue;
                            settingsTable.Rows[0]["Description"] = f.Description;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }

        private void EditSetting_Click(object sender, EventArgs e)
        {
            if (dgvSettings.SelectedRows.Count == 0) return;

            var row = dgvSettings.SelectedRows[0];
            using (var f = new SettingEditForm(row))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    using (SqlConnection con = Db.GetConnection())
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE MessageSettings
SET SettingName=@n, SettingValue=@v, Description=@d
WHERE SettingID=@id;", con))
                        {
                            cmd.Parameters.AddWithValue("@n", f.SettingName);
                            cmd.Parameters.AddWithValue("@v", f.SettingValue);
                            cmd.Parameters.AddWithValue("@d", (object)f.Description ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@id", row.Cells["SettingID"].Value);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    row.Cells["SettingName"].Value = f.SettingName;
                    row.Cells["SettingValue"].Value = f.SettingValue;
                    row.Cells["Description"].Value = f.Description;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }

        private void DeleteSetting_Click(object sender, EventArgs e)
        {
            if (dgvSettings.SelectedRows.Count == 0) return;

            if (MessageBox.Show("هل أنت متأكد من الحذف؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();
                    foreach (DataGridViewRow r in dgvSettings.SelectedRows)
                    {
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM MessageSettings WHERE SettingID=@id", con))
                        {
                            cmd.Parameters.AddWithValue("@id", r.Cells["SettingID"].Value);
                            cmd.ExecuteNonQuery();
                        }
                        dgvSettings.Rows.Remove(r);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ: " + ex.Message);
            }
        }

        // ===================== Templates CRUD =====================

        private void AddTemplate_Click(object sender, EventArgs e)
        {
            using (var f = new TemplateEditForm())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO MessageTemplates(TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt)
VALUES(@n,@t,@ty,@a,@l,@c);", con))
                    {
                        cmd.Parameters.AddWithValue("@n", f.TemplateName);
                        cmd.Parameters.AddWithValue("@t", f.TemplateText);
                        cmd.Parameters.AddWithValue("@ty", f.TemplateType);
                        cmd.Parameters.AddWithValue("@a", f.IsActive);
                        cmd.Parameters.AddWithValue("@l", f.Language);
                        cmd.Parameters.AddWithValue("@c", f.CreatedAt);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            LoadTemplatesData();
        }

        private void EditTemplate_Click(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedRows.Count == 0) return;

            var row = dgvTemplates.SelectedRows[0];
            using (var f = new TemplateEditForm(row))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;

                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
UPDATE MessageTemplates
SET TemplateName=@n, TemplateText=@t, TemplateType=@ty, IsActive=@a, Language=@l
WHERE TemplateID=@id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", row.Cells["TemplateID"].Value);
                        cmd.Parameters.AddWithValue("@n", f.TemplateName);
                        cmd.Parameters.AddWithValue("@t", f.TemplateText);
                        cmd.Parameters.AddWithValue("@ty", f.TemplateType);
                        cmd.Parameters.AddWithValue("@a", f.IsActive);
                        cmd.Parameters.AddWithValue("@l", f.Language);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            LoadTemplatesData();
        }

        private void PreviewTemplate_Click(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedRows.Count == 0) return;

            var row = dgvTemplates.SelectedRows[0];
            string name = row.Cells["TemplateName"].Value?.ToString() ?? "";
            string text = row.Cells["TemplateText"].Value?.ToString() ?? "";

            using (var p = new TemplatePreviewForm(name, text))
                p.ShowDialog(this);
        }

        // تعيين قالب افتراضي بسرعة (حسب TemplateType)
        private void SetDefaultByTemplateType_Click(object sender, EventArgs e)
        {
            if (dgvTemplates.SelectedRows.Count == 0) return;

            var row = dgvTemplates.SelectedRows[0];
            int templateId = Convert.ToInt32(row.Cells["TemplateID"].Value);
            string type = row.Cells["TemplateType"].Value?.ToString();

            if (type != "Invoice" && type != "Payment" && type != "Late")
            {
                MessageBox.Show("لا يمكن تعيين افتراضي إلا إذا كان TemplateType = Invoice أو Payment أو Late");
                return;
            }

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.MessageTemplateMap
SET TemplateID=@tid, IsEnabled=1, UpdatedAt=GETDATE()
WHERE EventKey=@k;", con))
                {
                    cmd.Parameters.AddWithValue("@tid", templateId);
                    cmd.Parameters.AddWithValue("@k", type);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadDefaultsData();
            MessageBox.Show($"تم تعيين القالب افتراضيًا لـ {type} ✅");
        }
    }
}

*/