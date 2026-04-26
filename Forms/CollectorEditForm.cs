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
using System.Drawing;
using System.Windows.Forms;
using water3.Services;
namespace water3.Forms
{
    public partial class CollectorEditForm : Form
    {


        //public class CollectorEditForm : Form
        //{
        private readonly CollectorService _service = new CollectorService();
            private readonly int? _collectorId;

            private TextBox txtName;
            private TextBox txtPhone;
            private Button btnSave;
            private Button btnCancel;
            private Label lblStatus;

            public CollectorEditForm(int? collectorId = null, string collectorName = null, string phone = null)
            {
                _collectorId = collectorId;
                InitializeComponent();

                txtName.Text = collectorName ?? string.Empty;
                txtPhone.Text = phone ?? string.Empty;
            }

            private void InitializeComponent()
            {
                Text = _collectorId.HasValue ? "تعديل المحصل" : "إضافة محصل جديد";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ClientSize = new Size(460, 230);
                BackColor = Color.White;

                Controls.Add(new Label { Text = "اسم المحصل", AutoSize = true, Location = new Point(350, 40) });
                Controls.Add(new Label { Text = "رقم الهاتف", AutoSize = true, Location = new Point(350, 85) });

                txtName = new TextBox { Location = new Point(90, 35), Size = new Size(240, 27) };
                txtPhone = new TextBox { Location = new Point(90, 80), Size = new Size(240, 27) };

                btnSave = new Button
                {
                    Text = "حفظ",
                    Location = new Point(90, 130),
                    Size = new Size(110, 35),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnSave.Click += BtnSave_Click;

                btnCancel = new Button
                {
                    Text = "إلغاء",
                    Location = new Point(210, 130),
                    Size = new Size(110, 35),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancel.Click += (s, e) => Close();

                lblStatus = new Label
                {
                    Location = new Point(20, 180),
                    Size = new Size(400, 24),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkRed
                };

                Controls.AddRange(new Control[]
                {
                txtName,
                txtPhone,
                btnSave,
                btnCancel,
                lblStatus
                });
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    if (_collectorId.HasValue)
                        _service.Update(_collectorId.Value, txtName.Text, txtPhone.Text);
                    else
                        _service.Insert(txtName.Text, txtPhone.Text);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }