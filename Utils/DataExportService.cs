using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace water3.Utils
{
    //    class DataExportService
    //    {


    //namespace water3.Utils
    //    {
    public static class DataExportService
        {
            public static void ShowExportDialog(DataGridView grid, string defaultFileName)
            {
                if (grid == null || grid.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات للتصدير.", "تصدير", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable dt = DataTableFromGrid(grid);

                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "تصدير البيانات";
                    dialog.FileName = SafeFileName(defaultFileName) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    dialog.Filter = "Excel File (*.xls)|*.xls|CSV File (*.csv)|*.csv|PDF File (*.pdf)|*.pdf";

                    if (dialog.ShowDialog() != DialogResult.OK) return;

                    string ext = Path.GetExtension(dialog.FileName).ToLowerInvariant();

                    if (ext == ".csv")
                        ExportCsv(dt, dialog.FileName);
                    else if (ext == ".pdf")
                        ExportPdfImage(dt, dialog.FileName, defaultFileName);
                    else
                        ExportExcelHtml(dt, dialog.FileName, defaultFileName);

                    MessageBox.Show("تم التصدير بنجاح:\n" + dialog.FileName, "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            public static DataTable DataTableFromGrid(DataGridView grid)
            {
                var dt = new DataTable();

                foreach (DataGridViewColumn col in grid.Columns)
                {
                    if (!col.Visible) continue;
                    string name = string.IsNullOrWhiteSpace(col.HeaderText) ? col.Name : col.HeaderText;
                    if (dt.Columns.Contains(name)) name = name + "_" + col.Index;
                    dt.Columns.Add(name);
                }

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;

                    var values = new List<object>();
                    foreach (DataGridViewColumn col in grid.Columns)
                    {
                        if (!col.Visible) continue;
                        values.Add(row.Cells[col.Index].FormattedValue == null ? "" : row.Cells[col.Index].FormattedValue.ToString());
                    }

                    dt.Rows.Add(values.ToArray());
                }

                return dt;
            }

            public static void ExportCsv(DataTable dt, string filePath)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(Csv(dt.Columns[i].ColumnName));
                }
                sb.AppendLine();

                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append(Csv(Convert.ToString(row[i])));
                    }
                    sb.AppendLine();
                }

                File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
            }

            public static void ExportExcelHtml(DataTable dt, string filePath, string title)
            {
                var sb = new StringBuilder();

                sb.AppendLine("<html><head><meta charset='utf-8'>");
                sb.AppendLine("<style>");
                sb.AppendLine("body{font-family:Tahoma;direction:rtl}");
                sb.AppendLine("table{border-collapse:collapse;width:100%}");
                sb.AppendLine("th{background:#1f4e79;color:white;font-weight:bold}");
                sb.AppendLine("td,th{border:1px solid #999;padding:6px;text-align:center}");
                sb.AppendLine("</style></head><body>");
                sb.AppendLine("<h2>" + Html(title) + "</h2>");
                sb.AppendLine("<table><tr>");

                foreach (DataColumn col in dt.Columns)
                    sb.AppendLine("<th>" + Html(col.ColumnName) + "</th>");

                sb.AppendLine("</tr>");

                foreach (DataRow row in dt.Rows)
                {
                    sb.AppendLine("<tr>");
                    foreach (DataColumn col in dt.Columns)
                        sb.AppendLine("<td>" + Html(Convert.ToString(row[col])) + "</td>");
                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</table></body></html>");

                File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
            }

            public static void ExportPdfImage(DataTable dt, string filePath, string title)
            {
                List<byte[]> pageImages = RenderTablePagesAsJpeg(dt, title);
                WriteImagePdf(pageImages, filePath);
            }

            private static List<byte[]> RenderTablePagesAsJpeg(DataTable dt, string title)
            {
                var result = new List<byte[]>();
                int width = 1240, height = 1754, margin = 50, rowHeight = 38, headerHeight = 46, titleHeight = 70;
                int maxRows = Math.Max(1, (height - margin * 2 - titleHeight - headerHeight - 40) / rowHeight);
                int start = 0;

                do
                {
                    using (var bmp = new Bitmap(width, height))
                    using (var g = Graphics.FromImage(bmp))
                    using (var titleFont = new Font("Tahoma", 24, FontStyle.Bold))
                    using (var headerFont = new Font("Tahoma", 13, FontStyle.Bold))
                    using (var cellFont = new Font("Tahoma", 12))
                    using (var pen = new Pen(Color.FromArgb(180, 180, 180)))
                    using (var headerBrush = new SolidBrush(Color.FromArgb(31, 78, 121)))
                    using (var whiteBrush = new SolidBrush(Color.White))
                    using (var blackBrush = new SolidBrush(Color.Black))
                    {
                        g.Clear(Color.White);
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            FormatFlags = StringFormatFlags.DirectionRightToLeft
                        };

                        g.DrawString(title ?? "تقرير", titleFont, blackBrush, new RectangleF(margin, margin, width - margin * 2, titleHeight), sf);

                        int cols = Math.Max(1, dt.Columns.Count);
                        int colWidth = (width - margin * 2) / cols;
                        int y = margin + titleHeight;

                        for (int c = 0; c < cols; c++)
                        {
                            var rect = new Rectangle(margin + c * colWidth, y, colWidth, headerHeight);
                            g.FillRectangle(headerBrush, rect);
                            g.DrawRectangle(pen, rect);
                            g.DrawString(dt.Columns[c].ColumnName, headerFont, whiteBrush, rect, sf);
                        }

                        y += headerHeight;

                        int end = Math.Min(dt.Rows.Count, start + maxRows);
                        for (int r = start; r < end; r++)
                        {
                            for (int c = 0; c < cols; c++)
                            {
                                var rect = new Rectangle(margin + c * colWidth, y, colWidth, rowHeight);
                                g.DrawRectangle(pen, rect);

                                string text = Convert.ToString(dt.Rows[r][c]);
                                if (text != null && text.Length > 55) text = text.Substring(0, 55) + "...";

                                g.DrawString(text, cellFont, blackBrush, rect, sf);
                            }
                            y += rowHeight;
                        }

                        using (var ms = new MemoryStream())
                        {
                            bmp.Save(ms, ImageFormat.Jpeg);
                            result.Add(ms.ToArray());
                        }
                    }

                    start += maxRows;

                } while (start < dt.Rows.Count);

                if (result.Count == 0)
                {
                    var empty = new DataTable();
                    empty.Columns.Add("لا توجد بيانات");
                    empty.Rows.Add("لا توجد بيانات");
                    return RenderTablePagesAsJpeg(empty, title);
                }

                return result;
            }

            private static void WriteImagePdf(List<byte[]> images, string filePath)
            {
                var offsets = new List<long>();

                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    Action<string> write = delegate (string s)
                    {
                        byte[] b = Encoding.ASCII.GetBytes(s);
                        fs.Write(b, 0, b.Length);
                    };

                    write("%PDF-1.4\n");

                    int pageCount = images.Count;
                    int nextObj = 3;

                    offsets.Add(0);
                    WriteObj(fs, offsets, 1, "<< /Type /Catalog /Pages 2 0 R >>");

                    var pageObjIds = new List<int>();
                    var imageObjIds = new List<int>();
                    var contentObjIds = new List<int>();

                    for (int i = 0; i < pageCount; i++)
                    {
                        pageObjIds.Add(nextObj++);
                        imageObjIds.Add(nextObj++);
                        contentObjIds.Add(nextObj++);
                    }

                    var kids = new StringBuilder();
                    foreach (int id in pageObjIds) kids.Append(id).Append(" 0 R ");
                    WriteObj(fs, offsets, 2, "<< /Type /Pages /Kids [" + kids + "] /Count " + pageCount + " >>");

                    for (int i = 0; i < pageCount; i++)
                    {
                        int pageObj = pageObjIds[i], imageObj = imageObjIds[i], contentObj = contentObjIds[i];
                        WriteObj(fs, offsets, pageObj, "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /XObject << /Im" + i + " " + imageObj + " 0 R >> >> /Contents " + contentObj + " 0 R >>");

                        offsets.Add(fs.Position);
                        write(imageObj + " 0 obj\n");
                        write("<< /Type /XObject /Subtype /Image /Width 1240 /Height 1754 /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length " + images[i].Length + " >>\nstream\n");
                        fs.Write(images[i], 0, images[i].Length);
                        write("\nendstream\nendobj\n");

                        string content = "q\n595 0 0 842 0 0 cm\n/Im" + i + " Do\nQ\n";
                        WriteStreamObj(fs, offsets, contentObj, content);
                    }

                    long xref = fs.Position;
                    write("xref\n0 " + offsets.Count + "\n");
                    write("0000000000 65535 f \n");
                    for (int i = 1; i < offsets.Count; i++)
                        write(offsets[i].ToString("0000000000") + " 00000 n \n");

                    write("trailer\n<< /Size " + offsets.Count + " /Root 1 0 R >>\nstartxref\n" + xref + "\n%%EOF");
                }
            }

            private static void WriteObj(FileStream fs, List<long> offsets, int id, string body)
            {
                offsets.Add(fs.Position);
                string s = id + " 0 obj\n" + body + "\nendobj\n";
                byte[] b = Encoding.ASCII.GetBytes(s);
                fs.Write(b, 0, b.Length);
            }

            private static void WriteStreamObj(FileStream fs, List<long> offsets, int id, string content)
            {
                byte[] data = Encoding.ASCII.GetBytes(content);
                offsets.Add(fs.Position);
                string head = id + " 0 obj\n<< /Length " + data.Length + " >>\nstream\n";
                byte[] h = Encoding.ASCII.GetBytes(head);
                fs.Write(h, 0, h.Length);
                fs.Write(data, 0, data.Length);
                byte[] tail = Encoding.ASCII.GetBytes("\nendstream\nendobj\n");
                fs.Write(tail, 0, tail.Length);
            }

            private static string Csv(string value)
            {
                value = value ?? "";
                value = value.Replace("\"", "\"\"");
                return "\"" + value + "\"";
            }

            private static string Html(string value)
            {
                if (value == null) return "";
                return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
            }

            private static string SafeFileName(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) value = "Export";
                foreach (char c in Path.GetInvalidFileNameChars())
                    value = value.Replace(c, '_');
                return value;
            }
        }
    }


