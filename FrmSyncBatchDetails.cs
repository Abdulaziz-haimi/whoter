using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace water3
{
    public partial class FrmSyncBatchDetails : Form
    {
        private readonly int _syncBatchId;
        private readonly string _connectionString = Db.ConnectionString;

        public FrmSyncBatchDetails(int syncBatchId)
        {
            _syncBatchId = syncBatchId;
            InitializeComponent();
            LoadBatchDetails();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadBatchDetails();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadBatchDetails()
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_MobileSync_GetBatchDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SyncBatchID", _syncBatchId);
                con.Open();

                using (var da = new SqlDataAdapter(cmd))
                {
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var r = ds.Tables[0].Rows[0];
                        var sb = new StringBuilder();
                        sb.AppendLine($"رقم الدفعة: {r["SyncBatchID"]}");
                        sb.AppendLine($"المحصل: {r["CollectorName"]}");
                        sb.AppendLine($"الجهاز: {r["DeviceName"]}");
                        sb.AppendLine($"الحالة: {r["SyncStatus"]}");
                        sb.AppendLine($"عدد السجلات: {r["DeclaredRows"]}");
                        sb.AppendLine($"الجديد: {r["NewCount"]} | المعتمد: {r["ApprovedCount"]} | المرفوض: {r["RejectedCount"]}");
                        sb.AppendLine($"الأخطاء: {r["ErrorCount"]}");
                        sb.AppendLine($"الإجمالي: {r["TotalReceivedSum"]}");
                        txtHeader.Text = sb.ToString();
                    }

                    if (ds.Tables.Count > 1)
                        dgvImports.DataSource = ds.Tables[1];

                    if (ds.Tables.Count > 2)
                        dgvErrors.DataSource = ds.Tables[2];
                }
            }
        }
    }
}