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
    public partial class DeviceNameEditForm : Form
    {

        //public class DeviceNameEditForm : Form
        //{
            private readonly CollectorDeviceService _service = new CollectorDeviceService();
            private readonly int _deviceId;

            private TextBox txtDeviceName;
            private Button btnSave;
            private Button btnCancel;
            private Label lblStatus;

            public DeviceNameEditForm(int deviceId, string currentDeviceName)
            {
                _deviceId = deviceId;
                InitializeComponent();
                txtDeviceName.Text = currentDeviceName ?? string.Empty;
            }

            private void InitializeComponent()
            {
                Text = "تعديل اسم الجهاز";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ClientSize = new Size(430, 180);
                BackColor = Color.White;

                Controls.Add(new Label { Text = "اسم الجهاز", AutoSize = true, Location = new Point(330, 35) });

                txtDeviceName = new TextBox { Location = new Point(60, 30), Size = new Size(250, 27) };

                btnSave = new Button
                {
                    Text = "حفظ",
                    Location = new Point(60, 80),
                    Size = new Size(100, 35),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnSave.Click += BtnSave_Click;

                btnCancel = new Button
                {
                    Text = "إلغاء",
                    Location = new Point(170, 80),
                    Size = new Size(100, 35),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancel.Click += (s, e) => Close();

                lblStatus = new Label
                {
                    Location = new Point(20, 130),
                    Size = new Size(380, 24),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkRed
                };

                Controls.AddRange(new Control[] { txtDeviceName, btnSave, btnCancel, lblStatus });
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    _service.UpdateDeviceName(_deviceId, txtDeviceName.Text);
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