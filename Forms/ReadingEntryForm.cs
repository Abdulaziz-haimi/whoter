using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

using System.Globalization;

using System.Drawing.Printing;

namespace water3.Forms
{
    public partial class ReadingEntryForm : Form
    {


            private readonly string connStr = Db.ConnectionString;

            private DataTable _table = new DataTable();

            private readonly PrintDocument _printDocument = new PrintDocument();
            private readonly PrintPreviewDialog _previewDialog = new PrintPreviewDialog();

            private int _printRowIndex = 0;
            private int _printPageNo = 1;
            private List<DataRowView> _printRows = new List<DataRowView>();

            public ReadingEntryForm()
            {
                InitializeComponent();

                ApplyTheme();
                WireEvents();
                BuildGridSchema();
                LoadAreas();

                dtpReadingDate.Value = DateTime.Today;

                _previewDialog.Document = _printDocument;
                _previewDialog.WindowState = FormWindowState.Maximized;

                _printDocument.DefaultPageSettings.Landscape = true;
                _printDocument.PrintPage += PrintDocument_PrintPage;
            }

            #region Theme

            private void ApplyTheme()
            {
                BackColor = Color.FromArgb(243, 246, 251);

                StyleCard(pnlHeaderCard, Color.White);
                StyleCard(pnlFiltersCard, Color.White);
                StyleCard(pnlGridCard, Color.White);

                StyleStatCard(pnlTotalCard, Color.FromArgb(237, 244, 255));
                StyleStatCard(pnlDoneCard, Color.FromArgb(235, 250, 242));
                StyleStatCard(pnlPendingCard, Color.FromArgb(255, 244, 232));

                lblTitle.ForeColor = Color.FromArgb(19, 52, 121);
                lblSubtitle.ForeColor = Color.FromArgb(107, 114, 128);
                lblGridTitle.ForeColor = Color.FromArgb(37, 43, 54);

                txtSearch.BorderStyle = BorderStyle.FixedSingle;
                txtSearch.BackColor = Color.White;
                txtSearch.Font = new Font("Tahoma", 10.5F);

                cmbArea.Font = new Font("Tahoma", 10.5F);
                dtpReadingDate.Font = new Font("Tahoma", 10.5F);

                StyleButton(btnLoad, Color.FromArgb(37, 99, 235));
                StyleButton(btnSaveAll, Color.FromArgb(22, 163, 74));
                StyleButton(btnPrintSheet, Color.FromArgb(14, 165, 233));
                StyleButton(btnRefresh, Color.FromArgb(100, 116, 139));

                lblTotalCaption.ForeColor = Color.FromArgb(30, 41, 59);
                lblDoneCaption.ForeColor = Color.FromArgb(30, 41, 59);
                lblPendingCaption.ForeColor = Color.FromArgb(30, 41, 59);

                lblTotalValue.ForeColor = Color.FromArgb(37, 99, 235);
                lblDoneValue.ForeColor = Color.FromArgb(22, 163, 74);
                lblPendingValue.ForeColor = Color.FromArgb(234, 88, 12);

                dgvReadings.EnableHeadersVisualStyles = false;
                dgvReadings.BackgroundColor = Color.White;
                dgvReadings.BorderStyle = BorderStyle.None;
                dgvReadings.GridColor = Color.FromArgb(230, 235, 241);
                dgvReadings.RowHeadersVisible = false;

                dgvReadings.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dgvReadings.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
                dgvReadings.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
                dgvReadings.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
                dgvReadings.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvReadings.ColumnHeadersHeight = 42;

                dgvReadings.DefaultCellStyle.BackColor = Color.White;
                dgvReadings.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
                dgvReadings.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
                dgvReadings.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
                dgvReadings.DefaultCellStyle.Font = new Font("Tahoma", 10F);
                dgvReadings.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvReadings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);
                dgvReadings.RowTemplate.Height = 36;
            }

            private void StyleCard(Panel panel, Color backColor)
            {
                panel.BackColor = backColor;
                panel.BorderStyle = BorderStyle.FixedSingle;
            }

            private void StyleStatCard(Panel panel, Color backColor)
            {
                panel.BackColor = backColor;
                panel.BorderStyle = BorderStyle.FixedSingle;
            }

