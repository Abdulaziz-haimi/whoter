using System.Windows.Forms;
using System.Drawing;

namespace water3.Forms
{
    partial class ReadingEntryForm
    {
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel rootLayout;

        private Panel pnlHeaderCard;
        private TableLayoutPanel headerLayout;
        private Label lblTitle;
        private Label lblSubtitle;

        private TableLayoutPanel statsLayout;
        private Panel pnlTotalCard;
        private Panel pnlDoneCard;
        private Panel pnlPendingCard;
        private TableLayoutPanel totalCardLayout;
        private TableLayoutPanel doneCardLayout;
        private TableLayoutPanel pendingCardLayout;
        private Label lblTotalCaption;
        private Label lblTotalValue;
        private Label lblDoneCaption;
        private Label lblDoneValue;
        private Label lblPendingCaption;
        private Label lblPendingValue;

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
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();

            this.statsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTotalCard = new System.Windows.Forms.Panel();
            this.pnlDoneCard = new System.Windows.Forms.Panel();
            this.pnlPendingCard = new System.Windows.Forms.Panel();
            this.totalCardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.doneCardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pendingCardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTotalCaption = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.lblDoneCaption = new System.Windows.Forms.Label();
            this.lblDoneValue = new System.Windows.Forms.Label();
            this.lblPendingCaption = new System.Windows.Forms.Label();
            this.lblPendingValue = new System.Windows.Forms.Label();

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
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.dgvReadings = new System.Windows.Forms.DataGridView();

            this.rootLayout.SuspendLayout();
            this.pnlHeaderCard.SuspendLayout();
            this.headerLayout.SuspendLayout();

            this.statsLayout.SuspendLayout();
            this.pnlTotalCard.SuspendLayout();
            this.pnlDoneCard.SuspendLayout();
            this.pnlPendingCard.SuspendLayout();
            this.totalCardLayout.SuspendLayout();
            this.doneCardLayout.SuspendLayout();
            this.pendingCardLayout.SuspendLayout();

            this.pnlFiltersCard.SuspendLayout();
            this.filtersLayout.SuspendLayout();
            this.pnlButtons.SuspendLayout();

