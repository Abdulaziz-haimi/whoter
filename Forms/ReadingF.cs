using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using water3.Models;
using water3.Repositories;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
    public partial class ReadingF : Form
    {
        private int? SelectedSubscriberID = null;
        private int? SelectedMeterID = null;

        private DateTime? _latestReadingDate = null;
        private TariffInfo _tariff = new TariffInfo();
        private bool _suppressSearch = false;

        private readonly SubscriberMeterRepository _searchRepo = new SubscriberMeterRepository();
        private readonly TariffService _tariffService = new TariffService(new TariffRepository());
        private readonly ReadingService _readingService = new ReadingService(new ReadingsRepository());
        private readonly ReadingImportService _readingImportService = new ReadingImportService();

        private List<ReadingImportRow> _previewRows = new List<ReadingImportRow>();

        public ReadingF()
        {
            InitializeComponent();

            InitializeForm();
            ApplyTheme();
            WireEventsCore();
            WireSuggestionEvents();

            Shown += (s, e) =>
            {
                txtSubscriberSearch.Focus();
                LoadDefaultTariff();
            };
        }

        private void InitializeForm()
        {
            Text = "إدخال قراءة عداد";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;

            dtpReadingDate.Value = DateTime.Today;
        }

        private void WireEventsCore()
        {
            txtCurrentReading.KeyPress += TxtCurrentReading_KeyPress;
            txtCurrentReading.TextChanged += TxtCurrentReading_TextChanged;

            btnAdd.Click += BtnAdd_Click;
            btnImportExcel.Click += BtnImportExcel_Click;
            btnImportExcelPreview.Click += BtnImportExcelPreview_Click;
            btnConfirmImport.Click += BtnConfirmImport_Click;
            dtpReadingDate.ValueChanged += DtpReadingDate_ValueChanged;

            Resize += (s, e) => PositionSuggestionList();
            Move += (s, e) => PositionSuggestionList();
            LocationChanged += (s, e) => PositionSuggestionList();
        }

        // ===================== Tariff =====================
        private void LoadDefaultTariff()
        {
            LoadDefaultTariff(dtpReadingDate.Value.Date);
        }

        private void LoadDefaultTariff(DateTime asOfDate)
        {
            _tariff = _tariffService.LoadDefaultTariffSafe(asOfDate);

            lblTariff.Text = _tariff.UnitPrice > 0
                ? $"التعرفة: (عامة) سعر {_tariff.UnitPrice:N2} + رسوم {_tariff.ServiceFees:N2}"
                : "التعرفة: لا توجد ثوابت نشطة";
        }

        private void LoadTariffForSubscriber(int subscriberId, DateTime asOfDate)
        {
            _tariff = _tariffService.LoadTariffForSubscriberOrDefault(subscriberId, asOfDate);
            lblTariff.Text = _tariff.ToLabelText();
        }

        // ===================== Search Repo Wrapper =====================
        private List<SubscriberMeterSuggestion> SearchSubscribersAndMeters(string keyword)
            => _searchRepo.SearchSubscribersAndMeters(keyword);

        // ===================== Date Change =====================
        private void DtpReadingDate_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedSubscriberID.HasValue && SelectedMeterID.HasValue)
            {
                LoadTariffForSubscriber(SelectedSubscriberID.Value, dtpReadingDate.Value.Date);
                RefreshSelectedMeterData(clearCurrentInput: false);
            }
            else
            {
                LoadDefaultTariff();
                UpdateReadingValidationState();
            }
        }

        // ===================== Selected Meter Data =====================
        private void RefreshSelectedMeterData(bool clearCurrentInput)
        {
            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
                return;

            try
            {
                decimal prev = _readingService.GetPreviousReading(
                    SelectedSubscriberID.Value,
                    SelectedMeterID.Value,
                    dtpReadingDate.Value.Date);

                txtPreviousReading.Text = prev.ToString("0.##");

                if (clearCurrentInput)
                    txtCurrentReading.Clear();

                txtConsumption.Clear();

                var lastR = _readingService.GetLastReadingInfo(SelectedMeterID.Value);
                _latestReadingDate = lastR.ReadingDate?.Date;

                if (lastR.ReadingDate.HasValue)
                {
                    lblLastReading.Text =
                        $"آخر قراءة: {lastR.ReadingDate.Value:yyyy-MM-dd} " +
                        $"(السابق: {lastR.PreviousReading:0.##} / الحالي: {lastR.CurrentReading:0.##})";
                }
                else
                {
                    lblLastReading.Text = "آخر قراءة: لا توجد";
                }

                var lastInv = _readingService.GetLastInvoiceInfo(SelectedMeterID.Value);
                if (lastInv.InvoiceDate.HasValue)
                {
                    lblLastInvoice.Text =
                        $"آخر فاتورة: {lastInv.InvoiceDate.Value:yyyy-MM-dd} | " +
                        $"المبلغ: {lastInv.TotalAmount:0.##} | الحالة: {lastInv.Status}";
                }
                else
                {
                    lblLastInvoice.Text = "آخر فاتورة: لا توجد";
                }

                UpdateReadingValidationState();
            }
            catch (Exception ex)
            {
                _latestReadingDate = null;
                txtPreviousReading.Text = "0";
                txtConsumption.Clear();
                btnAdd.Enabled = false;

                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ تعذر تحميل بيانات العداد: " + ex.Message;
            }
        }

        // ===================== Import Excel =====================
        private void BtnImportExcel_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "اختر ملف القراءات";
                ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    Cursor = Cursors.WaitCursor;
                    btnImportExcel.Enabled = false;
                    btnAdd.Enabled = false;

                    var rows = _readingImportService.ReadFromExcel(ofd.FileName);

                    if (rows == null || rows.Count == 0)
                    {
                        MessageBox.Show("الملف لا يحتوي على بيانات.", "استيراد",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    ImportReadings(rows);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر استيراد الملف:\n" + ex.Message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btnImportExcel.Enabled = true;
                    UpdateReadingValidationState();
                }
            }
        }

        private void BtnImportExcelPreview_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "اختر ملف القراءات";
                ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                ofd.Multiselect = false;

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    Cursor = Cursors.WaitCursor;
                    btnImportExcelPreview.Enabled = false;

                    var rows = _readingImportService.ReadFromExcel(ofd.FileName);
                    DisplayExcelPreview(rows);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر فتح الملف:\n" + ex.Message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btnImportExcelPreview.Enabled = true;
                }
            }
        }

        private void DisplayExcelPreview(List<ReadingImportRow> rows)
        {
            var validRows = new List<ReadingImportRow>();
            var invalidRows = new List<ReadingImportRow>();

            foreach (var row in rows)
            {
                if (row.IsValid)
                    validRows.Add(row);
                else
                    invalidRows.Add(row);
            }

            _previewRows = validRows;

            dgvExcelPreview.AutoGenerateColumns = true;
            dgvExcelPreview.DataSource = null;
            dgvExcelPreview.DataSource = _previewRows;
            dgvExcelPreview.Visible = true;
            dgvExcelPreview.Refresh();

            btnConfirmImport.Enabled = _previewRows.Count > 0;

            if (invalidRows.Count > 0)
            {
                var lines = invalidRows
                    .Select(r => "الصف " + r.RowNumber + ": " + r.ErrorMessage)
                    .ToList();

                MessageBox.Show(
                    string.Join(Environment.NewLine, lines),
                    "أخطاء في البيانات",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            if (_previewRows.Count == 0)
            {
                MessageBox.Show(
                    "لا توجد صفوف صحيحة جاهزة للاستيراد.",
                    "المعاينة",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnConfirmImport_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnConfirmImport.Enabled = false;

                if (_previewRows == null || _previewRows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات مؤكدة للاستيراد.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ImportReadings(_previewRows);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء الاستيراد:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ImportReadings(List<ReadingImportRow> rows)
        {
            int successCount = 0;
            int failCount = 0;
            var errors = new List<string>();

            foreach (var row in rows)
            {
                try
                {
                    if (!row.IsValid)
                        throw new Exception(row.ErrorMessage ?? "الصف غير صالح");

                    ImportSingleRow(row);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    errors.Add("الصف " + row.RowNumber + ": " + ex.Message);
                }
            }

            var msg = new StringBuilder();
            msg.AppendLine("تمت العملية.");
            msg.AppendLine("الناجح: " + successCount);
            msg.AppendLine("الفاشل: " + failCount);

            if (errors.Count > 0)
            {
                msg.AppendLine();
                msg.AppendLine("تفاصيل الأخطاء:");
                foreach (var err in errors.Take(15))
                    msg.AppendLine(err);

                if (errors.Count > 15)
                    msg.AppendLine("...");
            }

            MessageBox.Show(
                msg.ToString(),
                "نتيجة الاستيراد",
                MessageBoxButtons.OK,
                failCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            dgvExcelPreview.DataSource = null;
            dgvExcelPreview.Visible = false;
            btnConfirmImport.Enabled = false;
            _previewRows.Clear();
        }

        private void ImportSingleRow(ReadingImportRow row)
        {
            if (row.ReadingDate.Date > DateTime.Today)
                throw new Exception("تاريخ القراءة مستقبلي");

            decimal previous = _readingService.GetPreviousReading(
                row.SubscriberID,
                row.MeterID,
                row.ReadingDate.Date);

            if (row.CurrentReading < previous)
                throw new Exception($"القراءة الحالية ({row.CurrentReading}) أقل من السابقة ({previous})");

            var lastInfo = _readingService.GetLastReadingInfo(row.MeterID);
            if (lastInfo.ReadingDate.HasValue && row.ReadingDate.Date <= lastInfo.ReadingDate.Value.Date)
                throw new Exception($"تاريخ القراءة يجب أن يكون بعد آخر قراءة ({lastInfo.ReadingDate.Value:yyyy-MM-dd})");

            var tariff = _tariffService.LoadTariffForSubscriberOrDefault(row.SubscriberID, row.ReadingDate.Date);

            decimal consumption = row.CurrentReading - previous;
            decimal unitPriceToUse = _tariffService.ResolveUnitPrice(tariff, consumption);
            decimal serviceFeesToUse = tariff.ServiceFees;

            if (unitPriceToUse <= 0)
                throw new Exception("لا توجد تعرفة صحيحة لهذا الصف");

            _readingService.AddReadingAndInvoice(new AddReadingRequest
            {
                SubscriberID = row.SubscriberID,
                MeterID = row.MeterID,
                ReadingDate = row.ReadingDate.Date,
                CurrentReading = row.CurrentReading,
                UnitPrice = unitPriceToUse,
                ServiceFees = serviceFeesToUse,
                Notes = row.Notes ?? string.Empty
            });
        }

        // ===================== Validation Helpers =====================
        private bool IsReadingDateValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
            {
                errorMessage = "⚠️ اختر مشترك/عداد أولاً";
                return false;
            }

            if (_latestReadingDate.HasValue && dtpReadingDate.Value.Date <= _latestReadingDate.Value.Date)
            {
                errorMessage = $"⚠️ تاريخ القراءة يجب أن يكون بعد آخر قراءة مسجلة ({_latestReadingDate.Value:yyyy-MM-dd})";
                return false;
            }

            return true;
        }

        private void UpdateReadingValidationState()
        {
            lblMessage.ForeColor = Color.Red;
            btnAdd.Enabled = false;

            if (!IsReadingDateValid(out string dateError))
            {
                txtConsumption.Clear();

                if (!string.IsNullOrWhiteSpace(txtCurrentReading.Text) || SelectedMeterID.HasValue)
                    lblMessage.Text = dateError;
                else
                    lblMessage.Text = string.Empty;

                return;
            }

            string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

            if (!NumberParser.TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            if (string.IsNullOrWhiteSpace(curText))
            {
                txtConsumption.Clear();
                lblMessage.Text = string.Empty;
                return;
            }

            if (!NumberParser.TryParseDecimal(curText, out decimal current))
            {
                txtConsumption.Clear();
                lblMessage.Text = "❌ القيمة المدخلة غير صحيحة";
                return;
            }

            if (current < previous)
            {
                txtConsumption.Clear();
                lblMessage.Text = "⚠️ القراءة الحالية أقل من السابقة";
                return;
            }

            decimal cons = current - previous;
            txtConsumption.Text = cons.ToString("N2");

            btnAdd.Enabled = true;
            lblMessage.ForeColor = Color.Green;
            lblMessage.Text = "✓ جاهز للإضافة";
        }

        // ===================== Input Validation =====================
        private void TxtCurrentReading_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            char ch = NumberParser.NormalizeArabicDigit(e.KeyChar);

            if (char.IsDigit(ch))
            {
                e.KeyChar = ch;
                return;
            }

            if (ch == '.' || ch == ',')
            {
                if (!txtCurrentReading.Text.Contains("."))
                    e.KeyChar = '.';
                else
                    e.Handled = true;

                return;
            }

            e.Handled = true;
        }

        private void TxtCurrentReading_TextChanged(object sender, EventArgs e)
        {
            UpdateReadingValidationState();
        }

        // ===================== Add Reading =====================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ اختر مشترك/عداد صحيح.";
                return;
            }

            if (!IsReadingDateValid(out string dateError))
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = dateError;
                return;
            }

            string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

            if (!NumberParser.TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            if (!NumberParser.TryParseDecimal(curText, out decimal current) || current < previous)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ القراءة الحالية غير صحيحة.";
                return;
            }

            decimal consumption = current - previous;
            decimal unitPriceToUse = _tariffService.ResolveUnitPrice(_tariff, consumption);
            decimal serviceFeesToUse = _tariff.ServiceFees;

            if (unitPriceToUse <= 0)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ لا توجد تعرفة صحيحة.";
                return;
            }

            try
            {
                _readingService.AddReadingAndInvoice(new AddReadingRequest
                {
                    SubscriberID = SelectedSubscriberID.Value,
                    MeterID = SelectedMeterID.Value,
                    ReadingDate = dtpReadingDate.Value.Date,
                    CurrentReading = current,
                    UnitPrice = unitPriceToUse,
                    ServiceFees = serviceFeesToUse,
                    Notes = txtNotes.Text
                });

                txtNotes.Clear();
                RefreshSelectedMeterData(clearCurrentInput: true);

                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "✅ تمت الإضافة وإنشاء الفاتورة بنجاح!";

                txtSubscriberSearch.Focus();
                txtSubscriberSearch.SelectAll();
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ خطأ: " + ex.Message;
            }
        }
    }
}
/*using System;
using System.Drawing;
using System.Windows.Forms;
using water3.Repositories;
using water3.Models;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
    public partial class ReadingF : Form
    {
        private int? SelectedSubscriberID = null;
        private int? SelectedMeterID = null;

        private DateTime? _latestReadingDate = null;
        private TariffInfo _tariff = new TariffInfo();
        private bool _suppressSearch = false;

        // Services
        private readonly SubscriberMeterRepository _searchRepo = new SubscriberMeterRepository();
        private readonly TariffService _tariffService = new TariffService(new TariffRepository());
        private readonly ReadingService _readingService = new ReadingService(new ReadingsRepository());

        public ReadingF()
        {
            InitializeComponent();

            InitializeForm();
            ApplyTheme();
            WireEventsCore();
            WireSuggestionEvents();

            Shown += (s, e) =>
            {
                txtSubscriberSearch.Focus();
                LoadDefaultTariff();
            };
        }

        private void InitializeForm()
        {
            Text = "إدخال قراءة عداد";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;

            dtpReadingDate.Value = DateTime.Today;
        }

        private void WireEventsCore()
        {
            txtCurrentReading.KeyPress += TxtCurrentReading_KeyPress;
            txtCurrentReading.TextChanged += TxtCurrentReading_TextChanged;

            btnAdd.Click += BtnAdd_Click;
            dtpReadingDate.ValueChanged += DtpReadingDate_ValueChanged;

            Resize += (s, e) => PositionSuggestionList();
            Move += (s, e) => PositionSuggestionList();
            LocationChanged += (s, e) => PositionSuggestionList();
        }

        // ===================== Tariff =====================
        private void LoadDefaultTariff()
        {
            LoadDefaultTariff(dtpReadingDate.Value.Date);
        }

        private void LoadDefaultTariff(DateTime asOfDate)
        {
            _tariff = _tariffService.LoadDefaultTariffSafe(asOfDate);

            lblTariff.Text = _tariff.UnitPrice > 0
                ? $"التعرفة: (عامة) سعر {_tariff.UnitPrice:N2} + رسوم {_tariff.ServiceFees:N2}"
                : "التعرفة: لا توجد ثوابت نشطة";
        }
        private void LoadTariffForSubscriber(int subscriberId, DateTime asOfDate)
        {
            _tariff = _tariffService.LoadTariffForSubscriberOrDefault(subscriberId, asOfDate);
            lblTariff.Text = _tariff.ToLabelText();
        }

        // ===================== Search Repo Wrapper =====================
        private System.Collections.Generic.List<SubscriberMeterSuggestion> SearchSubscribersAndMeters(string keyword)
            => _searchRepo.SearchSubscribersAndMeters(keyword);

        // ===================== Date Change =====================
        private void DtpReadingDate_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedSubscriberID.HasValue && SelectedMeterID.HasValue)
            {
                LoadTariffForSubscriber(SelectedSubscriberID.Value, dtpReadingDate.Value.Date);
                RefreshSelectedMeterData(clearCurrentInput: false);
            }
            else
            {
                LoadDefaultTariff();
                UpdateReadingValidationState();
            }
        }

        // ===================== Selected Meter Data =====================
        private void RefreshSelectedMeterData(bool clearCurrentInput)
        {
            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
                return;

            try
            {
                decimal prev = _readingService.GetPreviousReading(
                    SelectedSubscriberID.Value,
                    SelectedMeterID.Value,
                    dtpReadingDate.Value.Date);

                txtPreviousReading.Text = prev.ToString("0.##");

                if (clearCurrentInput)
                    txtCurrentReading.Clear();

                txtConsumption.Clear();

                var lastR = _readingService.GetLastReadingInfo(SelectedMeterID.Value);
                _latestReadingDate = lastR.ReadingDate?.Date;

                if (lastR.ReadingDate.HasValue)
                {
                    lblLastReading.Text =
                        $"آخر قراءة: {lastR.ReadingDate.Value:yyyy-MM-dd} " +
                        $"(السابق: {lastR.PreviousReading:0.##} / الحالي: {lastR.CurrentReading:0.##})";
                }
                else
                {
                    lblLastReading.Text = "آخر قراءة: لا توجد";
                }

                var lastInv = _readingService.GetLastInvoiceInfo(SelectedMeterID.Value);
                if (lastInv.InvoiceDate.HasValue)
                {
                    lblLastInvoice.Text =
                        $"آخر فاتورة: {lastInv.InvoiceDate.Value:yyyy-MM-dd} | " +
                        $"المبلغ: {lastInv.TotalAmount:0.##} | الحالة: {lastInv.Status}";
                }
                else
                {
                    lblLastInvoice.Text = "آخر فاتورة: لا توجد";
                }

                UpdateReadingValidationState();
            }
            catch (Exception ex)
            {
                _latestReadingDate = null;
                txtPreviousReading.Text = "0";
                txtConsumption.Clear();
                btnAdd.Enabled = false;

                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ تعذر تحميل بيانات العداد: " + ex.Message;
            }
        }

        // ===================== Validation Helpers =====================
        private bool IsReadingDateValid(out string errorMessage)
        {
            errorMessage = "";

            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
            {
                errorMessage = "⚠️ اختر مشترك/عداد أولاً";
                return false;
            }

            if (_latestReadingDate.HasValue && dtpReadingDate.Value.Date <= _latestReadingDate.Value.Date)
            {
                errorMessage = $"⚠️ تاريخ القراءة يجب أن يكون بعد آخر قراءة مسجلة ({_latestReadingDate.Value:yyyy-MM-dd})";
                return false;
            }

            return true;
        }

        private void UpdateReadingValidationState()
        {
            lblMessage.ForeColor = Color.Red;
            btnAdd.Enabled = false;

            if (!IsReadingDateValid(out string dateError))
            {
                txtConsumption.Clear();

                if (!string.IsNullOrWhiteSpace(txtCurrentReading.Text) || SelectedMeterID.HasValue)
                    lblMessage.Text = dateError;
                else
                    lblMessage.Text = "";

                return;
            }

            string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

            decimal previous;
            if (!NumberParser.TryParseDecimal(prevText, out previous))
                previous = 0;

            if (string.IsNullOrWhiteSpace(curText))
            {
                txtConsumption.Clear();
                lblMessage.Text = "";
                return;
            }

            decimal current;
            if (!NumberParser.TryParseDecimal(curText, out current))
            {
                txtConsumption.Clear();
                lblMessage.Text = "❌ القيمة المدخلة غير صحيحة";
                return;
            }

            if (current < previous)
            {
                txtConsumption.Clear();
                lblMessage.Text = "⚠️ القراءة الحالية أقل من السابقة";
                return;
            }

            decimal cons = current - previous;
            txtConsumption.Text = cons.ToString("N2");

            btnAdd.Enabled = true;
            lblMessage.ForeColor = Color.Green;
            lblMessage.Text = "✓ جاهز للإضافة";
        }

        private bool CheckCurrentReadingValid()
        {
            if (!IsReadingDateValid(out _))
                return false;

            string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

            decimal previous;
            if (!NumberParser.TryParseDecimal(prevText, out previous))
                previous = 0;

            decimal current;
            return NumberParser.TryParseDecimal(curText, out current) && current >= previous;
        }

        // ===================== Input Validation =====================
        private void TxtCurrentReading_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            char ch = NumberParser.NormalizeArabicDigit(e.KeyChar);

            if (char.IsDigit(ch))
            {
                e.KeyChar = ch;
                return;
            }

            if (ch == '.' || ch == ',')
            {
                if (!txtCurrentReading.Text.Contains("."))
                    e.KeyChar = '.';
                else
                    e.Handled = true;

                return;
            }

            e.Handled = true;
        }

        private void TxtCurrentReading_TextChanged(object sender, EventArgs e)
        {
            UpdateReadingValidationState();
        }

        // ===================== Add Reading =====================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ اختر مشترك/عداد صحيح.";
                return;
            }

            if (!IsReadingDateValid(out string dateError))
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = dateError;
                return;
            }

            string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

            decimal previous;
            if (!NumberParser.TryParseDecimal(prevText, out previous))
                previous = 0;

            decimal current;
            if (!NumberParser.TryParseDecimal(curText, out current) || current < previous)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ القراءة الحالية غير صحيحة.";
                return;
            }

            decimal consumption = current - previous;

            decimal unitPriceToUse = _tariffService.ResolveUnitPrice(_tariff, consumption);
            decimal serviceFeesToUse = _tariff.ServiceFees;

            if (unitPriceToUse <= 0)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ لا توجد تعرفة صحيحة.";
                return;
            }

            try
            {
                _readingService.AddReadingAndInvoice(new AddReadingRequest
                {
                    SubscriberID = SelectedSubscriberID.Value,
                    MeterID = SelectedMeterID.Value,
                    ReadingDate = dtpReadingDate.Value.Date,
                    CurrentReading = current,
                    UnitPrice = unitPriceToUse,
                    ServiceFees = serviceFeesToUse,
                    Notes = txtNotes.Text
                });

                txtNotes.Clear();
                RefreshSelectedMeterData(clearCurrentInput: true);

                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "✅ تمت الإضافة وإنشاء الفاتورة بنجاح!";

                txtSubscriberSearch.Focus();
                txtSubscriberSearch.SelectAll();
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ خطأ: " + ex.Message;
            }
        }
    }
}*/
/*using System;
using System.Drawing;
using System.Windows.Forms;
using water3.Repositories;
using water3.Models;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
public partial class ReadingF : Form
{
    private int? SelectedSubscriberID = null;
    private int? SelectedMeterID = null;

    private TariffInfo _tariff = new TariffInfo();

    private bool _suppressSearch = false;

    // Services
    private readonly SubscriberMeterRepository _searchRepo = new SubscriberMeterRepository();
    private readonly TariffService _tariffService = new TariffService(new TariffRepository());
    private readonly ReadingService _readingService = new ReadingService(new ReadingsRepository());

    public ReadingF()
    {
        InitializeComponent();

        InitializeForm();
        ApplyTheme();
        WireEventsCore();
        WireSuggestionEvents(); // Popup.cs

        Shown += (s, e) =>
        {
            txtSubscriberSearch.Focus();
            LoadDefaultTariff();
        };
    }

    private void InitializeForm()
    {
        Text = "إدخال قراءة عداد";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        BackColor = Color.White;
    }

    private void WireEventsCore()
    {
        txtCurrentReading.KeyPress += TxtCurrentReading_KeyPress;
        txtCurrentReading.TextChanged += TxtCurrentReading_TextChanged;

        btnAdd.Click += BtnAdd_Click;

        Resize += (s, e) => PositionSuggestionList();
        Move += (s, e) => PositionSuggestionList();
        LocationChanged += (s, e) => PositionSuggestionList();
        dtpReadingDate.ValueChanged += (s, e) =>
        {
            if (SelectedSubscriberID.HasValue)
                LoadTariffForSubscriber(SelectedSubscriberID.Value, dtpReadingDate.Value);
            else
                LoadDefaultTariff();
        };
    }

    // ===================== Tariff =====================
    private void LoadDefaultTariff()
    {
        _tariff = _tariffService.LoadDefaultTariffSafe();
        lblTariff.Text = _tariff.UnitPrice > 0 ? $"التعرفة: (عامة) سعر {_tariff.UnitPrice:N2} + رسوم {_tariff.ServiceFees:N2}"
                                               : "التعرفة: لا توجد ثوابت نشطة";
    }

    private void LoadTariffForSubscriber(int subscriberId, DateTime asOfDate)
    {
        _tariff = _tariffService.LoadTariffForSubscriberOrDefault(subscriberId, asOfDate);
        lblTariff.Text = _tariff.ToLabelText();
    }

    // ===================== Search Repo Wrapper =====================
    private System.Collections.Generic.List<SubscriberMeterSuggestion> SearchSubscribersAndMeters(string keyword)
        => _searchRepo.SearchSubscribersAndMeters(keyword);

    // ===================== Load Selected Meter Data =====================
    private void LoadSelectionData(int subscriberId, int meterId)
    {
        decimal prev = _readingService.GetPreviousReadingSafe(subscriberId, meterId);
        txtPreviousReading.Text = prev.ToString("0.##");

        txtCurrentReading.Text = "";
        txtConsumption.Text = "";

        var lastR = _readingService.GetLastReadingSafe(meterId);
        if (lastR.ReadingDate.HasValue)
            lblLastReading.Text = $"آخر قراءة: {lastR.ReadingDate.Value:yyyy-MM-dd} (السابق: {lastR.PreviousReading} / الحالي: {lastR.CurrentReading})";
        else
            lblLastReading.Text = "آخر قراءة: لا توجد";

        var lastInv = _readingService.GetLastInvoiceSafe(meterId);
        if (lastInv.InvoiceDate.HasValue)
            lblLastInvoice.Text = $"آخر فاتورة: {lastInv.InvoiceDate.Value:yyyy-MM-dd} | المبلغ: {lastInv.TotalAmount} | الحالة: {lastInv.Status}";
        else
            lblLastInvoice.Text = "آخر فاتورة: لا توجد";

        btnAdd.Enabled = CheckCurrentReadingValid();
    }

    // ===================== Input Validation =====================
    private void TxtCurrentReading_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (char.IsControl(e.KeyChar)) return;

        char ch = NumberParser.NormalizeArabicDigit(e.KeyChar);

        if (char.IsDigit(ch))
        {
            e.KeyChar = ch;
            return;
        }

        if (ch == '.' || ch == ',')
        {
            if (!txtCurrentReading.Text.Contains("."))
                e.KeyChar = '.';
            else
                e.Handled = true;

            return;
        }

        e.Handled = true;
    }

    private void TxtCurrentReading_TextChanged(object sender, EventArgs e)
    {
        lblMessage.ForeColor = Color.Red;
        btnAdd.Enabled = false;

        string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
        string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

        decimal previous;
        if (!NumberParser.TryParseDecimal(prevText, out previous)) previous = 0;

        decimal current;
        if (NumberParser.TryParseDecimal(curText, out current))
        {
            if (SelectedMeterID == null)
            {
                lblMessage.Text = "⚠️ اختر مشترك/عداد أولاً";
                txtConsumption.Text = "";
                return;
            }

            if (current < previous)
            {
                lblMessage.Text = "⚠️ القراءة الحالية أقل من السابقة";
                txtConsumption.Text = "";
                return;
            }

            decimal cons = current - previous;
            txtConsumption.Text = cons.ToString("N2");

            btnAdd.Enabled = SelectedMeterID.HasValue;
            if (btnAdd.Enabled)
            {
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "✓ جاهز للإضافة";
            }
        }
        else
        {
            txtConsumption.Text = "";
            if (!string.IsNullOrWhiteSpace(txtCurrentReading.Text))
                lblMessage.Text = "❌ القيمة المدخلة غير صحيحة";
        }
    }

    private bool CheckCurrentReadingValid()
    {
        if (!SelectedMeterID.HasValue) return false;

        string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
        string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

        decimal previous;
        if (!NumberParser.TryParseDecimal(prevText, out previous)) previous = 0;

        decimal current;
        return NumberParser.TryParseDecimal(curText, out current) && current >= previous;
    }

    // ===================== Add Reading =====================
    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = "❌ اختر مشترك/عداد صحيح.";
            return;
        }

        string curText = NumberParser.NormalizeNumberString(txtCurrentReading.Text);
        string prevText = NumberParser.NormalizeNumberString(txtPreviousReading.Text);

        decimal previous;
        if (!NumberParser.TryParseDecimal(prevText, out previous)) previous = 0;

        decimal current;
        if (!NumberParser.TryParseDecimal(curText, out current) || current < previous)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = "❌ القراءة الحالية غير صحيحة.";
            return;
        }

        decimal consumption = current - previous;

        decimal unitPriceToUse = _tariffService.ResolveUnitPrice(_tariff, consumption);
        decimal serviceFeesToUse = _tariff.ServiceFees;

        if (unitPriceToUse <= 0)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = "❌ لا توجد تعرفة صحيحة.";
            return;
        }

        try
        {
            _readingService.AddReadingAndInvoice(new AddReadingRequest
            {
                SubscriberID = SelectedSubscriberID.Value,
                MeterID = SelectedMeterID.Value,
                ReadingDate = dtpReadingDate.Value.Date,
                CurrentReading = current,
                UnitPrice = unitPriceToUse,
                ServiceFees = serviceFeesToUse,
                Notes = txtNotes.Text
            });

            LoadSelectionData(SelectedSubscriberID.Value, SelectedMeterID.Value);

            btnAdd.Enabled = false;
            lblMessage.ForeColor = Color.Green;
            lblMessage.Text = "✅ تمت الإضافة وإنشاء الفاتورة بنجاح!";

            // تفريغ الحقول
            txtNotes.Clear();
            txtCurrentReading.Clear();
            txtConsumption.Clear();

            txtSubscriberSearch.Focus();
            txtSubscriberSearch.SelectAll();
        }
        catch (Exception ex)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = "❌ خطأ: " + ex.Message;
        }
    }
}
}*/



