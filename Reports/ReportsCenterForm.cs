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
using water3.Security;
using water3.Utils;
using water3.Forms;

namespace water3.Reports
{
    //public partial class ReportsCenterForm : Form
    //{


        public partial class ReportsCenterForm : Form
        {
            public ReportsCenterForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "REPORTS_CENTER_VIEW");
            }

            private void InitializeComponent()
            {
                Text = "مركز التقارير";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                ClientSize = new Size(900, 520);

                Controls.Add(new Label
                {
                    Text = "مركز التقارير",
                    Font = new Font("Tahoma", 18F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(350, 30)
                });

                FlowLayoutPanel panel = new FlowLayoutPanel
                {
                    Location = new Point(70, 90),
                    Size = new Size(760, 360),
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = true,
                    AutoScroll = true
                };

                panel.Controls.Add(MakeTile("تقرير الفواتير", () => new InvoiceReportForm().ShowDialog(), "REPORT_INVOICES_VIEW"));
                panel.Controls.Add(MakeTile("تقرير المدفوعات", () => new PaymentsReportForm().ShowDialog(), "REPORT_PAYMENTS_VIEW"));
                panel.Controls.Add(MakeTile("تقرير الإيصالات", () => new ReceiptsReportForm().ShowDialog(), "REPORT_RECEIPTS_VIEW"));
                panel.Controls.Add(MakeTile("كشف الحساب", () => new AccountStatementReportForm().ShowDialog(), "REPORT_ACCOUNTSTATEMENT_VIEW"));
                panel.Controls.Add(MakeTile("أعمار الذمم", () => new AgingReceivablesReportForm().ShowDialog(), "REPORT_AGING_VIEW"));
                panel.Controls.Add(MakeTile("دفتر اليومية", () => new GeneralJournalReportForm().ShowDialog(), "REPORT_GENERALJOURNAL_VIEW"));
                panel.Controls.Add(MakeTile("ميزان المراجعة", () => new TrialBalanceReportForm().ShowDialog(), "REPORT_TRIALBALANCE_VIEW"));
            panel.Controls.Add(MakeTile("تحصيلات المحصلين", () => new CollectorCollectionsReportForm().ShowDialog(), "REPORT_COLLECTOR_COLLECTIONS_VIEW"));
            panel.Controls.Add(MakeTile("أجهزة المحصلين", () => new CollectorDevicesReportForm().ShowDialog(), "REPORT_COLLECTOR_DEVICES_VIEW"));
            panel.Controls.Add(MakeTile("دفعات المزامنة", () => new MobileSyncBatchesReportForm().ShowDialog(), "REPORT_MOBILE_BATCHES_VIEW"));
            panel.Controls.Add(MakeTile("أخطاء المزامنة", () => new MobileSyncErrorsReportForm().ShowDialog(), "REPORT_MOBILE_ERRORS_VIEW"));
            panel.Controls.Add(MakeTile("تخصيص المشتركين", () => new CollectorSubscribersReportForm().ShowDialog(), "REPORT_COLLECTOR_SUBSCRIBERS_VIEW"));
            panel.Controls.Add(MakeTile("التنزيل إلى الهاتف", () => new PhonePushReportForm().ShowDialog(), "REPORT_PHONE_PUSH_VIEW"));

            Controls.Add(panel);
            }

            private Control MakeTile(string text, Action action, string permissionKey)
            {
                Button btn = new Button
                {
                    Text = text,
                    Size = new Size(220, 80),
                    Margin = new Padding(15),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 11F, FontStyle.Bold)
                };

                btn.Click += (s, e) => action();
                PermissionHelper.ApplyControlPermission(btn, permissionKey);
                return btn;
            }
        }
    }