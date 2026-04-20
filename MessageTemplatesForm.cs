//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using System.Data;
//using System.Data.SqlClient;

//namespace water3
//{
//        public partial class MessageTemplatesForm : Form
//        {

//            private DataTable templatesTable;
//            private DataGridView templatesDataGridView;
//            private  

//            public MessageTemplatesForm()
//            {
//                InitializeTemplatesForm();
//                CreateTemplatesControls();
//                LoadTemplatesDataFromDB();
//            }

//            private void InitializeTemplatesForm()
//            {
//                this.Text = "إدارة قوالب الرسائل";
//                this.Size = new Size(1000, 700);
//                this.StartPosition = FormStartPosition.CenterParent;
//                this.Font = new Font("Tahoma", 9);
//            }

//            private void CreateTemplatesControls()
//            {
//                Panel toolPanel = new Panel
//                {
//                    Dock = DockStyle.Top,
//                    Height = 50,
//                    BackColor = Color.FromArgb(155, 89, 182)
//                };

//                Button addBtn = CreateTemplateToolButton("قالب جديد", 10, Color.FromArgb(46, 204, 113));
//                addBtn.Click += AddTemplate_Click;

//                Button editBtn = CreateTemplateToolButton("تعديل", 120, Color.FromArgb(241, 196, 15));
//                editBtn.Click += EditTemplate_Click;

//                Button previewBtn = CreateTemplateToolButton("معاينة", 230, Color.FromArgb(52, 152, 219));
//                previewBtn.Click += PreviewTemplate_Click;

//                Button activateBtn = CreateTemplateToolButton("تفعيل/تعطيل", 340, Color.FromArgb(142, 68, 173));
//                activateBtn.Click += ToggleActivation_Click;

//                Button refreshBtn = CreateTemplateToolButton("تحديث", 470, Color.FromArgb(149, 165, 166));
//                refreshBtn.Click += RefreshTemplates_Click;

//                toolPanel.Controls.AddRange(new Control[] { addBtn, editBtn, previewBtn, activateBtn, refreshBtn });

//                templatesDataGridView = new DataGridView
//                {
//                    Dock = DockStyle.Fill,
//                    BackgroundColor = Color.White,
//                    BorderStyle = BorderStyle.None,
//                    AllowUserToAddRows = false,
//                    AllowUserToDeleteRows = false,
//                    ReadOnly = true,
//                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
//                    RowHeadersVisible = false
//                };

//                templatesDataGridView.Columns.AddRange(new DataGridViewColumn[]
//                {
//                new DataGridViewTextBoxColumn { HeaderText="رقم القالب", Name="TemplateID", Width=80 },
//                new DataGridViewTextBoxColumn { HeaderText="اسم القالب", Name="TemplateName", Width=150 },
//                new DataGridViewTextBoxColumn { HeaderText="النوع", Name="TemplateType", Width=120 },
//                new DataGridViewTextBoxColumn { HeaderText="اللغة", Name="Language", Width=80 },
//                new DataGridViewTextBoxColumn { HeaderText="الحالة", Name="IsActive", Width=80 },
//                new DataGridViewTextBoxColumn { HeaderText="تاريخ الإنشاء", Name="CreatedAt", Width=120 },
//                new DataGridViewTextBoxColumn { Name="TemplateText", Visible=false }
//                });

//                templatesDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(155, 89, 182);
//                templatesDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
//                templatesDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
//                templatesDataGridView.EnableHeadersVisualStyles = false;
//                templatesDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

//                this.Controls.AddRange(new Control[] { templatesDataGridView, toolPanel });
//            }

//            private Button CreateTemplateToolButton(string text, int x, Color backColor)
//            {
//                return new Button
//                {
//                    Text = text,
//                    Size = text.Contains("/") ? new Size(120, 30) : new Size(100, 30),
//                    Location = new Point(x, 10),
//                    BackColor = backColor,
//                    ForeColor = Color.White,
//                    FlatStyle = FlatStyle.Flat,
//                    Font = new Font("Tahoma", 9, FontStyle.Regular)
//                };
//            }

//            private void LoadTemplatesDataFromDB()
//            {
//                try
//                {
//                    templatesTable = new DataTable();
//                    templatesTable.Columns.Add("TemplateID", typeof(int));
//                    templatesTable.Columns.Add("TemplateName", typeof(string));
//                    templatesTable.Columns.Add("TemplateText", typeof(string));
//                    templatesTable.Columns.Add("TemplateType", typeof(string));
//                    templatesTable.Columns.Add("IsActive", typeof(bool));
//                    templatesTable.Columns.Add("Language", typeof(string));
//                    templatesTable.Columns.Add("CreatedAt", typeof(DateTime));

