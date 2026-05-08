using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace water3.Reports.Dynamic
{
    public class DynamicGridPrintService
    {
        private DataGridView _grid;
        private string _title;
        private int _rowIndex;
        private int _pageNo;
        private List<DataGridViewColumn> _columns;

        private PrintDocument _document;
        private PrintPreviewDialog _preview;

        public void PrintPreview(DataGridView grid, string title)
        {
            if (grid == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _grid = grid;
            _title = string.IsNullOrWhiteSpace(title) ? "تقرير" : title;
            _rowIndex = 0;
            _pageNo = 1;

            _columns = grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            if (_columns.Count == 0)
            {
                MessageBox.Show("لا توجد أعمدة ظاهرة للطباعة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _document = new PrintDocument();
            _document.DefaultPageSettings.Landscape = true;
            _document.PrintPage += Document_PrintPage;

            _preview = new PrintPreviewDialog();
            _preview.Document = _document;
            _preview.WindowState = FormWindowState.Maximized;
            _preview.ShowDialog();
        }

        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.MarginBounds;

            using (Font titleFont = new Font("Tahoma", 14, FontStyle.Bold))
            using (Font headerFont = new Font("Tahoma", 8, FontStyle.Bold))
            using (Font cellFont = new Font("Tahoma", 8))
            using (Font smallFont = new Font("Tahoma", 8))
            using (Pen pen = new Pen(Color.Black))
            {
                int y = bounds.Top;

                g.DrawString(_title, titleFont, Brushes.Black, bounds.Left, y);
                y += 30;

                g.DrawString("تاريخ الطباعة: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    smallFont, Brushes.Black, bounds.Left, y);

                g.DrawString("صفحة: " + _pageNo,
                    smallFont, Brushes.Black, bounds.Right - 80, y);

                y += 25;

                int rowHeight = 28;
                int headerHeight = 32;

                int[] widths = CalculateColumnWidths(bounds.Width);

                int x = bounds.Left;

                for (int i = 0; i < _columns.Count; i++)
                {
                    Rectangle rect = new Rectangle(x, y, widths[i], headerHeight);
                    g.FillRectangle(Brushes.LightGray, rect);
                    g.DrawRectangle(pen, rect);
                    DrawCentered(g, _columns[i].HeaderText, headerFont, rect);
                    x += widths[i];
                }

                y += headerHeight;

                while (_rowIndex < _grid.Rows.Count)
                {
                    DataGridViewRow row = _grid.Rows[_rowIndex];

                    if (row.IsNewRow || !row.Visible)
                    {
                        _rowIndex++;
                        continue;
                    }

                    if (y + rowHeight > bounds.Bottom)
                    {
                        e.HasMorePages = true;
                        _pageNo++;
                        return;
                    }

                    x = bounds.Left;

                    for (int i = 0; i < _columns.Count; i++)
                    {
                        object value = row.Cells[_columns[i].Index].Value;
                        string text = value == null ? "" : Convert.ToString(value);

                        Rectangle rect = new Rectangle(x, y, widths[i], rowHeight);
                        g.DrawRectangle(pen, rect);
                        DrawCell(g, text, cellFont, rect);

                        x += widths[i];
                    }

                    y += rowHeight;
                    _rowIndex++;
                }

                e.HasMorePages = false;
                _rowIndex = 0;
                _pageNo = 1;
            }
        }

        private int[] CalculateColumnWidths(int totalWidth)
        {
            int count = _columns.Count;
            int[] widths = new int[count];

            if (count == 0)
                return widths;

            int baseWidth = totalWidth / count;
            int used = 0;

            for (int i = 0; i < count; i++)
            {
                widths[i] = baseWidth;
                used += widths[i];
            }

            if (used < totalWidth)
                widths[count - 1] += totalWidth - used;

            return widths;
        }

        private void DrawCentered(Graphics g, string text, Font font, Rectangle rect)
        {
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                g.DrawString(text ?? "", font, Brushes.Black, rect, sf);
            }
        }

        private void DrawCell(Graphics g, string text, Font font, Rectangle rect)
        {
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;

                Rectangle padded = new Rectangle(rect.X + 3, rect.Y, rect.Width - 6, rect.Height);
                g.DrawString(text ?? "", font, Brushes.Black, padded, sf);
            }
        }
    }
}