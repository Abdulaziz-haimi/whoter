namespace water3.DB
{
    partial class DbSettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel root;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.TableLayoutPanel grid;

        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.ComboBox cboServer;
        private System.Windows.Forms.Button btnScanServers;

        private System.Windows.Forms.Label lblAuth;
        private System.Windows.Forms.FlowLayoutPanel pnlAuth;
        private System.Windows.Forms.RadioButton rbWindows;
        private System.Windows.Forms.RadioButton rbSql;

        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;

        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPass;

        private System.Windows.Forms.Label lblDb;
        private System.Windows.Forms.TableLayoutPanel dbRow;
        private System.Windows.Forms.ComboBox cboDb;
        private System.Windows.Forms.Button btnLoadDbs;

        private System.Windows.Forms.FlowLayoutPanel pnlInCardBtns;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnRestore;

        private System.Windows.Forms.Label lblHint;

        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.FlowLayoutPanel pnlBottom;
        private System.Windows.Forms.Button btnClose;
       
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.root = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.grid = new System.Windows.Forms.TableLayoutPanel();
            this.lblServer = new System.Windows.Forms.Label();
            this.cboServer = new System.Windows.Forms.ComboBox();
            this.btnScanServers = new System.Windows.Forms.Button();
            this.lblAuth = new System.Windows.Forms.Label();
            this.pnlAuth = new System.Windows.Forms.FlowLayoutPanel();
            this.rbWindows = new System.Windows.Forms.RadioButton();
            this.rbSql = new System.Windows.Forms.RadioButton();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblDb = new System.Windows.Forms.Label();
            this.dbRow = new System.Windows.Forms.TableLayoutPanel();
            this.cboDb = new System.Windows.Forms.ComboBox();
            this.btnLoadDbs = new System.Windows.Forms.Button();
            this.pnlInCardBtns = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.root.SuspendLayout();
            this.pnlCard.SuspendLayout();
            this.grid.SuspendLayout();
            this.pnlAuth.SuspendLayout();
            this.dbRow.SuspendLayout();
            this.pnlInCardBtns.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // root
            // 
            this.root.ColumnCount = 1;
            this.root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.root.Controls.Add(this.pnlCard, 0, 0);
            this.root.Controls.Add(this.pnlStatus, 0, 1);
            this.root.Controls.Add(this.pnlBottom, 0, 2);
            this.root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.root.Location = new System.Drawing.Point(0, 0);
            this.root.Name = "root";
            this.root.Padding = new System.Windows.Forms.Padding(14);
            this.root.RowCount = 3;
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.root.Size = new System.Drawing.Size(720, 460);
            this.root.TabIndex = 0;
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.Controls.Add(this.grid);
            this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard.Location = new System.Drawing.Point(17, 17);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(14);
            this.pnlCard.Size = new System.Drawing.Size(686, 328);
            this.pnlCard.TabIndex = 0;
            // 
            // grid
            // 
            this.grid.ColumnCount = 3;
            this.grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.grid.Controls.Add(this.lblServer, 0, 0);
            this.grid.Controls.Add(this.cboServer, 1, 0);
            this.grid.Controls.Add(this.btnScanServers, 2, 0);
            this.grid.Controls.Add(this.lblAuth, 0, 1);
            this.grid.Controls.Add(this.pnlAuth, 1, 1);
            this.grid.Controls.Add(this.lblUser, 0, 2);
            this.grid.Controls.Add(this.txtUser, 1, 2);
            this.grid.Controls.Add(this.lblPass, 0, 3);
            this.grid.Controls.Add(this.txtPass, 1, 3);
            this.grid.Controls.Add(this.lblDb, 0, 4);
            this.grid.Controls.Add(this.dbRow, 1, 4);
            this.grid.Controls.Add(this.pnlInCardBtns, 0, 6);
            this.grid.Controls.Add(this.lblHint, 0, 7);
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(14, 14);
            this.grid.Name = "grid";
            this.grid.RowCount = 8;
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.grid.Size = new System.Drawing.Size(658, 300);
            this.grid.TabIndex = 0;
            // 
            // lblServer
            // 
            this.lblServer.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(521, 11);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(53, 19);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "السيرفر";
            // 
            // cboServer
            // 
            this.cboServer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboServer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboServer.Location = new System.Drawing.Point(203, 3);
            this.cboServer.Name = "cboServer";
            this.cboServer.Size = new System.Drawing.Size(312, 25);
            this.cboServer.TabIndex = 1;
            // 
            // btnScanServers
            // 
            this.btnScanServers.Location = new System.Drawing.Point(37, 3);
            this.btnScanServers.Name = "btnScanServers";
            this.btnScanServers.Size = new System.Drawing.Size(160, 34);
            this.btnScanServers.TabIndex = 2;
            this.btnScanServers.Text = "بحث عن السيرفرات";
            // 
            // lblAuth
            // 
            this.lblAuth.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAuth.AutoSize = true;
            this.lblAuth.Location = new System.Drawing.Point(521, 53);
            this.lblAuth.Name = "lblAuth";
            this.lblAuth.Size = new System.Drawing.Size(91, 19);
            this.lblAuth.TabIndex = 3;
            this.lblAuth.Text = "طريقة الدخول";
            // 
            // pnlAuth
            // 
            this.grid.SetColumnSpan(this.pnlAuth, 2);
            this.pnlAuth.Controls.Add(this.rbWindows);
            this.pnlAuth.Controls.Add(this.rbSql);
            this.pnlAuth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAuth.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlAuth.Location = new System.Drawing.Point(3, 45);
            this.pnlAuth.Name = "pnlAuth";
            this.pnlAuth.Size = new System.Drawing.Size(512, 36);
            this.pnlAuth.TabIndex = 4;
            this.pnlAuth.WrapContents = false;
            // 
            // rbWindows
            // 
            this.rbWindows.AutoSize = true;
            this.rbWindows.Checked = true;
            this.rbWindows.Location = new System.Drawing.Point(3, 3);
            this.rbWindows.Name = "rbWindows";
            this.rbWindows.Size = new System.Drawing.Size(83, 23);
            this.rbWindows.TabIndex = 0;
            this.rbWindows.TabStop = true;
            this.rbWindows.Text = "Windows";
            // 
            // rbSql
            // 
            this.rbSql.AutoSize = true;
            this.rbSql.Location = new System.Drawing.Point(92, 3);
            this.rbSql.Name = "rbSql";
            this.rbSql.Size = new System.Drawing.Size(90, 23);
            this.rbSql.TabIndex = 1;
            this.rbSql.Text = "SQL Login";
            // 
            // lblUser
            // 
            this.lblUser.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(521, 95);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(94, 19);
            this.lblUser.TabIndex = 5;
            this.lblUser.Text = "اسم المستخدم";
            // 
            // txtUser
            // 
            this.grid.SetColumnSpan(this.txtUser, 2);
            this.txtUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUser.Location = new System.Drawing.Point(3, 87);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(512, 25);
            this.txtUser.TabIndex = 6;
            // 
            // lblPass
            // 
            this.lblPass.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(521, 137);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(75, 19);
            this.lblPass.TabIndex = 7;
            this.lblPass.Text = "كلمة المرور";
            // 
            // txtPass
            // 
            this.grid.SetColumnSpan(this.txtPass, 2);
            this.txtPass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPass.Location = new System.Drawing.Point(3, 129);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(512, 25);
            this.txtPass.TabIndex = 8;
            this.txtPass.UseSystemPasswordChar = true;
            // 
            // lblDb
            // 
            this.lblDb.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDb.AutoSize = true;
            this.lblDb.Location = new System.Drawing.Point(521, 179);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(91, 19);
            this.lblDb.TabIndex = 9;
            this.lblDb.Text = "قاعدة البيانات";
            // 
            // dbRow
            // 
            this.dbRow.ColumnCount = 2;
            this.grid.SetColumnSpan(this.dbRow, 2);
            this.dbRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dbRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.dbRow.Controls.Add(this.cboDb, 0, 0);
            this.dbRow.Controls.Add(this.btnLoadDbs, 1, 0);
            this.dbRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbRow.Location = new System.Drawing.Point(3, 171);
            this.dbRow.Name = "dbRow";
            this.dbRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.dbRow.Size = new System.Drawing.Size(512, 36);
            this.dbRow.TabIndex = 10;
            // 
            // cboDb
            // 
            this.cboDb.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboDb.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboDb.Location = new System.Drawing.Point(203, 3);
            this.cboDb.Name = "cboDb";
            this.cboDb.Size = new System.Drawing.Size(306, 25);
            this.cboDb.TabIndex = 0;
            // 
            // btnLoadDbs
            // 
            this.btnLoadDbs.Location = new System.Drawing.Point(37, 3);
            this.btnLoadDbs.Name = "btnLoadDbs";
            this.btnLoadDbs.Size = new System.Drawing.Size(160, 30);
            this.btnLoadDbs.TabIndex = 1;
            this.btnLoadDbs.Text = "تحميل القواعد";
            // 
            // pnlInCardBtns
            // 
            this.grid.SetColumnSpan(this.pnlInCardBtns, 3);
            this.pnlInCardBtns.Controls.Add(this.btnSave);
            this.pnlInCardBtns.Controls.Add(this.btnTest);
            this.pnlInCardBtns.Controls.Add(this.btnRestore);
            this.pnlInCardBtns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInCardBtns.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlInCardBtns.Location = new System.Drawing.Point(3, 255);
            this.pnlInCardBtns.Name = "pnlInCardBtns";
            this.pnlInCardBtns.Size = new System.Drawing.Size(652, 36);
            this.pnlInCardBtns.TabIndex = 11;
            this.pnlInCardBtns.WrapContents = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(160, 34);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "حفظ";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(169, 3);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(160, 34);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "اختبار اتصال";
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(335, 3);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(200, 34);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "استعادة Backup (.bak)";
            // 
            // lblHint
            // 
            this.grid.SetColumnSpan(this.lblHint, 3);
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblHint.Location = new System.Drawing.Point(3, 294);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(652, 42);
            this.lblHint.TabIndex = 12;
            this.lblHint.Text = "✅ يمكنك كتابة السيرفر يدويًا (مثال: . أو localhost أو 192.168.1.10 أو SERVER\\SQLE" +
    "XPRESS)";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.White;
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStatus.Location = new System.Drawing.Point(17, 351);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(8);
            this.pnlStatus.Size = new System.Drawing.Size(686, 40);
            this.pnlStatus.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(8, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(670, 24);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlBottom.Location = new System.Drawing.Point(17, 397);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(8);
            this.pnlBottom.Size = new System.Drawing.Size(686, 46);
            this.pnlBottom.TabIndex = 2;
            this.pnlBottom.WrapContents = false;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(11, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 34);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "إغلاق";
            // 
            // DbSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(720, 460);
            this.Controls.Add(this.root);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DbSettingsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "إعداد اتصال SQL Server";
            this.root.ResumeLayout(false);
            this.pnlCard.ResumeLayout(false);
            this.grid.ResumeLayout(false);
            this.grid.PerformLayout();
            this.pnlAuth.ResumeLayout(false);
            this.pnlAuth.PerformLayout();
            this.dbRow.ResumeLayout(false);
            this.pnlInCardBtns.ResumeLayout(false);
            this.pnlStatus.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        //private void SetupGrid()
        //{
        //    for (int i = 0; i < 8; i++)
        //        this.grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        //}
    }
}
