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
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
namespace water3
{
    public partial class FrmMobileReceiptsQueue : Form
    {

            private readonly string _connectionString = Db.ConnectionString;
            private int? _selectedImportId = null;

            public FrmMobileReceiptsQueue()
            {
                InitializeComponent();
                InitializeUi();
                WireEvents();
            }

            private void InitializeUi()
            {
                Text = "التحصيلات المستوردة من الجوال";
                StartPosition = FormStartPosition.CenterScreen;
                dgvQueue.AutoGenerateColumns = false;
                dgvQueue.MultiSelect = false;
                dgvQueue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvQueue.ReadOnly = true;
                dgvQueue.AllowUserToAddRows = false;
                dgvQueue.AllowUserToDeleteRows = false;
                dgvQueue.RowHeadersVisible = false;
                dgvQueue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                cboStatus.Items.Clear();
                cboStatus.Items.Add("الكل");
                cboStatus.Items.Add("New");
                cboStatus.Items.Add("Approved");
                cboStatus.Items.Add("Rejected");
                cboStatus.SelectedIndex = 0;

                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;

                BuildGridColumns();
                LoadQueue();
            }

            private void WireEvents()
            {
                btnSearch.Click += (s, e) => LoadQueue();
                btnRefresh.Click += (s, e) => LoadQueue();
                btnDetails.Click += (s, e) => OpenDetails();
                btnApprove.Click += (s, e) => ApproveSelected();
                btnReject.Click += (s, e) => RejectSelected();
                btnResetToNew.Click += (s, e) => ResetSelected();
                btnApproveBatch.Click += (s, e) => ApproveBatch();
                dgvQueue.SelectionChanged += (s, e) => UpdateSelectionState();
                dgvQueue.CellDoubleClick += (s, e) => OpenDetails();
            }

            private void BuildGridColumns()
            {
                dgvQueue.Columns.Clear();

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ImportID",
                    HeaderText = "رقم السجل",
                    DataPropertyName = "ImportID",
                    Width = 80,
                    FillWeight = 70
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ImportStatus",
                    HeaderText = "الحالة",
                    DataPropertyName = "ImportStatus",
                    Width = 90,
                    FillWeight = 80
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "LocalReceiptNo",
                    HeaderText = "السند المؤقت",
                    DataPropertyName = "LocalReceiptNo",
                    Width = 120,
                    FillWeight = 110
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "SubscriberName",
                    HeaderText = "المشترك",
                    DataPropertyName = "SubscriberName",
                    Width = 180,
                    FillWeight = 150
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CollectorName",
                    HeaderText = "المحصل",
                    DataPropertyName = "CollectorName",
                    Width = 140,
                    FillWeight = 110
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaymentDate",
                    HeaderText = "تاريخ التحصيل",
                    DataPropertyName = "PaymentDate",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" },
                    Width = 110,
                    FillWeight = 100
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "TotalReceived",
                    HeaderText = "المبلغ",
                    DataPropertyName = "TotalReceived",
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" },
                    Width = 110,
                    FillWeight = 90
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PaymentMethod",
                    HeaderText = "طريقة الدفع",
                    DataPropertyName = "PaymentMethod",
                    Width = 100,
                    FillWeight = 90
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ReceiptNumber",
                    HeaderText = "السند النهائي",
                    DataPropertyName = "ReceiptNumber",
                    Width = 120,
                    FillWeight = 110
                });

