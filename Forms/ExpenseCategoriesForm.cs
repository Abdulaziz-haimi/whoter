using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using water3.Models;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
    public partial class ExpenseCategoriesForm : Form
    {
 

     
            private readonly ExpenseService _service = new ExpenseService();

            private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();
            private List<AccountLookupItem> _accounts = new List<AccountLookupItem>();

            private DataGridView dgvCategories;
            private TextBox txtCategoryName;
            private ComboBox cboCategoryType;
            private ComboBox cboDefaultAccount;
            private TextBox txtNotes;
            private CheckBox chkIsActive;
            private Button btnNew;
            private Button btnSave;
            private Button btnDelete;
            private Button btnRefresh;
            private Label lblStatus;

            private int _selectedCategoryId = 0;

            public ExpenseCategoriesForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "EXPENSE_CATEGORIES_MANAGE");
                LoadAccounts();
                LoadCategories();
                ClearEditor();
            }

            private void InitializeComponent()
            {
                Text = "تصنيفات المصروفات";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                ClientSize = new Size(1100, 620);

                Label lblTitle = new Label
                {
                    Text = "إدارة تصنيفات المصروفات والمشتريات والمخاسير",
                    Font = new Font("Tahoma", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(300, 20)
                };

                Panel pnlEditor = new Panel
                {
                    Location = new Point(20, 60),
                    Size = new Size(1060, 150),
                    BackColor = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle
                };

                pnlEditor.Controls.Add(MakeLabel("اسم التصنيف", 980, 20));
                pnlEditor.Controls.Add(MakeLabel("النوع", 980, 55));
                pnlEditor.Controls.Add(MakeLabel("الحساب الافتراضي", 980, 90));

                txtCategoryName = new TextBox { Location = new Point(700, 18), Size = new Size(260, 27) };
                cboCategoryType = new ComboBox { Location = new Point(700, 53), Size = new Size(260, 27), DropDownStyle = ComboBoxStyle.DropDownList };
                cboDefaultAccount = new ComboBox { Location = new Point(700, 88), Size = new Size(260, 27), DropDownStyle = ComboBoxStyle.DropDownList };

                cboCategoryType.Items.AddRange(new object[] { "Expense", "Purchase", "Loss" });

                pnlEditor.Controls.AddRange(new Control[] { txtCategoryName, cboCategoryType, cboDefaultAccount });

                pnlEditor.Controls.Add(MakeLabel("ملاحظات", 640, 20));
                txtNotes = new TextBox
                {
                    Location = new Point(280, 18),
                    Size = new Size(340, 62),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical
                };
                pnlEditor.Controls.Add(txtNotes);

                chkIsActive = new CheckBox
                {
                    Text = "نشط",
                    Location = new Point(540, 95),
                    AutoSize = true,
                    Checked = true
                };
                pnlEditor.Controls.Add(chkIsActive);

                btnNew = MakeButton("جديد", 20, 20, 110, Color.SteelBlue);
                btnSave = MakeButton("حفظ", 20, 60, 110, Color.FromArgb(0, 153, 76));
                btnDelete = MakeButton("حذف", 20, 100, 110, Color.IndianRed);
                btnRefresh = MakeButton("تحديث", 145, 20, 110, Color.Gray);

                btnNew.Click += BtnNew_Click;
                btnSave.Click += BtnSave_Click;
                btnDelete.Click += BtnDelete_Click;
                btnRefresh.Click += BtnRefresh_Click;

                pnlEditor.Controls.AddRange(new Control[] { btnNew, btnSave, btnDelete, btnRefresh });

                dgvCategories = new DataGridView
                {
                    Location = new Point(20, 230),
                    Size = new Size(1060, 320),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاسم", DataPropertyName = "CategoryName", FillWeight = 180 });
                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "النوع", DataPropertyName = "CategoryType", FillWeight = 90 });
                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الحساب الافتراضي", DataPropertyName = "DefaultAccountID", FillWeight = 90 });
                dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ملاحظات", DataPropertyName = "Notes", FillWeight = 180 });
                dgvCategories.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "نشط", DataPropertyName = "IsActive", FillWeight = 70 });

                dgvCategories.SelectionChanged += DgvCategories_SelectionChanged;
                dgvCategories.DoubleClick += (s, e) => LoadSelectedCategoryToEditor();

                lblStatus = new Label
                {
                    Location = new Point(20, 565),
                    Size = new Size(1060, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[] { lblTitle, pnlEditor, dgvCategories, lblStatus });
            }

            private Label MakeLabel(string text, int left, int top)
            {
                return new Label
                {
                    Text = text,
                    AutoSize = true,
                    Location = new Point(left, top + 5)
                };
            }

            private Button MakeButton(string text, int left, int top, int width, Color color)
            {
                return new Button
                {
                    Text = text,
                    Location = new Point(left, top),
                    Size = new Size(width, 32),
                    BackColor = color,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
            }

            private void LoadAccounts()
            {
                try
                {
                    _accounts = _service.GetAccounts();
                    _accounts.Insert(0, new AccountLookupItem { AccountID = 0, AccountCode = "", AccountName = "بدون حساب افتراضي" });

                    cboDefaultAccount.DataSource = null;
                    cboDefaultAccount.DataSource = _accounts;
                    cboDefaultAccount.DisplayMember = "AccountName";
                    cboDefaultAccount.ValueMember = "AccountID";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void LoadCategories()
            {
                try
                {
                    _categories = _service.GetCategories();
                    dgvCategories.DataSource = null;
                    dgvCategories.DataSource = _categories;
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد التصنيفات: {_categories.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private ExpenseCategoryItem GetSelectedCategory()
            {
                return dgvCategories.CurrentRow?.DataBoundItem as ExpenseCategoryItem;
            }

            private void DgvCategories_SelectionChanged(object sender, EventArgs e)
            {
                LoadSelectedCategoryToEditor();
            }

            private void LoadSelectedCategoryToEditor()
            {
                var item = GetSelectedCategory();
                if (item == null)
                    return;

                _selectedCategoryId = item.CategoryID;
                txtCategoryName.Text = item.CategoryName;
                cboCategoryType.SelectedItem = item.CategoryType;
                cboDefaultAccount.SelectedValue = item.DefaultAccountID.HasValue ? item.DefaultAccountID.Value : 0;
                txtNotes.Text = item.Notes;
                chkIsActive.Checked = item.IsActive;
            }

            private void ClearEditor()
            {
                _selectedCategoryId = 0;
                txtCategoryName.Clear();
                txtNotes.Clear();
                chkIsActive.Checked = true;

                if (cboCategoryType.Items.Count > 0)
                    cboCategoryType.SelectedIndex = 0;

                if (cboDefaultAccount.Items.Count > 0)
                    cboDefaultAccount.SelectedIndex = 0;

                txtCategoryName.Focus();
            }

            private void BtnNew_Click(object sender, EventArgs e)
            {
                ClearEditor();
                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "أدخل بيانات التصنيف الجديد ثم اضغط حفظ.";
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                    {
                        MessageBox.Show("اسم التصنيف مطلوب.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (cboCategoryType.SelectedItem == null)
                    {
                        MessageBox.Show("اختر نوع التصنيف.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int? defaultAccountId = null;
                    if (cboDefaultAccount.SelectedValue is int accountId && accountId > 0)
                        defaultAccountId = accountId;

                    ExpenseCategoryItem item = new ExpenseCategoryItem
                    {
                        CategoryID = _selectedCategoryId,
                        CategoryName = txtCategoryName.Text.Trim(),
                        CategoryType = Convert.ToString(cboCategoryType.SelectedItem),
                        DefaultAccountID = defaultAccountId,
                        Notes = txtNotes.Text.Trim(),
                        IsActive = chkIsActive.Checked
                    };

                    int savedId = _service.SaveCategory(item);
                    LoadCategories();

                    var savedItem = _categories.FirstOrDefault(x => x.CategoryID == savedId);
                    if (savedItem != null)
                    {
                        foreach (DataGridViewRow row in dgvCategories.Rows)
                        {
                            if (row.DataBoundItem is ExpenseCategoryItem current && current.CategoryID == savedId)
                            {
                                row.Selected = true;
                                dgvCategories.CurrentCell = row.Cells[0];
                                break;
                            }
                        }
                    }

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = _selectedCategoryId > 0 ? "تم تعديل التصنيف بنجاح." : "تم إضافة التصنيف بنجاح.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnDelete_Click(object sender, EventArgs e)
            {
                try
                {
                    if (_selectedCategoryId <= 0)
                    {
                        MessageBox.Show("اختر تصنيفًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show("هل تريد حذف التصنيف المحدد؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    _service.DeleteCategory(_selectedCategoryId);
                    LoadCategories();
                    ClearEditor();

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حذف التصنيف بنجاح.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnRefresh_Click(object sender, EventArgs e)
            {
                LoadAccounts();
                LoadCategories();
                ClearEditor();
                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "تم تحديث البيانات.";
            }
        }
    }