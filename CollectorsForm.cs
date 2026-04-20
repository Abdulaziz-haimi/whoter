using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class CollectorsForm : Form
    {
        private CollectorService _svc;

        private int _currentId = 0;
        private bool _isEdit = false;

        public CollectorsForm()
        {
            InitializeComponent();

            _svc = new CollectorService(GetConnectionString());

            ApplyUiDefaults();
            WireEvents();

            LoadData();
        }

        public CollectorsForm(string connectionString)
        {
            InitializeComponent();

            _svc = new CollectorService(connectionString);

            ApplyUiDefaults();
            WireEvents();

            LoadData();
        }

        private string GetConnectionString()
        {
            return "Data Source=.;Initial Catalog=WaterBillingDB;Integrated Security=True";
        }

        private void ApplyUiDefaults()
        {
            Font = new Font("Segoe UI", 10f);

            lblSearchHint.ForeColor = Color.DimGray;
            lblIdValue.ForeColor = Color.Navy;

            // Grid
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;

            btnNew.Width = btnSave.Width = btnDelete.Width = btnRefresh.Width = 90;
            btnNew.Height = btnSave.Height = btnDelete.Height = btnRefresh.Height = 36;

            NewRecord();
        }

        private void WireEvents()
        {
            txtSearch.TextChanged += (s, e) => ApplySearch();
            btnRefresh.Click += (s, e) => LoadData();

            grid.CellClick += Grid_CellClick;

            btnNew.Click += (s, e) => NewRecord();
            btnSave.Click += (s, e) => SaveRecord();
            btnDelete.Click += (s, e) => DeleteRecord();

            // Enter => حفظ (اختياري)
            txtName.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SaveRecord(); } };
            txtPhone.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SaveRecord(); } };
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            LoadSelected();
        }

        private void LoadData()
        {
            try
            {
                if (_svc == null) _svc = new CollectorService(GetConnectionString());

                var dt = _svc.GetAll();
                grid.DataSource = dt;

                if (grid.Columns["CollectorID"] != null) grid.Columns["CollectorID"].HeaderText = "الرقم";
                if (grid.Columns["Name"] != null) grid.Columns["Name"].HeaderText = "الاسم";
                if (grid.Columns["Phone"] != null) grid.Columns["Phone"].HeaderText = "الهاتف";

                stCount.Text = $"العدد: {dt.Rows.Count}";
                stSelected.Text = "المحدد: -";

                NewRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplySearch()
        {
            try
            {
                if (_svc == null) _svc = new CollectorService(GetConnectionString());

                var q = (txtSearch.Text ?? "").Trim();
                var dt = string.IsNullOrEmpty(q) ? _svc.GetAll() : _svc.Search(q);

                grid.DataSource = dt;
                stCount.Text = $"العدد: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSelected()
        {
            if (grid.CurrentRow == null) return;

            var row = grid.CurrentRow;
            if (row.Cells["CollectorID"] == null) return;

            _currentId = Convert.ToInt32(row.Cells["CollectorID"].Value);

            txtName.Text = Convert.ToString(row.Cells["Name"].Value) ?? "";
            txtPhone.Text = Convert.ToString(row.Cells["Phone"].Value) ?? "";

            lblIdValue.Text = _currentId.ToString();
            _isEdit = true;

            stSelected.Text = $"المحدد: {_currentId}";
        }

        private void NewRecord()
        {
            _currentId = 0;
            _isEdit = false;

            lblIdValue.Text = "-";
            txtName.Text = "";
            txtPhone.Text = "";

            stSelected.Text = "المحدد: -";
            txtName.Focus();
        }

        private void SaveRecord()
        {
            try
            {
                if (_svc == null) _svc = new CollectorService(GetConnectionString());

                var name = (txtName.Text ?? "").Trim();
                var phone = (txtPhone.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("اسم المتحصل مطلوب.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (_isEdit && _currentId > 0)
                {
                    _svc.Update(_currentId, name, phone);
                    MessageBox.Show("تم التحديث بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var newId = _svc.Insert(name, phone);
                    MessageBox.Show($"تمت الإضافة بنجاح (ID={newId}).", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadData();
                ApplySearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRecord()
        {
            try
            {
                if (_svc == null) _svc = new CollectorService(GetConnectionString());

                if (!_isEdit || _currentId <= 0)
                {
                    MessageBox.Show("اختر متحصل من الجدول أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var ok = MessageBox.Show("هل أنت متأكد من الحذف؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ok != DialogResult.Yes) return;

                _svc.Delete(_currentId);
                MessageBox.Show("تم الحذف.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
                ApplySearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // Service (كما هو عندك)
        // =========================
        public class CollectorService
        {
            private readonly string _cs;

            public CollectorService(string connectionString)
            {
                _cs = connectionString;
            }

            private DataTable ExecTable(string proc, Action<SqlParameterCollection> p = null)
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(proc, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    p?.Invoke(cmd.Parameters);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }

            private void ExecNonQuery(string proc, Action<SqlParameterCollection> p = null)
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(proc, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    p?.Invoke(cmd.Parameters);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public DataTable GetAll() => ExecTable("dbo.Collectors_GetAll");

            public DataTable Search(string q) => ExecTable("dbo.Collectors_Search", ps =>
            {
                ps.AddWithValue("@q", q ?? "");
            });

            public int Insert(string name, string phone)
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand("dbo.Collectors_Insert", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }

            public void Update(int id, string name, string phone)
            {
                ExecNonQuery("dbo.Collectors_Update", ps =>
                {
                    ps.AddWithValue("@CollectorID", id);
                    ps.AddWithValue("@Name", name);
                    ps.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);
                });
            }

            public void Delete(int id)
            {
                ExecNonQuery("dbo.Collectors_Delete", ps =>
                {
                    ps.AddWithValue("@CollectorID", id);
                });
            }
        }
    }
}
