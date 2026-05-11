using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace water3.Utils
{
    public static class ProductionUi
    {
        public static readonly Color Bg = Color.FromArgb(245, 247, 250);
        public static readonly Color Card = Color.White;
        public static readonly Color Primary = Color.FromArgb(31, 78, 121);
        public static readonly Color Success = Color.FromArgb(25, 135, 84);
        public static readonly Color Danger = Color.FromArgb(190, 40, 40);
        public static readonly Color Warning = Color.FromArgb(180, 120, 0);
        public static readonly Color Text = Color.FromArgb(35, 35, 35);
        public static readonly Color Border = Color.FromArgb(225, 229, 235);
        public static readonly Color DarkText = Color.FromArgb(225, 229, 235);

        public static void PrepareForm(Form form, string title)
        {
            form.Text = title;
            form.BackColor = Bg;
            form.Font = new Font("Tahoma", 10F);
            form.RightToLeft = RightToLeft.Yes;
            form.RightToLeftLayout = true;
            form.AutoScaleMode = AutoScaleMode.Dpi;
        }

        public static Panel CardPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Card,
                Padding = new Padding(14),
                Margin = new Padding(8)
            };

            panel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };

            return panel;
        }

        public static Label Header(string title, string subtitle)
        {
            return new Label
            {
                Text = title + Environment.NewLine + subtitle,
                Dock = DockStyle.Top,
                Height = 58,
                Font = new Font("Tahoma", 12F, FontStyle.Bold),
                ForeColor = Primary,
                TextAlign = ContentAlignment.MiddleRight
            };
        }

        public static Label Label(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Text,
                AutoEllipsis = true
            };
        }

        public static TextBox TextBox(bool multiline = false)
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = multiline,
                Height = multiline ? 72 : 30,
                Margin = new Padding(4, 5, 4, 5),
                ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None
            };
        }

        public static ComboBox Combo()
        {
            return new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4, 5, 4, 5)
            };
        }

        public static Button Button(string text, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(6),
                Height = 38
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        public static DataGridView Grid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                MultiSelect = false
            };

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            EnableDoubleBuffer(grid);
            return grid;
        }

        public static void EnableDoubleBuffer(Control control)
        {
            try
            {
                typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(control, true, null);
            }
            catch { }
        }
    }
}
