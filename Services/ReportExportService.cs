using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace water3.Services
{
    public class ReportExportService
    {
        public void ExportGridToCsv(DataGridView grid, string defaultFileName)
        {
            if (grid == null)
            {
                MessageBox.Show(
                    "جدول التقرير غير موجود.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var exportRows = grid.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow && r.Visible)
                .ToList();

            if (exportRows.Count == 0)
            {
                MessageBox.Show(
                    "لا توجد بيانات للتصدير. قم بتحميل التقرير أولاً.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            var exportColumns = grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            if (exportColumns.Count == 0)
            {
                MessageBox.Show(
                    "لا توجد أعمدة ظاهرة للتصدير.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (string.IsNullOrWhiteSpace(defaultFileName))
                defaultFileName = "Report";

            foreach (char c in Path.GetInvalidFileNameChars())
                defaultFileName = defaultFileName.Replace(c, '_');

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.Title = "تصدير التقرير";
                sfd.FileName = defaultFileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                StringBuilder sb = new StringBuilder();

                var headers = exportColumns
                    .Select(c => Escape(c.HeaderText));

                sb.AppendLine(string.Join(",", headers));

                foreach (DataGridViewRow row in exportRows)
                {
                    var cells = exportColumns
                        .Select(col => Escape(Convert.ToString(row.Cells[col.Index].Value)));

                    sb.AppendLine(string.Join(",", cells));
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));

                MessageBox.Show(
                    "تم تصدير التقرير بنجاح.",
                    "نجاح",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private string Escape(string value)
        {
            if (value == null)
                value = string.Empty;

            value = value.Replace("\"", "\"\"");

            return "\"" + value + "\"";
        }
    }
}