/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace water3
{
    public partial class ReadingF : Form
    {
        private int? SelectedSubscriberID = null;
        private int? SelectedMeterID = null;

        // Tariff state
        private int? CurrentTariffPlanID = null;
        private string CurrentPricingModel = "DEFAULT"; // DEFAULT / Fixed / Tiered
        private decimal CurrentUnitPrice = 0;
        private decimal CurrentServiceFees = 0;

        private bool _suppressSearch = false;

        public ReadingF()
        {
            InitializeComponent();

            InitializeForm();
            ApplyTheme();
            WireEvents();
        }

        private void InitializeForm()
        {
            Text = "إدخال قراءة عداد";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;
        }

        private void ApplyTheme()
        {
            // نفس شكل كودك
            var baseFont = new Font("Tahoma", 11, FontStyle.Regular);

            lblTariff.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblTariff.ForeColor = Color.FromArgb(20, 80, 40);

            lblLastInvoice.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblLastInvoice.ForeColor = Color.FromArgb(50, 60, 70);

            lblLastReading.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblLastReading.ForeColor = Color.FromArgb(50, 60, 70);

            group.Font = new Font("Tahoma", 12, FontStyle.Bold);
            group.ForeColor = Color.FromArgb(34, 49, 89);

            txtSubscriberSearch.Font = baseFont;
            dtpReadingDate.Font = baseFont;

            txtPreviousReading.Font = baseFont;
            txtCurrentReading.Font = baseFont;
            txtConsumption.Font = baseFont;
            txtNotes.Font = baseFont;

            txtPreviousReading.BackColor = Color.Gainsboro;
            txtConsumption.BackColor = Color.Gainsboro;

            btnAdd.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnAdd.BackColor = Color.FromArgb(80, 199, 110);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;

            lblMessage.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblMessage.ForeColor = Color.DarkGreen;

            lstSubscriberSuggestions.Font = new Font("Tahoma", 11);
        }

        private void WireEvents()
        {
            txtSubscriberSearch.TextChanged += TxtSubscriberSearch_TextChanged;
            txtSubscriberSearch.KeyDown += TxtSubscriberSearch_KeyDown;
            txtSubscriberSearch.Leave += (s, e) =>
                BeginInvoke((MethodInvoker)(() =>
                {
                    if (!lstSubscriberSuggestions.Focused)
                        lstSubscriberSuggestions.Visible = false;
                }));

            lstSubscriberSuggestions.MouseClick += (s, e) => SelectSubscriberFromList();
            lstSubscriberSuggestions.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { SelectSubscriberFromList(); e.Handled = true; }
                else if (e.KeyCode == Keys.Escape) { lstSubscriberSuggestions.Visible = false; txtSubscriberSearch.Focus(); }
            };

            txtCurrentReading.KeyPress += TxtCurrentReading_KeyPress;
            txtCurrentReading.TextChanged += TxtCurrentReading_TextChanged;

            btnAdd.Click += BtnAdd_Click;

            this.Shown += (s, e) =>
            {
                txtSubscriberSearch.Focus();
                LoadDefaultTariff();
            };

            this.Resize += (s, e) => PositionSuggestionList();
            this.Move += (s, e) => PositionSuggestionList();
            this.LocationChanged += (s, e) => PositionSuggestionList();
        }

        // ===================== Tariff =====================
        private void LoadDefaultTariff()
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.GetActiveBillingConstant", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            CurrentPricingModel = "DEFAULT";
                            CurrentTariffPlanID = null;
                            CurrentUnitPrice = Convert.ToDecimal(dr["UnitPrice"]);
                            CurrentServiceFees = Convert.ToDecimal(dr["ServiceFees"]);
                            lblTariff.Text = $"التعرفة: (عامة) سعر {CurrentUnitPrice:N2} + رسوم {CurrentServiceFees:N2}";
                        }
                        else
                        {
                            CurrentUnitPrice = 0;
                            CurrentServiceFees = 0;
                            lblTariff.Text = "التعرفة: لا توجد ثوابت نشطة";
                        }
                    }
                }
            }
            catch
            {
                CurrentUnitPrice = 0;
                CurrentServiceFees = 0;
                lblTariff.Text = "التعرفة: خطأ في التحميل";
            }
        }

        private void LoadTariffForSubscriber(int subscriberId, DateTime asOfDate)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.GetTariffForSubscriber", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                    cmd.Parameters.AddWithValue("@AsOfDate", asOfDate.Date);

                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            CurrentTariffPlanID = dr["TariffPlanID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["TariffPlanID"]);
                            CurrentPricingModel = dr["PricingModel"]?.ToString() ?? "DEFAULT";
                            CurrentUnitPrice = Convert.ToDecimal(dr["UnitPrice"]);
                            CurrentServiceFees = Convert.ToDecimal(dr["ServiceFees"]);

                            string planText = CurrentTariffPlanID.HasValue ? $"خطة:{CurrentPricingModel}" : "عامة";
                            lblTariff.Text = $"التعرفة: ({planText}) سعر {CurrentUnitPrice:N2} + رسوم {CurrentServiceFees:N2}";
                        }
                        else
                        {
                            LoadDefaultTariff();
                        }
                    }
                }
            }
            catch
            {
                LoadDefaultTariff();
            }
        }

        private decimal ResolveTieredUnitPrice(int tariffPlanId, decimal consumption)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 UnitPrice
