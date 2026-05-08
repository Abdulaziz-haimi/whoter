namespace water3
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel rootPanel;
        private System.Windows.Forms.Panel topBar;
        private System.Windows.Forms.TableLayoutPanel topLayout;
        private System.Windows.Forms.Button btnTheme;
        private System.Windows.Forms.Button btnToggleSidebar;
        private System.Windows.Forms.TabControl mainTabControl;

        private System.Windows.Forms.Panel bodyPanel;
        private System.Windows.Forms.Panel sidebarHost;
        private System.Windows.Forms.Panel sidebarViewport;
        private System.Windows.Forms.Panel sidebarContent;
        private System.Windows.Forms.Panel contentCard;
        private System.Windows.Forms.Panel contentHost;

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel stReady;
        private System.Windows.Forms.ToolStripStatusLabel stClock;
        private System.Windows.Forms.ToolStripStatusLabel stSpring;
        private System.Windows.Forms.ToolStripStatusLabel stVersion;

        private System.Windows.Forms.Timer updateTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.rootPanel = new System.Windows.Forms.Panel();
            this.topBar = new System.Windows.Forms.Panel();
            this.topLayout = new System.Windows.Forms.TableLayoutPanel();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.btnTheme = new System.Windows.Forms.Button();
            this.btnToggleSidebar = new System.Windows.Forms.Button();

            this.bodyPanel = new System.Windows.Forms.Panel();
            this.contentCard = new System.Windows.Forms.Panel();
            this.contentHost = new System.Windows.Forms.Panel();
            this.sidebarHost = new System.Windows.Forms.Panel();
            this.sidebarViewport = new System.Windows.Forms.Panel();
            this.sidebarContent = new System.Windows.Forms.Panel();

            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.stReady = new System.Windows.Forms.ToolStripStatusLabel();
            this.stClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.stSpring = new System.Windows.Forms.ToolStripStatusLabel();
            this.stVersion = new System.Windows.Forms.ToolStripStatusLabel();

            this.updateTimer = new System.Windows.Forms.Timer(this.components);

            this.rootPanel.SuspendLayout();
            this.topBar.SuspendLayout();
            this.topLayout.SuspendLayout();
            this.bodyPanel.SuspendLayout();
            this.contentCard.SuspendLayout();
            this.sidebarHost.SuspendLayout();
            this.sidebarViewport.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();

            // 
            // rootPanel
            // 
            this.rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootPanel.Name = "rootPanel";
            this.rootPanel.Padding = new System.Windows.Forms.Padding(0);
            this.rootPanel.Controls.Add(this.bodyPanel);
            this.rootPanel.Controls.Add(this.topBar);

            // 
            // topBar
            // 
            this.topBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBar.Height = 50;
            this.topBar.Name = "topBar";
            this.topBar.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.topBar.Controls.Add(this.topLayout);
            this.topBar.Paint += new System.Windows.Forms.PaintEventHandler(this.topBar_Paint);

            // 
            // topLayout
            // 
            this.topLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topLayout.Name = "topLayout";
            this.topLayout.ColumnCount = 3;
            this.topLayout.RowCount = 1;
            this.topLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.topLayout.Padding = new System.Windows.Forms.Padding(0);
            this.topLayout.Margin = new System.Windows.Forms.Padding(0);

            this.topLayout.Controls.Add(this.mainTabControl, 0, 0);
            this.topLayout.Controls.Add(this.btnTheme, 1, 0);
            this.topLayout.Controls.Add(this.btnToggleSidebar, 2, 0);

            // 
            // mainTabControl
            // 
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mainTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mainTabControl.ItemSize = new System.Drawing.Size(170, 34);
            this.mainTabControl.Padding = new System.Drawing.Point(12, 4);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.MainTabControl_DrawItem);
            this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.mainTabControl_SelectedIndexChanged);
            this.mainTabControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainTabControl_MouseDown);

            // 
            // btnTheme
            // 
            this.btnTheme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnTheme.Name = "btnTheme";
            this.btnTheme.Text = "◐";
            this.btnTheme.UseVisualStyleBackColor = false;
            this.btnTheme.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTheme.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.btnTheme.Click += new System.EventHandler(this.btnTheme_Click);

            // 
            // btnToggleSidebar
            // 
            this.btnToggleSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnToggleSidebar.Name = "btnToggleSidebar";
            this.btnToggleSidebar.Text = "☰";
            this.btnToggleSidebar.UseVisualStyleBackColor = false;
            this.btnToggleSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleSidebar.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.btnToggleSidebar.Click += new System.EventHandler(this.btnToggleSidebar_Click);

            // 
            // bodyPanel
            // 
            this.bodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyPanel.Name = "bodyPanel";
            this.bodyPanel.Padding = new System.Windows.Forms.Padding(0);
            this.bodyPanel.Controls.Add(this.contentCard);
            this.bodyPanel.Controls.Add(this.sidebarHost);

            // 
            // contentCard
            // 
            this.contentCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentCard.Name = "contentCard";
            this.contentCard.Padding = new System.Windows.Forms.Padding(8);
            this.contentCard.Controls.Add(this.contentHost);
            this.contentCard.Paint += new System.Windows.Forms.PaintEventHandler(this.contentCard_Paint);

            // 
            // contentHost
            // 
            this.contentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentHost.Name = "contentHost";
            this.contentHost.Padding = new System.Windows.Forms.Padding(0);

            // 
            // sidebarHost
            // 
            this.sidebarHost.Dock = System.Windows.Forms.DockStyle.Right;
            this.sidebarHost.Width = 318;
            this.sidebarHost.Name = "sidebarHost";
            this.sidebarHost.Padding = new System.Windows.Forms.Padding(0);
            this.sidebarHost.Controls.Add(this.sidebarViewport);

            // 
            // sidebarViewport
            // 
            this.sidebarViewport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidebarViewport.Name = "sidebarViewport";
            this.sidebarViewport.AutoScroll = true;
            this.sidebarViewport.Padding = new System.Windows.Forms.Padding(0);
            this.sidebarViewport.Controls.Add(this.sidebarContent);

            // 
            // sidebarContent
            // 
            this.sidebarContent.Name = "sidebarContent";
            this.sidebarContent.Location = new System.Drawing.Point(12, 14);
            this.sidebarContent.Size = new System.Drawing.Size(280, 600);

            // 
            // statusBar
            // 
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBar.Name = "statusBar";
            this.statusBar.SizingGrip = false;
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.stReady,
                this.stClock,
                this.stSpring,
                this.stVersion
            });

            // 
            // stReady
            // 
            this.stReady.Name = "stReady";
            this.stReady.Text = "جاهز";

            // 
            // stClock
            // 
            this.stClock.Name = "stClock";
            this.stClock.Text = "00/00/0000 00:00:00";

            // 
            // stSpring
            // 
            this.stSpring.Name = "stSpring";
            this.stSpring.Spring = true;

            // 
            // stVersion
            // 
            this.stVersion.Name = "stVersion";
            this.stVersion.Text = "الإصدار";

            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);

            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.rootPanel);
            this.Controls.Add(this.statusBar);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "نظام إدارة المياه";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);

            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.sidebarViewport.ResumeLayout(false);
            this.sidebarHost.ResumeLayout(false);
            this.contentCard.ResumeLayout(false);
            this.bodyPanel.ResumeLayout(false);
            this.topLayout.ResumeLayout(false);
            this.topBar.ResumeLayout(false);
            this.rootPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
