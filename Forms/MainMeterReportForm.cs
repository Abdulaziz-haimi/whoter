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
    public partial class MainMeterReportForm : Form
    {
     
        //public class MainMeterReportForm : Form
        //{
            private readonly MainMeterReportService _service = new MainMeterReportService();

            private ComboBox cboMainMeters;
            private DateTimePicker dtpReportDate;
            private TextBox txtPrevReading;
            private TextBox txtCurrentReading;
            private TextBox txtMainDiff;
            private TextBox txtSubTotalDiff;
            private TextBox txtWaterLoss;
            private TextBox txtWaterLossPercent;
            private TextBox txtConsumptionAmount;
            private TextBox txtServiceFees;
            private TextBox txtTotalDue;
            private TextBox txtArrears;
            private TextBox txtTotalFromQasem;
            private TextBox txtNotes;

            private Button btnLoadPrevious;
            private Button btnCalculateAndSave;
            private Button btnRefresh;
            private Button btnPrint;

            private DataGridView dgvSubMeters;
            private Label lblStatus;

            private List<MainMeterLookupItem> _mainMeters = new List<MainMeterLookupItem>();

            public MainMeterReportForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "REPORTS_VIEW");
                LoadMainMeters();
            }

            private void InitializeComponent()
            {
                Text = "تقرير العداد الرئيسي والفاقد";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                WindowState = FormWindowState.Maximized;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;

                Label lblTitle = new Label
                {
                    Text = "تقرير العداد الرئيسي وحساب الفاقد",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                Panel pnlInputs = new Panel
                {
                    Location = new Point(20, 60),
                    Size = new Size(1240, 220),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(248, 250, 252)
                };

                int labelX = 1120;
                int controlX = 900;
                int row1 = 20;
                int row2 = 60;
                int row3 = 100;
                int row4 = 140;
                int row5 = 180;

                pnlInputs.Controls.Add(MakeLabel("العداد الرئيسي", labelX, row1));
                pnlInputs.Controls.Add(MakeLabel("تاريخ التقرير", labelX, row2));
                pnlInputs.Controls.Add(MakeLabel("القراءة السابقة", labelX, row3));
                pnlInputs.Controls.Add(MakeLabel("القراءة الحالية", labelX, row4));
                pnlInputs.Controls.Add(MakeLabel("من قاسم", labelX, row5));

                cboMainMeters = new ComboBox { Location = new Point(controlX, row1), Size = new Size(200, 27), DropDownStyle = ComboBoxStyle.DropDownList };
                dtpReportDate = new DateTimePicker { Location = new Point(controlX, row2), Size = new Size(200, 27), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                txtPrevReading = MakeReadOnlyTextBox(controlX, row3);
                txtCurrentReading = new TextBox { Location = new Point(controlX, row4), Size = new Size(200, 27) };
                txtTotalFromQasem = new TextBox { Location = new Point(controlX, row5), Size = new Size(200, 27), Text = "0" };

                pnlInputs.Controls.AddRange(new Control[] { cboMainMeters, dtpReportDate, txtPrevReading, txtCurrentReading, txtTotalFromQasem });

                int labelX2 = 760;
                int controlX2 = 540;

                pnlInputs.Controls.Add(MakeLabel("استهلاك الرئيسي", labelX2, row1));
                pnlInputs.Controls.Add(MakeLabel("إجمالي الفرعي", labelX2, row2));
                pnlInputs.Controls.Add(MakeLabel("الفاقد", labelX2, row3));
                pnlInputs.Controls.Add(MakeLabel("نسبة الفاقد %", labelX2, row4));
                pnlInputs.Controls.Add(MakeLabel("الملاحظات", labelX2, row5));

                txtMainDiff = MakeReadOnlyTextBox(controlX2, row1);
                txtSubTotalDiff = MakeReadOnlyTextBox(controlX2, row2);
                txtWaterLoss = MakeReadOnlyTextBox(controlX2, row3);
                txtWaterLossPercent = MakeReadOnlyTextBox(controlX2, row4);
                txtNotes = new TextBox { Location = new Point(controlX2, row5), Size = new Size(200, 27) };

                pnlInputs.Controls.AddRange(new Control[] { txtMainDiff, txtSubTotalDiff, txtWaterLoss, txtWaterLossPercent, txtNotes });

                int labelX3 = 410;
                int controlX3 = 190;

                pnlInputs.Controls.Add(MakeLabel("مبلغ الاستهلاك", labelX3, row1));
                pnlInputs.Controls.Add(MakeLabel("رسوم الخدمة", labelX3, row2));
                pnlInputs.Controls.Add(MakeLabel("الإجمالي", labelX3, row3));
                pnlInputs.Controls.Add(MakeLabel("المتأخرات", labelX3, row4));

                txtConsumptionAmount = MakeReadOnlyTextBox(controlX3, row1);
                txtServiceFees = MakeReadOnlyTextBox(controlX3, row2);
                txtTotalDue = MakeReadOnlyTextBox(controlX3, row3);
                txtArrears = MakeReadOnlyTextBox(controlX3, row4);

                pnlInputs.Controls.AddRange(new Control[] { txtConsumptionAmount, txtServiceFees, txtTotalDue, txtArrears });

                btnLoadPrevious = new Button
                {
                    Text = "تحميل السابقة",
                    Location = new Point(20, 20),
                    Size = new Size(140, 35),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnLoadPrevious.Click += BtnLoadPrevious_Click;

                btnCalculateAndSave = new Button
                {
                    Text = "حساب وحفظ التقرير",
                    Location = new Point(20, 65),
                    Size = new Size(140, 35),
                    BackColor = Color.FromArgb(0, 153, 76),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCalculateAndSave.Click += BtnCalculateAndSave_Click;

                btnRefresh = new Button
                {
                    Text = "تحديث",
                    Location = new Point(20, 110),
                    Size = new Size(140, 35),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnRefresh.Click += BtnRefresh_Click;

                btnPrint = new Button
                {
                    Text = "طباعة",
                    Location = new Point(20, 155),
                    Size = new Size(140, 35),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Enabled = false
                };
                btnPrint.Click += BtnPrint_Click;

                pnlInputs.Controls.AddRange(new Control[] { btnLoadPrevious, btnCalculateAndSave, btnRefresh, btnPrint });

                Label lblGrid = new Label
                {
                    Text = "تفاصيل العدادات الفرعية لنفس الدورة",
                    Font = new Font("Tahoma", 11F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(20, 295)
                };

                dgvSubMeters = new DataGridView
                {
                    Location = new Point(20, 325),
                    Size = new Size(1240, 320),
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

                dgvSubMeters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المشترك", DataPropertyName = "SubscriberName", FillWeight = 180 });
                dgvSubMeters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "رقم العداد", DataPropertyName = "MeterNumber", FillWeight = 120 });
                dgvSubMeters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "القراءة السابقة", DataPropertyName = "PreviousReading", FillWeight = 110 });
                dgvSubMeters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "القراءة الحالية", DataPropertyName = "CurrentReading", FillWeight = 110 });
                dgvSubMeters.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الاستهلاك", DataPropertyName = "Consumption", FillWeight = 110 });

                lblStatus = new Label
                {
                    Location = new Point(20, 655),
                    Size = new Size(1240, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                pnlInputs,
                lblGrid,
                dgvSubMeters,
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

            private void LoadMainMeters()
            {
                try
                {
                    _mainMeters = _service.GetMainMeters();
                    cboMainMeters.DataSource = _mainMeters;
                    cboMainMeters.DisplayMember = "MeterNumber";
                    cboMainMeters.ValueMember = "MeterID";

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم تحميل العدادات الرئيسية.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnLoadPrevious_Click(object sender, EventArgs e)
            {
                try
                {
                    if (!(cboMainMeters.SelectedValue is int mainMeterId) || mainMeterId <= 0)
                    {
                        MessageBox.Show("اختر العداد الرئيسي أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    txtPrevReading.Text = _service.GetLastReading(mainMeterId).ToString("N2");
                    LoadSubMetersGrid();
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم تحميل القراءة السابقة وتفاصيل العدادات الفرعية.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnCalculateAndSave_Click(object sender, EventArgs e)
            {
                try
                {
                    if (!(cboMainMeters.SelectedValue is int mainMeterId) || mainMeterId <= 0)
                    {
                        MessageBox.Show("اختر العداد الرئيسي أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!decimal.TryParse(txtCurrentReading.Text, out decimal currentReading))
                    {
                        MessageBox.Show("أدخل القراءة الحالية بشكل صحيح.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    decimal? totalFromQasem = null;
                    if (decimal.TryParse(txtTotalFromQasem.Text, out decimal qasemValue))
                        totalFromQasem = qasemValue;

                    var result = _service.SaveCycle(
                        mainMeterId,
                        dtpReportDate.Value,
                        currentReading,
                        txtNotes.Text,
                        totalFromQasem);

                    FillSummary(result);
                    LoadSubMetersGrid();
                    btnPrint.Enabled = true;

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حساب وحفظ تقرير العداد الرئيسي بنجاح.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void FillSummary(MainMeterReportResult result)
            {
                txtPrevReading.Text = result.MainMeterPrev.ToString("N2");
                txtCurrentReading.Text = result.MainMeterCurr.ToString("N2");
                txtMainDiff.Text = result.MainMeterDiff.ToString("N2");
                txtSubTotalDiff.Text = result.TotalSubMetersDiff.ToString("N2");
                txtWaterLoss.Text = result.WaterLoss.ToString("N2");
                txtWaterLossPercent.Text = result.WaterLossPercent.ToString("N2");
                txtConsumptionAmount.Text = result.TotalConsumptionAmount.ToString("N2");
                txtServiceFees.Text = result.TotalServiceFees.ToString("N2");
                txtTotalDue.Text = result.TotalDue.ToString("N2");
                txtArrears.Text = result.Arrears.ToString("N2");
            }

            private void LoadSubMetersGrid()
            {
                try
                {
                    dgvSubMeters.DataSource = _service.GetSubMeterSummary(dtpReportDate.Value);
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnRefresh_Click(object sender, EventArgs e)
            {
                txtPrevReading.Clear();
                txtCurrentReading.Clear();
                txtMainDiff.Clear();
                txtSubTotalDiff.Clear();
                txtWaterLoss.Clear();
                txtWaterLossPercent.Clear();
                txtConsumptionAmount.Clear();
                txtServiceFees.Clear();
                txtTotalDue.Clear();
                txtArrears.Clear();
                txtNotes.Clear();
                txtTotalFromQasem.Text = "0";
                dgvSubMeters.DataSource = null;
                btnPrint.Enabled = false;

                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "تمت إعادة تهيئة الشاشة.";
            }

            private void BtnPrint_Click(object sender, EventArgs e)
            {
                MessageBox.Show("يمكن لاحقًا ربط هذه الشاشة مع RDLC لطباعة تقرير الفاقد.", "طباعة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }