using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using water3.Models;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
    public partial class CollectorUserLinkForm : Form
    {
        private readonly CollectorUserLinkService _service = new CollectorUserLinkService();

        private List<CollectorUserLinkItem> _collectors = new List<CollectorUserLinkItem>();
        private List<UserLookupItem> _users = new List<UserLookupItem>();

        private TableLayoutPanel rootLayout;
        private TableLayoutPanel contentLayout;
        private TableLayoutPanel actionsLayout;

        private Panel pnlHeaderCard;
        private Panel pnlCollectorsCard;
        private Panel pnlUsersCard;
        private Panel pnlActionsCard;
        private Panel pnlStatusBar;

        private Label lblTitle;
        private Label lblSubtitle;

        private Label lblCollectorsTitle;
        private Label lblUsersTitle;
        private Label lblCollectorsCount;
        private Label lblUsersCount;

        private Label lblCollectorSearch;
        private Label lblUserSearch;

        private TextBox txtCollectorSearch;
        private TextBox txtUserSearch;

        private Button btnSearchCollectors;
        private Button btnSearchUsers;
        private Button btnRefresh;
        private Button btnAssign;
        private Button btnUnassign;

        private DataGridView dgvCollectors;
        private DataGridView dgvUsers;

        private Label lblSelectedCollector;
        private Label lblStatus;

        private FlowLayoutPanel collectorSearchPanel;
        private FlowLayoutPanel userSearchPanel;
        private FlowLayoutPanel actionsButtonsPanel;

        public CollectorUserLinkForm()
        {
            InitializeComponent();

            PermissionHelper.EnforceFormPermission(this, "COLLECTORS_LINK_USER");

            EnableDoubleBuffering();
            ApplyTheme();
            WireEvents();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "ربط المحصل بالمستخدم";
            StartPosition = FormStartPosition.CenterScreen;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = new Font("Tahoma", 10F);
            BackColor = Color.FromArgb(244, 247, 251);
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1050, 650);

            rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(14),
                BackColor = Color.FromArgb(244, 247, 251)
            };

            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 98F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));

            CreateHeaderCard();
            CreateContentArea();
            CreateActionsCard();
            CreateStatusBar();

            rootLayout.Controls.Add(pnlHeaderCard, 0, 0);
            rootLayout.Controls.Add(contentLayout, 0, 1);
            rootLayout.Controls.Add(pnlActionsCard, 0, 2);
            rootLayout.Controls.Add(pnlStatusBar, 0, 3);

            Controls.Add(rootLayout);
        }

        #region Layout Builders

        private void CreateHeaderCard()
        {
            pnlHeaderCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18, 10, 18, 10),
                Margin = new Padding(3)
            };

            TableLayoutPanel headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblTitle = new Label
            {
                Text = "ربط المحصل بالمستخدم",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            lblSubtitle = new Label
            {
                Text = "إدارة ربط حسابات المحصلين بالمستخدمين وفك الربط عند الحاجة",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 9.5F),
                TextAlign = ContentAlignment.MiddleRight
            };

            headerLayout.Controls.Add(lblTitle, 0, 0);
            headerLayout.Controls.Add(lblSubtitle, 0, 1);

            pnlHeaderCard.Controls.Add(headerLayout);
        }

        private void CreateContentArea()
        {
            contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 8, 0, 8)
            };

            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            CreateUsersCard();
            CreateCollectorsCard();

            // المستخدمون يسارًا والمحصلون يمينًا
            contentLayout.Controls.Add(pnlUsersCard, 0, 0);
            contentLayout.Controls.Add(pnlCollectorsCard, 1, 0);
        }

        private void CreateCollectorsCard()
        {
            pnlCollectorsCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14),
                Margin = new Padding(6, 0, 0, 0)
            };

            TableLayoutPanel collectorsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            collectorsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            collectorsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            collectorsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            collectorsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            TableLayoutPanel collectorsHeader = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            collectorsHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            collectorsHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            lblCollectorsCount = new Label
            {
                Text = "0 محصل",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblCollectorsTitle = new Label
            {
                Text = "قائمة المحصلين",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            collectorsHeader.Controls.Add(lblCollectorsCount, 0, 0);
            collectorsHeader.Controls.Add(lblCollectorsTitle, 1, 0);

            collectorSearchPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0)
            };

            lblCollectorSearch = new Label
            {
                Text = "بحث",
                Width = 42,
                Height = 32,
                TextAlign = ContentAlignment.MiddleRight
            };

            txtCollectorSearch = new TextBox
            {
                Width = 230,
                Height = 28,
                Margin = new Padding(6, 3, 6, 3)
            };

            btnSearchCollectors = MakeButton("بحث المحصلين", 125);

            collectorSearchPanel.Controls.Add(lblCollectorSearch);
            collectorSearchPanel.Controls.Add(txtCollectorSearch);
            collectorSearchPanel.Controls.Add(btnSearchCollectors);

            dgvCollectors = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };

            dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCollectorID",
                HeaderText = "الرقم",
                DataPropertyName = "CollectorID",
                FillWeight = 55
            });

            dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCollectorName",
                HeaderText = "المحصل",
                DataPropertyName = "CollectorName",
                FillWeight = 145
            });

            dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCollectorPhone",
                HeaderText = "الهاتف",
                DataPropertyName = "CollectorPhone",
                FillWeight = 105
            });

            dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUserName",
                HeaderText = "اسم المستخدم",
                DataPropertyName = "UserName",
                FillWeight = 110
            });

            dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRoleName",
                HeaderText = "الدور",
                DataPropertyName = "RoleName",
                FillWeight = 95
            });

            collectorsLayout.Controls.Add(collectorsHeader, 0, 0);
            collectorsLayout.Controls.Add(collectorSearchPanel, 0, 1);
            collectorsLayout.Controls.Add(dgvCollectors, 0, 2);

            pnlCollectorsCard.Controls.Add(collectorsLayout);
        }

        private void CreateUsersCard()
        {
            pnlUsersCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14),
                Margin = new Padding(0, 0, 6, 0)
            };

            TableLayoutPanel usersLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };

            usersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            usersLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            usersLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            usersLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            TableLayoutPanel usersHeader = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            usersHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            usersHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            lblUsersCount = new Label
            {
                Text = "0 مستخدم",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblUsersTitle = new Label
            {
                Text = "المستخدمون المتاحون",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            usersHeader.Controls.Add(lblUsersCount, 0, 0);
            usersHeader.Controls.Add(lblUsersTitle, 1, 0);

            userSearchPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0)
            };

            lblUserSearch = new Label
            {
                Text = "بحث",
                Width = 42,
                Height = 32,
                TextAlign = ContentAlignment.MiddleRight
            };

            txtUserSearch = new TextBox
            {
                Width = 230,
                Height = 28,
                Margin = new Padding(6, 3, 6, 3)
            };

            btnSearchUsers = MakeButton("بحث المستخدمين", 135);

            userSearchPanel.Controls.Add(lblUserSearch);
            userSearchPanel.Controls.Add(txtUserSearch);
            userSearchPanel.Controls.Add(btnSearchUsers);

            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUserID",
                HeaderText = "الرقم",
                DataPropertyName = "UserID",
                FillWeight = 55
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUserName2",
                HeaderText = "اسم المستخدم",
                DataPropertyName = "UserName",
                FillWeight = 110
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFullName",
                HeaderText = "الاسم الكامل",
                DataPropertyName = "FullName",
                FillWeight = 160
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRoleName2",
                HeaderText = "الدور",
                DataPropertyName = "RoleName",
                FillWeight = 100
            });

            usersLayout.Controls.Add(usersHeader, 0, 0);
            usersLayout.Controls.Add(userSearchPanel, 0, 1);
            usersLayout.Controls.Add(dgvUsers, 0, 2);

            pnlUsersCard.Controls.Add(usersLayout);
        }

        private void CreateActionsCard()
        {
            pnlActionsCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(14, 10, 14, 10),
                Margin = new Padding(3)
            };

            actionsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };

            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblSelectedCollector = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            actionsButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 4, 0, 0)
            };

            btnAssign = MakeButton("ربط المستخدم المحدد بالمحصل", 250);
            btnUnassign = MakeButton("فك الربط عن المحصل", 190);
            btnRefresh = MakeButton("تحديث الكل", 120);

            actionsButtonsPanel.Controls.Add(btnAssign);
            actionsButtonsPanel.Controls.Add(btnUnassign);
            actionsButtonsPanel.Controls.Add(btnRefresh);

            actionsLayout.Controls.Add(lblSelectedCollector, 0, 0);
            actionsLayout.Controls.Add(actionsButtonsPanel, 0, 1);

            pnlActionsCard.Controls.Add(actionsLayout);
        }

        private void CreateStatusBar()
        {
            pnlStatusBar = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 0, 12, 0),
                Margin = new Padding(3, 0, 3, 3)
            };

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            pnlStatusBar.Controls.Add(lblStatus);
        }

        #endregion

        #region Theme

        private void ApplyTheme()
        {
            StyleCard(pnlHeaderCard, Color.White);
            StyleCard(pnlCollectorsCard, Color.White);
            StyleCard(pnlUsersCard, Color.White);
            StyleCard(pnlActionsCard, Color.White);

            pnlStatusBar.BackColor = Color.FromArgb(241, 245, 249);

            lblTitle.ForeColor = Color.FromArgb(15, 76, 129);
            lblSubtitle.ForeColor = Color.FromArgb(100, 116, 139);

            lblCollectorsTitle.ForeColor = Color.FromArgb(30, 41, 59);
            lblUsersTitle.ForeColor = Color.FromArgb(30, 41, 59);

            lblCollectorsCount.ForeColor = Color.FromArgb(37, 99, 235);
            lblUsersCount.ForeColor = Color.FromArgb(22, 163, 74);

            lblCollectorSearch.ForeColor = Color.FromArgb(51, 65, 85);
            lblUserSearch.ForeColor = Color.FromArgb(51, 65, 85);

            txtCollectorSearch.Font = new Font("Tahoma", 10F);
            txtCollectorSearch.BorderStyle = BorderStyle.FixedSingle;

            txtUserSearch.Font = new Font("Tahoma", 10F);
            txtUserSearch.BorderStyle = BorderStyle.FixedSingle;

            StyleButton(btnSearchCollectors, Color.FromArgb(37, 99, 235));
            StyleButton(btnSearchUsers, Color.FromArgb(37, 99, 235));
            StyleButton(btnAssign, Color.FromArgb(22, 163, 74));
            StyleButton(btnUnassign, Color.FromArgb(234, 88, 12));
            StyleButton(btnRefresh, Color.FromArgb(100, 116, 139));

            lblSelectedCollector.ForeColor = Color.FromArgb(15, 76, 129);
            lblStatus.ForeColor = Color.FromArgb(22, 101, 52);

            StyleGrid(dgvCollectors);
            StyleGrid(dgvUsers);
        }

        private void StyleCard(Panel panel, Color backColor)
        {
            panel.BackColor = backColor;
            panel.BorderStyle = BorderStyle.None;

            panel.Paint -= DrawCardBorder;
            panel.Paint += DrawCardBorder;
        }

        private void DrawCardBorder(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            Rectangle rect = panel.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using (Pen pen = new Pen(Color.FromArgb(220, 228, 238)))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void StyleButton(Button btn, Color backColor)
        {
            btn.Height = 36;
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.08F);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.08F);
            btn.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(6, 0, 0, 0);
        }

        private void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Color.White;
            grid.GridColor = Color.FromArgb(226, 232, 240);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 40;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            grid.DefaultCellStyle.Font = new Font("Tahoma", 10F);
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            grid.RowTemplate.Height = 36;
        }

        private Button MakeButton(string text, int width)
        {
            return new Button
            {
                Text = text,
                Width = width,
                Height = 36,
                UseVisualStyleBackColor = false
            };
        }

        #endregion

        #region Events

        private void WireEvents()
        {
            btnSearchCollectors.Click += (s, e) => LoadCollectors();
            btnSearchUsers.Click += (s, e) => LoadUsers();
            btnRefresh.Click += (s, e) => LoadData();

            btnAssign.Click += BtnAssign_Click;
            btnUnassign.Click += BtnUnassign_Click;

            dgvCollectors.SelectionChanged += (s, e) =>
            {
                UpdateSelectedCollectorInfo();
                UpdateActionButtons();
            };

            dgvUsers.SelectionChanged += (s, e) =>
            {
                UpdateSelectedCollectorInfo();
                UpdateActionButtons();
            };

            txtCollectorSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    LoadCollectors();
                }
            };

            txtUserSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    LoadUsers();
                }
            };
        }

        #endregion

        #region Data Loading

        private void LoadData()
        {
            LoadCollectors(false);
            LoadUsers(false);
            UpdateSelectedCollectorInfo();
            UpdateActionButtons();

            SetStatus($"تم تحميل البيانات — المحصلون: {_collectors.Count}، المستخدمون المتاحون: {_users.Count}", false);
        }

        private void LoadCollectors(bool showStatus = true)
        {
            try
            {
                _collectors = _service.GetCollectorsWithUsers(txtCollectorSearch.Text);

                dgvCollectors.DataSource = null;
                dgvCollectors.DataSource = _collectors;

                lblCollectorsCount.Text = $"{_collectors.Count} محصل";

                UpdateSelectedCollectorInfo();
                UpdateActionButtons();

                if (showStatus)
                    SetStatus($"تم تحميل {_collectors.Count} محصل.", false);
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        private void LoadUsers(bool showStatus = true)
        {
            try
            {
                _users = _service.GetAvailableUsers(txtUserSearch.Text, onlyActive: true);

                dgvUsers.DataSource = null;
                dgvUsers.DataSource = _users;

                lblUsersCount.Text = $"{_users.Count} مستخدم";

                UpdateSelectedCollectorInfo();
                UpdateActionButtons();

                if (showStatus)
                    SetStatus($"تم تحميل {_users.Count} مستخدم متاح.", false);
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        #endregion

        #region Selection Helpers

        private CollectorUserLinkItem GetSelectedCollector()
        {
            return dgvCollectors.CurrentRow?.DataBoundItem as CollectorUserLinkItem;
        }

        private UserLookupItem GetSelectedUser()
        {
            return dgvUsers.CurrentRow?.DataBoundItem as UserLookupItem;
        }

        private void UpdateSelectedCollectorInfo()
        {
            var collector = GetSelectedCollector();
            var user = GetSelectedUser();

            if (collector == null)
            {
                lblSelectedCollector.Text = "اختر محصلًا من قائمة المحصلين، ثم اختر مستخدمًا لربطه به.";
                return;
            }

            if (collector.UserID.HasValue)
            {
                lblSelectedCollector.Text =
                    $"المحصل المحدد: {collector.CollectorName}   |   المستخدم الحالي: {collector.UserName} ({collector.RoleName})";
            }
            else
            {
                lblSelectedCollector.Text =
                    $"المحصل المحدد: {collector.CollectorName}   |   لا يوجد مستخدم مربوط حاليًا";
            }

            if (user != null)
            {
                lblSelectedCollector.Text += $"   |   المستخدم المحدد للربط: {user.UserName}";
            }
        }

        private void UpdateActionButtons()
        {
            var collector = GetSelectedCollector();
            var user = GetSelectedUser();

            btnAssign.Enabled = collector != null && user != null;
            btnUnassign.Enabled = collector != null && collector.UserID.HasValue;
        }

        #endregion

        #region Actions

        private void BtnAssign_Click(object sender, EventArgs e)
        {
            try
            {
                var collector = GetSelectedCollector();
                var user = GetSelectedUser();

                if (collector == null)
                {
                    MessageBox.Show("اختر محصلًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (user == null)
                {
                    MessageBox.Show("اختر مستخدمًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show(
                    $"هل تريد ربط المستخدم {user.UserName} بالمحصل {collector.CollectorName}؟",
                    "تأكيد الربط",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                _service.AssignUserToCollector(collector.CollectorID, user.UserID);

                LoadData();
                SetStatus("تم الربط بنجاح.", false);
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        private void BtnUnassign_Click(object sender, EventArgs e)
        {
            try
            {
                var collector = GetSelectedCollector();

                if (collector == null)
                {
                    MessageBox.Show("اختر محصلًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!collector.UserID.HasValue)
                {
                    MessageBox.Show("هذا المحصل غير مربوط بأي مستخدم حاليًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (MessageBox.Show(
                    $"هل تريد فك ربط المستخدم عن المحصل {collector.CollectorName}؟",
                    "تأكيد فك الربط",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                _service.UnassignUserFromCollector(collector.CollectorID);

                LoadData();
                SetStatus("تم فك الربط بنجاح.", false);
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        #endregion

        #region Helpers

        private void SetStatus(string text, bool isError)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = isError
                ? Color.FromArgb(185, 28, 28)
                : Color.FromArgb(22, 101, 52);
        }

        private void EnableDoubleBuffering()
        {
            DoubleBuffered = true;

            SetGridDoubleBuffered(dgvCollectors);
            SetGridDoubleBuffered(dgvUsers);
        }

        private void SetGridDoubleBuffered(DataGridView grid)
        {
            try
            {
                PropertyInfo prop = typeof(DataGridView).GetProperty(
                    "DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (prop != null)
                    prop.SetValue(grid, true, null);
            }
            catch
            {
                // لا يؤثر على عمل البرنامج إذا تعذر التفعيل
            }
        }

        #endregion
    }
}