using System.Drawing;
using System.Windows.Forms;

namespace water3.Forms
{
    public partial class ReadingF : Form
    {
        private void ApplyTheme()
        {
            var baseFont = new Font("Tahoma", 11, FontStyle.Regular);

            lblTariff.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblTariff.ForeColor = Color.FromArgb(20, 80, 40);

            lblLastInvoice.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblLastInvoice.ForeColor = Color.FromArgb(50, 60, 70);

            lblLastReading.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblLastReading.ForeColor = Color.FromArgb(50, 60, 70);

            group.Font = new Font("Tahoma", 12, FontStyle.Bold);
            group.ForeColor = Color.FromArgb(34, 49, 89);

            txtSubscriberSearch.Font = baseFont;
            dtpReadingDate.Font = baseFont;

            txtPreviousReading.Font = baseFont;
            txtCurrentReading.Font = baseFont;
            txtConsumption.Font = baseFont;
            txtNotes.Font = baseFont;

            txtPreviousReading.BackColor = Color.Gainsboro;
            txtConsumption.BackColor = Color.Gainsboro;

            btnAdd.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnAdd.BackColor = Color.FromArgb(80, 199, 110);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;

            lblMessage.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblMessage.ForeColor = Color.DarkGreen;

            lstSubscriberSuggestions.Font = new Font("Tahoma", 11);
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
namespace water3.Forms
{


        public partial class ReadingF : Form
        {
            private void ApplyTheme()
            {
                var baseFont = new Font("Tahoma", 11, FontStyle.Regular);

                lblTariff.Font = new Font("Tahoma", 11, FontStyle.Bold);
                lblTariff.ForeColor = Color.FromArgb(20, 80, 40);

                lblLastInvoice.Font = new Font("Tahoma", 11, FontStyle.Bold);
                lblLastInvoice.ForeColor = Color.FromArgb(50, 60, 70);

                lblLastReading.Font = new Font("Tahoma", 11, FontStyle.Bold);
                lblLastReading.ForeColor = Color.FromArgb(50, 60, 70);

                group.Font = new Font("Tahoma", 12, FontStyle.Bold);
                group.ForeColor = Color.FromArgb(34, 49, 89);

                txtSubscriberSearch.Font = baseFont;
                dtpReadingDate.Font = baseFont;

                txtPreviousReading.Font = baseFont;
                txtCurrentReading.Font = baseFont;
                txtConsumption.Font = baseFont;
                txtNotes.Font = baseFont;

                txtPreviousReading.BackColor = Color.Gainsboro;
                txtConsumption.BackColor = Color.Gainsboro;

                btnAdd.Font = new Font("Tahoma", 11, FontStyle.Bold);
                btnAdd.BackColor = Color.FromArgb(80, 199, 110);
                btnAdd.ForeColor = Color.White;
                btnAdd.FlatStyle = FlatStyle.Flat;
                btnAdd.FlatAppearance.BorderSize = 0;
                btnAdd.Cursor = Cursors.Hand;

                lblMessage.Font = new Font("Tahoma", 10, FontStyle.Bold);
                lblMessage.ForeColor = Color.DarkGreen;

                lstSubscriberSuggestions.Font = new Font("Tahoma", 11);
            }
        }
    }


*/