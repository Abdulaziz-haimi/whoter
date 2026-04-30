using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Reporting.WinForms;
using water3.Models.Reports;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;
using water3.Forms.Reports;
using water3.Models.Reports;
namespace water3.Services.Reports
{
  

        public class ReportPrintService
        {
            public void ShowRdlcReport(
                string title,
                string reportPath,
                IEnumerable<ReportDataSource> dataSources,
                IEnumerable<ReportParameterItem> parameters = null,
                bool useEmbeddedReport = false)
            {
                using (var frm = new Forms.Reports.ReportViewerForm(title, reportPath, dataSources, parameters, useEmbeddedReport))
                {
                    frm.ShowDialog();
                }
            }
        }
    }
