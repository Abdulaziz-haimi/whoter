using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Windows.Forms;
using water3.Theming;

namespace water3
{
    internal static class ControlStyler
    {
        // =========================
        // Rounded Helpers
        // =========================
        public static GraphicsPath GetRoundedPath(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        public static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.Width <= 0 || control.Height <= 0)
                return;

            using (var path = GetRoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius))
            {
                if (control.Region != null)
                    control.Region.Dispose();

                control.Region = new Region(path);
            }
        }

        public static void BindRoundedRegionOnResize(Control control, int radius)
        {
            if (control == null)
                return;

            control.Resize -= ControlRoundedResize;
            control.Resize += ControlRoundedResize;
            control.Tag = radius;
            ApplyRoundedRegion(control, radius);
        }

        private static void ControlRoundedResize(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                return;

            int radius = AppTheme.CardRadius;

            if (control.Tag is int)
                radius = (int)control.Tag;

            ApplyRoundedRegion(control, radius);
        }

        // =========================
        // Form / Card
        // =========================
        public static void StyleForm(Form form)
        {
            if (form == null)
                return;

            form.BackColor = AppTheme.FormBackColor;
            form.Font = AppTheme.DefaultFont;
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.RightToLeft = RightToLeft.Yes;
            form.RightToLeftLayout = true;

            //form.SetStyle(
            //    ControlStyles.UserPaint |
            //    ControlStyles.AllPaintingInWmPaint |
            //    ControlStyles.OptimizedDoubleBuffer |
            //    ControlStyles.ResizeRedraw, true);
        }

        public static void StyleCard(Panel panel)
        {
            if (panel == null)
                return;

            panel.BackColor = AppTheme.CardBackColor;
            panel.BorderStyle = BorderStyle.None;
            panel.Padding = new Padding(16);
            panel.Margin = new Padding(0, 0, 0, 12);

            panel.Paint -= CardPanel_Paint;
            panel.Paint += CardPanel_Paint;

            BindRoundedRegionOnResize(panel, AppTheme.CardRadius);
        }

        private static void CardPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);

            using (var path = GetRoundedPath(rect, AppTheme.CardRadius))
            using (var fillBrush = new SolidBrush(panel.BackColor))
            using (var borderPen = new Pen(AppTheme.CardBorderColor, 1f))
            {
                e.Graphics.FillPath(fillBrush, path);
                e.Graphics.DrawPath(borderPen, path);
            }
        }

        // =========================
        // Inputs
        // =========================
        public static void StyleTextBox(TextBox box, bool center = false)
        {
            if (box == null)
                return;

            box.BorderStyle = BorderStyle.FixedSingle;
            box.BackColor = Color.White;
            box.ForeColor = AppTheme.TextInput;
            box.Font = AppTheme.DefaultFont;
            box.Margin = new Padding(3, 6, 3, 6);
            box.MinimumSize = new Size(0, AppTheme.InputHeight);

            if (center)
                box.TextAlign = HorizontalAlignment.Center;
        }

        public static void StyleComboBox(ComboBox combo)
        {
            if (combo == null)
                return;

            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = Color.White;
            combo.ForeColor = AppTheme.TextInput;
            combo.Font = AppTheme.DefaultFont;
            combo.Margin = new Padding(3, 6, 3, 6);
            combo.IntegralHeight = false;
            combo.DropDownHeight = 250;
        }

        public static void StyleDatePicker(DateTimePicker picker)
        {
            if (picker == null)
                return;

            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = "yyyy/MM/dd";
            picker.Font = AppTheme.DefaultFont;
            picker.CalendarForeColor = AppTheme.TextInput;
            picker.CalendarMonthBackground = Color.White;
            picker.MinimumSize = new Size(150, AppTheme.InputHeight);
        }

        public static void StyleCheckBox(CheckBox chk)
        {
            if (chk == null)
                return;

            chk.Font = AppTheme.DefaultBoldFont;
            chk.ForeColor = Color.FromArgb(55, 65, 81);
        }

        public static void StyleLabel(Label lbl, bool bold = false, bool secondary = false)
        {
            if (lbl == null)
                return;

            lbl.AutoSize = true;
            lbl.Font = bold ? AppTheme.DefaultBoldFont : AppTheme.DefaultFont;
            lbl.ForeColor = secondary ? AppTheme.TextSecondary : AppTheme.TextPrimary;
        }

        // =========================
        // Buttons
        // =========================
        public static void StyleActionButton(Button button, Color backColor)
        {
            if (button == null)
                return;

            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.10f);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.08f);
            button.Cursor = Cursors.Hand;
            button.UseCompatibleTextRendering = false;
            button.Margin = new Padding(8, 4, 0, 4);

            button.Width = AppTheme.DefaultButtonSize;
            button.Height = AppTheme.DefaultButtonSize;
            button.MinimumSize = new Size(AppTheme.DefaultButtonSize, AppTheme.DefaultButtonSize);
            button.MaximumSize = new Size(AppTheme.DefaultButtonSize, AppTheme.DefaultButtonSize);

            button.Text = string.Empty;
            button.Padding = Padding.Empty;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.TextImageRelation = TextImageRelation.Overlay;

            BindRoundedRegionOnResize(button, AppTheme.ButtonRadius);
        }

        public static void SetButtonGlyph(Button button, string glyph, string toolTipText, ToolTip toolTip)
        {
            if (button == null)
                return;

            if (button.Image != null)
                button.Image.Dispose();

            button.Text = string.Empty;
            button.Image = CreateGlyphImage(glyph, Color.White, 16, 18, 18);
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.Padding = Padding.Empty;

            if (toolTip != null && !string.IsNullOrWhiteSpace(toolTipText))
                toolTip.SetToolTip(button, toolTipText);
        }

        public static Bitmap CreateGlyphImage(string glyph, Color color, int fontSize, int width, int height)
        {
            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            using (var brush = new SolidBrush(color))
            using (var font = new Font("Segoe MDL2 Assets", fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new RectangleF(0, 0, width, height);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(glyph, font, brush, rect, sf);
            }

            return bitmap;
        }

        // =========================
        // Grids
        // =========================
        public static void StyleGrid(DataGridView grid)
        {
            if (grid == null)
                return;

            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = AppTheme.GridLineColor;

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeaderBackColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderForeColor;
            grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.GridHeaderFont;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = AppTheme.GridHeaderHeight;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            grid.DefaultCellStyle.SelectionBackColor = AppTheme.GridSelectionBackColor;
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.DefaultCellStyle.Padding = new Padding(5);
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridAltRowColor;

            grid.RowTemplate.Height = AppTheme.GridRowHeight;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        public static void EnableDoubleBuffer(Control control)
        {
            if (control == null)
                return;

            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(control, true, null);
        }

        // =========================
        // Card Headers
        // =========================
        public static Panel BuildCardHeader(string title, string subTitle)
        {
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46,
                BackColor = Color.White,
                Padding = new Padding(2, 0, 2, 8)
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Text = title,
                Font = AppTheme.HeaderFont,
                ForeColor = AppTheme.TextPrimary,
                TextAlign = ContentAlignment.MiddleRight
            };

            var lblSub = new Label
            {
                Dock = DockStyle.Top,
                Height = 18,
                Text = subTitle,
                Font = AppTheme.SubHeaderFont,
                ForeColor = AppTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleRight
            };

            header.Controls.Add(lblSub);
            header.Controls.Add(lblTitle);

            return header;
        }
    }
}