//                    using (SqlConnection con = Db.GetConnection())
//                    {
//                        con.Open();
//                        string query = "SELECT TemplateID, TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt FROM MessageTemplates ORDER BY CreatedAt DESC";
//                        using (SqlCommand cmd = new SqlCommand(query, con))
//                        using (SqlDataReader reader = cmd.ExecuteReader())
//                        {
//                            while (reader.Read())
//                            {
//                                templatesTable.Rows.Add(
//                                    reader["TemplateID"],
//                                    reader["TemplateName"],
//                                    reader["TemplateText"],
//                                    reader["TemplateType"],
//                                    reader["IsActive"],
//                                    reader["Language"],
//                                    reader["CreatedAt"]
//                                );
//                            }
//                        }
//                    }

//                    templatesDataGridView.DataSource = templatesTable;

//                    // تنسيق عمود الحالة
//                    foreach (DataGridViewRow row in templatesDataGridView.Rows)
//                    {
//                        if (row.Cells["IsActive"].Value != null)
//                        {
//                            bool isActive = Convert.ToBoolean(row.Cells["IsActive"].Value);
//                            row.Cells["IsActive"].Value = isActive ? "نشط" : "غير نشط";
//                            row.Cells["IsActive"].Style.ForeColor = isActive ? Color.Green : Color.Red;
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ",
//                        MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }

//            private void AddTemplate_Click(object sender, EventArgs e)
//            {
//                TemplateEditForm editForm = new TemplateEditForm(null);
//                if (editForm.ShowDialog() == DialogResult.OK)
//                {
//                    try
//                    {
//                        using (SqlConnection con = Db.GetConnection())
//                        {
//                            con.Open();
//                            string insertQuery = @"INSERT INTO MessageTemplates (TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt)
//                                               VALUES (@name, @text, @type, @active, @lang, @created);
//                                               SELECT SCOPE_IDENTITY()";
//                            using (SqlCommand cmd = new SqlCommand(insertQuery, con))
//                            {
//                                cmd.Parameters.AddWithValue("@name", editForm.TemplateName);
//                                cmd.Parameters.AddWithValue("@text", editForm.TemplateText);
//                                cmd.Parameters.AddWithValue("@type", editForm.TemplateType);
//                                cmd.Parameters.AddWithValue("@active", editForm.IsActive);
//                                cmd.Parameters.AddWithValue("@lang", editForm.Language);
//                                cmd.Parameters.AddWithValue("@created", editForm.CreatedAt);
//                                int newID = Convert.ToInt32(cmd.ExecuteScalar());
//                                templatesTable.Rows.Add(newID, editForm.TemplateName, editForm.TemplateText,
//                                    editForm.TemplateType, editForm.IsActive, editForm.Language, editForm.CreatedAt);
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"خطأ في إضافة القالب: {ex.Message}", "خطأ",
//                            MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    }
//                }
//            }

//            private void EditTemplate_Click(object sender, EventArgs e)
//            {
//                if (templatesDataGridView.SelectedRows.Count == 0) return;
//                DataGridViewRow selectedRow = templatesDataGridView.SelectedRows[0];
//                TemplateEditForm editForm = new TemplateEditForm(selectedRow);

//                if (editForm.ShowDialog() == DialogResult.OK)
//                {
//                    try
//                    {
//                        using (SqlConnection con = Db.GetConnection())
//                        {
//                            con.Open();
//                            string updateQuery = @"UPDATE MessageTemplates 
//                                               SET TemplateName=@name, TemplateText=@text, TemplateType=@type, 
//                                                   IsActive=@active, Language=@lang
//                                               WHERE TemplateID=@id";
//                            using (SqlCommand cmd = new SqlCommand(updateQuery, con))
//                            {
//                                cmd.Parameters.AddWithValue("@id", selectedRow.Cells["TemplateID"].Value);
//                                cmd.Parameters.AddWithValue("@name", editForm.TemplateName);
//                                cmd.Parameters.AddWithValue("@text", editForm.TemplateText);
//                                cmd.Parameters.AddWithValue("@type", editForm.TemplateType);
//                                cmd.Parameters.AddWithValue("@active", editForm.IsActive);
//                                cmd.Parameters.AddWithValue("@lang", editForm.Language);
//                                cmd.ExecuteNonQuery();
//                            }
//                        }

