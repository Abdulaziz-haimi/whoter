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
    public partial class CollectorsManagementForm : Form
    {

            private readonly CollectorService _service = new CollectorService();
            private List<CollectorItem> _collectors = new List<CollectorItem>();

            private Label lblTitle;
            private TextBox txtSearch;
            private Button btnSearch;
            private Button btnRefresh;
            private Button btnNew;
            private Button btnEdit;
            private Button btnDelete;
            private DataGridView dgvCollectors;
            private Label lblStatus;
        private Button btnLinkUser;
        public CollectorsManagementForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "COLLECTORS_VIEW");
                ApplyPermissions();
                LoadCollectors();
            }

            private void InitializeComponent()
            {
                Text = "إدارة المحصلين";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                WindowState = FormWindowState.Maximized;

                lblTitle = new Label
                {
                    Text = "إدارة المحصلين",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                txtSearch = new TextBox
                {
                    Location = new Point(20, 60),
                    Size = new Size(260, 27)
                };
                txtSearch.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                        SearchCollectors();
                };
            btnLinkUser = MakeButton("ربط المحصل بالمستخدم", 870, 58, 190);
            btnLinkUser.Click += BtnLinkUser_Click;

            btnSearch = MakeButton("بحث", 290, 58, 100);
                btnSearch.Click += (s, e) => SearchCollectors();

                btnRefresh = MakeButton("تحديث", 400, 58, 100);
                btnRefresh.Click += (s, e) =>
                {
                    txtSearch.Clear();
                    LoadCollectors();
                };

                btnNew = MakeButton("إضافة محصل", 510, 58, 130);
                btnNew.Click += BtnNew_Click;

                btnEdit = MakeButton("تعديل", 650, 58, 100);
                btnEdit.Click += BtnEdit_Click;

                btnDelete = MakeButton("حذف", 760, 58, 100);
                btnDelete.Click += BtnDelete_Click;

                dgvCollectors = new DataGridView
                {
                    Location = new Point(20, 100),
                    Size = new Size(1200, 560),
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
                dgvCollectors.DoubleClick += (s, e) => EditSelectedCollector();

                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colCollectorID",
                    HeaderText = "الرقم",
                    DataPropertyName = "CollectorID",
                    FillWeight = 70
                });

                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colName",
                    HeaderText = "اسم المحصل",
                    DataPropertyName = "Name",
                    FillWeight = 180
                });

                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "colPhone",
                    HeaderText = "رقم الهاتف",
                    DataPropertyName = "Phone",
                    FillWeight = 140
                });

                lblStatus = new Label
                {
                    Location = new Point(20, 670),
                    Size = new Size(1200, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                txtSearch,
                btnSearch,
                btnRefresh,
                btnNew,
                btnEdit,
                btnDelete,
                dgvCollectors,
                lblStatus,
                btnLinkUser
                });
            }
        private void BtnLinkUser_Click(object sender, EventArgs e)
        {
            using (var frm = new CollectorUserLinkForm())
            {
                frm.ShowDialog();
            }
        }

        private Button MakeButton(string text, int left, int top, int width)
            {
                return new Button
                {
                    Text = text,
                    Location = new Point(left, top),
                    Size = new Size(width, 32),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
            }

            private void ApplyPermissions()
            {
                PermissionHelper.ApplyControlPermission(btnNew, "COLLECTORS_MANAGE");
                PermissionHelper.ApplyControlPermission(btnEdit, "COLLECTORS_MANAGE");
                PermissionHelper.ApplyControlPermission(btnDelete, "COLLECTORS_MANAGE");
            PermissionHelper.ApplyControlPermission(btnLinkUser, "COLLECTORS_LINK_USER");
        }

            private void LoadCollectors()
            {
                try
                {
                    _collectors = _service.GetAll();
                    dgvCollectors.DataSource = null;
                    dgvCollectors.DataSource = _collectors;
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد المحصلين: {_collectors.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void SearchCollectors()
            {
                try
                {
                    _collectors = _service.Search(txtSearch.Text);
                    dgvCollectors.DataSource = null;
                    dgvCollectors.DataSource = _collectors;
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد النتائج: {_collectors.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private CollectorItem GetSelectedCollector()
            {
                return dgvCollectors.CurrentRow?.DataBoundItem as CollectorItem;
            }

            private void BtnNew_Click(object sender, EventArgs e)
            {
                using (var frm = new CollectorEditForm())
                {
                    frm.ShowDialog();
                }
                LoadCollectors();
            }

            private void BtnEdit_Click(object sender, EventArgs e)
            {
                EditSelectedCollector();
            }

            private void EditSelectedCollector()
            {
                var item = GetSelectedCollector();
                if (item == null)
                {
                    MessageBox.Show("اختر محصلًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new CollectorEditForm(item.CollectorID, item.Name, item.Phone))
                {
                    frm.ShowDialog();
                }
                LoadCollectors();
            }
        private void btnDevices_Click(object sender, EventArgs e)
        {
            using (var frm = new CollectorDevicesForm())
            {
                frm.ShowDialog();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
            {
                try
                {
                    var item = GetSelectedCollector();
                    if (item == null)
                    {
                        MessageBox.Show("اختر محصلًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show(
                        $"هل تريد حذف المحصل: {item.Name}؟",
                        "تأكيد الحذف",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    _service.Delete(item.CollectorID, item.Name);
                    LoadCollectors();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }