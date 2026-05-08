using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace water3
{
    public enum AppThemeName
    {
        ClassicBlue = 0,
        Light = 1,
        Dark = 2
    }

    public class AppThemePalette
    {
        public Color Primary;
        public Color PrimaryDark;
        public Color Accent;
        public Color Hover;
        public Color Bg;
        public Color Card;
        public Color Border;
        public Color Soft;
        public Color Muted;
        public Color Text;
        public Color Selected;
        public Color SelectedText;
        public Color Success;
        public Color Danger;
        public Color Warning;

        public string DisplayName;
    }

    public static class AppThemeManager
    {
        private static readonly string Dir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "water3");

        private static readonly string FilePath =
            Path.Combine(Dir, "theme.txt");

        public static AppThemeName CurrentTheme { get; private set; } = AppThemeName.ClassicBlue;

        public static AppThemePalette Palette
        {
            get { return GetPalette(CurrentTheme); }
        }

        public static Font RegularFont
        {
            get { return new Font("Segoe UI", 10F, FontStyle.Regular); }
        }

        public static Font BoldFont
        {
            get { return new Font("Segoe UI", 10F, FontStyle.Bold); }
        }

        public static Font TitleFont
        {
            get { return new Font("Segoe UI", 13F, FontStyle.Bold); }
        }

        public static void LoadTheme()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return;

                AppThemeName t;
                if (Enum.TryParse(File.ReadAllText(FilePath).Trim(), out t))
                    CurrentTheme = t;
            }
            catch
            {
                CurrentTheme = AppThemeName.ClassicBlue;
            }
        }

        public static void SaveTheme(AppThemeName theme)
        {
            CurrentTheme = theme;

            try
            {
                if (!Directory.Exists(Dir))
                    Directory.CreateDirectory(Dir);

                File.WriteAllText(FilePath, theme.ToString());
            }
            catch
            {
            }
        }

        public static AppThemeName NextTheme()
        {
            if (CurrentTheme == AppThemeName.ClassicBlue)
                return AppThemeName.Light;

            if (CurrentTheme == AppThemeName.Light)
                return AppThemeName.Dark;

            return AppThemeName.ClassicBlue;
        }

        public static void ToggleTheme()
        {
            SaveTheme(NextTheme());
        }

        public static void ToggleThemeAndApply()
        {
            ToggleTheme();
            ApplyToOpenForms();
        }

        public static void ApplyToOpenForms()
        {
            foreach (Form form in Application.OpenForms)
            {
                ApplyTheme(form);
                form.Invalidate(true);
            }
        }

        public static void ApplyTheme(Form form)
        {
            if (form == null)
                return;

            AppThemePalette p = Palette;

            form.SuspendLayout();

            form.Font = RegularFont;
            form.BackColor = p.Bg;
            form.ForeColor = p.Text;

            // مهم جدًا:
            // يجعل النص عربي RTL، لكن يمنع قلب التصميم نفسه.
            form.RightToLeft = RightToLeft.Yes;
            form.RightToLeftLayout = false;

            ApplyControlTheme(form);

            form.ResumeLayout(true);
        }

        public static void ApplyControlTheme(Control control)
        {
            if (control == null)
                return;

            AppThemePalette p = Palette;

            if (control is Form)
            {
                control.BackColor = p.Bg;
                control.ForeColor = p.Text;
                control.Font = RegularFont;
            }
            else if (control is TableLayoutPanel)
            {
                StyleLayoutPanel(control);
            }
            else if (control is FlowLayoutPanel)
            {
                StyleFlowPanel((FlowLayoutPanel)control);
            }
            else if (control is Panel)
            {
                StylePanel(control);
            }
            else if (control is GroupBox)
            {
                StyleGroupBox((GroupBox)control);
            }
            else if (control is Label)
            {
                StyleLabel((Label)control);
            }
            else if (control is Button)
            {
                StyleButton((Button)control);
            }
            else if (control is TextBox)
            {
                StyleTextBox((TextBox)control);
            }
            else if (control is ComboBox)
            {
                StyleComboBox((ComboBox)control);
            }
            else if (control is DateTimePicker)
            {
                StyleDateTimePicker((DateTimePicker)control);
            }
            else if (control is DataGridView)
            {
                StyleGrid((DataGridView)control);
            }
            else if (control is TabControl)
            {
                StyleTabControl((TabControl)control);
            }
            else if (control is TabPage)
            {
                StyleTabPage((TabPage)control);
            }
            else if (control is CheckBox)
            {
                StyleCheckBox((CheckBox)control);
            }
            else if (control is RadioButton)
            {
                StyleRadioButton((RadioButton)control);
            }
            else if (control is ListBox)
            {
                StyleListBox((ListBox)control);
            }
            else
            {
                control.BackColor = p.Card;
                control.ForeColor = p.Text;
                control.Font = RegularFont;
            }

            foreach (Control child in control.Controls)
            {
                ApplyControlTheme(child);
            }
        }

        private static void StyleLayoutPanel(Control control)
        {
            AppThemePalette p = Palette;

            control.BackColor = p.Bg;
            control.ForeColor = p.Text;
            control.Font = RegularFont;

            // يمنع مشاكل انقلاب الأعمدة في WinForms
            control.RightToLeft = RightToLeft.No;
        }

        private static void StyleFlowPanel(FlowLayoutPanel panel)
        {
            AppThemePalette p = Palette;

            panel.BackColor = IsSidebarControl(panel) ? p.PrimaryDark : p.Bg;
            panel.ForeColor = p.Text;
            panel.Font = RegularFont;

            // نترك اتجاه العناصر حسب تصميمك، لكن بدون قلب عام
            if (IsSidebarControl(panel))
                panel.RightToLeft = RightToLeft.Yes;
            else
                panel.RightToLeft = RightToLeft.No;
        }

        private static void StylePanel(Control panel)
        {
            AppThemePalette p = Palette;

            if (IsSidebarControl(panel))
            {
                panel.BackColor = p.PrimaryDark;
                panel.ForeColor = Color.White;
            }
            else if (IsTopBarControl(panel))
            {
                panel.BackColor = p.Card;
                panel.ForeColor = p.Text;
            }
            else if (IsCardControl(panel))
            {
                panel.BackColor = p.Card;
                panel.ForeColor = p.Text;
            }
            else
            {
                panel.BackColor = p.Bg;
                panel.ForeColor = p.Text;
            }

            panel.Font = RegularFont;
        }

        private static void StyleGroupBox(GroupBox group)
        {
            AppThemePalette p = Palette;

            group.BackColor = p.Card;
            group.ForeColor = p.Text;
            group.Font = BoldFont;
            group.RightToLeft = RightToLeft.Yes;
        }

        private static void StyleLabel(Label label)
        {
            AppThemePalette p = Palette;

            string name = SafeName(label);

            label.BackColor = Color.Transparent;
            label.RightToLeft = RightToLeft.Yes;

            if (name.Contains("title") || name.Contains("hdr") || name.Contains("header"))
            {
                label.ForeColor = p.Text;
                label.Font = TitleFont;
            }
            else if (name.Contains("subtitle") || name.Contains("period") || name.Contains("note"))
            {
                label.ForeColor = p.Muted;
                label.Font = RegularFont;
            }
            else
            {
                label.ForeColor = p.Text;
                label.Font = RegularFont;
            }

            if (label.TextAlign == ContentAlignment.TopLeft ||
                label.TextAlign == ContentAlignment.MiddleLeft ||
                label.TextAlign == ContentAlignment.BottomLeft)
            {
                label.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        private static void StyleButton(Button button)
        {
            AppThemePalette p = Palette;

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Font = BoldFont;
            button.RightToLeft = RightToLeft.Yes;

            Color baseColor = GetButtonBaseColor(button);

            button.BackColor = baseColor;
            button.ForeColor = Color.White;

            button.MouseEnter -= Button_MouseEnter;
            button.MouseLeave -= Button_MouseLeave;
            button.MouseDown -= Button_MouseDown;
            button.MouseUp -= Button_MouseUp;

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
            button.MouseDown += Button_MouseDown;
            button.MouseUp += Button_MouseUp;
        }

        private static Color GetButtonBaseColor(Button button)
        {
            AppThemePalette p = Palette;
            string name = SafeName(button);
            string text = button.Text == null ? string.Empty : button.Text.ToLowerInvariant();

            if (name.Contains("delete") || name.Contains("remove") || text.Contains("حذف"))
                return p.Danger;

            if (name.Contains("save") || name.Contains("add") || text.Contains("حفظ") || text.Contains("إضافة"))
                return p.Success;

            if (name.Contains("warning") || text.Contains("تنبيه"))
                return p.Warning;

            if (IsSidebarControl(button))
                return p.PrimaryDark;

            return p.Accent;
        }

        private static void Button_MouseEnter(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            button.BackColor = Palette.Hover;
        }

        private static void Button_MouseLeave(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            button.BackColor = GetButtonBaseColor(button);
        }

        private static void Button_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            button.BackColor = ControlPaint.Dark(GetButtonBaseColor(button), 0.15f);
        }

        private static void Button_MouseUp(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            button.BackColor = GetButtonBaseColor(button);
        }

        private static void StyleTextBox(TextBox textBox)
        {
            AppThemePalette p = Palette;

            textBox.BackColor = IsDarkTheme()
                ? Color.FromArgb(15, 23, 42)
                : Color.White;

            textBox.ForeColor = p.Text;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = RegularFont;
            textBox.RightToLeft = RightToLeft.Yes;
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            AppThemePalette p = Palette;

            comboBox.BackColor = IsDarkTheme()
                ? Color.FromArgb(15, 23, 42)
                : Color.White;

            comboBox.ForeColor = p.Text;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = RegularFont;
            comboBox.RightToLeft = RightToLeft.Yes;
        }

        private static void StyleDateTimePicker(DateTimePicker dateTimePicker)
        {
            AppThemePalette p = Palette;

            dateTimePicker.BackColor = p.Card;
            dateTimePicker.ForeColor = p.Text;
            dateTimePicker.Font = RegularFont;
            dateTimePicker.RightToLeft = RightToLeft.Yes;
            dateTimePicker.RightToLeftLayout = true;
        }

        private static void StyleGrid(DataGridView grid)
        {
            AppThemePalette p = Palette;

            grid.BackgroundColor = p.Card;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = p.Border;

            grid.RowHeadersVisible = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToOrderColumns = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            grid.RightToLeft = RightToLeft.Yes;
            grid.Font = RegularFont;

            grid.ColumnHeadersHeight = 38;
            grid.RowTemplate.Height = 34;

            grid.ColumnHeadersDefaultCellStyle.BackColor = p.Soft;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = p.Text;
            grid.ColumnHeadersDefaultCellStyle.Font = BoldFont;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.DefaultCellStyle.BackColor = p.Card;
            grid.DefaultCellStyle.ForeColor = p.Text;
            grid.DefaultCellStyle.SelectionBackColor = p.Selected;
            grid.DefaultCellStyle.SelectionForeColor = p.SelectedText;
            grid.DefaultCellStyle.Font = RegularFont;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.AlternatingRowsDefaultCellStyle.BackColor = p.Soft;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = p.Text;

            try
            {
                typeof(DataGridView)
                    .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(grid, true, null);
            }
            catch
            {
            }
        }

        private static void StyleTabControl(TabControl tabs)
        {
            AppThemePalette p = Palette;

            tabs.BackColor = p.Card;
            tabs.ForeColor = p.Text;
            tabs.Font = BoldFont;
            tabs.RightToLeft = RightToLeft.Yes;

            // مهم لتفادي انعكاس التبويبات مع التصميم
            tabs.RightToLeftLayout = false;
        }

        private static void StyleTabPage(TabPage page)
        {
            AppThemePalette p = Palette;

            page.BackColor = p.Card;
            page.ForeColor = p.Text;
            page.Font = RegularFont;
            page.RightToLeft = RightToLeft.Yes;
            page.UseVisualStyleBackColor = false;
        }

        private static void StyleCheckBox(CheckBox checkBox)
        {
            AppThemePalette p = Palette;

            checkBox.BackColor = Color.Transparent;
            checkBox.ForeColor = p.Text;
            checkBox.Font = RegularFont;
            checkBox.RightToLeft = RightToLeft.Yes;
        }

        private static void StyleRadioButton(RadioButton radioButton)
        {
            AppThemePalette p = Palette;

            radioButton.BackColor = Color.Transparent;
            radioButton.ForeColor = p.Text;
            radioButton.Font = RegularFont;
            radioButton.RightToLeft = RightToLeft.Yes;
        }

        private static void StyleListBox(ListBox listBox)
        {
            AppThemePalette p = Palette;

            listBox.BackColor = p.Card;
            listBox.ForeColor = p.Text;
            listBox.Font = RegularFont;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.RightToLeft = RightToLeft.Yes;
        }

        private static bool IsDarkTheme()
        {
            return CurrentTheme == AppThemeName.Dark;
        }

        private static string SafeName(Control control)
        {
            if (control == null || control.Name == null)
                return string.Empty;

            return control.Name.ToLowerInvariant();
        }

        private static bool IsSidebarControl(Control control)
        {
            string name = SafeName(control);

            if (name.Contains("sidebar") ||
                name.Contains("side") ||
                name.Contains("menu") ||
                name.Contains("nav"))
                return true;

            if (control.Parent != null)
            {
                string parentName = SafeName(control.Parent);

                if (parentName.Contains("sidebar") ||
                    parentName.Contains("side") ||
                    parentName.Contains("menu") ||
                    parentName.Contains("nav"))
                    return true;
            }

            return false;
        }

        private static bool IsTopBarControl(Control control)
        {
            string name = SafeName(control);

            return name.Contains("top") ||
                   name.Contains("header") ||
                   name.Contains("bar") ||
                   name.Contains("toolbar");
        }

        private static bool IsCardControl(Control control)
        {
            string name = SafeName(control);

            return name.Contains("card") ||
                   name.Contains("box") ||
                   name.Contains("container") ||
                   name.Contains("wrap");
        }

        public static AppThemePalette GetPalette(AppThemeName theme)
        {
            if (theme == AppThemeName.Dark)
                return new AppThemePalette
                {
                    DisplayName = "Dark",
                    Primary = Color.FromArgb(17, 24, 39),
                    PrimaryDark = Color.FromArgb(2, 6, 23),
                    Accent = Color.FromArgb(59, 130, 246),
                    Hover = Color.FromArgb(31, 41, 55),
                    Bg = Color.FromArgb(15, 23, 42),
                    Card = Color.FromArgb(30, 41, 59),
                    Border = Color.FromArgb(51, 65, 85),
                    Soft = Color.FromArgb(51, 65, 85),
                    Muted = Color.FromArgb(203, 213, 225),
                    Text = Color.FromArgb(241, 245, 249),
                    Selected = Color.FromArgb(96, 165, 250),
                    SelectedText = Color.White,
                    Success = Color.FromArgb(34, 197, 94),
                    Danger = Color.FromArgb(248, 113, 113),
                    Warning = Color.FromArgb(251, 191, 36)
                };

            if (theme == AppThemeName.Light)
                return new AppThemePalette
                {
                    DisplayName = "Light",
                    Primary = Color.FromArgb(71, 85, 105),
                    PrimaryDark = Color.FromArgb(51, 65, 85),
                    Accent = Color.FromArgb(37, 99, 235),
                    Hover = Color.FromArgb(29, 78, 216),
                    Bg = Color.FromArgb(248, 250, 252),
                    Card = Color.White,
                    Border = Color.FromArgb(226, 232, 240),
                    Soft = Color.FromArgb(241, 245, 249),
                    Muted = Color.FromArgb(100, 116, 139),
                    Text = Color.FromArgb(15, 23, 42),
                    Selected = Color.FromArgb(191, 219, 254),
                    SelectedText = Color.FromArgb(15, 23, 42),
                    Success = Color.FromArgb(22, 163, 74),
                    Danger = Color.FromArgb(220, 38, 38),
                    Warning = Color.FromArgb(217, 119, 6)
                };

            return new AppThemePalette
            {
                DisplayName = "Classic Blue",
                Primary = Color.FromArgb(24, 74, 111),
                PrimaryDark = Color.FromArgb(15, 52, 96),
                Accent = Color.FromArgb(47, 128, 237),
                Hover = Color.FromArgb(33, 93, 140),
                Bg = Color.FromArgb(244, 247, 250),
                Card = Color.White,
                Border = Color.FromArgb(222, 229, 237),
                Soft = Color.FromArgb(248, 250, 252),
                Muted = Color.FromArgb(100, 116, 139),
                Text = Color.FromArgb(15, 23, 42),
                Selected = Color.FromArgb(220, 235, 255),
                SelectedText = Color.FromArgb(15, 23, 42),
                Success = Color.FromArgb(22, 101, 52),
                Danger = Color.FromArgb(185, 28, 28),
                Warning = Color.FromArgb(245, 158, 11)
            };
        }
    }
}