                dgvQueue.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "InvoiceList",
                    HeaderText = "الفواتير/التوزيع",
                    DataPropertyName = "InvoiceList",
                    Width = 180,
                    FillWeight = 150
                });
            }

            private void LoadQueue()
            {
                try
                {
                    using (var con = new SqlConnection(_connectionString))
                    using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_Search", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SearchText", (object)txtSearch.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ImportStatus", GetStatusValueOrDbNull());
                        cmd.Parameters.AddWithValue("@CollectorID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@SubscriberID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@SyncBatchID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateFrom", chkUseDate.Checked ? (object)dtFrom.Value.Date : DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateTo", chkUseDate.Checked ? (object)dtTo.Value.Date : DBNull.Value);

                        var dt = new DataTable();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        dgvQueue.DataSource = dt;
                        lblCount.Text = $"عدد السجلات: {dt.Rows.Count}";
                    }

                    UpdateSelectionState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر تحميل البيانات:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private object GetStatusValueOrDbNull()
            {
                if (cboStatus.SelectedIndex <= 0)
                    return DBNull.Value;

                return cboStatus.SelectedItem?.ToString() ?? (object)DBNull.Value;
            }

            private void UpdateSelectionState()
            {
                if (dgvQueue.CurrentRow == null || dgvQueue.CurrentRow.DataBoundItem == null)
                {
                    _selectedImportId = null;
                    btnDetails.Enabled = false;
                    btnApprove.Enabled = false;
                    btnReject.Enabled = false;
                    btnResetToNew.Enabled = false;
                    return;
                }

                _selectedImportId = TryGetCurrentImportId();
                string status = Convert.ToString(dgvQueue.CurrentRow.Cells["ImportStatus"].Value) ?? string.Empty;

                btnDetails.Enabled = _selectedImportId.HasValue;
                btnApprove.Enabled = _selectedImportId.HasValue && status == "New";
                btnReject.Enabled = _selectedImportId.HasValue && status == "New";
                btnResetToNew.Enabled = _selectedImportId.HasValue && status == "Rejected";
            }

            private int? TryGetCurrentImportId()
            {
                if (dgvQueue.CurrentRow == null)
                    return null;

                if (int.TryParse(Convert.ToString(dgvQueue.CurrentRow.Cells["ImportID"].Value), out int id))
                    return id;

                return null;
            }

            private void OpenDetails()
            {
                if (!_selectedImportId.HasValue)
                    return;

                using (var frm = new FrmMobileReceiptDetails(_selectedImportId.Value))
                {
                    frm.ShowDialog(this);
                }

                LoadQueue();
            }

            private void ApproveSelected()
            {
                if (!_selectedImportId.HasValue)
                    return;

                if (MessageBox.Show("هل تريد اعتماد هذا السجل؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                try
                {
                    using (var con = new SqlConnection(_connectionString))
                    using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_Approve", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ImportID", _selectedImportId.Value);
                        cmd.Parameters.AddWithValue("@ApprovedByUserID", CurrentUser.UserID);
                        cmd.Parameters.AddWithValue("@SendSms", 1);

                        con.Open();
                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                string receiptNumber = rd["ReceiptNumber"]?.ToString() ?? "";
                                MessageBox.Show("تم اعتماد السجل بنجاح.\nرقم السند: " + receiptNumber, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }

                    LoadQueue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل الاعتماد:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void RejectSelected()
            {
                if (!_selectedImportId.HasValue)
                    return;

                string reason = Prompt.ShowDialog("سبب الرفض", "أدخل سبب الرفض");
                if (string.IsNullOrWhiteSpace(reason))
                    return;

                try
                {
                    using (var con = new SqlConnection(_connectionString))
                    using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_Reject", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ImportID", _selectedImportId.Value);
                        cmd.Parameters.AddWithValue("@RejectedReason", reason.Trim());
                        cmd.Parameters.AddWithValue("@RejectedByUserID", CurrentUser.UserID);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("تم رفض السجل.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadQueue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل الرفض:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void ResetSelected()
            {
                if (!_selectedImportId.HasValue)
                    return;

                if (MessageBox.Show("هل تريد إعادة السجل إلى الحالة New؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                try
                {
                    using (var con = new SqlConnection(_connectionString))
                    using (var cmd = new SqlCommand("dbo.usp_MobileReceipt_ResetToNew", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ImportID", _selectedImportId.Value);
                        cmd.Parameters.AddWithValue("@ResetByUserID", CurrentUser.UserID);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("تمت إعادة السجل إلى New.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadQueue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل إعادة السجل:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void ApproveBatch()
            {
                using (var frm = new FrmApproveBatch())
                {
                    frm.ShowDialog(this);
                }

                LoadQueue();
            }
        }
    }


