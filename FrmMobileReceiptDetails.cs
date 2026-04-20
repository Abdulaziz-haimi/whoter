using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace water3
{
    public partial class FrmMobileReceiptDetails : Form
    {
        private readonly int _importId;
        private readonly string _connectionString = Db.ConnectionString;

        public FrmMobileReceiptDetails(int importId)
        {
            _importId = importId;
            InitializeComponent();
            LoadDetails();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void LoadDetails()
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_GetDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ImportID", _importId);
                con.Open();

                using (var da = new SqlDataAdapter(cmd))
                {
                    var ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var r = ds.Tables[0].Rows[0];
                        lblHeader.Text = $"السجل #{r["ImportID"]} - {r["SubscriberName"]} - {r["ImportStatus"]}";
                        txtInfo.Text =
                            $"السند المؤقت: {r["LocalReceiptNo"]}{Environment.NewLine}" +
                            $"التاريخ: {r["PaymentDate"]}{Environment.NewLine}" +
                            $"المبلغ: {r["TotalReceived"]}{Environment.NewLine}" +
                            $"المحصل: {r["CollectorName"]}{Environment.NewLine}" +
                            $"الجهاز: {r["DeviceName"]}{Environment.NewLine}" +
                            $"السند النهائي: {r["ReceiptNumber"]}{Environment.NewLine}" +
                            $"سبب الرفض: {r["RejectedReason"]}";
                    }

                    if (ds.Tables.Count > 1)
                    {
                        dgvLines.DataSource = ds.Tables[1];
                    }
                }
            }
        }
    }
}