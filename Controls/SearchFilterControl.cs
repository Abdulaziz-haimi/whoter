using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Windows.Forms;
using water3.Theming;

namespace water3.Controls
{
    public partial class SearchFilterControl : UserControl
    {
  

            private readonly ToolTip _toolTip = new ToolTip
            {
                ShowAlways = true,
                InitialDelay = 150,
                ReshowDelay = 100,
                AutoPopDelay = 5000
            };

            public SearchFilterControl()
            {
                InitializeComponent();
                ApplyUi();
            }

            public TextBox SearchTextBox => txtSearch;
            public Button SearchButton => btnSearch;
            public DateTimePicker FilterDatePicker => dateFilter;
            public Button FilterDateButton => btnFilterDate;

            private void ApplyUi()
            {
                ControlStyler.StyleCard(pnlCard);
                ControlStyler.StyleTextBox(txtSearch);
                ControlStyler.StyleDatePicker(dateFilter);

                ControlStyler.StyleActionButton(btnSearch, AppTheme.Primary);
                ControlStyler.StyleActionButton(btnFilterDate, AppTheme.Purple);

                ControlStyler.SetButtonGlyph(btnSearch, "\uE721", "بحث بالاسم", _toolTip);
                ControlStyler.SetButtonGlyph(btnFilterDate, "\uE787", "تصفية حسب التاريخ", _toolTip);

                var header = ControlStyler.BuildCardHeader(
                    "البحث والتصفية",
                    "بحث سريع بالاسم أو بتاريخ الإضافة");

                pnlCard.Controls.Add(header);
                header.BringToFront();

                pnlSearch.FlowDirection = FlowDirection.RightToLeft;
                pnlSearch.WrapContents = false;
                pnlSearch.AutoScroll = false;
                pnlSearch.Padding = new Padding(0, 8, 0, 0);

                txtSearch.Width = 340;
                txtSearch.MinimumSize = new System.Drawing.Size(340, AppTheme.InputHeight);
            }
        }
    }