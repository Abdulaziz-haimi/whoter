using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using water3.Forms;

namespace water3
{
    public partial class FrmSyncBatches : Form
    {
        private readonly string _connectionString = Db.ConnectionString;
        private int? _selectedBatchId = null;

        public FrmSyncBatches()
        {
            InitializeComponent();
            InitializeUi();
            WireEvents();
        }

        private void InitializeUi()
        {
            Text = "دفعات المزامنة";
            StartPosition = FormStartPosition.CenterScreen;
            dgvBatches.AutoGenerateColumns = true;
            dgvBatches.MultiSelect = false;
            dgvBatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBatches.ReadOnly = true;
            dgvBatches.AllowUserToAddRows = false;
            dgvBatches.AllowUserToDeleteRows = false;
            dgvBatches.RowHeadersVisible = false;
            dgvBatches.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            cboStatus.Items.Clear();
            cboStatus.Items.Add("الكل");
            cboStatus.Items.Add("Pending");
            cboStatus.Items.Add("Imported");
            cboStatus.Items.Add("Partial");
            cboStatus.Items.Add("Rejected");
            cboStatus.SelectedIndex = 0;

            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;

            LoadBatches();
        }

        private void WireEvents()
        {
            btnSearch.Click += (s, e) => LoadBatches();
            btnRefresh.Click += (s, e) => LoadBatches();
            btnDetails.Click += (s, e) => OpenBatchDetails();
            btnApproveBatch.Click += (s, e) => RunApproveBatch();
            dgvBatches.SelectionChanged += (s, e) => UpdateSelection();
            dgvBatches.CellDoubleClick += (s, e) => OpenBatchDetails();
        }

        private void LoadBatches()
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("dbo.usp_MobileSync_GetBatchDashboard", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SyncStatus", GetStatusValueOrDbNull());
                    cmd.Parameters.AddWithValue("@CollectorID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeviceID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", chkUseDate.Checked ? (object)dtFrom.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", chkUseDate.Checked ? (object)dtTo.Value.Date : DBNull.Value);

                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    dgvBatches.DataSource = dt;
                    lblCount.Text = $"عدد الدفعات: {dt.Rows.Count}";
                }

                UpdateSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("تعذر تحميل دفعات المزامنة:" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private object GetStatusValueOrDbNull()
        {
            if (cboStatus.SelectedIndex <= 0)
                return DBNull.Value;

            return cboStatus.SelectedItem?.ToString() ?? (object)DBNull.Value;
        }

        private void UpdateSelection()
        {
            _selectedBatchId = null;
            btnDetails.Enabled = false;
            btnApproveBatch.Enabled = false;

            if (dgvBatches.CurrentRow == null)
                return;

            if (int.TryParse(Convert.ToString(dgvBatches.CurrentRow.Cells["SyncBatchID"].Value), out int batchId))
            {
                _selectedBatchId = batchId;
                btnDetails.Enabled = true;
                btnApproveBatch.Enabled = true;
            }
        }

        private void OpenBatchDetails()
        {
            if (!_selectedBatchId.HasValue)
                return;

            using (var frm = new FrmSyncBatchDetails(_selectedBatchId.Value))
            {
                frm.ShowDialog(this);
            }

            LoadBatches();
        }

        private void RunApproveBatch()
        {
            if (!_selectedBatchId.HasValue)
                return;

            using (var frm = new FrmApproveBatch())
            {
                frm.ShowDialog(this);
            }

            LoadBatches();
        }
    }
}