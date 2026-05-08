using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using water3.Services;
namespace water3.Reports.Dynamic
{
    public partial class DynamicReportDesignerForm : Form
    {




            private ComboBox cboReportType;
            private ComboBox cboTemplates;
            private ComboBox cboSubscriber;
            private ComboBox cboCategoryType;

            private CheckedListBox chkColumns;

            private CheckBox chkUseFromDate;
            private CheckBox chkUseToDate;
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;

            private TextBox txtSearch;
            private TextBox txtTemplateName;

            private Button btnPreview;
            private Button btnSaveTemplate;
            private Button btnLoadTemplate;
            private Button btnDeleteTemplate;
            private Button btnExportCsv;
            private Button btnPrint;
            private Button btnSelectAll;
            private Button btnClearAll;

            private DataGridView dgv;
            private Label lblStatus;

            private readonly DynamicReportService _service = new DynamicReportService();
            private readonly DynamicGridPrintService _printService = new DynamicGridPrintService();

            private DataTable _currentData;

            public DynamicReportDesignerForm()
            {
                BuildUi();
                LoadReportTypes();
                LoadSubscribers();
            }

            private void BuildUi()
            {
                Text = "مصمم التقارير الديناميكي";
                WindowState = FormWindowState.Maximized;
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;

                Panel pnlTop = new Panel();
                pnlTop.Dock = DockStyle.Top;
                pnlTop.Height = 230;
                pnlTop.BackColor = Color.FromArgb(248, 250, 252);
                pnlTop.BorderStyle = BorderStyle.FixedSingle;

                Label lblTitle = new Label();
                lblTitle.Text = "مصمم التقارير الديناميكي";
                lblTitle.Font = new Font("Tahoma", 16F, FontStyle.Bold);
                lblTitle.ForeColor = Color.FromArgb(0, 102, 204);
                lblTitle.AutoSize = true;
                lblTitle.Location = new Point(20, 15);

                Label lblReportType = MakeLabel("نوع التقرير", 1120, 55);
                cboReportType = new ComboBox();
                cboReportType.Location = new Point(900, 50);
                cboReportType.Size = new Size(200, 28);
                cboReportType.DropDownStyle = ComboBoxStyle.DropDownList;
                cboReportType.SelectedIndexChanged += CboReportType_SelectedIndexChanged;

                Label lblTemplate = MakeLabel("التصميمات", 700, 55);
                cboTemplates = new ComboBox();
                cboTemplates.Location = new Point(480, 50);
                cboTemplates.Size = new Size(200, 28);
                cboTemplates.DropDownStyle = ComboBoxStyle.DropDownList;

                btnLoadTemplate = MakeButton("تحميل التصميم", 350, 48, 120, Color.SteelBlue);
                btnLoadTemplate.Click += BtnLoadTemplate_Click;

                btnDeleteTemplate = MakeButton("حذف التصميم", 220, 48, 120, Color.Firebrick);
                btnDeleteTemplate.Click += BtnDeleteTemplate_Click;

                Label lblTemplateName = MakeLabel("اسم التصميم", 1120, 95);
                txtTemplateName = new TextBox();
                txtTemplateName.Location = new Point(900, 90);
                txtTemplateName.Size = new Size(200, 28);

                btnSaveTemplate = MakeButton("حفظ التصميم", 770, 88, 120, Color.SeaGreen);
                btnSaveTemplate.Click += BtnSaveTemplate_Click;

                chkUseFromDate = new CheckBox();
                chkUseFromDate.Text = "من تاريخ";
                chkUseFromDate.Location = new Point(650, 92);
                chkUseFromDate.AutoSize = true;

                dtFrom = new DateTimePicker();
                dtFrom.Location = new Point(500, 90);
                dtFrom.Size = new Size(140, 28);
                dtFrom.Format = DateTimePickerFormat.Short;
                dtFrom.Value = DateTime.Today.AddMonths(-1);

                chkUseToDate = new CheckBox();
                chkUseToDate.Text = "إلى تاريخ";
                chkUseToDate.Location = new Point(380, 92);
                chkUseToDate.AutoSize = true;

                dtTo = new DateTimePicker();
                dtTo.Location = new Point(230, 90);
                dtTo.Size = new Size(140, 28);
                dtTo.Format = DateTimePickerFormat.Short;
                dtTo.Value = DateTime.Today;

                Label lblSubscriber = MakeLabel("المشترك", 1120, 135);
                cboSubscriber = new ComboBox();
                cboSubscriber.Location = new Point(900, 130);
                cboSubscriber.Size = new Size(200, 28);
                cboSubscriber.DropDownStyle = ComboBoxStyle.DropDownList;

                Label lblCategoryType = MakeLabel("نوع المصروف", 700, 135);
                cboCategoryType = new ComboBox();
                cboCategoryType.Location = new Point(480, 130);
                cboCategoryType.Size = new Size(200, 28);
                cboCategoryType.DropDownStyle = ComboBoxStyle.DropDownList;
                cboCategoryType.Items.AddRange(new object[] { "الكل", "Expense", "Purchase", "Loss" });
                cboCategoryType.SelectedIndex = 0;

                Label lblSearch = MakeLabel("بحث", 1120, 175);
                txtSearch = new TextBox();
                txtSearch.Location = new Point(900, 170);
                txtSearch.Size = new Size(200, 28);

                btnPreview = MakeButton("معاينة", 770, 168, 120, Color.FromArgb(0, 122, 204));
                btnPreview.Click += BtnPreview_Click;

                btnExportCsv = MakeButton("تصدير CSV", 640, 168, 120, Color.FromArgb(22, 163, 74));
                btnExportCsv.Click += BtnExportCsv_Click;

                btnPrint = MakeButton("طباعة", 510, 168, 120, Color.FromArgb(14, 165, 233));
                btnPrint.Click += BtnPrint_Click;

                chkColumns = new CheckedListBox();
                chkColumns.Location = new Point(20, 50);
                chkColumns.Size = new Size(190, 110);
                chkColumns.CheckOnClick = true;
                chkColumns.DisplayMember = "ColumnTitle";

                btnSelectAll = MakeButton("تحديد الكل", 20, 168, 90, Color.Gray);
                btnSelectAll.Click += BtnSelectAll_Click;

                btnClearAll = MakeButton("إلغاء الكل", 115, 168, 90, Color.Gray);
                btnClearAll.Click += BtnClearAll_Click;

                lblStatus = new Label();
                lblStatus.Location = new Point(220, 200);
                lblStatus.Size = new Size(1000, 25);
                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.TextAlign = ContentAlignment.MiddleRight;

                pnlTop.Controls.Add(lblTitle);
                pnlTop.Controls.Add(lblReportType);
                pnlTop.Controls.Add(cboReportType);
                pnlTop.Controls.Add(lblTemplate);
                pnlTop.Controls.Add(cboTemplates);
                pnlTop.Controls.Add(btnLoadTemplate);
                pnlTop.Controls.Add(btnDeleteTemplate);
                pnlTop.Controls.Add(lblTemplateName);
                pnlTop.Controls.Add(txtTemplateName);
                pnlTop.Controls.Add(btnSaveTemplate);
                pnlTop.Controls.Add(chkUseFromDate);
                pnlTop.Controls.Add(dtFrom);
                pnlTop.Controls.Add(chkUseToDate);
                pnlTop.Controls.Add(dtTo);
                pnlTop.Controls.Add(lblSubscriber);
                pnlTop.Controls.Add(cboSubscriber);
                pnlTop.Controls.Add(lblCategoryType);
                pnlTop.Controls.Add(cboCategoryType);
                pnlTop.Controls.Add(lblSearch);
                pnlTop.Controls.Add(txtSearch);
                pnlTop.Controls.Add(btnPreview);
                pnlTop.Controls.Add(btnExportCsv);
                pnlTop.Controls.Add(btnPrint);
                pnlTop.Controls.Add(chkColumns);
                pnlTop.Controls.Add(btnSelectAll);
                pnlTop.Controls.Add(btnClearAll);
                pnlTop.Controls.Add(lblStatus);

                dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.ReadOnly = true;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.BackgroundColor = Color.White;
                dgv.BorderStyle = BorderStyle.None;
                dgv.RowHeadersVisible = false;

                Controls.Add(dgv);
                Controls.Add(pnlTop);
            }