FROM TariffRates
WHERE TariffPlanID = @Plan
  AND @C >= FromQty
  AND (ToQty IS NULL OR @C <= ToQty)
ORDER BY FromQty DESC, TariffRateID DESC;", con))
            {
                cmd.Parameters.Add("@Plan", SqlDbType.Int).Value = tariffPlanId;

                var pC = cmd.Parameters.Add("@C", SqlDbType.Decimal);
                pC.Precision = 18; pC.Scale = 3;
                pC.Value = consumption;

                con.Open();
                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return CurrentUnitPrice;
                return Convert.ToDecimal(o);
            }
        }

        // ===================== Search Subscribers+Meters =====================
        private void TxtSubscriberSearch_TextChanged(object sender, EventArgs e)
        {
            if (_suppressSearch) return;

            SelectedSubscriberID = null;
            SelectedMeterID = null;

            btnAdd.Enabled = false;
            lblMessage.Text = "";
            txtPreviousReading.Text = "0";
            txtConsumption.Text = "";
            lblLastReading.Text = "آخر قراءة: ---";
            lblLastInvoice.Text = "آخر فاتورة: ---";
            LoadDefaultTariff();

            string text = (txtSubscriberSearch.Text ?? "").Trim();
            if (text.Length < 2)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            DataTable dt = SearchSubscribersAndMeters(text);
            if (dt.Rows.Count == 0)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            lstSubscriberSuggestions.DataSource = dt;
            lstSubscriberSuggestions.DisplayMember = "DisplayText";
            lstSubscriberSuggestions.ValueMember = "MeterID";

            PositionSuggestionList();
            lstSubscriberSuggestions.Visible = true;
        }

        private DataTable SearchSubscribersAndMeters(string keyword)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 30
    s.SubscriberID,
    m.MeterID,
    (CAST(s.SubscriberID AS NVARCHAR(20)) + ' - ' + s.Name
     + ' | عداد: ' + m.MeterNumber
     + CASE WHEN ISNULL(m.Location,'')<>'' THEN ' | موقع: ' + m.Location ELSE '' END
    ) AS DisplayText
