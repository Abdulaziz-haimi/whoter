using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Theming;

namespace water3.Controls
{
    public partial class GridCardControl : UserControl
    {
 
            public GridCardControl()
            {
                InitializeComponent();
                ApplyUi();
            }

            public DataGridView Grid => dgvGrid;

            public string Title
            {
                get => lblTitle.Text;
                set => lblTitle.Text = "   " + value;
            }

            public string SubTitle
            {
                get => lblSubTitle.Text;
                set => lblSubTitle.Text = value;
            }

            private void ApplyUi()
            {
                ControlStyler.StyleCard(pnlCard);
                ControlStyler.StyleGrid(dgvGrid);
                ControlStyler.EnableDoubleBuffer(dgvGrid);

                lblTitle.Font = AppTheme.HeaderFont;
                lblTitle.ForeColor = AppTheme.TextPrimary;

                lblSubTitle.Font = AppTheme.SubHeaderFont;
                lblSubTitle.ForeColor = AppTheme.TextSecondary;

                pnlSeparator.BackColor = AppTheme.CardBorderColor;
            }
        }
    }