            private Label MakeLabel(string text, int left, int top)
            {
                Label lbl = new Label();
                lbl.Text = text;
                lbl.AutoSize = true;
                lbl.Location = new Point(left, top + 4);
                return lbl;
            }

            private Button MakeButton(string text, int left, int top, int width, Color color)
            {
                Button btn = new Button();
                btn.Text = text;
                btn.Location = new Point(left, top);
                btn.Size = new Size(width, 32);
                btn.BackColor = color;
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.Cursor = Cursors.Hand;
                return btn;
            }

            private void LoadReportTypes()
            {
                List<DynamicReportDefinition> reports = DynamicReportDefinitions.GetAll();

                cboReportType.DataSource = null;
                cboReportType.DisplayMember = "Title";
                cboReportType.ValueMember = "ReportKey";
                cboReportType.DataSource = reports;

                if (reports.Count > 0)
                    cboReportType.SelectedIndex = 0;
            }

            private void LoadSubscribers()
            {
                try
                {
                    DataTable dt = _service.GetSubscribersLookup();

                    cboSubscriber.DataSource = null;
                    cboSubscriber.DisplayMember = "Name";
                    cboSubscriber.ValueMember = "SubscriberID";
                    cboSubscriber.DataSource = dt;
                }
                catch
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("SubscriberID", typeof(int));
                    dt.Columns.Add("Name", typeof(string));
                    dt.Rows.Add(0, "الكل");

                    cboSubscriber.DataSource = null;
                    cboSubscriber.DisplayMember = "Name";
                    cboSubscriber.ValueMember = "SubscriberID";
                    cboSubscriber.DataSource = dt;
                }
            }

