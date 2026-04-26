using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Models;
namespace water3.Reports
{
    //public partial class AccountStatementReportForm : Form
    //{
    
        public partial class AccountStatementReportForm : BaseReportForm
        {
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private ComboBox cboSubscribers;
            private List<SubscriberLookupItem> _subs;

            public AccountStatementReportForm()
            {
                Text = "كشف حساب المشترك";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_ACCOUNTSTATEMENT_VIEW");
                BuildFilters();
                LoadSubscribers();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("من تاريخ", 1120, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("إلى تاريخ", 850, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("المشترك", 580, 20));

                dtFrom = new DateTimePicker { Location = new System.Drawing.Point(950, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
                dtTo = new DateTimePicker { Location = new System.Drawing.Point(680, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                cboSubscribers = new ComboBox { Location = new System.Drawing.Point(260, 18), Size = new System.Drawing.Size(300, 28), DropDownStyle = ComboBoxStyle.DropDownList };

                pnlFilters.Controls.AddRange(new Control[] { dtFrom, dtTo, cboSubscribers });
            }

            private void LoadSubscribers()
            {
                _subs = ReportsService.GetSubscribers();
                cboSubscribers.DataSource = _subs;
                cboSubscribers.DisplayMember = "Name";
                cboSubscribers.ValueMember = "SubscriberID";
            }

            protected override void LoadReportData()
            {
                try
                {
                    if (!(cboSubscribers.SelectedValue is int subId) || subId <= 0)
                    {
                        lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                        lblStatus.Text = "اختر المشترك أولًا.";
                        return;
                    }

                    dgvReport.DataSource = ReportsService.GetAccountStatement(dtFrom.Value, dtTo.Value, subId);
                    lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                    lblStatus.Text = "تم تحميل كشف الحساب.";
                    ReportsService.LogReportOpen("AccountStatementReport", $"SubscriberID={subId}");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            protected override void RefreshReport()
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;
                dgvReport.DataSource = null;
                lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                lblStatus.Text = "تمت إعادة التهيئة.";
            }
        }
    }