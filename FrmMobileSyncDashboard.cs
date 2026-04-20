using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using water3.Controls;

namespace water3
{
    public partial class FrmMobileSyncDashboard : Form
    {
        private readonly string _connectionString = Db.ConnectionString;

        public FrmMobileSyncDashboard()
        {
            InitializeComponent();
            InitializeCards();
            WireEvents();
            LoadDashboard();
        }

        private void WireEvents()
        {
            btnRefresh.Click += (s, e) => LoadDashboard();
            btnOpenBatches.Click += (s, e) => OpenBatches();
            btnOpenQueue.Click += (s, e) => OpenQueue();
            btnApproveBatch.Click += (s, e) => OpenApproveBatch();
        }

        private void InitializeCards()
        {
            flowCards.Controls.Clear();

            flowCards.Controls.Add(cardTotalImports);
            flowCards.Controls.Add(cardTotalBatches);
            flowCards.Controls.Add(cardNew);
            flowCards.Controls.Add(cardApproved);
            flowCards.Controls.Add(cardRejected);
            flowCards.Controls.Add(cardErrors);
            flowCards.Controls.Add(cardTotalAmount);
            flowCards.Controls.Add(cardApprovedAmount);
        }

        private void LoadDashboard()
        {
            LoadSummaryCards();
            LoadLatestBatches();
            LoadLatestQueue();
        }

        private void LoadSummaryCards()
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_GetSummaryCards", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CollectorID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SyncBatchID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", chkUseDate.Checked ? (object)dtFrom.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", chkUseDate.Checked ? (object)dtTo.Value.Date : DBNull.Value);

                    con.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            cardTotalImports.SetData("إجمالي السجلات", Convert.ToString(rd["TotalImports"]), "كل التحصيلات المستوردة");
                            cardTotalBatches.SetData("إجمالي الدفعات", Convert.ToString(rd["TotalBatches"]), "دفعات المزامنة");
                            cardNew.SetData("جديد", Convert.ToString(rd["NewCount"]), "بانتظار المراجعة");
                            cardApproved.SetData("معتمد", Convert.ToString(rd["ApprovedCount"]), "تم تحويله إلى سندات فعلية");
                            cardRejected.SetData("مرفوض", Convert.ToString(rd["RejectedCount"]), "يتطلب مراجعة السبب");
                            cardErrors.SetData("الأخطاء", Convert.ToString(rd["ErrorCount"]), "أخطاء المزامنة والمعالجة");
                            cardTotalAmount.SetData("إجمالي المبالغ", string.Format("{0:N2}", rd["TotalReceivedAmount"]), "المبالغ المستوردة");
                            cardApprovedAmount.SetData("المبالغ المعتمدة", string.Format("{0:N2}", rd["ApprovedAmount"]), "ما تم ترحيله فعليًا");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("تعذر تحميل بطاقات الملخص:" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLatestBatches()
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("dbo.usp_MobileSync_GetBatchDashboard", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SyncStatus", DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollectorID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeviceID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", chkUseDate.Checked ? (object)dtFrom.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", chkUseDate.Checked ? (object)dtTo.Value.Date : DBNull.Value);

                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    dgvLatestBatches.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("تعذر تحميل أحدث الدفعات:" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLatestQueue()
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_GetQueue", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ImportStatus", DBNull.Value);
                    cmd.Parameters.AddWithValue("@CollectorID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubscriberID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateFrom", chkUseDate.Checked ? (object)dtFrom.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateTo", chkUseDate.Checked ? (object)dtTo.Value.Date : DBNull.Value);

                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    dgvLatestQueue.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("تعذر تحميل قائمة السجلات:" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenBatches()
        {
            using (var frm = new FrmSyncBatches())
            {
                frm.ShowDialog(this);
            }
            LoadDashboard();
        }

        private void OpenQueue()
        {
            using (var frm = new FrmMobileReceiptsQueue())
            {
                frm.ShowDialog(this);
            }
            LoadDashboard();
        }

        private void OpenApproveBatch()
        {
            using (var frm = new FrmApproveBatch())
            {
                frm.ShowDialog(this);
            }
            LoadDashboard();
        }
    }
}