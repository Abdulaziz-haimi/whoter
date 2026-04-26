using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
    {
        public class RolePermissionItem
        {
            public int PermissionID { get; set; }
            public string PermissionKey { get; set; }
            public string PermissionName { get; set; }
            public string Category { get; set; }
            public bool IsAllowed { get; set; }

            public override string ToString()
            {
                return $"{PermissionName} ({PermissionKey})";
            }
        }
    }