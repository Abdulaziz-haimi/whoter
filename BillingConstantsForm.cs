using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace water3
{
    public partial class BillingConstantsForm : Form
    {
        private DataTable table;

        public BillingConstantsForm()
        {
            InitializeComponent();

            // ربط الأحداث (خارج InitializeComponent)
            btnAdd.Click += Add_Click;
            btnEdit.Click += Edit_Click;
            btnDelete.Click += Delete_Click;
            btnToggleActive.Click += ToggleActive_Click;
            btnRefresh.Click += (s, e) => LoadData();

            Load += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlDataAdapter da = new SqlDataAdapter("EXEC dbo.GetBillingConstants", con))
            {
                table = new DataTable();
                da.Fill(table);
                dgv.DataSource = table;
            }

            SetupGrid();
        }

        private void SetupGrid()
        {
            if (dgv.Columns.Contains("ConstantID"))
                dgv.Columns["ConstantID"].HeaderText = "رقم";

            if (dgv.Columns.Contains("EffectiveFrom"))
                dgv.Columns["EffectiveFrom"].HeaderText = "ساري من تاريخ";

            if (dgv.Columns.Contains("UnitPrice"))
                dgv.Columns["UnitPrice"].HeaderText = "سعر الوحدة";

            if (dgv.Columns.Contains("ServiceFees"))
                dgv.Columns["ServiceFees"].HeaderText = "رسوم الخدمة";

            if (dgv.Columns.Contains("IsActive"))
                dgv.Columns["IsActive"].HeaderText = "نشط";

            if (dgv.Columns.Contains("Notes"))
                dgv.Columns["Notes"].HeaderText = "ملاحظات";

            if (dgv.Columns.Contains("CreatedAt"))
                dgv.Columns["CreatedAt"].HeaderText = "تاريخ الإدخال";

            // تنسيق آمن (تحقق من وجود الأعمدة قبل التنسيق)
            if (dgv.Columns.Contains("UnitPrice"))
                dgv.Columns["UnitPrice"].DefaultCellStyle.Format = "N2";

            if (dgv.Columns.Contains("ServiceFees"))
                dgv.Columns["ServiceFees"].DefaultCellStyle.Format = "N2";

            if (dgv.Columns.Contains("CreatedAt"))
                dgv.Columns["CreatedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            if (dgv.Columns.Contains("EffectiveFrom"))
                dgv.Columns["EffectiveFrom"].DefaultCellStyle.Format = "yyyy-MM-dd";
        }

        private DataGridViewRow SelectedRow => dgv.SelectedRows.Count > 0 ? dgv.SelectedRows[0] : null;

        private void Add_Click(object sender, EventArgs e)
        {
            var f = new BillingConstantEditForm(null);
            if (f.ShowDialog() == DialogResult.OK)
            {
                SaveConstant(null, f.EffectiveFrom, f.UnitPrice, f.ServiceFees, f.IsActive, f.Notes);
                LoadData();
            }
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            var row = SelectedRow;
            if (row == null) return;

            var f = new BillingConstantEditForm(row);
            if (f.ShowDialog() == DialogResult.OK)
            {
                int id = Convert.ToInt32(row.Cells["ConstantID"].Value);
                SaveConstant(id, f.EffectiveFrom, f.UnitPrice, f.ServiceFees, f.IsActive, f.Notes);
                LoadData();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            var row = SelectedRow;
            if (row == null) return;

            if (MessageBox.Show("هل أنت متأكد من الحذف؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            int id = Convert.ToInt32(row.Cells["ConstantID"].Value);

            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("EXEC dbo.DeleteBillingConstant @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الحذف: {ex.Message}");
            }
        }

        private void ToggleActive_Click(object sender, EventArgs e)
        {
            var row = SelectedRow;
            if (row == null) return;

            int id = Convert.ToInt32(row.Cells["ConstantID"].Value);
            bool current = Convert.ToBoolean(row.Cells["IsActive"].Value);

            DateTime eff = Convert.ToDateTime(row.Cells["EffectiveFrom"].Value);
            decimal unit = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
            decimal fees = Convert.ToDecimal(row.Cells["ServiceFees"].Value);
            string notes = row.Cells["Notes"].Value?.ToString();

            SaveConstant(id, eff.Date, unit, fees, !current, notes);
            LoadData();
        }

        private void SaveConstant(int? id, DateTime effectiveFrom, decimal unitPrice, decimal serviceFees, bool isActive, string notes)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.SaveBillingConstant", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ConstantID", (object)id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EffectiveFrom", effectiveFrom.Date);
                    cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                    cmd.Parameters.AddWithValue("@ServiceFees", serviceFees);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الحفظ: {ex.Message}");
            }
        }
    }
}
