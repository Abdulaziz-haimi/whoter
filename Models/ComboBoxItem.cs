using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
  
        public class ComboBoxItem
        {
            public string Text { get; }
            public string Value { get; }
            public ComboBoxItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() => Text;
        }
    }