            private void CboReportType_SelectedIndexChanged(object sender, EventArgs e)
            {
                LoadColumnsForSelectedReport();
                LoadTemplatesForSelectedReport();
                ApplyFilterAvailability();
            }

            private void LoadColumnsForSelectedReport()
            {
                chkColumns.Items.Clear();

                DynamicReportDefinition def = GetSelectedDefinition();

                foreach (DynamicReportColumn col in def.Columns.OrderBy(c => c.DisplayOrder))
                {
                    int index = chkColumns.Items.Add(col);
                    chkColumns.SetItemChecked(index, col.IsDefaultVisible);
                }
            }

            private void LoadTemplatesForSelectedReport()
            {
                try
                {
                    string reportKey = GetSelectedReportKey();

                    List<DynamicReportTemplateInfo> templates =
                        _service.GetTemplates(reportKey);

                    cboTemplates.DataSource = null;
                    cboTemplates.DisplayMember = "TemplateName";
                    cboTemplates.ValueMember = "TemplateID";
                    cboTemplates.DataSource = templates;
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void ApplyFilterAvailability()
            {
                DynamicReportDefinition def = GetSelectedDefinition();

                chkUseFromDate.Enabled = def.HasDateFilter;
                chkUseToDate.Enabled = def.HasDateFilter;
                dtFrom.Enabled = def.HasDateFilter;
                dtTo.Enabled = def.HasDateFilter;

                cboSubscriber.Enabled = def.HasSubscriberFilter;
                cboCategoryType.Enabled = def.HasCategoryTypeFilter;
            }

            private DynamicReportDefinition GetSelectedDefinition()
            {
                string key = GetSelectedReportKey();
                return DynamicReportDefinitions.GetByKey(key);
            }

            private string GetSelectedReportKey()
            {
                if (cboReportType.SelectedValue == null)
                    return "Invoices";

                return Convert.ToString(cboReportType.SelectedValue);
            }

            private List<string> GetSelectedColumnKeys()
            {
                return chkColumns.CheckedItems
                    .Cast<DynamicReportColumn>()
                    .Select(c => c.ColumnKey)
                    .ToList();
            }

            private int? GetSelectedSubscriberId()
            {
                if (cboSubscriber.SelectedValue == null ||
                    cboSubscriber.SelectedValue == DBNull.Value)
                    return null;

                int id;

                if (int.TryParse(Convert.ToString(cboSubscriber.SelectedValue), out id) && id > 0)
                    return id;

                return null;
            }

            private string GetSelectedCategoryType()
            {
                if (cboCategoryType.SelectedItem == null)
                    return "الكل";

                return Convert.ToString(cboCategoryType.SelectedItem);
            }

            private void BtnPreview_Click(object sender, EventArgs e)
            {
                LoadPreview();
            }

            private void LoadPreview()
            {
                try
                {
                    List<string> selectedKeys = GetSelectedColumnKeys();

                    DateTime? fromDate = chkUseFromDate.Checked ? (DateTime?)dtFrom.Value.Date : null;
                    DateTime? toDate = chkUseToDate.Checked ? (DateTime?)dtTo.Value.Date : null;

                    _currentData = _service.LoadReport(
                        GetSelectedReportKey(),
                        selectedKeys,
                        fromDate,
                        toDate,
                        GetSelectedSubscriberId(),
                        GetSelectedCategoryType(),
                        txtSearch.Text.Trim());

                    dgv.DataSource = null;
                    dgv.DataSource = _currentData;

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم تحميل التقرير. عدد السجلات: " + _currentData.Rows.Count;
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnSaveTemplate_Click(object sender, EventArgs e)
            {
                try
                {
                    DynamicReportTemplateState state = new DynamicReportTemplateState();

                    state.TemplateName = txtTemplateName.Text.Trim();
                    state.ReportKey = GetSelectedReportKey();
                    state.ColumnKeys = GetSelectedColumnKeys();

                    state.UseFromDate = chkUseFromDate.Checked;
                    state.UseToDate = chkUseToDate.Checked;
                    state.FromDate = dtFrom.Value.Date;
                    state.ToDate = dtTo.Value.Date;
                    state.SubscriberID = GetSelectedSubscriberId();
                    state.CategoryType = GetSelectedCategoryType();
                    state.SearchText = txtSearch.Text.Trim();

                    int templateId = _service.SaveTemplate(state, Environment.UserName);

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حفظ التصميم بنجاح. رقم التصميم: " + templateId;

                    LoadTemplatesForSelectedReport();
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnLoadTemplate_Click(object sender, EventArgs e)
            {
                try
                {
                    if (cboTemplates.SelectedValue == null)
                    {
                        MessageBox.Show("اختر تصميمًا أولًا.", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    int templateId;

                    if (!int.TryParse(Convert.ToString(cboTemplates.SelectedValue), out templateId))
                        return;

                    DynamicReportTemplateState state = _service.GetTemplate(templateId);

                    for (int i = 0; i < chkColumns.Items.Count; i++)
                    {
                        DynamicReportColumn col = chkColumns.Items[i] as DynamicReportColumn;
                        chkColumns.SetItemChecked(i, col != null && state.ColumnKeys.Contains(col.ColumnKey));
                    }

                    txtTemplateName.Text = state.TemplateName;
                    chkUseFromDate.Checked = state.UseFromDate;
                    chkUseToDate.Checked = state.UseToDate;

                    if (state.FromDate.HasValue)
                        dtFrom.Value = state.FromDate.Value;

                    if (state.ToDate.HasValue)
                        dtTo.Value = state.ToDate.Value;

                    if (state.SubscriberID.HasValue)
                        cboSubscriber.SelectedValue = state.SubscriberID.Value;
                    else
                        cboSubscriber.SelectedValue = 0;

                    if (!string.IsNullOrWhiteSpace(state.CategoryType) &&
                        cboCategoryType.Items.Contains(state.CategoryType))
                        cboCategoryType.SelectedItem = state.CategoryType;
                    else
                        cboCategoryType.SelectedIndex = 0;

                    txtSearch.Text = state.SearchText ?? "";

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم تحميل التصميم.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnDeleteTemplate_Click(object sender, EventArgs e)
            {
                try
                {
                    if (cboTemplates.SelectedValue == null)
                        return;

                    int templateId;

                    if (!int.TryParse(Convert.ToString(cboTemplates.SelectedValue), out templateId))
                        return;

                    DialogResult result = MessageBox.Show(
                        "هل تريد حذف هذا التصميم؟",
                        "تأكيد",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                        return;

                    _service.DeleteTemplate(templateId);
                    LoadTemplatesForSelectedReport();

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حذف التصميم.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnExportCsv_Click(object sender, EventArgs e)
            {
                try
                {
                    if (_currentData == null || _currentData.Rows.Count == 0)
                    {
                        MessageBox.Show("لا توجد بيانات للتصدير.", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ReportExportService exportService = new ReportExportService();
                    exportService.ExportGridToCsv(dgv, "DynamicReport");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في التصدير",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnPrint_Click(object sender, EventArgs e)
            {
                if (_currentData == null || _currentData.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _printService.PrintPreview(dgv, GetSelectedDefinition().Title);
            }

            private void BtnSelectAll_Click(object sender, EventArgs e)
            {
                for (int i = 0; i < chkColumns.Items.Count; i++)
                    chkColumns.SetItemChecked(i, true);
            }

            private void BtnClearAll_Click(object sender, EventArgs e)
            {
                for (int i = 0; i < chkColumns.Items.Count; i++)
                    chkColumns.SetItemChecked(i, false);
            }
        }
    }