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
  
     
        public static class Prompt
        {
            public static string ShowDialog(string title, string text)
            {
                using (Form prompt = new Form())
                {
                    prompt.Width = 500;
                    prompt.Height = 220;
                    prompt.Text = title;
                    prompt.StartPosition = FormStartPosition.CenterParent;
                    prompt.RightToLeft = RightToLeft.Yes;
                    prompt.RightToLeftLayout = true;
                    prompt.Font = new Font("Tahoma", 10f);

                    Label lbl = new Label() { Left = 20, Top = 20, Text = text, AutoSize = true };
                    TextBox txt = new TextBox() { Left = 20, Top = 50, Width = 440, Height = 60, Multiline = true };
                    Button ok = new Button() { Text = "موافق", Left = 280, Width = 80, Top = 130, DialogResult = DialogResult.OK };
                    Button cancel = new Button() { Text = "إلغاء", Left = 380, Width = 80, Top = 130, DialogResult = DialogResult.Cancel };

                    prompt.Controls.Add(lbl);
                    prompt.Controls.Add(txt);
                    prompt.Controls.Add(ok);
                    prompt.Controls.Add(cancel);
                    prompt.AcceptButton = ok;
                    prompt.CancelButton = cancel;

                    return prompt.ShowDialog() == DialogResult.OK ? txt.Text : string.Empty;
                }
            }
        }
    }