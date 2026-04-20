using System;
using System.Windows.Forms;

namespace water3
{
    public partial class TemplatePreviewForm : Form
    {
        public TemplatePreviewForm(string name, string text)
        {
            InitializeComponent();
            Text = "معاينة: " + (name ?? "");
            txtPreview.Text = text ?? "";
        }

        public TemplatePreviewForm()
        {
            InitializeComponent();
            Text = "معاينة";
        }
    }
}
/*using System;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class TemplatePreviewForm : Form
    {
        public TemplatePreviewForm(string name, string text)
        {
            InitializeComponent();

            Text = "معاينة: " + (name ?? "");
            txtPreview.Text = text ?? "";
        }

        // (اختياري) لو تريد Constructor بدون باراميتر للمصمم فقط:
        public TemplatePreviewForm()
        {
            InitializeComponent();
            Text = "معاينة";
        }
    }
}
*/