FROM Subscribers s
JOIN SubscriberMeters sm ON sm.SubscriberID = s.SubscriberID
JOIN Meters m ON m.MeterID = sm.MeterID
WHERE s.IsActive = 1 AND m.IsActive = 1
  AND (
        s.Name LIKE @K
     OR m.MeterNumber LIKE @K
     OR CAST(s.SubscriberID AS NVARCHAR(20)) = @Exact
     OR CAST(m.MeterID AS NVARCHAR(20)) = @Exact
  )
ORDER BY s.Name, m.MeterNumber;", con))
            {
                cmd.Parameters.AddWithValue("@K", "%" + keyword + "%");
                cmd.Parameters.AddWithValue("@Exact", keyword);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private void TxtSubscriberSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && lstSubscriberSuggestions.Visible)
            {
                lstSubscriberSuggestions.Focus();
                if (lstSubscriberSuggestions.Items.Count > 0)
                    lstSubscriberSuggestions.SelectedIndex = 0;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (lstSubscriberSuggestions.Visible && lstSubscriberSuggestions.Items.Count > 0)
                {
                    if (lstSubscriberSuggestions.SelectedIndex < 0)
                        lstSubscriberSuggestions.SelectedIndex = 0;

                    SelectSubscriberFromList();
                }
                e.Handled = true;
            }
        }

        private void PositionSuggestionList()
        {
            if (!txtSubscriberSearch.IsHandleCreated) return;

            var p = txtSubscriberSearch.PointToScreen(Point.Empty);
            var f = PointToClient(p);

            lstSubscriberSuggestions.Width = txtSubscriberSearch.Width;
            lstSubscriberSuggestions.Left = f.X;
            lstSubscriberSuggestions.Top = f.Y + txtSubscriberSearch.Height + 2;

            lstSubscriberSuggestions.BringToFront();
        }

        private void SelectSubscriberFromList()
        {
            if (lstSubscriberSuggestions.SelectedItem == null) return;

            var drv = (DataRowView)lstSubscriberSuggestions.SelectedItem;

            int subscriberId = Convert.ToInt32(drv["SubscriberID"]);
            int meterId = Convert.ToInt32(drv["MeterID"]);
            string display = drv["DisplayText"].ToString();

            _suppressSearch = true;
            try
            {
                SelectedSubscriberID = subscriberId;
                SelectedMeterID = meterId;

                txtSubscriberSearch.Text = display;
                txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
                lstSubscriberSuggestions.Visible = false;
            }
            finally { _suppressSearch = false; }

            LoadSelectionData(subscriberId, meterId);
            LoadTariffForSubscriber(subscriberId, dtpReadingDate.Value);

            txtCurrentReading.Focus();
            txtCurrentReading.SelectAll();
        }

        // ===================== Load Selected Meter Data =====================
        private void LoadSelectionData(int subscriberId, int meterId)
        {
            LoadPreviousReading(subscriberId, meterId);
            LoadLastReadingLabel(meterId);
            LoadLastInvoiceLabel(meterId);
            btnAdd.Enabled = CheckCurrentReadingValid();
        }

        private void LoadPreviousReading(int subscriberId, int meterId)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 CurrentReading