//                        selectedRow.Cells["TemplateName"].Value = editForm.TemplateName;
//                        selectedRow.Cells["TemplateText"].Value = editForm.TemplateText;
//                        selectedRow.Cells["TemplateType"].Value = editForm.TemplateType;
//                        selectedRow.Cells["IsActive"].Value = editForm.IsActive ? "نشط" : "غير نشط";
//                        selectedRow.Cells["IsActive"].Style.ForeColor = editForm.IsActive ? Color.Green : Color.Red;
//                        selectedRow.Cells["Language"].Value = editForm.Language;
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"خطأ في تعديل القالب: {ex.Message}", "خطأ",
//                            MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    }
//                }
//            }

//            private void ToggleActivation_Click(object sender, EventArgs e)
//            {
//                if (templatesDataGridView.SelectedRows.Count == 0) return;
//                DataGridViewRow selectedRow = templatesDataGridView.SelectedRows[0];
//                bool isActive = selectedRow.Cells["IsActive"].Value.ToString() == "نشط";

//                try
//                {
//                    using (SqlConnection con = Db.GetConnection())
//                    {
//                        con.Open();
//                        string updateQuery = @"UPDATE MessageTemplates SET IsActive=@active WHERE TemplateID=@id";
//                        using (SqlCommand cmd = new SqlCommand(updateQuery, con))
//                        {
//                            cmd.Parameters.AddWithValue("@id", selectedRow.Cells["TemplateID"].Value);
//                            cmd.Parameters.AddWithValue("@active", !isActive);
//                            cmd.ExecuteNonQuery();
//                        }
//                    }

//                    selectedRow.Cells["IsActive"].Value = !isActive ? "نشط" : "غير نشط";
//                    selectedRow.Cells["IsActive"].Style.ForeColor = !isActive ? Color.Green : Color.Red;
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"خطأ في تحديث الحالة: {ex.Message}", "خطأ",
//                        MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }

//            private void PreviewTemplate_Click(object sender, EventArgs e)
//            {
//                if (templatesDataGridView.SelectedRows.Count == 0) return;
//                DataGridViewRow selectedRow = templatesDataGridView.SelectedRows[0];

//                string templateText = selectedRow.Cells["TemplateText"].Value.ToString();
//                string templateName = selectedRow.Cells["TemplateName"].Value.ToString();

//                TemplatePreviewForm previewForm = new TemplatePreviewForm(templateName, templateText);
//                previewForm.ShowDialog();
//            }

//            private void RefreshTemplates_Click(object sender, EventArgs e)
//            {
//                LoadTemplatesDataFromDB();
//            }
//        }
//    // نموذج معاينة القالب
//    public class TemplatePreviewForm : Form
//    {
//        public TemplatePreviewForm(string templateName, string templateText)
//        {
//            InitializeTemplatePreviewForm(templateName, templateText);
//        }

//        private void InitializeTemplatePreviewForm(string templateName, string templateText)
//        {
//            this.Text = $"معاينة القالب: {templateName}";
//            this.Size = new Size(600, 400);
//            this.StartPosition = FormStartPosition.CenterParent;
//            this.Font = new Font("Tahoma", 9);

//            Panel mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

//            Label titleLabel = new Label
//            {
//                Text = templateName,
//                Font = new Font("Arial", 14, FontStyle.Bold),
//                ForeColor = Color.FromArgb(155, 89, 182),
//                AutoSize = true,
//                Location = new Point(20, 20)
//            };

//            TextBox textBox = new TextBox
//            {
//                Location = new Point(20, 60),
//                Size = new Size(540, 250),
//                Multiline = true,
//                Text = templateText,
//                ReadOnly = true,
//                Font = new Font("Arial", 11),
//                BackColor = Color.FromArgb(250, 250, 250),
//                BorderStyle = BorderStyle.FixedSingle,
//                ScrollBars = ScrollBars.Vertical
//            };

//            Label varsLabel = new Label
//            {
//                Text = "المتغيرات المتاحة: {Amount}, {DueDate}, {PaymentDate}, {SubscriberName}",
//                Location = new Point(20, 320),
//                Size = new Size(540, 40),
//                Font = new Font("Arial", 9),
//                ForeColor = Color.Gray
//            };

//            Button closeBtn = new Button
//            {
//                Text = "إغلاق",
//                Location = new Point(460, 320),
//                Size = new Size(100, 30),
//                DialogResult = DialogResult.OK
//            };

//            mainPanel.Controls.Add(titleLabel);
//            mainPanel.Controls.Add(textBox);
//            mainPanel.Controls.Add(varsLabel);
//            mainPanel.Controls.Add(closeBtn);
//            this.Controls.Add(mainPanel);
//        }
//    }

//}
