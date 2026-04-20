using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace water3
{
    public partial class FrmApproveBatch : Form
    {
        private readonly string _connectionString = Db.ConnectionString;

        public FrmApproveBatch()
        {
            InitializeComponent();
            chkSendSms.Checked = true;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_ApproveBatch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SyncBatchID", string.IsNullOrWhiteSpace(txtSyncBatchID.Text) ? (object)DBNull.Value : int.Parse(txtSyncBatchID.Text));
                    cmd.Parameters.AddWithValue("@CollectorID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", chkUseDate.Checked ? (object)dtFrom.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", chkUseDate.Checked ? (object)dtTo.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LimitCount", (object)numLimit.Value);
                    cmd.Parameters.AddWithValue("@ApprovedByUserID", CurrentUser.UserID);
                    cmd.Parameters.AddWithValue("@SendSms", chkSendSms.Checked ? 1 : 0);

                    con.Open();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        da.Fill(ds);

                        var sb = new StringBuilder();
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            var r = ds.Tables[0].Rows[0];
                            sb.AppendLine($"إجمالي المختار: {r["TotalSelected"]}");
                            sb.AppendLine($"تم الاعتماد: {r["ApprovedCount"]}");
                            sb.AppendLine($"فشل: {r["FailedCount"]}");
                            sb.AppendLine($"معتمد مسبقاً: {r["AlreadyApprovedCount"]}");
                            sb.AppendLine($"الحالة: {r["BatchResultStatus"]}");
                        }

                        txtResult.Text = sb.ToString();

                        if (ds.Tables.Count > 1)
                            dgvResult.DataSource = ds.Tables[1];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("فشل الاعتماد الجماعي:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}