FROM Readings
WHERE SubscriberID=@SID AND MeterID=@MID
ORDER BY ReadingDate DESC, ReadingID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@SID", subscriberId);
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    txtPreviousReading.Text = (result == null || result == DBNull.Value)
                        ? "0"
                        : Convert.ToDecimal(result).ToString("0.##");
                }

                txtCurrentReading.Text = "";
                txtConsumption.Text = "";
            }
            catch
            {
                txtPreviousReading.Text = "0";
                txtCurrentReading.Text = "";
                txtConsumption.Text = "";
            }
        }

        private void LoadLastReadingLabel(int meterId)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 ReadingDate, PreviousReading, CurrentReading
FROM Readings
WHERE MeterID=@MID
ORDER BY ReadingDate DESC, ReadingID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var prev = dr["PreviousReading"]?.ToString();
                            var curr = dr["CurrentReading"]?.ToString();
                            var date = Convert.ToDateTime(dr["ReadingDate"]).ToString("yyyy-MM-dd");
                            lblLastReading.Text = $"آخر قراءة: {date} (السابق: {prev} / الحالي: {curr})";
                        }
                        else
                            lblLastReading.Text = "آخر قراءة: لا توجد";
                    }
                }
            }
            catch { lblLastReading.Text = "آخر قراءة: خطأ في التحميل"; }
        }

        private void LoadLastInvoiceLabel(int meterId)
        {
            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 InvoiceDate, TotalAmount, Status
FROM Invoices
WHERE MeterID=@MID
ORDER BY InvoiceDate DESC, InvoiceID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string date = Convert.ToDateTime(dr["InvoiceDate"]).ToString("yyyy-MM-dd");
                            lblLastInvoice.Text = $"آخر فاتورة: {date} | المبلغ: {dr["TotalAmount"]} | الحالة: {dr["Status"]}";
                        }
                        else
                            lblLastInvoice.Text = "آخر فاتورة: لا توجد";
                    }
                }
            }
            catch { lblLastInvoice.Text = "آخر فاتورة: خطأ في التحميل"; }
        }

        // ===================== Input Validation =====================
        private void TxtCurrentReading_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            char ch = NormalizeArabicDigit(e.KeyChar);

            if (char.IsDigit(ch))
            {
                e.KeyChar = ch;
                return;
            }

            if (ch == '.' || ch == ',')
            {
                if (!txtCurrentReading.Text.Contains("."))
                    e.KeyChar = '.';
                else
                    e.Handled = true;

                return;
            }

            e.Handled = true;
        }

        private void TxtCurrentReading_TextChanged(object sender, EventArgs e)
        {
            lblMessage.ForeColor = Color.Red;
            btnAdd.Enabled = false;

            string curText = NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NormalizeNumberString(txtPreviousReading.Text);

            if (!TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            if (TryParseDecimal(curText, out decimal current))
            {
                if (SelectedMeterID == null)
                {
                    lblMessage.Text = "⚠️ اختر مشترك/عداد أولاً";
                    txtConsumption.Text = "";
                    return;
                }

                if (current < previous)
                {
                    lblMessage.Text = "⚠️ القراءة الحالية أقل من السابقة";
                    txtConsumption.Text = "";
                    return;
                }

                decimal cons = current - previous;
                txtConsumption.Text = cons.ToString("N2");

                btnAdd.Enabled = SelectedMeterID.HasValue && (current >= previous);
                if (btnAdd.Enabled)
                {
                    lblMessage.ForeColor = Color.Green;
                    lblMessage.Text = "✓ جاهز للإضافة";
                }
            }
            else
            {
                txtConsumption.Text = "";
                if (!string.IsNullOrWhiteSpace(txtCurrentReading.Text))
                    lblMessage.Text = "❌ القيمة المدخلة غير صحيحة";
            }
        }

        private bool CheckCurrentReadingValid()
        {
            if (!SelectedMeterID.HasValue) return false;

            string curText = NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NormalizeNumberString(txtPreviousReading.Text);

            if (!TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            return TryParseDecimal(curText, out decimal current) && current >= previous;
        }

        private static bool TryParseDecimal(string s, out decimal value)
        {
            return decimal.TryParse(
      s,
      NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
      CultureInfo.InvariantCulture,
      out value);
        }

        private static string NormalizeNumberString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            var sb = new StringBuilder();
            bool dotAdded = false;

            foreach (char c0 in input.Trim())
            {
                char c = NormalizeArabicDigit(c0);

                if (char.IsDigit(c))
                    sb.Append(c);
                else if ((c == '.' || c == ',') && !dotAdded)
                {
                    sb.Append('.');
                    dotAdded = true;
                }
            }

            return sb.ToString();
        }

        private static char NormalizeArabicDigit(char c)
        {
            if (c >= '٠' && c <= '٩') return (char)('0' + (c - '٠'));
            if (c >= '۰' && c <= '۹') return (char)('0' + (c - '۰'));
            return c;
        }

        // ===================== Add Reading (Meter-based) =====================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!SelectedSubscriberID.HasValue || !SelectedMeterID.HasValue)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ اختر مشترك/عداد صحيح.";
                return;
            }

            string curText = NormalizeNumberString(txtCurrentReading.Text);
            string prevText = NormalizeNumberString(txtPreviousReading.Text);

            if (!TryParseDecimal(prevText, out decimal previous))
                previous = 0;

            if (!TryParseDecimal(curText, out decimal current) || current < previous)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ القراءة الحالية غير صحيحة.";
                return;
            }

            decimal consumption = current - previous;

            decimal unitPriceToUse = CurrentUnitPrice;
            decimal serviceFeesToUse = CurrentServiceFees;

            if (CurrentTariffPlanID.HasValue &&
                string.Equals(CurrentPricingModel, "Tiered", StringComparison.OrdinalIgnoreCase))
            {
                unitPriceToUse = ResolveTieredUnitPrice(CurrentTariffPlanID.Value, consumption);
                lblTariff.Text = $"التعرفة: (Tiered) سعر الشريحة {unitPriceToUse:N2} + رسوم {serviceFeesToUse:N2}";
            }

            if (unitPriceToUse <= 0)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ لا توجد تعرفة صحيحة.";
                return;
            }

            try
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.AddReadingAndGenerateInvoice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = SelectedSubscriberID.Value;
                    cmd.Parameters.Add("@MeterID", SqlDbType.Int).Value = SelectedMeterID.Value;
                    cmd.Parameters.Add("@ReadingDate", SqlDbType.Date).Value = dtpReadingDate.Value.Date;

                    // decimals: حدّد Precision/Scale
                    var pCur = cmd.Parameters.Add("@CurrentReading", SqlDbType.Decimal);
                    pCur.Precision = 18; pCur.Scale = 3;
                    pCur.Value = current;

                    var pUnit = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                    pUnit.Precision = 18; pUnit.Scale = 3;
                    pUnit.Value = unitPriceToUse;

                    var pFees = cmd.Parameters.Add("@ServiceFees", SqlDbType.Decimal);
                    pFees.Precision = 18; pFees.Scale = 3;
                    pFees.Value = serviceFeesToUse;

                    cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim();

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadSelectionData(SelectedSubscriberID.Value, SelectedMeterID.Value);

                btnAdd.Enabled = false;
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "✅ تمت الإضافة وإنشاء الفاتورة بنجاح!";

                txtNotes.Text = "";
                txtCurrentReading.Text = "";
                txtSubscriberSearch.Focus();
                txtSubscriberSearch.SelectAll();
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "❌ خطأ: " + ex.Message;
            }
        }
    }
}
*/
