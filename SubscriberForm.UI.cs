using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using water3.Forms;
namespace water3
{
    public partial class SubscriberForm : Form
    {

        //    public partial class SubscriberForm
        //{
        private bool _responsiveConfigured;

        public void ApplyLayoutAfterSidebarToggle()
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                ApplyResponsiveLayout();
                PerformLayout();
                Invalidate(true);
            }));
        }

        private void ApplyCustomStyles()
        {
            SetupLabel(lblName, "اسم المشترك:");
            SetupLabel(lblPhone, "رقم الهاتف:");
            SetupLabel(lblAddress, "العنوان:");
            SetupLabel(lblAccount, "حساب الذمة:");
            SetupLabel(lblMeterReading, "قراءة العداد:");
            SetupLabel(lblMeterNo, "رقم العداد:");
            SetupLabel(lblMeterLocation, "موقع العداد:");

            SetupTextBox(txtName);
            SetupTextBox(txtPhone);
            SetupTextBox(txtAddress);
            SetupTextBox(txtMeterReading);
            SetupTextBox(txtNewMeterNumber);
            SetupTextBox(txtNewMeterLocation);
            SetupTextBox(txtSearch);

            txtMeterReading.TextAlign = HorizontalAlignment.Center;
            txtSearch.Width = 280;

            SetupButton(btnAdd, "إضافة", Color.FromArgb(40, 167, 69), Color.White, 90);
            SetupButton(btnUpdate, "تعديل", Color.FromArgb(0, 123, 255), Color.White, 90);
            SetupButton(btnClear, "تفريغ", Color.FromArgb(108, 117, 125), Color.White, 90);
            SetupButton(btnCreateAccount, "إنشاء حساب ذمة", Color.FromArgb(255, 193, 7), Color.Black, 135);
            SetupButton(btnAddMeter, "إضافة عداد", Color.FromArgb(40, 167, 69), Color.White, 95);
            SetupButton(btnSetPrimary, "تعيين أساسي", Color.FromArgb(23, 162, 184), Color.White, 110);
            SetupButton(btnUnlinkMeter, "فك الربط", Color.FromArgb(220, 53, 69), Color.White, 90);
            SetupButton(btnImportData, "استيراد", Color.FromArgb(23, 162, 184), Color.White, 90);
            SetupButton(btnSaveImported, "حفظ", Color.FromArgb(0, 123, 255), Color.White, 90);
            SetupButton(btnSearch, "بحث بالاسم", Color.FromArgb(0, 123, 255), Color.White, 110);
            SetupButton(btnFilterDate, "تصفية", Color.FromArgb(23, 162, 184), Color.White, 90);

            cboAccount.Dock = DockStyle.Fill;
            cboAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAccount.FlatStyle = FlatStyle.Flat;
            cboAccount.Font = new Font("Tahoma", 9F);
            cboAccount.Margin = new Padding(6);
            cboAccount.RightToLeft = RightToLeft.Yes;

            chkIsPrimary.AutoSize = true;
            chkIsPrimary.Checked = true;
            chkIsPrimary.CheckState = CheckState.Checked;
            chkIsPrimary.Dock = DockStyle.Right;
            chkIsPrimary.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            chkIsPrimary.ForeColor = Color.FromArgb(55, 65, 81);
            chkIsPrimary.Margin = new Padding(6, 10, 6, 0);
            chkIsPrimary.Text = "عداد أساسي";

            dateFilter.Format = DateTimePickerFormat.Short;
            dateFilter.Width = 170;
            dateFilter.Font = new Font("Tahoma", 9F);
            dateFilter.Margin = new Padding(6);
        }

        private void SetupLabel(Label lbl, string text)
        {
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(55, 65, 81);
            lbl.Text = text;
            lbl.TextAlign = ContentAlignment.MiddleRight;
            lbl.Margin = new Padding(6);
        }

        private void SetupTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Dock = DockStyle.Fill;
            txt.Font = new Font("Tahoma", 9F);
            txt.Margin = new Padding(6);
            txt.TextAlign = HorizontalAlignment.Right;
        }

        private void SetupButton(Button btn, string text, Color backColor, Color foreColor, int width)
        {
            btn.Text = text;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            btn.Height = 30;
            btn.Width = width;
            btn.Margin = new Padding(6, 2, 0, 2);
            btn.UseVisualStyleBackColor = false;
        }

        private void ApplyResponsiveLayout()
        {
            if (!IsHandleCreated || pnlFieldsCard == null || pnlSearchCard == null || fieldsGrid == null || buttonsLayout == null)
                return;

            int availableWidth = pnlFieldsCard.ClientSize.Width - pnlFieldsCard.Padding.Horizontal;
            if (availableWidth <= 0)
                return;

            SuspendLayout();
            mainLayout.SuspendLayout();
            pnlFieldsCard.SuspendLayout();
            pnlSearchCard.SuspendLayout();
            fieldsCardLayout.SuspendLayout();

            try
            {
                bool narrow = availableWidth < 860;

                ConfigureFieldsGrid(narrow);
                ConfigureButtonsLayout(narrow);

                fieldsGrid.PerformLayout();
                buttonsLayout.PerformLayout();
                pnlSubscriberButtons.PerformLayout();
                pnlMeterButtons.PerformLayout();
                pnlSearch.PerformLayout();

                int fieldsHeight = narrow ? 314 : 166;
                fieldsGrid.MinimumSize = new Size(0, fieldsHeight);
                fieldsGrid.MaximumSize = new Size(0, fieldsHeight);
                fieldsGrid.Height = fieldsHeight;

                int buttonsWidth = System.Math.Max(availableWidth, 400);
                int subButtonsHeight = pnlSubscriberButtons.GetPreferredSize(new Size(buttonsWidth, 0)).Height;
                int meterButtonsHeight = pnlMeterButtons.GetPreferredSize(new Size(buttonsWidth, 0)).Height;

                int buttonsHeight = narrow
                    ? subButtonsHeight + meterButtonsHeight + 12
                    : System.Math.Max(subButtonsHeight, meterButtonsHeight);

                buttonsHeight = System.Math.Max(buttonsHeight, 38);
                buttonsLayout.MinimumSize = new Size(0, buttonsHeight);
                buttonsLayout.MaximumSize = new Size(0, buttonsHeight);
                buttonsLayout.Height = buttonsHeight;

                if (fieldsCardLayout.RowStyles.Count >= 2)
                {
                    fieldsCardLayout.RowStyles[0].SizeType = SizeType.Absolute;
                    fieldsCardLayout.RowStyles[0].Height = fieldsHeight;
                    fieldsCardLayout.RowStyles[1].SizeType = SizeType.Absolute;
                    fieldsCardLayout.RowStyles[1].Height = buttonsHeight;
                }

                int searchWidth = System.Math.Max(280, pnlSearchCard.ClientSize.Width - pnlSearchCard.Padding.Horizontal);
                int searchHeight = pnlSearch.GetPreferredSize(new Size(searchWidth, 0)).Height + pnlSearchCard.Padding.Vertical + 4;
                pnlSearchCard.Height = System.Math.Max(70, searchHeight);

                pnlFieldsCard.Height = fieldsHeight + buttonsHeight + pnlFieldsCard.Padding.Vertical + 18;

                SetSafeSplitterDistance();
                _responsiveConfigured = true;
            }
            finally
            {
                fieldsCardLayout.ResumeLayout(true);
                pnlSearchCard.ResumeLayout(true);
                pnlFieldsCard.ResumeLayout(true);
                mainLayout.ResumeLayout(true);
                ResumeLayout(true);
            }
        }

        private void ConfigureFieldsGrid(bool narrow)
        {
            fieldsGrid.SuspendLayout();

            try
            {
                fieldsGrid.Controls.Clear();
                fieldsGrid.ColumnStyles.Clear();
                fieldsGrid.RowStyles.Clear();

                if (narrow)
                {
                    fieldsGrid.ColumnCount = 2;
                    fieldsGrid.RowCount = 8;

                    fieldsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
                    fieldsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72F));

                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // اسم المشترك
                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // رقم الهاتف
                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // العنوان
                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F)); // عداد أساسي
                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // حساب الذمة
                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // قراءة العداد
                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // رقم العداد
                    fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // موقع العداد

                    fieldsGrid.Height = 314;

                    AddFieldRow(lblName, txtName, 0);
                    AddFieldRow(lblPhone, txtPhone, 1);
                    AddFieldRow(lblAddress, txtAddress, 2);

                    chkIsPrimary.Dock = DockStyle.Right;
                    chkIsPrimary.Margin = new Padding(6, 6, 6, 0);
                    fieldsGrid.Controls.Add(chkIsPrimary, 0, 3);
                    fieldsGrid.SetColumnSpan(chkIsPrimary, 2);

                    AddFieldRow(lblAccount, cboAccount, 4);
                    AddFieldRow(lblMeterReading, txtMeterReading, 5);
                    AddFieldRow(lblMeterNo, txtNewMeterNumber, 6);
                    AddFieldRow(lblMeterLocation, txtNewMeterLocation, 7);
                }
                else
                {
                    fieldsGrid.ColumnCount = 4;
                    fieldsGrid.RowCount = 4;

                    fieldsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
                    fieldsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
                    fieldsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
                    fieldsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));

                    for (int i = 0; i < 4; i++)
                        fieldsGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

                    fieldsGrid.Height = 166;

                    // العمود الأول: اسم المشترك / رقم الهاتف / العنوان / عداد أساسي
                    // العمود الثاني: حساب الذمة / قراءة العداد / رقم العداد / موقع العداد

                    fieldsGrid.Controls.Add(lblName, 0, 0);
                    fieldsGrid.Controls.Add(txtName, 1, 0);
                    fieldsGrid.Controls.Add(lblAccount, 2, 0);
                    fieldsGrid.Controls.Add(cboAccount, 3, 0);

                    fieldsGrid.Controls.Add(lblPhone, 0, 1);
                    fieldsGrid.Controls.Add(txtPhone, 1, 1);
                    fieldsGrid.Controls.Add(lblMeterReading, 2, 1);
                    fieldsGrid.Controls.Add(txtMeterReading, 3, 1);

                    fieldsGrid.Controls.Add(lblAddress, 0, 2);
                    fieldsGrid.Controls.Add(txtAddress, 1, 2);
                    fieldsGrid.Controls.Add(lblMeterNo, 2, 2);
                    fieldsGrid.Controls.Add(txtNewMeterNumber, 3, 2);

                    fieldsGrid.Controls.Add(chkIsPrimary, 0, 3);
                    fieldsGrid.SetColumnSpan(chkIsPrimary, 2);
                    chkIsPrimary.Dock = DockStyle.Right;
                    chkIsPrimary.Margin = new Padding(6, 10, 6, 0);

                    fieldsGrid.Controls.Add(lblMeterLocation, 2, 3);
                    fieldsGrid.Controls.Add(txtNewMeterLocation, 3, 3);
                }

                foreach (Control control in fieldsGrid.Controls)
                {
                    if (control is TextBox || control is ComboBox)
                    {
                        control.Dock = DockStyle.Fill;
                        control.Margin = new Padding(6);
                    }
                    else if (control is Label)
                    {
                        control.Dock = DockStyle.Fill;
                        control.Margin = new Padding(6);
                    }
                }
            }
            finally
            {
                fieldsGrid.ResumeLayout(true);
            }
        }
        private void AddFieldRow(Label label, Control editor, int rowIndex)
        {
            fieldsGrid.Controls.Add(label, 0, rowIndex);
            fieldsGrid.Controls.Add(editor, 1, rowIndex);
        }

        private void ConfigureButtonsLayout(bool narrow)
        {
            buttonsLayout.SuspendLayout();

            try
            {
                buttonsLayout.Controls.Clear();
                buttonsLayout.ColumnStyles.Clear();
                buttonsLayout.RowStyles.Clear();

                if (narrow)
                {
                    buttonsLayout.ColumnCount = 1;
                    buttonsLayout.RowCount = 2;
                    buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    buttonsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
                    buttonsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));

                    pnlSubscriberButtons.Margin = new Padding(0, 0, 0, 6);
                    pnlMeterButtons.Margin = new Padding(0);

                    buttonsLayout.Controls.Add(pnlSubscriberButtons, 0, 0);
                    buttonsLayout.Controls.Add(pnlMeterButtons, 0, 1);
                }
                else
                {
                    buttonsLayout.ColumnCount = 2;
                    buttonsLayout.RowCount = 1;
                    buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                    buttonsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));

                    pnlSubscriberButtons.Margin = new Padding(0, 0, 6, 0);
                    pnlMeterButtons.Margin = new Padding(6, 0, 0, 0);

                    buttonsLayout.Controls.Add(pnlSubscriberButtons, 0, 0);
                    buttonsLayout.Controls.Add(pnlMeterButtons, 1, 0);
                }
            }
            finally
            {
                buttonsLayout.ResumeLayout(true);
            }
        }

        private void SetSafeSplitterDistance()
        {
            if (splitGrids == null || !splitGrids.IsHandleCreated)
                return;

            int total = splitGrids.Orientation == Orientation.Horizontal
                ? splitGrids.ClientSize.Height
                : splitGrids.ClientSize.Width;

            if (total <= 0)
                return;

            int min = System.Math.Max(splitGrids.Panel1MinSize, 120);
            int max = total - System.Math.Max(splitGrids.Panel2MinSize, 100) - splitGrids.SplitterWidth;

            if (max <= min)
                return;

            int desired = _responsiveConfigured ? (int)(total * 0.62) : (int)(total * 0.65);
            desired = System.Math.Max(min, System.Math.Min(max, desired));

            if (splitGrids.SplitterDistance != desired)
                splitGrids.SplitterDistance = desired;
        }

        private void EnableDoubleBuffer()
        {
            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(mainLayout, true, null);

            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(dgvSubscribers, true, null);

            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(dgvMeters, true, null);
        }

        private void CardPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = GetRoundedRectangle(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 12))
            using (Pen pen = new Pen(Color.FromArgb(229, 231, 235), 1))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