            private void StyleButton(Button btn, Color backColor)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.BackColor = backColor;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Tahoma", 10F, FontStyle.Bold);
                btn.Cursor = Cursors.Hand;
            }

            #endregion

            #region Events

            private void WireEvents()
            {
                btnLoad.Click += (s, e) => LoadMetersForFollowUp();
                btnSaveAll.Click += (s, e) => SaveAllReadingsWithProcedure();
                btnPrintSheet.Click += (s, e) => PreviewSheet();
                btnRefresh.Click += (s, e) => LoadMetersForFollowUp();

                txtSearch.TextChanged += (s, e) => ApplyFilter();

                dgvReadings.CellEndEdit += DgvReadings_CellEndEdit;
                dgvReadings.DataError += (s, e) => { e.Cancel = false; };
                dgvReadings.CurrentCellDirtyStateChanged += (s, e) =>
                {
                    if (dgvReadings.IsCurrentCellDirty)
                        dgvReadings.CommitEdit(DataGridViewDataErrorContexts.Commit);
                };
            }

            #endregion

            #region Loaders

            private void LoadAreas()
            {
                try
                {
                    DataTable dt = new DataTable();

                    using (SqlConnection cn = new SqlConnection(connStr))
                    using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT DISTINCT
    ISNULL(NULLIF(LTRIM(RTRIM(Location)), ''), N'بدون تحديد') AS AreaName
FROM dbo.Meters
WHERE IsActive = 1
ORDER BY AreaName;", cn))
                    {
                        da.Fill(dt);
                    }

                    DataRow allRow = dt.NewRow();
                    allRow["AreaName"] = "الكل";
                    dt.Rows.InsertAt(allRow, 0);

                    cmbArea.DisplayMember = "AreaName";
                    cmbArea.ValueMember = "AreaName";
                    cmbArea.DataSource = dt;
                }
                catch
                {
                    cmbArea.Items.Clear();
                    cmbArea.Items.Add("الكل");
                    cmbArea.SelectedIndex = 0;
                }
            }

            private void BuildGridSchema()
            {
                _table = new DataTable();

                _table.Columns.Add("SubscriberID", typeof(int));
                _table.Columns.Add("MeterID", typeof(int));
                _table.Columns.Add("م", typeof(int));
                _table.Columns.Add("رقم الحساب", typeof(string));
                _table.Columns.Add("اسم المشترك", typeof(string));
                _table.Columns.Add("رقم العداد", typeof(string));
                _table.Columns.Add("الموقع", typeof(string));
                _table.Columns.Add("القراءة السابقة", typeof(decimal));
                _table.Columns.Add("القراءة الجديدة", typeof(string));
                _table.Columns.Add("الاستهلاك", typeof(decimal));
                _table.Columns.Add("ملاحظات", typeof(string));
                _table.Columns.Add("تم", typeof(bool));

                dgvReadings.DataSource = _table;

                FormatGrid();
                UpdateSummary();
            }

            private void LoadMetersForFollowUp()
            {
                try
                {
                    string area = cmbArea.Text.Trim();

                    using (SqlConnection cn = new SqlConnection(connStr))
                    using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT DISTINCT
    s.SubscriberID,
    m.MeterID,
    ISNULL(a.AccountCode, CAST(s.AccountID AS nvarchar(50))) AS AccountNo,
    s.Name AS SubscriberName,
    m.MeterNumber,
    ISNULL(NULLIF(LTRIM(RTRIM(m.Location)), ''), N'بدون تحديد') AS MeterLocation,
    ISNULL(lastR.CurrentReading, 0) AS PreviousReading
FROM dbo.Subscribers s
INNER JOIN dbo.SubscriberMeters sm ON sm.SubscriberID = s.SubscriberID
INNER JOIN dbo.Meters m ON m.MeterID = sm.MeterID
LEFT JOIN dbo.Accounts a ON a.AccountID = s.AccountID
OUTER APPLY
(
    SELECT TOP 1 r.CurrentReading
    FROM dbo.Readings r
    WHERE r.SubscriberID = s.SubscriberID
      AND r.MeterID = m.MeterID
    ORDER BY r.ReadingDate DESC, r.ReadingID DESC
) lastR
WHERE s.IsActive = 1
  AND m.IsActive = 1
  AND
  (
      @Area = N'الكل'
      OR ISNULL(NULLIF(LTRIM(RTRIM(m.Location)), ''), N'بدون تحديد') = @Area
  )
ORDER BY MeterLocation, s.Name, m.MeterNumber;", cn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@Area", area);

                        DataTable source = new DataTable();
                        da.Fill(source);

                        BuildGridSchema();

                        int i = 1;
                        foreach (DataRow src in source.Rows)
                        {
                            DataRow row = _table.NewRow();
                            row["SubscriberID"] = Convert.ToInt32(src["SubscriberID"]);
                            row["MeterID"] = Convert.ToInt32(src["MeterID"]);
                            row["م"] = i++;
                            row["رقم الحساب"] = src["AccountNo"] == DBNull.Value ? "" : src["AccountNo"].ToString();
                            row["اسم المشترك"] = src["SubscriberName"] == DBNull.Value ? "" : src["SubscriberName"].ToString();
                            row["رقم العداد"] = src["MeterNumber"] == DBNull.Value ? "" : src["MeterNumber"].ToString();
                            row["الموقع"] = src["MeterLocation"] == DBNull.Value ? "" : src["MeterLocation"].ToString();
                            row["القراءة السابقة"] = src["PreviousReading"] == DBNull.Value ? 0m : Convert.ToDecimal(src["PreviousReading"]);
                            row["القراءة الجديدة"] = "";
                            row["الاستهلاك"] = 0m;
                            row["ملاحظات"] = "";
                            row["تم"] = false;
                            _table.Rows.Add(row);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                        ApplyFilter();
                    else
                        UpdateSummary();

                    lblGridTitle.Text = "بيانات المتابعة";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر تحميل كشف العدادات.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void FormatGrid()
            {
                if (dgvReadings.Columns.Count == 0) return;

                foreach (DataGridViewColumn col in dgvReadings.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvReadings.Columns["SubscriberID"].Visible = false;
                dgvReadings.Columns["MeterID"].Visible = false;

                dgvReadings.Columns["م"].FillWeight = 45;
                dgvReadings.Columns["رقم الحساب"].FillWeight = 90;
                dgvReadings.Columns["اسم المشترك"].FillWeight = 170;
                dgvReadings.Columns["رقم العداد"].FillWeight = 95;
                dgvReadings.Columns["الموقع"].FillWeight = 140;
                dgvReadings.Columns["القراءة السابقة"].FillWeight = 95;
                dgvReadings.Columns["القراءة الجديدة"].FillWeight = 95;
                dgvReadings.Columns["الاستهلاك"].FillWeight = 90;
                dgvReadings.Columns["ملاحظات"].FillWeight = 150;
                dgvReadings.Columns["تم"].FillWeight = 55;

                dgvReadings.Columns["القراءة السابقة"].ReadOnly = true;
                dgvReadings.Columns["الاستهلاك"].ReadOnly = true;
                dgvReadings.Columns["تم"].ReadOnly = true;

                foreach (DataGridViewColumn col in dgvReadings.Columns)
                {
                    if (col.Name != "القراءة الجديدة" &&
                        col.Name != "ملاحظات" &&
                        col.Name != "SubscriberID" &&
                        col.Name != "MeterID")
                    {
                        col.ReadOnly = true;
                    }
                }

                dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235);
                dgvReadings.Columns["ملاحظات"].DefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
                dgvReadings.Columns["تم"].DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
            }

            #endregion

            #region Filter + Summary

            private void ApplyFilter()
            {
                if (_table == null || _table.DefaultView == null)
                    return;

                string keyword = txtSearch.Text.Trim().Replace("'", "''");

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    _table.DefaultView.RowFilter = "";
                }
                else
                {
                    _table.DefaultView.RowFilter =
                        "[اسم المشترك] LIKE '%" + keyword + "%' " +
                        "OR [رقم الحساب] LIKE '%" + keyword + "%' " +
                        "OR [رقم العداد] LIKE '%" + keyword + "%' " +
                        "OR [الموقع] LIKE '%" + keyword + "%'";
                }

                UpdateSummaryFromView();
            }

            private void UpdateSummary()
            {
                int total = _table.Rows.Count;
                int done = _table.Rows.Cast<DataRow>()
                    .Count(r => r["تم"] != DBNull.Value && Convert.ToBoolean(r["تم"]));

                lblTotalValue.Text = total.ToString();
                lblDoneValue.Text = done.ToString();
                lblPendingValue.Text = (total - done).ToString();
            }

            private void UpdateSummaryFromView()
            {
                int total = _table.DefaultView.Count;
                int done = 0;

                foreach (DataRowView drv in _table.DefaultView)
                {
                    if (drv["تم"] != DBNull.Value && Convert.ToBoolean(drv["تم"]))
                        done++;
                }

                lblTotalValue.Text = total.ToString();
                lblDoneValue.Text = done.ToString();
                lblPendingValue.Text = (total - done).ToString();
            }

            #endregion

            #region Grid Editing

            private void DgvReadings_CellEndEdit(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0) return;

                DataGridViewRow row = dgvReadings.Rows[e.RowIndex];
                string colName = dgvReadings.Columns[e.ColumnIndex].Name;

                if (colName == "القراءة الجديدة")
                {
                    HandleReadingEdit(row, e.RowIndex);
                }
                else if (colName == "ملاحظات")
                {
                    string current = Convert.ToString(row.Cells["القراءة الجديدة"].Value);
                    decimal temp;
                    if (TryParseDecimal(current, out temp))
                        row.Cells["تم"].Value = true;
                }

                UpdateSummaryFromView();
            }

            private void HandleReadingEdit(DataGridViewRow row, int rowIndex)
            {
                decimal previous = ToDecimal(row.Cells["القراءة السابقة"].Value);
                decimal current;

                string currentText = row.Cells["القراءة الجديدة"].Value == null
                    ? ""
                    : row.Cells["القراءة الجديدة"].Value.ToString().Trim();

                if (string.IsNullOrWhiteSpace(currentText))
                {
                    row.Cells["الاستهلاك"].Value = 0m;
                    row.Cells["تم"].Value = false;
                    ResetRowStyle(row);
                    return;
                }

                if (!TryParseDecimal(currentText, out current))
                {
                    row.Cells["الاستهلاك"].Value = 0m;
                    row.Cells["تم"].Value = false;
                    MarkRowInvalid(row);

                    MessageBox.Show("القراءة الجديدة غير صحيحة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (current < previous)
                {
                    row.Cells["القراءة الجديدة"].Value = "";
                    row.Cells["الاستهلاك"].Value = 0m;
                    row.Cells["تم"].Value = false;
                    MarkRowInvalid(row);

                    MessageBox.Show("القراءة الجديدة لا يمكن أن تكون أقل من القراءة السابقة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                row.Cells["القراءة الجديدة"].Value = current.ToString("0.##");
                row.Cells["الاستهلاك"].Value = current - previous;
                row.Cells["تم"].Value = true;
                MarkRowValid(row);

                if (rowIndex < dgvReadings.Rows.Count - 1)
                {
                    dgvReadings.CurrentCell = dgvReadings.Rows[rowIndex + 1].Cells["القراءة الجديدة"];
                    dgvReadings.BeginEdit(true);
                }
            }

            private void ResetRowStyle(DataGridViewRow row)
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            }

            private void MarkRowValid(DataGridViewRow row)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(236, 253, 245);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
            }

            private void MarkRowInvalid(DataGridViewRow row)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
            }

            #endregion

            #region Save With Procedure

            private void SaveAllReadingsWithProcedure()
            {
                dgvReadings.EndEdit();

                if (_table.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد عدادات محملة للحفظ.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<DataRow> rowsToSave = new List<DataRow>();

                foreach (DataRow row in _table.Rows)
                {
                    decimal currentReading;
                    if (TryParseDecimal(Convert.ToString(row["القراءة الجديدة"]), out currentReading))
                        rowsToSave.Add(row);
                }

                if (rowsToSave.Count == 0)
                {
                    MessageBox.Show("لم يتم إدخال أي قراءة جديدة بعد.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int successCount = 0;
                List<string> errors = new List<string>();

                using (SqlConnection cn = new SqlConnection(connStr))
                {
                    cn.Open();

                    foreach (DataRow row in rowsToSave)
                    {
                        int subscriberId = Convert.ToInt32(row["SubscriberID"]);
                        int meterId = Convert.ToInt32(row["MeterID"]);
                        string subscriberName = Convert.ToString(row["اسم المشترك"]);
                        string meterNumber = Convert.ToString(row["رقم العداد"]);
                        decimal currentReading = ToDecimal(row["القراءة الجديدة"]);
                        string notes = row["ملاحظات"] == DBNull.Value ? "" : Convert.ToString(row["ملاحظات"]);

                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("dbo.AddReadingAndGenerateInvoice", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 120;

                                cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                                cmd.Parameters.AddWithValue("@MeterID", meterId);
                                cmd.Parameters.AddWithValue("@ReadingDate", dtpReadingDate.Value.Date);
                                cmd.Parameters.AddWithValue("@CurrentReading", currentReading);

                                // نجعل الإجراء يحددها تلقائياً من الخطة/الثوابت
                                cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = DBNull.Value;
                                cmd.Parameters.Add("@ServiceFees", SqlDbType.Decimal).Value = DBNull.Value;

                                if (string.IsNullOrWhiteSpace(notes))
                                    cmd.Parameters.AddWithValue("@Notes", DBNull.Value);
                                else
                                    cmd.Parameters.AddWithValue("@Notes", notes);

                                cmd.ExecuteNonQuery();
                            }

                            successCount++;
                            row["تم"] = true;
                            MarkRowValidByDataRow(row);
                        }
                        catch (SqlException ex)
                        {
                            string errorText = ExtractSqlErrorMessage(ex);
                            errors.Add($"- {subscriberName} / {meterNumber}: {errorText}");
                            row["تم"] = false;
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"- {subscriberName} / {meterNumber}: {ex.Message}");
                            row["تم"] = false;
                        }
                    }
                }

                UpdateSummary();

                if (successCount > 0 && errors.Count == 0)
                {
                    MessageBox.Show($"تم حفظ {successCount} قراءة وإنشاء الفواتير بنجاح ✅", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMetersForFollowUp();
                    return;
                }

                if (successCount > 0 && errors.Count > 0)
                {
                    string msg =
                        $"تم حفظ {successCount} قراءة بنجاح، وتعذر حفظ {errors.Count} قراءة.\n\n" +
                        string.Join("\n", errors.Take(10));

                    if (errors.Count > 10)
                        msg += $"\n\n... وهناك {errors.Count - 10} أخطاء إضافية";

                    MessageBox.Show(msg, "حفظ جزئي", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadMetersForFollowUp();
                    return;
                }

                MessageBox.Show(
                    "تعذر حفظ القراءات.\n\n" + string.Join("\n", errors.Take(10)),
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            private string ExtractSqlErrorMessage(SqlException ex)
            {
                if (ex == null) return "خطأ غير معروف";

                foreach (SqlError err in ex.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(err.Message))
                        return err.Message;
                }

                return ex.Message;
            }

            private void MarkRowValidByDataRow(DataRow row)
            {
                foreach (DataGridViewRow gridRow in dgvReadings.Rows)
                {
                    if (gridRow.DataBoundItem is DataRowView drv && drv.Row == row)
                    {
                        MarkRowValid(gridRow);
                        break;
                    }
                }
            }

            #endregion

            #region Print

            private void PreviewSheet()
            {
                if (_table.Rows.Count == 0)
                {
                    MessageBox.Show("قم بتحميل العدادات أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ResetPrintState();
                _previewDialog.ShowDialog(this);
            }

            private void ResetPrintState()
            {
                _printRowIndex = 0;
                _printPageNo = 1;
                _printRows = _table.DefaultView.Cast<DataRowView>().ToList();
            }

            private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
            {
                Graphics g = e.Graphics;
                Rectangle m = e.MarginBounds;

                using (Font titleFont = new Font("Tahoma", 15, FontStyle.Bold))
                using (Font normalFont = new Font("Tahoma", 9, FontStyle.Regular))
                using (Font headerFont = new Font("Tahoma", 9, FontStyle.Bold))
                using (Pen pen = new Pen(Color.Black))
                {
                    int y = m.Top;

                    g.DrawString("كشف متابعة تسجيل القراءات الجديدة", titleFont, Brushes.Black, m.Left, y);
                    y += 28;

                    g.DrawString("المنطقة: " + cmbArea.Text, normalFont, Brushes.Black, m.Left, y);
                    g.DrawString("تاريخ القراءة: " + dtpReadingDate.Value.ToString("yyyy/MM/dd"), normalFont, Brushes.Black, m.Left + 260, y);
                    g.DrawString("عدد السجلات: " + _printRows.Count, normalFont, Brushes.Black, m.Left + 520, y);
                    y += 24;

                    string[] headers =
                    {
                    "م",
                    "رقم الحساب",
                    "اسم المشترك",
                    "رقم العداد",
                    "الموقع",
                    "القراءة السابقة",
                    "القراءة الجديدة",
                    "الاستهلاك",
                    "ملاحظات"
                };

                    int[] widths = GetPrintColumnWidths(m.Width);

                    int x = m.Left;
                    int headerHeight = 28;
                    int rowHeight = 26;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        Rectangle rect = new Rectangle(x, y, widths[i], headerHeight);
                        g.FillRectangle(Brushes.LightGray, rect);
                        g.DrawRectangle(pen, rect);
                        DrawCentered(g, headers[i], headerFont, rect);
                        x += widths[i];
                    }

                    y += headerHeight;

                    while (_printRowIndex < _printRows.Count)
                    {
                        if (y + rowHeight > m.Bottom - 35)
                        {
                            e.HasMorePages = true;
                            _printPageNo++;
                            return;
                        }

                        DataRowView drv = _printRows[_printRowIndex];

                        string[] vals =
                        {
                        Convert.ToString(drv["م"]),
                        Convert.ToString(drv["رقم الحساب"]),
                        Convert.ToString(drv["اسم المشترك"]),
                        Convert.ToString(drv["رقم العداد"]),
                        Convert.ToString(drv["الموقع"]),
                        FormatNumber(drv["القراءة السابقة"]),
                        Convert.ToString(drv["القراءة الجديدة"]),
                        FormatNumber(drv["الاستهلاك"]),
                        Convert.ToString(drv["ملاحظات"])
                    };

                        x = m.Left;

                        for (int i = 0; i < vals.Length; i++)
                        {
                            Rectangle rect = new Rectangle(x, y, widths[i], rowHeight);
                            g.DrawRectangle(pen, rect);
                            DrawCell(g, vals[i], normalFont, rect);
                            x += widths[i];
                        }

                        y += rowHeight;
                        _printRowIndex++;
                    }

                    y += 10;
                    g.DrawString("صفحة: " + _printPageNo, normalFont, Brushes.Black, m.Right - 80, y);

                    e.HasMorePages = false;
                    _printRowIndex = 0;
                }
            }

            private int[] GetPrintColumnWidths(int totalWidth)
            {
                double[] ratios = { 0.05, 0.10, 0.18, 0.10, 0.15, 0.10, 0.10, 0.08, 0.14 };
                int[] widths = new int[ratios.Length];

                int used = 0;
                for (int i = 0; i < ratios.Length; i++)
                {
                    widths[i] = (int)(totalWidth * ratios[i]);
                    used += widths[i];
                }

                if (used < totalWidth)
                    widths[widths.Length - 1] += (totalWidth - used);

                return widths;
            }

            private void DrawCentered(Graphics g, string text, Font font, Rectangle rect)
            {
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                    g.DrawString(text ?? "", font, Brushes.Black, rect, sf);
                }
            }

            private void DrawCell(Graphics g, string text, Font font, Rectangle rect)
            {
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;

                    Rectangle padded = new Rectangle(rect.X + 3, rect.Y, rect.Width - 6, rect.Height);
                    g.DrawString(text ?? "", font, Brushes.Black, padded, sf);
                }
            }

            #endregion

            #region Helpers

            private decimal ToDecimal(object value)
            {
                if (value == null || value == DBNull.Value)
                    return 0m;

                decimal d;
                string s = NormalizeDigits(value.ToString());

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    return d;

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    return d;

                if (decimal.TryParse(s, out d))
                    return d;

                return 0m;
            }

            private bool TryParseDecimal(string input, out decimal value)
            {
                input = NormalizeDigits(input);

                return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                    || decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out value)
                    || decimal.TryParse(input, out value);
            }

            private string NormalizeDigits(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                return input
                    .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
                    .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9')
                    .Replace('٫', '.')
                    .Replace('٬', ',')
                    .Trim();
            }

            private string FormatNumber(object value)
            {
                decimal d = ToDecimal(value);
                return d.ToString("0.##");
            }

            #endregion
        }
    }