            this.pnlGridCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReadings)).BeginInit();

            this.SuspendLayout();

            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.pnlHeaderCard, 0, 0);
            this.rootLayout.Controls.Add(this.statsLayout, 0, 1);
            this.rootLayout.Controls.Add(this.pnlFiltersCard, 0, 2);
            this.rootLayout.Controls.Add(this.pnlGridCard, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 118F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(1200, 680);
            this.rootLayout.TabIndex = 0;

            // 
            // pnlHeaderCard
            // 
            this.pnlHeaderCard.Controls.Add(this.headerLayout);
            this.pnlHeaderCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeaderCard.Location = new System.Drawing.Point(15, 15);
            this.pnlHeaderCard.Name = "pnlHeaderCard";
            this.pnlHeaderCard.Padding = new System.Windows.Forms.Padding(18, 10, 18, 10);
            this.pnlHeaderCard.Size = new System.Drawing.Size(1170, 72);
            this.pnlHeaderCard.TabIndex = 0;

            // 
            // headerLayout
            // 
            this.headerLayout.ColumnCount = 1;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Controls.Add(this.lblTitle, 0, 0);
            this.headerLayout.Controls.Add(this.lblSubtitle, 0, 1);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(18, 10);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 2;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(1134, 52);
            this.headerLayout.TabIndex = 0;

            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1128, 34);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "كشف متابعة تسجيل القراءات الجديدة";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubtitle.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.lblSubtitle.Location = new System.Drawing.Point(3, 34);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(1128, 18);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "تحميل العدادات وتسجيل القراءة الجديدة لكل عداد بشكل سريع ومنظم";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // statsLayout
            // 
            this.statsLayout.ColumnCount = 3;
            this.statsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.statsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.statsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.statsLayout.Controls.Add(this.pnlTotalCard, 0, 0);
            this.statsLayout.Controls.Add(this.pnlDoneCard, 1, 0);
            this.statsLayout.Controls.Add(this.pnlPendingCard, 2, 0);
            this.statsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsLayout.Location = new System.Drawing.Point(15, 93);
            this.statsLayout.Name = "statsLayout";
            this.statsLayout.RowCount = 1;
            this.statsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statsLayout.Size = new System.Drawing.Size(1170, 76);
            this.statsLayout.TabIndex = 1;

            // 
            // pnlTotalCard
            // 
            this.pnlTotalCard.Controls.Add(this.totalCardLayout);
            this.pnlTotalCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTotalCard.Location = new System.Drawing.Point(783, 3);
            this.pnlTotalCard.Name = "pnlTotalCard";
            this.pnlTotalCard.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlTotalCard.Size = new System.Drawing.Size(384, 70);
            this.pnlTotalCard.TabIndex = 0;

            // 
            // totalCardLayout
            // 
            this.totalCardLayout.ColumnCount = 1;
            this.totalCardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.totalCardLayout.Controls.Add(this.lblTotalCaption, 0, 0);
            this.totalCardLayout.Controls.Add(this.lblTotalValue, 0, 1);
            this.totalCardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalCardLayout.Location = new System.Drawing.Point(16, 8);
            this.totalCardLayout.Name = "totalCardLayout";
            this.totalCardLayout.RowCount = 2;
            this.totalCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.totalCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.totalCardLayout.Size = new System.Drawing.Size(352, 54);
            this.totalCardLayout.TabIndex = 0;

            // 
            // lblTotalCaption
            // 
            this.lblTotalCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalCaption.Location = new System.Drawing.Point(3, 0);
            this.lblTotalCaption.Name = "lblTotalCaption";
            this.lblTotalCaption.Size = new System.Drawing.Size(346, 22);
            this.lblTotalCaption.TabIndex = 0;
            this.lblTotalCaption.Text = "إجمالي العدادات";
            this.lblTotalCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblTotalValue
            // 
            this.lblTotalValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.Location = new System.Drawing.Point(3, 22);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(346, 32);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "0";
            this.lblTotalValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // pnlDoneCard
            // 
            this.pnlDoneCard.Controls.Add(this.doneCardLayout);
            this.pnlDoneCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDoneCard.Location = new System.Drawing.Point(393, 3);
            this.pnlDoneCard.Name = "pnlDoneCard";
            this.pnlDoneCard.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlDoneCard.Size = new System.Drawing.Size(384, 70);
            this.pnlDoneCard.TabIndex = 1;

            // 
            // doneCardLayout
            // 
            this.doneCardLayout.ColumnCount = 1;
            this.doneCardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.doneCardLayout.Controls.Add(this.lblDoneCaption, 0, 0);
            this.doneCardLayout.Controls.Add(this.lblDoneValue, 0, 1);
            this.doneCardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doneCardLayout.Location = new System.Drawing.Point(16, 8);
            this.doneCardLayout.Name = "doneCardLayout";
            this.doneCardLayout.RowCount = 2;
            this.doneCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.doneCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.doneCardLayout.Size = new System.Drawing.Size(352, 54);
            this.doneCardLayout.TabIndex = 0;

            // 
            // lblDoneCaption
            // 
            this.lblDoneCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDoneCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblDoneCaption.Location = new System.Drawing.Point(3, 0);
            this.lblDoneCaption.Name = "lblDoneCaption";
            this.lblDoneCaption.Size = new System.Drawing.Size(346, 22);
            this.lblDoneCaption.TabIndex = 0;
            this.lblDoneCaption.Text = "المسجلة";
            this.lblDoneCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblDoneValue
            // 
            this.lblDoneValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDoneValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblDoneValue.Location = new System.Drawing.Point(3, 22);
            this.lblDoneValue.Name = "lblDoneValue";
            this.lblDoneValue.Size = new System.Drawing.Size(346, 32);
            this.lblDoneValue.TabIndex = 1;
            this.lblDoneValue.Text = "0";
            this.lblDoneValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // pnlPendingCard
            // 
            this.pnlPendingCard.Controls.Add(this.pendingCardLayout);
            this.pnlPendingCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPendingCard.Location = new System.Drawing.Point(3, 3);
            this.pnlPendingCard.Name = "pnlPendingCard";
            this.pnlPendingCard.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.pnlPendingCard.Size = new System.Drawing.Size(384, 70);
            this.pnlPendingCard.TabIndex = 2;

            // 
            // pendingCardLayout
            // 
            this.pendingCardLayout.ColumnCount = 1;
            this.pendingCardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pendingCardLayout.Controls.Add(this.lblPendingCaption, 0, 0);
            this.pendingCardLayout.Controls.Add(this.lblPendingValue, 0, 1);
            this.pendingCardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pendingCardLayout.Location = new System.Drawing.Point(16, 8);
            this.pendingCardLayout.Name = "pendingCardLayout";
            this.pendingCardLayout.RowCount = 2;
            this.pendingCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.pendingCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pendingCardLayout.Size = new System.Drawing.Size(352, 54);
            this.pendingCardLayout.TabIndex = 0;

            // 
            // lblPendingCaption
            // 
            this.lblPendingCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPendingCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblPendingCaption.Location = new System.Drawing.Point(3, 0);
            this.lblPendingCaption.Name = "lblPendingCaption";
            this.lblPendingCaption.Size = new System.Drawing.Size(346, 22);
            this.lblPendingCaption.TabIndex = 0;
            this.lblPendingCaption.Text = "المتبقية";
            this.lblPendingCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // lblPendingValue
            // 
            this.lblPendingValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPendingValue.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.lblPendingValue.Location = new System.Drawing.Point(3, 22);
            this.lblPendingValue.Name = "lblPendingValue";
            this.lblPendingValue.Size = new System.Drawing.Size(346, 32);
            this.lblPendingValue.TabIndex = 1;
            this.lblPendingValue.Text = "0";
            this.lblPendingValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // pnlFiltersCard
            // 
            this.pnlFiltersCard.Controls.Add(this.filtersLayout);
            this.pnlFiltersCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFiltersCard.Location = new System.Drawing.Point(15, 175);
            this.pnlFiltersCard.Name = "pnlFiltersCard";
            this.pnlFiltersCard.Padding = new System.Windows.Forms.Padding(14);
            this.pnlFiltersCard.Size = new System.Drawing.Size(1170, 112);
            this.pnlFiltersCard.TabIndex = 2;

            // 
            // filtersLayout
            // 
            this.filtersLayout.ColumnCount = 6;
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filtersLayout.Controls.Add(this.lblArea, 0, 0);
            this.filtersLayout.Controls.Add(this.cmbArea, 1, 0);
            this.filtersLayout.Controls.Add(this.lblReadingDate, 2, 0);
            this.filtersLayout.Controls.Add(this.dtpReadingDate, 3, 0);
            this.filtersLayout.Controls.Add(this.lblSearch, 0, 1);
            this.filtersLayout.Controls.Add(this.txtSearch, 1, 1);
            this.filtersLayout.Controls.Add(this.pnlButtons, 0, 2);
            this.filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersLayout.Location = new System.Drawing.Point(14, 14);
            this.filtersLayout.Name = "filtersLayout";
            this.filtersLayout.RowCount = 3;
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filtersLayout.Size = new System.Drawing.Size(1142, 84);
            this.filtersLayout.TabIndex = 0;

            // 
            // lblArea
            // 
            this.lblArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblArea.Location = new System.Drawing.Point(1073, 0);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(66, 30);
            this.lblArea.TabIndex = 0;
            this.lblArea.Text = "المنطقة";
            this.lblArea.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // cmbArea
            // 
            this.cmbArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArea.Location = new System.Drawing.Point(849, 4);
            this.cmbArea.Margin = new System.Windows.Forms.Padding(4);
            this.cmbArea.Name = "cmbArea";
            this.cmbArea.Size = new System.Drawing.Size(212, 24);
            this.cmbArea.TabIndex = 1;

            // 
            // lblReadingDate
            // 
            this.lblReadingDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblReadingDate.Location = new System.Drawing.Point(755, 0);
            this.lblReadingDate.Name = "lblReadingDate";
            this.lblReadingDate.Size = new System.Drawing.Size(86, 30);
            this.lblReadingDate.TabIndex = 2;
            this.lblReadingDate.Text = "تاريخ القراءة";
            this.lblReadingDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // dtpReadingDate
            // 
            this.dtpReadingDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpReadingDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpReadingDate.Location = new System.Drawing.Point(571, 4);
            this.dtpReadingDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpReadingDate.Name = "dtpReadingDate";
            this.dtpReadingDate.RightToLeftLayout = true;
            this.dtpReadingDate.Size = new System.Drawing.Size(172, 24);
            this.dtpReadingDate.TabIndex = 3;

            // 
            // lblSearch
            // 
            this.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSearch.Location = new System.Drawing.Point(1073, 30);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(66, 30);
            this.lblSearch.TabIndex = 4;
            this.lblSearch.Text = "بحث";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // txtSearch
            // 
            this.filtersLayout.SetColumnSpan(this.txtSearch, 5);
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(4, 35);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(1057, 24);
            this.txtSearch.TabIndex = 5;

            // 
            // pnlButtons
            // 
            this.filtersLayout.SetColumnSpan(this.pnlButtons, 6);
            this.pnlButtons.Controls.Add(this.btnLoad);
            this.pnlButtons.Controls.Add(this.btnSaveAll);
            this.pnlButtons.Controls.Add(this.btnPrintSheet);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(3, 63);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.pnlButtons.Size = new System.Drawing.Size(1136, 18);
            this.pnlButtons.TabIndex = 6;
            this.pnlButtons.WrapContents = false;

            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(1003, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(130, 36);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "تحميل العدادات";
            this.btnLoad.UseVisualStyleBackColor = true;

            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Location = new System.Drawing.Point(887, 5);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(110, 36);
            this.btnSaveAll.TabIndex = 1;
            this.btnSaveAll.Text = "حفظ الكل";
            this.btnSaveAll.UseVisualStyleBackColor = true;

            // 
            // btnPrintSheet
            // 
            this.btnPrintSheet.Location = new System.Drawing.Point(761, 5);
            this.btnPrintSheet.Name = "btnPrintSheet";
            this.btnPrintSheet.Size = new System.Drawing.Size(120, 36);
            this.btnPrintSheet.TabIndex = 2;
            this.btnPrintSheet.Text = "طباعة الكشف";
            this.btnPrintSheet.UseVisualStyleBackColor = true;

            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(660, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(95, 36);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.UseVisualStyleBackColor = true;

            // 
            // pnlGridCard
            // 
            this.pnlGridCard.Controls.Add(this.dgvReadings);
            this.pnlGridCard.Controls.Add(this.lblGridTitle);
            this.pnlGridCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridCard.Location = new System.Drawing.Point(15, 293);
            this.pnlGridCard.Name = "pnlGridCard";
            this.pnlGridCard.Padding = new System.Windows.Forms.Padding(14);
            this.pnlGridCard.Size = new System.Drawing.Size(1170, 372);
            this.pnlGridCard.TabIndex = 3;

            // 
            // lblGridTitle
            // 
            this.lblGridTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblGridTitle.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.Location = new System.Drawing.Point(14, 14);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Size = new System.Drawing.Size(1142, 28);
            this.lblGridTitle.TabIndex = 0;
            this.lblGridTitle.Text = "بيانات المتابعة";
            this.lblGridTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // dgvReadings
            // 
            this.dgvReadings.AllowUserToAddRows = false;
            this.dgvReadings.AllowUserToDeleteRows = false;
            this.dgvReadings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReadings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReadings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvReadings.Location = new System.Drawing.Point(14, 42);
            this.dgvReadings.MultiSelect = false;
            this.dgvReadings.Name = "dgvReadings";
            this.dgvReadings.RowHeadersVisible = false;
            this.dgvReadings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvReadings.Size = new System.Drawing.Size(1142, 316);
            this.dgvReadings.TabIndex = 1;

            // 
            // ReadingEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 680);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Tahoma", 10F);
            this.MinimumSize = new System.Drawing.Size(950, 560);
            this.Name = "ReadingEntryForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "كشف متابعة تسجيل القراءات الجديدة";

            this.rootLayout.ResumeLayout(false);

            this.pnlHeaderCard.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);

            this.statsLayout.ResumeLayout(false);
            this.pnlTotalCard.ResumeLayout(false);
            this.pnlDoneCard.ResumeLayout(false);
            this.pnlPendingCard.ResumeLayout(false);
            this.totalCardLayout.ResumeLayout(false);
            this.doneCardLayout.ResumeLayout(false);
            this.pendingCardLayout.ResumeLayout(false);

            this.pnlFiltersCard.ResumeLayout(false);
            this.filtersLayout.ResumeLayout(false);
            this.filtersLayout.PerformLayout();
            this.pnlButtons.ResumeLayout(false);

            this.pnlGridCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReadings)).EndInit();

            this.ResumeLayout(false);
        }
    }
}