/*
namespace water3
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel layoutRoot;

        private System.Windows.Forms.Panel topBar;
        private System.Windows.Forms.Button btnToggleSidebar;
        private System.Windows.Forms.TabControl mainTabControl;

        private System.Windows.Forms.FlowLayoutPanel sidebar;
        private System.Windows.Forms.Panel contentCard;   // card container
        private System.Windows.Forms.Panel contentHost;

        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel stReady;
        private System.Windows.Forms.ToolStripStatusLabel stClock;
        private System.Windows.Forms.ToolStripStatusLabel stSpring;
        private System.Windows.Forms.ToolStripStatusLabel stVersion;

        private System.Windows.Forms.Timer updateTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.layoutRoot = new System.Windows.Forms.TableLayoutPanel();

            this.topBar = new System.Windows.Forms.Panel();
            this.btnToggleSidebar = new System.Windows.Forms.Button();
            this.mainTabControl = new System.Windows.Forms.TabControl();

            this.sidebar = new System.Windows.Forms.FlowLayoutPanel();

            this.contentCard = new System.Windows.Forms.Panel();
            this.contentHost = new System.Windows.Forms.Panel();

            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.stReady = new System.Windows.Forms.ToolStripStatusLabel();
            this.stClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.stSpring = new System.Windows.Forms.ToolStripStatusLabel();
            this.stVersion = new System.Windows.Forms.ToolStripStatusLabel();

            this.updateTimer = new System.Windows.Forms.Timer(this.components);

            this.layoutRoot.SuspendLayout();
            this.topBar.SuspendLayout();
            this.contentCard.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();

            // ========= Form =========
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "Form1";
            this.Text = "water3";
            this.KeyPreview = true;

            // ========= layoutRoot =========
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.ColumnCount = 2;
            this.layoutRoot.RowCount = 2;
            this.layoutRoot.Padding = new System.Windows.Forms.Padding(10);
            this.layoutRoot.Margin = new System.Windows.Forms.Padding(0);

            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 290F));

            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // ========= topBar =========
            this.topBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topBar.Padding = new System.Windows.Forms.Padding(6);
            this.topBar.Margin = new System.Windows.Forms.Padding(0);

            // Toggle button
            this.btnToggleSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnToggleSidebar.Width = 40;
            this.btnToggleSidebar.Height = 32;
            this.btnToggleSidebar.Text = "≡";
            this.btnToggleSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleSidebar.Margin = new System.Windows.Forms.Padding(0);
            this.btnToggleSidebar.UseVisualStyleBackColor = true;

            // mainTabControl
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mainTabControl.ItemSize = new System.Drawing.Size(210, 30);
            this.mainTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mainTabControl.Padding = new System.Drawing.Point(14, 3);

            this.topBar.Controls.Add(this.mainTabControl);
            this.topBar.Controls.Add(this.btnToggleSidebar);

            // ========= sidebar =========
            this.sidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidebar.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.sidebar.WrapContents = false;
            this.sidebar.AutoScroll = true;
            this.sidebar.Padding = new System.Windows.Forms.Padding(12);
            this.sidebar.Margin = new System.Windows.Forms.Padding(0);

            // ========= contentCard + contentHost =========
            this.contentCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentCard.Padding = new System.Windows.Forms.Padding(6);
            this.contentCard.Margin = new System.Windows.Forms.Padding(0);

            this.contentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentHost.Margin = new System.Windows.Forms.Padding(0);

            this.contentCard.Controls.Add(this.contentHost);

            // ========= statusBar =========
            this.statusBar.SizingGrip = false;

            this.stReady.Text = "✅ جاهز";
            this.stClock.Text = "";
            this.stSpring.Spring = true;
            this.stVersion.Text = "";

            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.stReady, this.stClock, this.stSpring, this.stVersion
            });

            // ========= timer =========
            this.updateTimer.Interval = 1000;

            // ========= add to layout =========
            this.layoutRoot.Controls.Add(this.topBar, 0, 0);
            this.layoutRoot.SetColumnSpan(this.topBar, 2);

            this.layoutRoot.Controls.Add(this.contentCard, 0, 1);
            this.layoutRoot.Controls.Add(this.sidebar, 1, 1);

            // ========= add to form =========
            this.Controls.Add(this.layoutRoot);
            this.Controls.Add(this.statusBar);

            // ========= events (بدون Lambda) =========
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);

            this.btnToggleSidebar.Click += new System.EventHandler(this.btnToggleSidebar_Click);

            this.mainTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.MainTabControl_DrawItem);
            this.mainTabControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainTabControl_MouseDown);
            this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.mainTabControl_SelectedIndexChanged);

            this.topBar.Paint += new System.Windows.Forms.PaintEventHandler(this.topBar_Paint);
            this.contentCard.Paint += new System.Windows.Forms.PaintEventHandler(this.contentCard_Paint);
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);

            this.layoutRoot.ResumeLayout(false);
            this.topBar.ResumeLayout(false);
            this.contentCard.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

*/