using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
    
        public class MessageTemplate
        {
            public int TemplateID { get; set; }
            public string TemplateName { get; set; }
            public string TemplateText { get; set; }
        public string TemplateType { get; set; } // Invoice / Payment / Late
        public bool IsActive { get; set; }
            public string Language { get; set; }
            public DateTime CreatedAt { get; set; }

            public override string ToString() => TemplateName;
        }
    }