using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace water3
{
 
    public partial class MainForm : Form
    {
        private Button currentButton;
        private Form activeForm;

        public MainForm()
        {
            InitializeComponent();
            ConfigureForm();
        }

        private void ConfigureForm()
        {
            // إعدادات النموذج
            this.Text = "نظام محاسبة مشروع الماء";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);

            // إنشاء عناصر الواجهة ديناميكياً
            CreateSidebar();
            CreateMainContent();
        }

        private void CreateSidebar()
        {
            Panel sidebar = new Panel
            {
                BackColor = Color.FromArgb(44, 62, 80),
                Dock = DockStyle.Left,
                Width = 250
            };

            // عنوان الشريط الجانبي
            Label title = new Label
            {
                Text = "نظام المياه\nإدارة المشروع المائي",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(10)
            };

            // قائمة التنقل
            Panel navPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            string[] menuItems = {
                "لوحة التحكم", "المشتركين", "الفواتير", "قراءات العدادات",
                "المدفوعات", "التقارير", "الحسابات", "المستخدمين", "الإعدادات"
            };

            string[] menuIcons = {
                "📊", "👥", "🧾", "📈", "💰", "📋", "📒", "👤", "⚙️"
            };

            for (int i = 0; i < menuItems.Length; i++)
            {
                Button menuButton = new Button
                {
                    Text = $"  {menuIcons[i]}  {menuItems[i]}",
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Height = 50,
                    Dock = DockStyle.Top,
                    Tag = menuItems[i],
                    Padding = new Padding(10),
                    TextImageRelation = TextImageRelation.TextBeforeImage
                };

                menuButton.FlatAppearance.BorderSize = 0;
                menuButton.Click += MenuButton_Click;

                if (i == 0)
                {
                    menuButton.BackColor = Color.FromArgb(52, 152, 219, 100);
                    currentButton = menuButton;
                }

                navPanel.Controls.Add(menuButton);
            }

            // إضافة عناصر للشريط الجانبي
            navPanel.Controls.Add(title);
            sidebar.Controls.Add(navPanel);
            sidebar.Controls.Add(title);

            this.Controls.Add(sidebar);
        }

        private void CreateMainContent()
        {
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // رأس المنطقة الرئيسية
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.Transparent
            };

            Label headerTitle = new Label
            {
                Text = "لوحة التحكم",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Dock = DockStyle.Left,
                AutoSize = true
            };

            Button btnAddNew = new Button
            {
                Text = "➕ إضافة جديد",
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 40,
                Width = 150,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnAddNew.FlatAppearance.BorderSize = 0;

            headerPanel.Controls.Add(headerTitle);
            headerPanel.Controls.Add(btnAddNew);

            // منطقة المحتوى الديناميكي
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // إضافة إحصائيات
            CreateStatsPanel(contentPanel);

            mainPanel.Controls.Add(contentPanel);
            mainPanel.Controls.Add(headerPanel);

            this.Controls.Add(mainPanel);
        }

        private void CreateStatsPanel(Panel parentPanel)
        {
            Panel statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.Transparent
            };

            Color[] statColors = {
                Color.FromArgb(44, 62, 80),    // Primary
                Color.FromArgb(39, 174, 96),   // Success
                Color.FromArgb(243, 156, 18),  // Warning
                Color.FromArgb(231, 76, 60)    // Danger
            };

            string[] statTitles = { "المشتركين", "إجمالي الإيرادات", "الديون المتأخرة", "نسبة الفاقد" };
            string[] statValues = { "1,245", "75,850 ر.س", "48,250 ر.س", "8.2%" };

            for (int i = 0; i < 4; i++)
            {
                Panel statCard = new Panel
                {
                    Width = 250,
                    Height = 120,
                    BackColor = statColors[i],
                    Padding = new Padding(15),
                    Margin = new Padding(10)
                };

                Label lblValue = new Label
                {
                    Text = statValues[i],
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    Dock = DockStyle.Top,
                    AutoSize = true
                };

                Label lblTitle = new Label
                {
                    Text = statTitles[i],
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.White,
                    Dock = DockStyle.Top,
                    AutoSize = true
                };

                statCard.Controls.Add(lblTitle);
                statCard.Controls.Add(lblValue);

                // تحديد المواقع
                statCard.Location = new Point(i * 270 + 10, 10);

                statsPanel.Controls.Add(statCard);
            }

            parentPanel.Controls.Add(statsPanel);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                // تحديث زر القائمة النشط
                if (currentButton != null)
                {
                    currentButton.BackColor = Color.Transparent;
                }
                button.BackColor = Color.FromArgb(52, 152, 219, 100);
                currentButton = button;

                // تحميل النموذج المناسب
                string formName = button.Tag.ToString();
                OpenChildForm(formName);
            }
        }

        private void OpenChildForm(string formName)
        {
            Form childForm = null;

            switch (formName)
            {
               
                case "الفواتير":
                    childForm = new InvoiceForm();
                    break;
               
                case "قراءات العدادات":
                    childForm = new DashboardForm();
                    break;
                case "التقارير":
                    childForm = new CollectorsReportForm();
                    break;
                case "المستخدمين":
                    childForm = new LoginForm();
                    break;
              
                case "الإعدادات":
                    childForm = new CollectorsReportForm();
                    break;
                default:
                    // لوحة التحكم
                    return;
            }

            if (childForm != null)
            {
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                // العثور على Panel الرئيسي وإضافة النموذج
                foreach (Control control in this.Controls)
                {
                    if (control is Panel panel && panel.Dock == DockStyle.Fill)
                    {
                        panel.Controls.Clear();
                        panel.Controls.Add(childForm);
                        break;
                    }
                }

                childForm.Show();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
