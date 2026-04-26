using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (grid == null || grid.Rows.Count == 0)
                    throw new InvalidOperationException("لا توجد بيانات للتصدير.");

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV Files (*.csv)|*.csv";
                    sfd.FileName = defaultFileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    StringBuilder sb = new StringBuilder();

                    var headers = grid.Columns.Cast<DataGridViewColumn>()
                        .Where(c => c.Visible)
                        .Select(c => Escape(c.HeaderText));
                    sb.AppendLine(string.Join(",", headers));

                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var cells = row.Cells.Cast<DataGridViewCell>()
                            .Where((c, i) => grid.Columns[i].Visible)
                            .Select(c => Escape(Convert.ToString(c.Value)));

                        sb.AppendLine(string.Join(",", cells));
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                }
            }

            private string Escape(string value)
            {
                if (value == null) value = string.Empty;
                value = value.Replace("\"", "\"\"");
                return "\"" + value + "\"";
            }
        }
    }