using System.Windows.Forms;
using System.Drawing;

namespace water3.Forms
{
    partial class ReadingEntryForm
    {


            private System.ComponentModel.IContainer components = null;

            private TableLayoutPanel rootLayout;

            private Panel pnlHeaderCard;
            private Label lblTitle;
            private Label lblSubtitle;

            private Panel pnlFiltersCard;
            private TableLayoutPanel filtersLayout;
            private Label lblArea;
            private ComboBox cmbArea;
            private Label lblReadingDate;
            private DateTimePicker dtpReadingDate;
            private Label lblSearch;
            private TextBox txtSearch;
            private FlowLayoutPanel pnlButtons;
            private Button btnLoad;
            private Button btnSaveAll;
            private Button btnPrintSheet;
            private Button btnRefresh;

            private Panel pnlGridCard;
            private Label lblGridTitle;
            private DataGridView dgvReadings;

            private TableLayoutPanel statsLayout;
            private Panel pnlTotalCard;
            private Panel pnlDoneCard;
            private Panel pnlPendingCard;
            private Label lblTotalCaption;
            private Label lblTotalValue;
            private Label lblDoneCaption;
            private Label lblDoneValue;
            private Label lblPendingCaption;
            private Label lblPendingValue;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                    components.Dispose();

                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeaderCard = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlFiltersCard = new System.Windows.Forms.Panel();
            this.filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblArea = new System.Windows.Forms.Label();
            this.cmbArea = new System.Windows.Forms.ComboBox();
            this.lblReadingDate = new System.Windows.Forms.Label();
            this.dtpReadingDate = new System.Windows.Forms.DateTimePicker();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnPrintSheet = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlGridCard = new System.Windows.Forms.Panel();
            this.dgvReadings = new System.Windows.Forms.DataGridView();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.statsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTotalCard = new System.Windows.Forms.Panel();
            this.lblTotalCaption = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.pnlDoneCard = new System.Windows.Forms.Panel();
            this.lblDoneCaption = new System.Windows.Forms.Label();
            this.lblDoneValue = new System.Windows.Forms.Label();
            this.pnlPendingCard = new System.Windows.Forms.Panel();
            this.lblPendingCaption = new System.Windows.Forms.Label();
            this.lblPendingValue = new System.Windows.Forms.Label();
            this.rootLayout.SuspendLayout();
            this.pnlHeaderCard.SuspendLayout();
            this.pnlFiltersCard.SuspendLayout();
            this.filtersLayout.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlGridCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReadings)).BeginInit();
            this.statsLayout.SuspendLayout();
            this.pnlTotalCard.SuspendLayout();
            this.pnlDoneCard.SuspendLayout();
            this.pnlPendingCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.pnlHeaderCard, 0, 0);
            this.rootLayout.Controls.Add(this.pnlFiltersCard, 0, 1);
            this.rootLayout.Controls.Add(this.pnlGridCard, 0, 2);
            this.rootLayout.Controls.Add(this.statsLayout, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(16);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 164F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.rootLayout.Size = new System.Drawing.Size(1370, 749);
            this.rootLayout.TabIndex = 0;
            // 
            // pnlHeaderCard
            // 
            this.pnlHeaderCard.Controls.Add(this.lblTitle);
            this.pnlHeaderCard.Controls.Add(this.lblSubtitle);
            this.pnlHeaderCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeaderCard.Location = new System.Drawing.Point(19, 19);
            this.pnlHeaderCard.Name = "pnlHeaderCard";
            this.pnlHeaderCard.Padding = new System.Windows.Forms.Padding(20, 14, 20, 14);
            this.pnlHeaderCard.Size = new System.Drawing.Size(1332, 75);
            this.pnlHeaderCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(482, 33);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "كشف متابعة تسجيل القراءات الجديدة";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Tahoma", 10F);
            this.lblSubtitle.Location = new System.Drawing.Point(0, 50);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(427, 17);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "تحميل جميع العدادات ثم تسجيل القراءة الجديدة لكل عداد بشكل جماعي";
            // 
            // pnlFiltersCard
            // 
            this.pnlFiltersCard.Controls.Add(this.filtersLayout);
            this.pnlFiltersCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFiltersCard.Location = new System.Drawing.Point(19, 100);
            this.pnlFiltersCard.Name = "pnlFiltersCard";
            this.pnlFiltersCard.Padding = new System.Windows.Forms.Padding(16);
            this.pnlFiltersCard.Size = new System.Drawing.Size(1332, 158);
            this.pnlFiltersCard.TabIndex = 1;
            // 
            // filtersLayout
            // 
            this.filtersLayout.ColumnCount = 6;
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filtersLayout.Controls.Add(this.lblArea, 0, 0);
            this.filtersLayout.Controls.Add(this.cmbArea, 1, 0);
            this.filtersLayout.Controls.Add(this.lblReadingDate, 2, 0);
            this.filtersLayout.Controls.Add(this.dtpReadingDate, 3, 0);
            this.filtersLayout.Controls.Add(this.lblSearch, 0, 1);
            this.filtersLayout.Controls.Add(this.txtSearch, 1, 1);
            this.filtersLayout.Controls.Add(this.pnlButtons, 1, 2);
            this.filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersLayout.Location = new System.Drawing.Point(16, 16);
            this.filtersLayout.Name = "filtersLayout";
            this.filtersLayout.RowCount = 3;
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.filtersLayout.Size = new System.Drawing.Size(1300, 126);
            this.filtersLayout.TabIndex = 0;
            // 
            // lblArea
            // 
            this.lblArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblArea.Location = new System.Drawing.Point(1213, 0);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(84, 28);
            this.lblArea.TabIndex = 0;
            this.lblArea.Text = "المنطقة";
            this.lblArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbArea
            // 
            this.cmbArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArea.Location = new System.Drawing.Point(973, 3);
            this.cmbArea.Name = "cmbArea";
            this.cmbArea.Size = new System.Drawing.Size(234, 24);
            this.cmbArea.TabIndex = 1;
            // 
            // lblReadingDate
            // 
            this.lblReadingDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReadingDate.Location = new System.Drawing.Point(873, 0);
            this.lblReadingDate.Name = "lblReadingDate";
            this.lblReadingDate.Size = new System.Drawing.Size(94, 28);
            this.lblReadingDate.TabIndex = 2;
            this.lblReadingDate.Text = "تاريخ القراءة";
            this.lblReadingDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpReadingDate
            // 
            this.dtpReadingDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpReadingDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpReadingDate.Location = new System.Drawing.Point(653, 3);
            this.dtpReadingDate.Name = "dtpReadingDate";
            this.dtpReadingDate.Size = new System.Drawing.Size(214, 24);
            this.dtpReadingDate.TabIndex = 3;
            // 
            // lblSearch
            // 
            this.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSearch.Location = new System.Drawing.Point(1213, 28);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(84, 29);
            this.lblSearch.TabIndex = 4;
            this.lblSearch.Text = "بحث";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSearch
            // 
            this.filtersLayout.SetColumnSpan(this.txtSearch, 5);
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(3, 31);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(1204, 24);
            this.txtSearch.TabIndex = 5;
            // 
            // pnlButtons
            // 
            this.filtersLayout.SetColumnSpan(this.pnlButtons, 5);
            this.pnlButtons.Controls.Add(this.btnLoad);
            this.pnlButtons.Controls.Add(this.btnSaveAll);
            this.pnlButtons.Controls.Add(this.btnPrintSheet);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(3, 60);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.pnlButtons.Size = new System.Drawing.Size(1204, 63);
            this.pnlButtons.TabIndex = 6;
            this.pnlButtons.WrapContents = false;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(3, 9);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(130, 36);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "تحميل العدادات";
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Location = new System.Drawing.Point(139, 9);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(110, 36);
            this.btnSaveAll.TabIndex = 1;
            this.btnSaveAll.Text = "حفظ الكل";
            // 
            // btnPrintSheet
            // 
            this.btnPrintSheet.Location = new System.Drawing.Point(255, 9);
            this.btnPrintSheet.Name = "btnPrintSheet";
            this.btnPrintSheet.Size = new System.Drawing.Size(120, 36);
            this.btnPrintSheet.TabIndex = 2;
            this.btnPrintSheet.Text = "طباعة الكشف";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(381, 9);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(95, 36);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "تحديث";
            // 
            // pnlGridCard
            // 
            this.pnlGridCard.Controls.Add(this.dgvReadings);
            this.pnlGridCard.Controls.Add(this.lblGridTitle);
            this.pnlGridCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridCard.Location = new System.Drawing.Point(19, 264);
            this.pnlGridCard.Name = "pnlGridCard";
            this.pnlGridCard.Padding = new System.Windows.Forms.Padding(16);
            this.pnlGridCard.Size = new System.Drawing.Size(1332, 376);
            this.pnlGridCard.TabIndex = 2;
            // 
            // dgvReadings
            // 
            this.dgvReadings.AllowUserToAddRows = false;
            this.dgvReadings.AllowUserToDeleteRows = false;
            this.dgvReadings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReadings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReadings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvReadings.Location = new System.Drawing.Point(16, 44);
            this.dgvReadings.Name = "dgvReadings";
            this.dgvReadings.RowHeadersVisible = false;
            this.dgvReadings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvReadings.Size = new System.Drawing.Size(1300, 316);
            this.dgvReadings.TabIndex = 0;
            // 
            // lblGridTitle
            // 
            this.lblGridTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblGridTitle.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.Location = new System.Drawing.Point(16, 16);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(1300, 28);
            this.lblGridTitle.TabIndex = 1;
            this.lblGridTitle.Text = "بيانات المتابعة";
            // 
            // statsLayout
            // 
            this.statsLayout.ColumnCount = 3;
            this.statsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.statsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.statsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.statsLayout.Controls.Add(this.pnlTotalCard, 0, 0);
            this.statsLayout.Controls.Add(this.pnlDoneCard, 1, 0);
            this.statsLayout.Controls.Add(this.pnlPendingCard, 2, 0);
            this.statsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsLayout.Location = new System.Drawing.Point(19, 646);
            this.statsLayout.Name = "statsLayout";
            this.statsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.statsLayout.Size = new System.Drawing.Size(1332, 84);
            this.statsLayout.TabIndex = 3;
            // 
            // pnlTotalCard
            // 
            this.pnlTotalCard.Controls.Add(this.lblTotalCaption);
            this.pnlTotalCard.Controls.Add(this.lblTotalValue);
            this.pnlTotalCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTotalCard.Location = new System.Drawing.Point(891, 3);
            this.pnlTotalCard.Name = "pnlTotalCard";
            this.pnlTotalCard.Padding = new System.Windows.Forms.Padding(18, 12, 18, 12);
            this.pnlTotalCard.Size = new System.Drawing.Size(438, 78);
            this.pnlTotalCard.TabIndex = 0;
            // 
            // lblTotalCaption
            // 
            this.lblTotalCaption.AutoSize = true;
            this.lblTotalCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalCaption.Location = new System.Drawing.Point(0, 10);
            this.lblTotalCaption.Name = "lblTotalCaption";
            this.lblTotalCaption.Size = new System.Drawing.Size(116, 17);
            this.lblTotalCaption.TabIndex = 0;
            this.lblTotalCaption.Text = "إجمالي العدادات";
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.AutoSize = true;
            this.lblTotalValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.Location = new System.Drawing.Point(0, 34);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(28, 29);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "0";
            // 
            // pnlDoneCard
            // 
            this.pnlDoneCard.Controls.Add(this.lblDoneCaption);
            this.pnlDoneCard.Controls.Add(this.lblDoneValue);
            this.pnlDoneCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDoneCard.Location = new System.Drawing.Point(447, 3);
            this.pnlDoneCard.Name = "pnlDoneCard";
            this.pnlDoneCard.Padding = new System.Windows.Forms.Padding(18, 12, 18, 12);
            this.pnlDoneCard.Size = new System.Drawing.Size(438, 78);
            this.pnlDoneCard.TabIndex = 1;
            // 
            // lblDoneCaption
            // 
            this.lblDoneCaption.AutoSize = true;
            this.lblDoneCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblDoneCaption.Location = new System.Drawing.Point(0, 10);
            this.lblDoneCaption.Name = "lblDoneCaption";
            this.lblDoneCaption.Size = new System.Drawing.Size(67, 17);
            this.lblDoneCaption.TabIndex = 0;
            this.lblDoneCaption.Text = "المسجلة";
            // 
            // lblDoneValue
            // 
            this.lblDoneValue.AutoSize = true;
            this.lblDoneValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblDoneValue.Location = new System.Drawing.Point(0, 34);
            this.lblDoneValue.Name = "lblDoneValue";
            this.lblDoneValue.Size = new System.Drawing.Size(28, 29);
            this.lblDoneValue.TabIndex = 1;
            this.lblDoneValue.Text = "0";
            // 
            // pnlPendingCard
            // 
            this.pnlPendingCard.Controls.Add(this.lblPendingCaption);
            this.pnlPendingCard.Controls.Add(this.lblPendingValue);
            this.pnlPendingCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPendingCard.Location = new System.Drawing.Point(3, 3);
            this.pnlPendingCard.Name = "pnlPendingCard";
            this.pnlPendingCard.Padding = new System.Windows.Forms.Padding(18, 12, 18, 12);
            this.pnlPendingCard.Size = new System.Drawing.Size(438, 78);
            this.pnlPendingCard.TabIndex = 2;
            // 
            // lblPendingCaption
            // 
            this.lblPendingCaption.AutoSize = true;
            this.lblPendingCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblPendingCaption.Location = new System.Drawing.Point(0, 10);
            this.lblPendingCaption.Name = "lblPendingCaption";
            this.lblPendingCaption.Size = new System.Drawing.Size(58, 17);
            this.lblPendingCaption.TabIndex = 0;
            this.lblPendingCaption.Text = "المتبقية";
            // 
            // lblPendingValue
            // 
            this.lblPendingValue.AutoSize = true;
            this.lblPendingValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblPendingValue.Location = new System.Drawing.Point(0, 34);
            this.lblPendingValue.Name = "lblPendingValue";
            this.lblPendingValue.Size = new System.Drawing.Size(28, 29);
            this.lblPendingValue.TabIndex = 1;
            this.lblPendingValue.Text = "0";
            // 
            // ReadingEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 749);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Tahoma", 10F);
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "ReadingEntryForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "كشف متابعة تسجيل القراءات الجديدة";
            this.rootLayout.ResumeLayout(false);
            this.pnlHeaderCard.ResumeLayout(false);
            this.pnlHeaderCard.PerformLayout();
            this.pnlFiltersCard.ResumeLayout(false);
            this.filtersLayout.ResumeLayout(false);
            this.filtersLayout.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlGridCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReadings)).EndInit();
            this.statsLayout.ResumeLayout(false);
            this.pnlTotalCard.ResumeLayout(false);
            this.pnlTotalCard.PerformLayout();
            this.pnlDoneCard.ResumeLayout(false);
            this.pnlDoneCard.PerformLayout();
            this.pnlPendingCard.ResumeLayout(false);
            this.pnlPendingCard.PerformLayout();
            this.ResumeLayout(false);

            }
        }
    }