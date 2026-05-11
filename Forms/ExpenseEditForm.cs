/*using System;
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
    public partial class ExpenseEditForm : Form
    {
            private readonly ExpenseService _service = new ExpenseService();
            private readonly int _expenseId;
            private readonly bool _readOnly;

            private ComboBox cboCategory;
            private TextBox txtCategoryType;
            private DateTimePicker dtExpenseDate;
            private TextBox txtSupplierName;
            private TextBox txtDescription;
            private TextBox txtNotes;
            private ComboBox cboPaymentMethod;

            private ComboBox cboCashAccount;
            private ComboBox cboCounterAccount;

            private DataGridView dgvLines;
            private Button btnAddLine;
            private Button btnRemoveLine;
            private Button btnSave;
            private Button btnCancel;
            private Label lblTotal;
            private Label lblStatus;

            private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();
            private List<AccountLookupItem> _accounts = new List<AccountLookupItem>();

            private BindingSource _linesSource = new BindingSource();
            private List<ExpenseLineItem> _lines = new List<ExpenseLineItem>();

            private bool _isLoadingExpense = false;

            public ExpenseEditForm(int expenseId = 0, bool readOnly = false)
            {
                _expenseId = expenseId;
                _readOnly = readOnly;

                BuildUi();

                PermissionHelper.EnforceFormPermission(this, "EXPENSES_VIEW");

                LoadAccounts();
                LoadCategories();

                if (_expenseId > 0)
                    LoadExpense();
                else
                    SelectDefaultCashAccountByPaymentMethod();

                ApplyReadOnlyMode();
                RecalcTotal();
            }

            private void BuildUi()
            {
                Text = _readOnly ? "عرض حركة مصروف/شراء" : "إدخال حركة مصروف/شراء";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                ClientSize = new Size(1100, 700);

                Panel pnlHeader = new Panel
                {
                    Location = new Point(20, 20),
                    Size = new Size(1060, 170),
                    BackColor = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle
                };

                pnlHeader.Controls.Add(MakeLabel("التصنيف", 980, 20));
                pnlHeader.Controls.Add(MakeLabel("النوع", 980, 55));
                pnlHeader.Controls.Add(MakeLabel("التاريخ", 980, 90));
                pnlHeader.Controls.Add(MakeLabel("طريقة الدفع", 980, 125));

                cboCategory = new ComboBox
                {
                    Location = new Point(760, 18),
                    Size = new Size(200, 27),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                txtCategoryType = MakeReadOnlyTextBox(760, 53);

                dtExpenseDate = new DateTimePicker
                {
                    Location = new Point(760, 88),
                    Size = new Size(200, 27),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today
                };

                cboPaymentMethod = new ComboBox
                {
                    Location = new Point(760, 123),
                    Size = new Size(200, 27),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                cboPaymentMethod.Items.AddRange(new object[]
                {
                "Cash",
                "Transfer",
                "Cheque",
                "Credit"
                });

                cboPaymentMethod.SelectedIndex = 0;

                cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;
                cboPaymentMethod.SelectedIndexChanged += CboPaymentMethod_SelectedIndexChanged;

                pnlHeader.Controls.AddRange(new Control[]
                {
                cboCategory,
                txtCategoryType,
                dtExpenseDate,
                cboPaymentMethod
                });

                pnlHeader.Controls.Add(MakeLabel("المورد/الجهة", 700, 20));
                pnlHeader.Controls.Add(MakeLabel("البيان", 700, 55));
                pnlHeader.Controls.Add(MakeLabel("ملاحظات", 700, 90));
                pnlHeader.Controls.Add(MakeLabel("حساب الدفع", 700, 125));

                txtSupplierName = new TextBox
                {
                    Location = new Point(430, 18),
                    Size = new Size(250, 27)
                };

                txtDescription = new TextBox
                {
                    Location = new Point(430, 53),
                    Size = new Size(250, 27)
                };

                txtNotes = new TextBox
                {
                    Location = new Point(430, 88),
                    Size = new Size(250, 27)
                };

                cboCashAccount = new ComboBox
                {
                    Location = new Point(430, 123),
                    Size = new Size(250, 27),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                pnlHeader.Controls.AddRange(new Control[]
                {
                txtSupplierName,
                txtDescription,
                txtNotes,
                cboCashAccount
                });

                pnlHeader.Controls.Add(MakeLabel("الحساب المقابل", 370, 20));

                cboCounterAccount = new ComboBox
                {
                    Location = new Point(120, 18),
                    Size = new Size(230, 27),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                pnlHeader.Controls.Add(cboCounterAccount);

                dgvLines = new DataGridView
                {
                    Location = new Point(20, 210),
                    Size = new Size(1060, 360),
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AutoGenerateColumns = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                dgvLines.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "اسم البند",
                    DataPropertyName = "ItemName",
                    FillWeight = 220
                });

                dgvLines.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "الكمية",
                    DataPropertyName = "Qty",
                    FillWeight = 90
                });

                dgvLines.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "سعر الوحدة",
                    DataPropertyName = "UnitPrice",
                    FillWeight = 90
                });

                dgvLines.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "الإجمالي",
                    DataPropertyName = "LineTotal",
                    ReadOnly = true,
                    FillWeight = 90
                });

                dgvLines.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "الحساب الهدف",
                    DataPropertyName = "TargetAccountID",
                    FillWeight = 90
                });

                dgvLines.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "ملاحظات",
                    DataPropertyName = "Notes",
                    FillWeight = 180
                });

                dgvLines.CellEndEdit += DgvLines_CellEndEdit;
                dgvLines.DataError += DgvLines_DataError;

                _linesSource.DataSource = _lines;
                dgvLines.DataSource = _linesSource;

                btnAddLine = MakeButton("إضافة سطر", 20, 585, 120, Color.SteelBlue);
                btnRemoveLine = MakeButton("حذف سطر", 150, 585, 120, Color.IndianRed);
                btnSave = MakeButton("حفظ", 820, 585, 120, Color.FromArgb(0, 153, 76));
                btnCancel = MakeButton("إلغاء", 950, 585, 120, Color.Gray);

                btnAddLine.Click += BtnAddLine_Click;
                btnRemoveLine.Click += BtnRemoveLine_Click;
                btnSave.Click += BtnSave_Click;
                btnCancel.Click += BtnCancel_Click;

                lblTotal = new Label
                {
                    Location = new Point(350, 590),
                    Size = new Size(300, 25),
                    Font = new Font("Tahoma", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                lblStatus = new Label
                {
                    Location = new Point(20, 640),
                    Size = new Size(1060, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                pnlHeader,
                dgvLines,
                btnAddLine,
                btnRemoveLine,
                btnSave,
                btnCancel,
                lblTotal,
                lblStatus
                });
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

            private TextBox MakeReadOnlyTextBox(int left, int top)
            {
                return new TextBox
                {
                    Location = new Point(left, top),
                    Size = new Size(200, 27),
                    ReadOnly = true,
                    BackColor = Color.WhiteSmoke
                };
            }

            private Button MakeButton(string text, int left, int top, int width, Color color)
            {
                return new Button
                {
                    Text = text,
                    Location = new Point(left, top),
                    Size = new Size(width, 35),
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

                    if (_accounts == null)
                        _accounts = new List<AccountLookupItem>();

                    cboCashAccount.DataSource = null;
                    cboCashAccount.DisplayMember = "AccountName";
                    cboCashAccount.ValueMember = "AccountID";
                    cboCashAccount.DataSource = BuildAccountList("اختر حساب الدفع");

                    cboCounterAccount.DataSource = null;
                    cboCounterAccount.DisplayMember = "AccountName";
                    cboCounterAccount.ValueMember = "AccountID";
                    cboCounterAccount.DataSource = BuildAccountList("حسب التصنيف");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private List<AccountLookupItem> BuildAccountList(string firstText)
            {
                List<AccountLookupItem> list = new List<AccountLookupItem>();

                list.Add(new AccountLookupItem
                {
                    AccountID = 0,
                    AccountCode = string.Empty,
                    AccountName = firstText
                });

                foreach (AccountLookupItem account in _accounts)
                {
                    string displayName = account.AccountName;

                    if (!string.IsNullOrWhiteSpace(account.AccountCode))
                        displayName = account.AccountCode + " - " + account.AccountName;

                    list.Add(new AccountLookupItem
                    {
                        AccountID = account.AccountID,
                        AccountCode = account.AccountCode,
                        AccountName = displayName
                    });
                }

                return list;
            }

            private void LoadCategories()
            {
                try
                {
                    _categories = _service.GetCategories();

                    if (_categories == null)
                        _categories = new List<ExpenseCategoryItem>();

                    cboCategory.DataSource = null;
                    cboCategory.DisplayMember = "CategoryName";
                    cboCategory.ValueMember = "CategoryID";
                    cboCategory.DataSource = _categories;

                    if (_categories.Count > 0)
                    {
                        cboCategory.SelectedIndex = 0;
                        txtCategoryType.Text = _categories[0].CategoryType;
                    }
                    else
                    {
                        txtCategoryType.Text = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
            {
                ExpenseCategoryItem cat = cboCategory.SelectedItem as ExpenseCategoryItem;

                if (cat != null)
                    txtCategoryType.Text = cat.CategoryType;
                else
                    txtCategoryType.Text = string.Empty;
            }

            private void CboPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (_isLoadingExpense)
                    return;

                SelectDefaultCashAccountByPaymentMethod();
            }

            private void SelectDefaultCashAccountByPaymentMethod()
            {
                if (cboPaymentMethod.SelectedItem == null || cboCashAccount.DataSource == null)
                    return;

                string method = Convert.ToString(cboPaymentMethod.SelectedItem);
                string accountCode = "1100";

                if (method == "Cash")
                    accountCode = "1100";
                else if (method == "Transfer" || method == "Cheque")
                    accountCode = "1000";
                else if (method == "Credit")
                    accountCode = "2100";

                int accountId = FindAccountIdByCode(accountCode);

                if (accountId > 0)
                    SetComboSelectedValue(cboCashAccount, accountId);
                else if (cboCashAccount.Items.Count > 0)
                    cboCashAccount.SelectedIndex = 0;
            }

            private int FindAccountIdByCode(string accountCode)
            {
                if (string.IsNullOrWhiteSpace(accountCode))
                    return 0;

                AccountLookupItem account = _accounts
                    .FirstOrDefault(x => string.Equals(x.AccountCode, accountCode, StringComparison.OrdinalIgnoreCase));

                return account == null ? 0 : account.AccountID;
            }

            private void SetComboSelectedValue(ComboBox combo, int? value)
            {
                if (combo == null)
                    return;

                if (!value.HasValue || value.Value <= 0)
                {
                    if (combo.Items.Count > 0)
                        combo.SelectedIndex = 0;

                    return;
                }

                bool exists = false;

                foreach (object item in combo.Items)
                {
                    AccountLookupItem account = item as AccountLookupItem;

                    if (account != null && account.AccountID == value.Value)
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                    combo.SelectedValue = value.Value;
                else if (combo.Items.Count > 0)
                    combo.SelectedIndex = 0;
            }

            private int? GetSelectedAccountId(ComboBox combo)
            {
                if (combo == null || combo.SelectedValue == null || combo.SelectedValue == DBNull.Value)
                    return null;

                int id;

                if (!int.TryParse(Convert.ToString(combo.SelectedValue), out id))
                    return null;

                if (id <= 0)
                    return null;

                return id;
            }

            private bool AccountExists(int accountId)
            {
                return _accounts.Any(x => x.AccountID == accountId);
            }

            private void LoadExpense()
            {
                try
                {
                    _isLoadingExpense = true;

                    ExpenseHeaderItem header = _service.GetExpenseHeader(_expenseId);

                    if (header == null)
                        return;

                    cboCategory.SelectedValue = header.CategoryID;
                    dtExpenseDate.Value = header.ExpenseDate;

                    txtSupplierName.Text = header.SupplierName;
                    txtDescription.Text = header.Description;
                    txtNotes.Text = header.Notes;

                    if (!string.IsNullOrWhiteSpace(header.PaymentMethod) &&
                        cboPaymentMethod.Items.Contains(header.PaymentMethod))
                    {
                        cboPaymentMethod.SelectedItem = header.PaymentMethod;
                    }
                    else
                    {
                        cboPaymentMethod.SelectedIndex = 0;
                    }

                    SetComboSelectedValue(cboCashAccount, header.CashAccountID);
                    SetComboSelectedValue(cboCounterAccount, header.CounterAccountID);

                    _lines = _service.GetExpenseLines(_expenseId);

                    if (_lines == null)
                        _lines = new List<ExpenseLineItem>();

                    _linesSource.DataSource = _lines;
                    dgvLines.DataSource = _linesSource;

                    RecalcTotal();
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
                finally
                {
                    _isLoadingExpense = false;
                }
            }

            private void ApplyReadOnlyMode()
            {
                if (!_readOnly)
                    return;

                cboCategory.Enabled = false;
                dtExpenseDate.Enabled = false;
                txtSupplierName.ReadOnly = true;
                txtDescription.ReadOnly = true;
                txtNotes.ReadOnly = true;
                cboPaymentMethod.Enabled = false;
                cboCashAccount.Enabled = false;
                cboCounterAccount.Enabled = false;
                dgvLines.ReadOnly = true;
                btnAddLine.Enabled = false;
                btnRemoveLine.Enabled = false;
                btnSave.Enabled = false;
            }

            private void BtnAddLine_Click(object sender, EventArgs e)
            {
                _lines.Add(new ExpenseLineItem
                {
                    Qty = 1,
                    UnitPrice = 0
                });

                _linesSource.ResetBindings(false);
                RecalcTotal();
            }

            private void BtnRemoveLine_Click(object sender, EventArgs e)
            {
                ExpenseLineItem line = null;

                if (dgvLines.CurrentRow != null)
                    line = dgvLines.CurrentRow.DataBoundItem as ExpenseLineItem;

                if (line == null)
                    return;

                _lines.Remove(line);
                _linesSource.ResetBindings(false);
                RecalcTotal();
            }

            private void DgvLines_CellEndEdit(object sender, DataGridViewCellEventArgs e)
            {
                NormalizeLineAccounts();
                RecalcTotal();
            }

            private void DgvLines_DataError(object sender, DataGridViewDataErrorEventArgs e)
            {
                e.ThrowException = false;
            }

            private void NormalizeLineAccounts()
            {
                foreach (ExpenseLineItem line in _lines)
                {
                    if (line == null)
                        continue;

                    if (line.TargetAccountID.HasValue && line.TargetAccountID.Value <= 0)
                        line.TargetAccountID = null;
                }
            }

            private void RecalcTotal()
            {
                if (_lines == null)
                    _lines = new List<ExpenseLineItem>();

                NormalizeLineAccounts();

                decimal total = _lines.Sum(x => x == null ? 0 : x.LineTotal);

                lblTotal.Text = "إجمالي الحركة: " + total.ToString("N2");

                _linesSource.ResetBindings(false);
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    if (_readOnly)
                        return;

                    if (!(cboCategory.SelectedItem is ExpenseCategoryItem cat))
                    {
                        MessageBox.Show(
                            "اختر التصنيف أولًا.",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    string paymentMethod = Convert.ToString(cboPaymentMethod.SelectedItem);

                    if (string.IsNullOrWhiteSpace(paymentMethod))
                    {
                        MessageBox.Show(
                            "اختر طريقة الدفع.",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    int? cashAccountId = GetSelectedAccountId(cboCashAccount);
                    int? counterAccountId = GetSelectedAccountId(cboCounterAccount);

                    if (!cashAccountId.HasValue)
                    {
                        MessageBox.Show(
                            "اختر حساب الدفع. للصندوق اختر 1100 - الصندوق، وللتحويل اختر 1000 - البنك، وللدفع الآجل اختر 2100 - ذمم موردين.",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    if (cashAccountId.HasValue && !AccountExists(cashAccountId.Value))
                    {
                        MessageBox.Show(
                            "حساب الدفع المختار غير موجود في دليل الحسابات.",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    if (counterAccountId.HasValue && !AccountExists(counterAccountId.Value))
                    {
                        MessageBox.Show(
                            "الحساب المقابل المختار غير موجود في دليل الحسابات.",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    NormalizeLineAccounts();

                    foreach (ExpenseLineItem line in _lines)
                    {
                        if (line == null)
                            continue;

                        if (line.TargetAccountID.HasValue &&
                            line.TargetAccountID.Value > 0 &&
                            !AccountExists(line.TargetAccountID.Value))
                        {
                            MessageBox.Show(
                                "يوجد حساب هدف غير موجود في أحد البنود. صحح الحساب الهدف أو اتركه فارغًا.",
                                "تنبيه",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            return;
                        }
                    }

                    ExpenseHeaderItem header = new ExpenseHeaderItem
                    {
                        ExpenseID = _expenseId,
                        ExpenseDate = dtExpenseDate.Value.Date,
                        CategoryID = cat.CategoryID,
                        SupplierName = txtSupplierName.Text.Trim(),
                        Description = txtDescription.Text.Trim(),
                        Notes = txtNotes.Text.Trim(),
                        PaymentMethod = paymentMethod,
                        CashAccountID = cashAccountId,
                        CounterAccountID = counterAccountId
                    };

                    ExpenseSaveResult result;

                    if (_expenseId > 0)
                        result = _service.UpdateExpense(header, _lines);
                    else
                        result = _service.SaveExpense(header, _lines);

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حفظ الحركة بنجاح. رقم السند: " + result.ExpenseNumber;

                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;

                    MessageBox.Show(
                        ex.Message,
                        "خطأ في الحفظ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            private void BtnCancel_Click(object sender, EventArgs e)
            {
                Close();
            }
        }
    }
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using water3.Models;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
    public partial class ExpenseEditForm : Form
    {
        private readonly ExpenseService _service = new ExpenseService();
        private readonly int _expenseId;
        private readonly bool _readOnly;

        private TableLayoutPanel mainLayout;
        private Panel headerPanel;
        private Panel formPanel;
        private Panel gridPanel;
        private Panel footerPanel;

        private ComboBox cboCategory;
        private TextBox txtCategoryType;
        private DateTimePicker dtExpenseDate;
        private TextBox txtSupplierName;
        private TextBox txtDescription;
        private TextBox txtNotes;
        private ComboBox cboPaymentMethod;
        private ComboBox cboCashAccount;
        private ComboBox cboCounterAccount;

        private DataGridView dgvLines;
        private DataGridViewComboBoxColumn colTargetAccount;

        private Button btnAddLine;
        private Button btnRemoveLine;
        private Button btnSave;
        private Button btnCancel;

        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblTotal;
        private Label lblStatus;

        private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();
        private List<AccountLookupItem> _accounts = new List<AccountLookupItem>();

        private readonly BindingSource _linesSource = new BindingSource();
        private List<ExpenseLineItem> _lines = new List<ExpenseLineItem>();

        private bool _isLoadingExpense = false;

        public ExpenseEditForm(int expenseId = 0, bool readOnly = false)
        {
            _expenseId = expenseId;
            _readOnly = readOnly;

            BuildUi();

            PermissionHelper.EnforceFormPermission(this, "EXPENSES_VIEW");

            LoadAccounts();
            LoadCategories();

            if (_expenseId > 0)
                LoadExpense();
            else
                SelectDefaultCashAccountByPaymentMethod();

            ApplyReadOnlyMode();
            RecalcTotal();
        }

        #region Build UI

        private void BuildUi()
        {
            Text = _readOnly ? "عرض حركة مصروف / شراء" : "إدخال حركة مصروف / شراء";
            StartPosition = FormStartPosition.CenterParent;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = new Font("Tahoma", 10F);
            BackColor = Color.FromArgb(244, 247, 251);
            MinimumSize = new Size(1050, 650);
            Size = new Size(1180, 700);
            DoubleBuffered = true;

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12),
                BackColor = Color.FromArgb(244, 247, 251)
            };

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // تم تخفيض الارتفاع حتى لا تنزل الأزرار أسفل الشاشة
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 246F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));

            BuildHeader();
            BuildFormPanel();
            BuildGridPanel();
            BuildFooter();

            mainLayout.Controls.Add(headerPanel, 0, 0);
            mainLayout.Controls.Add(formPanel, 0, 1);
            mainLayout.Controls.Add(gridPanel, 0, 2);
            mainLayout.Controls.Add(footerPanel, 0, 3);

            Controls.Add(mainLayout);

            Load += (s, e) =>
            {
                Rectangle workingArea = Screen.FromControl(this).WorkingArea;

                if (Height > workingArea.Height - 20)
                    Height = workingArea.Height - 20;

                if (Width > workingArea.Width - 20)
                    Width = workingArea.Width - 20;
            };
        }

        private void BuildHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(14),
                Margin = new Padding(0, 0, 0, 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            TableLayoutPanel headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));

            TableLayoutPanel titleLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            titleLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            titleLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));

            lblTitle = new Label
            {
                Dock = DockStyle.Fill,
                Text = _readOnly ? "عرض حركة مصروف / شراء" : "إدخال حركة مصروف / شراء",
                Font = new Font("Tahoma", 17F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 76, 117),
                TextAlign = ContentAlignment.MiddleRight
            };

            lblSubtitle = new Label
            {
                Dock = DockStyle.Fill,
                Text = "أدخل بيانات الحركة وأضف البنود قبل الحفظ",
                Font = new Font("Tahoma", 9.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleRight
            };

            titleLayout.Controls.Add(lblTitle, 0, 0);
            titleLayout.Controls.Add(lblSubtitle, 0, 1);

            lblTotal = new Label
            {
                Dock = DockStyle.Fill,
                Text = "إجمالي الحركة: 0.00",
                BackColor = Color.FromArgb(237, 244, 255),
                ForeColor = Color.FromArgb(37, 99, 235),
                Font = new Font("Tahoma", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            headerLayout.Controls.Add(titleLayout, 0, 0);
            headerLayout.Controls.Add(lblTotal, 1, 0);

            headerPanel.Controls.Add(headerLayout);
        }

        private void BuildFormPanel()
        {
            formPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(14),
                Margin = new Padding(0, 0, 0, 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            TableLayoutPanel outerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            outerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Label lblFormTitle = new Label
            {
                Text = "بيانات الحركة",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.MiddleRight
            };

            TableLayoutPanel formLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 4,
                BackColor = Color.White
            };

            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            cboCategory = MakeCombo();
            txtCategoryType = MakeTextBox(true);

            dtExpenseDate = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy/MM/dd",
                Value = DateTime.Today,
                RightToLeftLayout = true,
                Margin = Padding.Empty
            };

            cboPaymentMethod = MakeCombo();
            cboPaymentMethod.Items.AddRange(new object[]
 {
    "Cash",
    "Transfer",
    "Cheque",
    "Credit"
 });

            SelectFirstIfExists(cboPaymentMethod);

            txtSupplierName = MakeTextBox(false);
            txtDescription = MakeTextBox(false);
            txtNotes = MakeTextBox(false);
            cboCashAccount = MakeCombo();
            cboCounterAccount = MakeCombo();

            cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;
            cboPaymentMethod.SelectedIndexChanged += CboPaymentMethod_SelectedIndexChanged;

            // العمود الأيمن
            formLayout.Controls.Add(MakeInputBlock("التصنيف", cboCategory), 2, 0);
            formLayout.Controls.Add(MakeInputBlock("النوع", txtCategoryType), 2, 1);
            formLayout.Controls.Add(MakeInputBlock("التاريخ", dtExpenseDate), 2, 2);

            // العمود الأوسط
            formLayout.Controls.Add(MakeInputBlock("المورد / الجهة", txtSupplierName), 1, 0);
            formLayout.Controls.Add(MakeInputBlock("البيان", txtDescription), 1, 1);
            formLayout.Controls.Add(MakeInputBlock("ملاحظات", txtNotes), 1, 2);

            // العمود الأيسر
            formLayout.Controls.Add(MakeInputBlock("الحساب المقابل", cboCounterAccount), 0, 0);
            formLayout.Controls.Add(MakeInputBlock("حساب الدفع", cboCashAccount), 0, 1);
            formLayout.Controls.Add(MakeInputBlock("طريقة الدفع", cboPaymentMethod), 0, 2);

            Label hint = new Label
            {
                Dock = DockStyle.Fill,
                Text = "ملاحظة: اترك الحساب المقابل على «حسب التصنيف» إلا إذا أردت توجيه الحركة إلى حساب محدد.",
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(8, 4, 8, 4),
                Font = new Font("Tahoma", 9F),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            formLayout.SetColumnSpan(hint, 3);
            formLayout.Controls.Add(hint, 0, 3);

            outerLayout.Controls.Add(lblFormTitle, 0, 0);
            outerLayout.Controls.Add(formLayout, 0, 1);

            formPanel.Controls.Add(outerLayout);
        }

        private void BuildGridPanel()
        {
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12),
                Margin = new Padding(0, 0, 0, 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblLinesTitle = new Label
            {
                Text = "بنود الحركة",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Tahoma", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.MiddleRight
            };

            dgvLines = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(226, 232, 240),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EditMode = DataGridViewEditMode.EditOnEnter
            };

            dgvLines.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 76, 117);
            dgvLines.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLines.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            dgvLines.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvLines.ColumnHeadersHeight = 42;
            dgvLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvLines.DefaultCellStyle.Font = new Font("Tahoma", 9.5F);
            dgvLines.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            dgvLines.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvLines.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvLines.DefaultCellStyle.Padding = new Padding(4);
            dgvLines.RowTemplate.Height = 36;
            dgvLines.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colItemName",
                HeaderText = "اسم البند",
                DataPropertyName = "ItemName",
                FillWeight = 220,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQty",
                HeaderText = "الكمية",
                DataPropertyName = "Qty",
                FillWeight = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "N2"
                }
            });

            dgvLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUnitPrice",
                HeaderText = "سعر الوحدة",
                DataPropertyName = "UnitPrice",
                FillWeight = 95,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "N2"
                }
            });

            dgvLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLineTotal",
                HeaderText = "الإجمالي",
                DataPropertyName = "LineTotal",
                ReadOnly = true,
                FillWeight = 95,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "N2",
                    Font = new Font("Tahoma", 9.5F, FontStyle.Bold)
                }
            });

            colTargetAccount = new DataGridViewComboBoxColumn
            {
                Name = "colTargetAccount",
                HeaderText = "الحساب الهدف",
                DataPropertyName = "TargetAccountID",
                FillWeight = 150,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FlatStyle = FlatStyle.Flat
            };

            dgvLines.Columns.Add(colTargetAccount);

            dgvLines.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNotes",
                HeaderText = "ملاحظات",
                DataPropertyName = "Notes",
                FillWeight = 180,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvLines.CellEndEdit += DgvLines_CellEndEdit;
            dgvLines.DataError += DgvLines_DataError;

            _linesSource.DataSource = _lines;
            dgvLines.DataSource = _linesSource;

            gridPanel.Controls.Add(dgvLines);
            gridPanel.Controls.Add(lblLinesTitle);
        }

        private void BuildFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 8),
                BorderStyle = BorderStyle.FixedSingle
            };

            TableLayoutPanel footerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };

            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(22, 101, 52),
                Font = new Font("Tahoma", 9.5F)
            };

            FlowLayoutPanel buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(0, 3, 0, 0)
            };

            btnCancel = MakeButton("إلغاء", Color.FromArgb(100, 116, 139));
            btnSave = MakeButton("حفظ", Color.FromArgb(22, 163, 74));
            btnRemoveLine = MakeButton("حذف سطر", Color.FromArgb(220, 38, 38), 110);
            btnAddLine = MakeButton("إضافة سطر", Color.FromArgb(37, 99, 235), 115);

            btnAddLine.Click += BtnAddLine_Click;
            btnRemoveLine.Click += BtnRemoveLine_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            buttonsPanel.Controls.Add(btnCancel);
            buttonsPanel.Controls.Add(btnSave);
            buttonsPanel.Controls.Add(btnRemoveLine);
            buttonsPanel.Controls.Add(btnAddLine);

            footerLayout.Controls.Add(buttonsPanel, 0, 0);
            footerLayout.Controls.Add(lblStatus, 1, 0);

            footerPanel.Controls.Add(footerLayout);
        }

        private ComboBox MakeCombo()
        {
            return new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = Padding.Empty
            };
        }

        private TextBox MakeTextBox(bool readOnly)
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = readOnly,
                Margin = Padding.Empty,
                BackColor = readOnly ? Color.FromArgb(248, 250, 252) : Color.White
            };
        }

        private Panel MakeInputBlock(string caption, Control input)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(7, 3, 7, 3),
                BackColor = Color.White
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

            Label label = new Label
            {
                Text = caption,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(71, 85, 105),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Tahoma", 9F)
            };

            input.Dock = DockStyle.Fill;
            input.Font = new Font("Tahoma", 10F);

            layout.Controls.Add(label, 0, 0);
            layout.Controls.Add(input, 0, 1);

            panel.Controls.Add(layout);

            return panel;
        }

        private Button MakeButton(string text, Color color, int width = 96)
        {
            Button btn = new Button
            {
                Text = text,
                Width = width,
                Height = 36,
                Margin = new Padding(6, 0, 0, 0),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Tahoma", 9.5F, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.08F);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.08F);

            return btn;
        }

        #endregion

        #region Load Data

        private void LoadAccounts()
        {
            try
            {
                _accounts = _service.GetAccounts();

                if (_accounts == null)
                    _accounts = new List<AccountLookupItem>();

                cboCashAccount.DataSource = null;
                cboCashAccount.DisplayMember = "AccountName";
                cboCashAccount.ValueMember = "AccountID";
                cboCashAccount.DataSource = BuildAccountList("اختر حساب الدفع");

                cboCounterAccount.DataSource = null;
                cboCounterAccount.DisplayMember = "AccountName";
                cboCounterAccount.ValueMember = "AccountID";
                cboCounterAccount.DataSource = BuildAccountList("حسب التصنيف");

                colTargetAccount.DataSource = null;
                colTargetAccount.DisplayMember = "AccountName";
                colTargetAccount.ValueMember = "AccountID";
                colTargetAccount.DataSource = BuildAccountList("حسب التصنيف");
                colTargetAccount.DefaultCellStyle.NullValue = "حسب التصنيف";
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        private List<AccountLookupItem> BuildAccountList(string firstText)
        {
            List<AccountLookupItem> list = new List<AccountLookupItem>
            {
                new AccountLookupItem
                {
                    AccountID = 0,
                    AccountCode = string.Empty,
                    AccountName = firstText
                }
            };

            foreach (AccountLookupItem account in _accounts)
            {
                string displayName = account.AccountName;

                if (!string.IsNullOrWhiteSpace(account.AccountCode))
                    displayName = account.AccountCode + " - " + account.AccountName;

                list.Add(new AccountLookupItem
                {
                    AccountID = account.AccountID,
                    AccountCode = account.AccountCode,
                    AccountName = displayName
                });
            }

            return list;
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _service.GetCategories();

                if (_categories == null)
                    _categories = new List<ExpenseCategoryItem>();

                cboCategory.DataSource = null;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
                cboCategory.DataSource = _categories;

                if (_categories.Count > 0)
                {
                    SelectFirstIfExists(cboCategory);

                    ExpenseCategoryItem firstCategory =
                        cboCategory.SelectedItem as ExpenseCategoryItem;

                    txtCategoryType.Text = firstCategory == null
                        ? string.Empty
                        : firstCategory.CategoryType;
                }
                else
                {
                    txtCategoryType.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        #endregion

        #region Events + Helpers

        private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExpenseCategoryItem cat = cboCategory.SelectedItem as ExpenseCategoryItem;
            txtCategoryType.Text = cat == null ? string.Empty : cat.CategoryType;
        }

        private void CboPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoadingExpense)
                return;

            SelectDefaultCashAccountByPaymentMethod();
        }

        private void SelectDefaultCashAccountByPaymentMethod()
        {
            if (cboPaymentMethod.SelectedItem == null || cboCashAccount.DataSource == null)
                return;

            string method = Convert.ToString(cboPaymentMethod.SelectedItem);
            string accountCode = "1100";

            if (method == "Cash")
                accountCode = "1100";
            else if (method == "Transfer" || method == "Cheque")
                accountCode = "1000";
            else if (method == "Credit")
                accountCode = "2100";

            int accountId = FindAccountIdByCode(accountCode);

            if (accountId > 0)
                SetComboSelectedValue(cboCashAccount, accountId);
            else
                SelectFirstIfExists(cboCashAccount);
        }

        private int FindAccountIdByCode(string accountCode)
        {
            if (string.IsNullOrWhiteSpace(accountCode))
                return 0;

            AccountLookupItem account = _accounts
                .FirstOrDefault(x => string.Equals(
                    x.AccountCode,
                    accountCode,
                    StringComparison.OrdinalIgnoreCase));

            return account == null ? 0 : account.AccountID;
        }

        private void SetComboSelectedValue(ComboBox combo, int? value)
        {
            if (combo == null)
                return;

            if (!value.HasValue || value.Value <= 0)
            {
                SelectFirstIfExists(combo);
                return;
            }

            bool exists = false;

            foreach (object item in combo.Items)
            {
                AccountLookupItem account = item as AccountLookupItem;

                if (account != null && account.AccountID == value.Value)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
                combo.SelectedValue = value.Value;
            else
                SelectFirstIfExists(combo);
        }

        private int? GetSelectedAccountId(ComboBox combo)
        {
            if (combo == null || combo.SelectedValue == null || combo.SelectedValue == DBNull.Value)
                return null;

            int id;

            if (!int.TryParse(Convert.ToString(combo.SelectedValue), out id))
                return null;

            if (id <= 0)
                return null;

            return id;
        }

        private bool AccountExists(int accountId)
        {
            return _accounts.Any(x => x.AccountID == accountId);
        }

        private void LoadExpense()
        {
            try
            {
                _isLoadingExpense = true;

                ExpenseHeaderItem header = _service.GetExpenseHeader(_expenseId);

                if (header == null)
                    return;

                cboCategory.SelectedValue = header.CategoryID;
                dtExpenseDate.Value = header.ExpenseDate;

                txtSupplierName.Text = header.SupplierName;
                txtDescription.Text = header.Description;
                txtNotes.Text = header.Notes;

                if (!string.IsNullOrWhiteSpace(header.PaymentMethod) &&
                    cboPaymentMethod.Items.Contains(header.PaymentMethod))
                {
                    cboPaymentMethod.SelectedItem = header.PaymentMethod;
                }
                else
                {
                    SelectFirstIfExists(cboPaymentMethod);
                }

                SetComboSelectedValue(cboCashAccount, header.CashAccountID);
                SetComboSelectedValue(cboCounterAccount, header.CounterAccountID);

                _lines = _service.GetExpenseLines(_expenseId);

                if (_lines == null)
                    _lines = new List<ExpenseLineItem>();

                foreach (ExpenseLineItem line in _lines)
                {
                    if (line != null &&
                        line.TargetAccountID.HasValue &&
                        line.TargetAccountID.Value <= 0)
                    {
                        line.TargetAccountID = null;
                    }
                }

                _linesSource.DataSource = _lines;
                dgvLines.DataSource = _linesSource;

                RecalcTotal();
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
            finally
            {
                _isLoadingExpense = false;
            }
        }

        private void ApplyReadOnlyMode()
        {
            if (!_readOnly)
                return;

            cboCategory.Enabled = false;
            dtExpenseDate.Enabled = false;
            txtSupplierName.ReadOnly = true;
            txtDescription.ReadOnly = true;
            txtNotes.ReadOnly = true;
            cboPaymentMethod.Enabled = false;
            cboCashAccount.Enabled = false;
            cboCounterAccount.Enabled = false;
            dgvLines.ReadOnly = true;
            btnAddLine.Enabled = false;
            btnRemoveLine.Enabled = false;
            btnSave.Enabled = false;
        }

        private void BtnAddLine_Click(object sender, EventArgs e)
        {
            _lines.Add(new ExpenseLineItem
            {
                Qty = 1,
                UnitPrice = 0
            });

            _linesSource.ResetBindings(false);
            RecalcTotal();

            if (dgvLines.Rows.Count > 0)
            {
                int lastRow = dgvLines.Rows.Count - 1;
                dgvLines.CurrentCell = dgvLines.Rows[lastRow].Cells["colItemName"];
                dgvLines.BeginEdit(true);
            }
        }

        private void BtnRemoveLine_Click(object sender, EventArgs e)
        {
            ExpenseLineItem line = null;

            if (dgvLines.CurrentRow != null)
                line = dgvLines.CurrentRow.DataBoundItem as ExpenseLineItem;

            if (line == null)
                return;

            _lines.Remove(line);
            _linesSource.ResetBindings(false);
            RecalcTotal();
        }

        private void DgvLines_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            NormalizeLineAccounts();
            RecalcTotal();
        }

        private void DgvLines_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void NormalizeLineAccounts()
        {
            foreach (ExpenseLineItem line in _lines)
            {
                if (line == null)
                    continue;

                if (line.TargetAccountID.HasValue && line.TargetAccountID.Value <= 0)
                    line.TargetAccountID = null;
            }
        }

        private void RecalcTotal()
        {
            if (_lines == null)
                _lines = new List<ExpenseLineItem>();

            NormalizeLineAccounts();

            decimal total = _lines.Sum(x => x == null ? 0 : x.LineTotal);

            lblTotal.Text = "إجمالي الحركة: " + total.ToString("N2");

            _linesSource.ResetBindings(false);
        }

        #endregion

        #region Save

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_readOnly)
                    return;

                if (!(cboCategory.SelectedItem is ExpenseCategoryItem cat))
                {
                    MessageBox.Show(
                        "اختر التصنيف أولًا.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                string paymentMethod = Convert.ToString(cboPaymentMethod.SelectedItem);

                if (string.IsNullOrWhiteSpace(paymentMethod))
                {
                    MessageBox.Show(
                        "اختر طريقة الدفع.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int? cashAccountId = GetSelectedAccountId(cboCashAccount);
                int? counterAccountId = GetSelectedAccountId(cboCounterAccount);

                if (!cashAccountId.HasValue)
                {
                    MessageBox.Show(
                        "اختر حساب الدفع. للصندوق اختر 1100 - الصندوق، وللتحويل اختر 1000 - البنك، وللدفع الآجل اختر 2100 - ذمم موردين.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (cashAccountId.HasValue && !AccountExists(cashAccountId.Value))
                {
                    MessageBox.Show(
                        "حساب الدفع المختار غير موجود في دليل الحسابات.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (counterAccountId.HasValue && !AccountExists(counterAccountId.Value))
                {
                    MessageBox.Show(
                        "الحساب المقابل المختار غير موجود في دليل الحسابات.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                NormalizeLineAccounts();

                foreach (ExpenseLineItem line in _lines)
                {
                    if (line == null)
                        continue;

                    if (line.TargetAccountID.HasValue &&
                        line.TargetAccountID.Value > 0 &&
                        !AccountExists(line.TargetAccountID.Value))
                    {
                        MessageBox.Show(
                            "يوجد حساب هدف غير موجود في أحد البنود. صحح الحساب الهدف أو اتركه فارغًا.",
                            "تنبيه",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }

                ExpenseHeaderItem header = new ExpenseHeaderItem
                {
                    ExpenseID = _expenseId,
                    ExpenseDate = dtExpenseDate.Value.Date,
                    CategoryID = cat.CategoryID,
                    SupplierName = txtSupplierName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Notes = txtNotes.Text.Trim(),
                    PaymentMethod = paymentMethod,
                    CashAccountID = cashAccountId,
                    CounterAccountID = counterAccountId
                };

                ExpenseSaveResult result;

                if (_expenseId > 0)
                    result = _service.UpdateExpense(header, _lines);
                else
                    result = _service.SaveExpense(header, _lines);

                SetStatus("تم حفظ الحركة بنجاح. رقم السند: " + result.ExpenseNumber, false);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);

                MessageBox.Show(
                    ex.Message,
                    "خطأ في الحفظ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SelectFirstIfExists(ComboBox combo)
        {
            if (combo != null && combo.Items.Count > 0)
                combo.SelectedIndex = 0;
        }
        private void SetStatus(string message, bool isError)
        {
            lblStatus.ForeColor = isError
                ? Color.FromArgb(185, 28, 28)
                : Color.FromArgb(22, 101, 52);

            lblStatus.Text = message;
        }

        #endregion
    }
}