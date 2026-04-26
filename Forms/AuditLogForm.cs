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
using System.Drawing;
using System.Windows.Forms;
using water3.Models;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class AuditLogForm : Form
    {
   
        //public class AuditLogForm : Form
        //{
            private readonly AuditLogService _service = new AuditLogService();
            private List<AuditLogItem> _logs = new List<AuditLogItem>();

            private Label lblTitle;
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private TextBox txtSearch;
            private Button btnSearch;
            private Button btnRefresh;
            private DataGridView dgvLogs;
            private Label lblStatus;

            public AuditLogForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "REPORTS_VIEW");
                LoadLogs();
            }

            private void InitializeComponent()
            {
                Text = "سجل العمليات";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                WindowState = FormWindowState.Maximized;

                lblTitle = new Label
                {
                    Text = "سجل العمليات Audit Log",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                dtFrom = new DateTimePicker { Location = new Point(20, 60), Size = new Size(150, 27), Format = DateTimePickerFormat.Short };
                dtTo = new DateTimePicker { Location = new Point(180, 60), Size = new Size(150, 27), Format = DateTimePickerFormat.Short };
                txtSearch = new TextBox { Location = new Point(340, 60), Size = new Size(250, 27) };

                btnSearch = new Button { Text = "بحث", Location = new Point(600, 58), Size = new Size(100, 32), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnRefresh = new Button { Text = "تحديث", Location = new Point(710, 58), Size = new Size(100, 32), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

                btnSearch.Click += (s, e) => LoadLogs();
                btnRefresh.Click += (s, e) =>
                {
                    txtSearch.Clear();
                    dtFrom.Value = DateTime.Today;
                    dtTo.Value = DateTime.Today;
                    LoadLogs();
                };

                dgvLogs = new DataGridView
                {
                    Location = new Point(20, 100),
                    Size = new Size(1280, 560),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLogID", HeaderText = "الرقم", DataPropertyName = "LogID", FillWeight = 60 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colActionDate", HeaderText = "التاريخ", DataPropertyName = "ActionDate", FillWeight = 110 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserName", HeaderText = "المستخدم", DataPropertyName = "UserName", FillWeight = 100 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAction", HeaderText = "العملية", DataPropertyName = "Action", FillWeight = 120 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTableName", HeaderText = "الجدول", DataPropertyName = "TableName", FillWeight = 90 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRecordID", HeaderText = "RecordID", DataPropertyName = "RecordID", FillWeight = 70 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEntityName", HeaderText = "الكيان", DataPropertyName = "EntityName", FillWeight = 120 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetails", HeaderText = "التفاصيل", DataPropertyName = "Details", FillWeight = 220 });
                dgvLogs.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceName", HeaderText = "الجهاز", DataPropertyName = "DeviceName", FillWeight = 90 });

                lblStatus = new Label
                {
                    Location = new Point(20, 670),
                    Size = new Size(1280, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                dtFrom,
                dtTo,
                txtSearch,
                btnSearch,
                btnRefresh,
                dgvLogs,
                lblStatus
                });
            }

            private void LoadLogs()
            {
                try
                {
                    _logs = _service.GetLogs(dtFrom.Value.Date, dtTo.Value.Date, txtSearch.Text);
                    dgvLogs.DataSource = null;
                    dgvLogs.DataSource = _logs;
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد السجلات: {_logs.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }
