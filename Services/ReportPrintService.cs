using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
namespace water3.Services
{

        public class ReportPrintService
        {
            public void ShowSimplePrintPreview(DataGridView grid, string title)
            {
                MessageBox.Show(
                    "يمكنك لاحقًا ربط هذه الشاشة مع RDLC أو FastReport أو Crystal Reports.\n\n" +
                    "حاليًا تمت تهيئة الواجهة للطباعة، وهذه نقطة الربط المناسبة لتفعيل المعاينة والطباعة.",
                    title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }