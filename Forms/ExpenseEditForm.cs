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

            private ComboBox cboCategory;
            private TextBox txtCategoryType;
            private DateTimePicker dtExpenseDate;
            private TextBox txtSupplierName;
            private TextBox txtDescription;
            private TextBox txtNotes;
            private ComboBox cboPaymentMethod;
            private TextBox txtCashAccountID;
            private TextBox txtCounterAccountID;

            private DataGridView dgvLines;
            private Button btnAddLine;
            private Button btnRemoveLine;
            private Button btnSave;
            private Button btnCancel;
            private Label lblTotal;
            private Label lblStatus;

            private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();
            private BindingSource _linesSource = new BindingSource();
            private List<ExpenseLineItem> _lines = new List<ExpenseLineItem>();

            public ExpenseEditForm(int expenseId = 0, bool readOnly = false)
            {
                _expenseId = expenseId;
                _readOnly = readOnly;

                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "EXPENSES_VIEW");
                LoadCategories();

                if (_expenseId > 0)
                    LoadExpense();

                ApplyReadOnlyMode();
                RecalcTotal();
            }

            private void InitializeComponent()
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

                cboCategory = new ComboBox { Location = new Point(760, 18), Size = new Size(200, 27), DropDownStyle = ComboBoxStyle.DropDownList };
                txtCategoryType = MakeReadOnlyTextBox(760, 53);
                dtExpenseDate = new DateTimePicker { Location = new Point(760, 88), Size = new Size(200, 27), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                cboPaymentMethod = new ComboBox { Location = new Point(760, 123), Size = new Size(200, 27), DropDownStyle = ComboBoxStyle.DropDownList };
                cboPaymentMethod.Items.AddRange(new object[] { "Cash", "Transfer", "Cheque", "Credit" });
                cboPaymentMethod.SelectedIndex = 0;
                cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;

                pnlHeader.Controls.AddRange(new Control[] { cboCategory, txtCategoryType, dtExpenseDate, cboPaymentMethod });

                pnlHeader.Controls.Add(MakeLabel("المورد/الجهة", 700, 20));
                pnlHeader.Controls.Add(MakeLabel("البيان", 700, 55));
                pnlHeader.Controls.Add(MakeLabel("ملاحظات", 700, 90));
                pnlHeader.Controls.Add(MakeLabel("حساب الدفع", 700, 125));

                txtSupplierName = new TextBox { Location = new Point(430, 18), Size = new Size(250, 27) };
                txtDescription = new TextBox { Location = new Point(430, 53), Size = new Size(250, 27) };
                txtNotes = new TextBox { Location = new Point(430, 88), Size = new Size(250, 27) };
                txtCashAccountID = new TextBox { Location = new Point(430, 123), Size = new Size(250, 27) };

                pnlHeader.Controls.AddRange(new Control[] { txtSupplierName, txtDescription, txtNotes, txtCashAccountID });

                pnlHeader.Controls.Add(MakeLabel("الحساب المقابل", 370, 20));
                txtCounterAccountID = new TextBox { Location = new Point(120, 18), Size = new Size(230, 27) };
                pnlHeader.Controls.Add(txtCounterAccountID);

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

                dgvLines.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "اسم البند", DataPropertyName = "ItemName", FillWeight = 220 });
                dgvLines.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الكمية", DataPropertyName = "Qty", FillWeight = 90 });
                dgvLines.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "سعر الوحدة", DataPropertyName = "UnitPrice", FillWeight = 90 });
                dgvLines.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الإجمالي", DataPropertyName = "LineTotal", ReadOnly = true, FillWeight = 90 });
                dgvLines.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الحساب الهدف", DataPropertyName = "TargetAccountID", FillWeight = 90 });
                dgvLines.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ملاحظات", DataPropertyName = "Notes", FillWeight = 180 });

                dgvLines.CellEndEdit += (s, e) => RecalcTotal();

                _linesSource.DataSource = _lines;
                dgvLines.DataSource = _linesSource;

                btnAddLine = MakeButton("إضافة سطر", 20, 585, 120, Color.SteelBlue);
                btnRemoveLine = MakeButton("حذف سطر", 150, 585, 120, Color.IndianRed);
                btnSave = MakeButton("حفظ", 820, 585, 120, Color.FromArgb(0, 153, 76));
                btnCancel = MakeButton("إلغاء", 950, 585, 120, Color.Gray);

                btnAddLine.Click += BtnAddLine_Click;
                btnRemoveLine.Click += BtnRemoveLine_Click;
                btnSave.Click += BtnSave_Click;
                btnCancel.Click += (s, e) => Close();

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
                return new Label { Text = text, AutoSize = true, Location = new Point(left, top + 5) };
            }

            private TextBox MakeReadOnlyTextBox(int left, int top)
            {
                return new TextBox { Location = new Point(left, top), Size = new Size(200, 27), ReadOnly = true, BackColor = Color.WhiteSmoke };
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

            private void LoadCategories()
            {
                _categories = _service.GetCategories();
                cboCategory.DataSource = _categories;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";

                if (_categories.Count > 0)
                    txtCategoryType.Text = _categories[0].CategoryType;
            }

            private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (cboCategory.SelectedItem is ExpenseCategoryItem cat)
                    txtCategoryType.Text = cat.CategoryType;
            }

            private void LoadExpense()
            {
                try
                {
                    ExpenseHeaderItem header = _service.GetExpenseHeader(_expenseId);
                    if (header == null)
                        return;

                    cboCategory.SelectedValue = header.CategoryID;
                    dtExpenseDate.Value = header.ExpenseDate;
                    txtSupplierName.Text = header.SupplierName;
                    txtDescription.Text = header.Description;
                    txtNotes.Text = header.Notes;
                    cboPaymentMethod.SelectedItem = header.PaymentMethod;
                    txtCashAccountID.Text = header.CashAccountID?.ToString();
                    txtCounterAccountID.Text = header.CounterAccountID?.ToString();

                    _lines = _service.GetExpenseLines(_expenseId);
                    _linesSource.DataSource = _lines;
                    dgvLines.DataSource = _linesSource;
                    RecalcTotal();
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
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
                txtCashAccountID.ReadOnly = true;
                txtCounterAccountID.ReadOnly = true;
                dgvLines.ReadOnly = true;
                btnAddLine.Enabled = false;
                btnRemoveLine.Enabled = false;
                btnSave.Enabled = false;
            }

            private void BtnAddLine_Click(object sender, EventArgs e)
            {
                _lines.Add(new ExpenseLineItem { Qty = 1, UnitPrice = 0 });
                _linesSource.ResetBindings(false);
                RecalcTotal();
            }

            private void BtnRemoveLine_Click(object sender, EventArgs e)
            {
                if (dgvLines.CurrentRow?.DataBoundItem is ExpenseLineItem line)
                {
                    _lines.Remove(line);
                    _linesSource.ResetBindings(false);
                    RecalcTotal();
                }
            }

            private void RecalcTotal()
            {
                lblTotal.Text = $"إجمالي الحركة: {_lines.Sum(x => x.LineTotal):N2}";
                _linesSource.ResetBindings(false);
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    if (!(cboCategory.SelectedItem is ExpenseCategoryItem cat))
                    {
                        MessageBox.Show("اختر التصنيف أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ExpenseHeaderItem header = new ExpenseHeaderItem
                    {
                        ExpenseID = _expenseId,
                        ExpenseDate = dtExpenseDate.Value,
                        CategoryID = cat.CategoryID,
                        SupplierName = txtSupplierName.Text,
                        Description = txtDescription.Text,
                        Notes = txtNotes.Text,
                        PaymentMethod = Convert.ToString(cboPaymentMethod.SelectedItem),
                        CashAccountID = int.TryParse(txtCashAccountID.Text, out int cashId) ? (int?)cashId : null,
                        CounterAccountID = int.TryParse(txtCounterAccountID.Text, out int counterId) ? (int?)counterId : null
                    };

                ExpenseSaveResult result;

                if (_expenseId > 0 && !_readOnly)
                    result = _service.UpdateExpense(header, _lines);
                else
                    result = _service.SaveExpense(header, _lines);

                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = $"تم حفظ الحركة بنجاح. رقم السند: {result.ExpenseNumber}";

                DialogResult = DialogResult.OK;
                Close();

            }
            catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }