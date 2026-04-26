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
using water3.Utils;
namespace water3.Reports
{
    public partial class BaseReportForm : Form
    {
  
        //public class BaseReportForm : Form
        //{
            protected readonly ReportsService ReportsService = new ReportsService();
            protected readonly ReportExportService ExportService = new ReportExportService();
            protected readonly ReportPrintService PrintService = new ReportPrintService();

            protected Panel pnlFilters;
            protected Panel pnlActions;
            protected DataGridView dgvReport;
            protected Label lblTitle;
            protected Label lblStatus;
            protected Button btnView;
            protected Button btnRefresh;
            protected Button btnExport;
            protected Button btnPrint;

            public BaseReportForm()
            {
                InitializeBaseUi();
            }

            private void InitializeBaseUi()
            {
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                WindowState = FormWindowState.Maximized;

                lblTitle = new Label
                {
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                pnlFilters = new Panel
                {
                    Location = new Point(20, 55),
                    Size = new Size(1260, 85),
                    BackColor = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle
                };

                pnlActions = new Panel
                {
                    Location = new Point(20, 145),
                    Size = new Size(1260, 45),
                    BackColor = Color.White
                };

                btnView = MakeButton("عرض", 20, 5, 100, Color.FromArgb(0, 122, 204));
                btnRefresh = MakeButton("تحديث", 130, 5, 100, Color.Gray);
                btnExport = MakeButton("تصدير", 240, 5, 100, Color.SteelBlue);
                btnPrint = MakeButton("طباعة", 350, 5, 100, Color.FromArgb(0, 153, 76));

                btnView.Click += (s, e) => LoadReportData();
                btnRefresh.Click += (s, e) => RefreshReport();
                btnExport.Click += (s, e) => ExportCurrentReport();
                btnPrint.Click += (s, e) => PrintCurrentReport();

                pnlActions.Controls.AddRange(new Control[] { btnView, btnRefresh, btnExport, btnPrint });

                dgvReport = new DataGridView
                {
                    Location = new Point(20, 200),
                    Size = new Size(1260, 470),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                lblStatus = new Label
                {
                    Location = new Point(20, 680),
                    Size = new Size(1260, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[] { lblTitle, pnlFilters, pnlActions, dgvReport, lblStatus });
            }

            protected Button MakeButton(string text, int left, int top, int width, Color color)
            {
                return new Button
                {
                    Text = text,
                    Location = new Point(left, top),
                    Size = new Size(width, 32),
                    BackColor = color,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
            }

            protected Label MakeFilterLabel(string text, int left, int top)
            {
                return new Label
                {
                    Text = text,
                    AutoSize = true,
                    Location = new Point(left, top + 5)
                };
            }

            protected void ApplyReportPermission(string formPermissionKey)
            {
                PermissionHelper.EnforceFormPermission(this, formPermissionKey);
                PermissionHelper.ApplyControlPermission(btnExport, "REPORT_EXPORT");
                PermissionHelper.ApplyControlPermission(btnPrint, "REPORT_PRINT");
            }

            protected virtual void LoadReportData() { }
            protected virtual void RefreshReport() { }
            protected virtual void ExportCurrentReport() => ExportService.ExportGridToCsv(dgvReport, Text);
            protected virtual void PrintCurrentReport() => PrintService.ShowSimplePrintPreview(dgvReport, Text);
        }
    }