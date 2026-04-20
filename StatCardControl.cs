using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace water3
{
    public partial class StatCardControl : UserControl
    {

            private readonly Label lblTitle;
            private readonly Label lblValue;
            private readonly Label lblSubTitle;
            private readonly Panel cardPanel;

            public StatCardControl()
            {
                cardPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(14),
                    BorderStyle = BorderStyle.FixedSingle
                };

                lblTitle = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 26,
                    Font = new Font("Tahoma", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(55, 65, 81),
                    TextAlign = ContentAlignment.MiddleRight,
                    Text = "العنوان"
                };

                lblValue = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 42,
                    Font = new Font("Tahoma", 18F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(17, 24, 39),
                    TextAlign = ContentAlignment.MiddleRight,
                    Text = "0"
                };

                lblSubTitle = new Label
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Tahoma", 9F),
                    ForeColor = Color.DimGray,
                    TextAlign = ContentAlignment.TopRight,
                    Text = string.Empty
                };

                cardPanel.Controls.Add(lblSubTitle);
                cardPanel.Controls.Add(lblValue);
                cardPanel.Controls.Add(lblTitle);
                Controls.Add(cardPanel);

                Height = 120;
                Width = 220;
                Margin = new Padding(8);
            }

            public string Title
            {
                get => lblTitle.Text;
                set => lblTitle.Text = value;
            }

            public string ValueText
            {
                get => lblValue.Text;
                set => lblValue.Text = value;
            }

            public string SubTitle
            {
                get => lblSubTitle.Text;
                set => lblSubTitle.Text = value;
            }

            public Color CardBackColor
            {
                get => cardPanel.BackColor;
                set => cardPanel.BackColor = value;
            }

            public Color ValueForeColor
            {
                get => lblValue.ForeColor;
                set => lblValue.ForeColor = value;
            }

            public void SetData(string title, string value, string subTitle = "")
            {
                Title = title;
                ValueText = value;
                SubTitle = subTitle;
            }
        }
    }