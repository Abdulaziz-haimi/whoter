using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{


        public class AuditLogItem
        {
            public int LogID { get; set; }
            public int? UserID { get; set; }
            public string UserName { get; set; }
            public string Action { get; set; }
            public string TableName { get; set; }
            public int? RecordID { get; set; }
            public DateTime? ActionDate { get; set; }
            public string Details { get; set; }
            public string EntityName { get; set; }
            public string DeviceName { get; set; }
        }
    }