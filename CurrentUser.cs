
using System;
using System.Collections.Generic;
using System.Linq;
using water3.Models;

namespace water3
{
    public static class CurrentUser
    {
        public static int UserID { get; set; }
        public static string UserName { get; set; } = string.Empty;
        public static string FullName { get; set; } = string.Empty;
        public static int RoleID { get; set; }
        public static string RoleName { get; set; } = string.Empty;
        public static bool IsLoggedIn { get; set; }

        public static List<AppPermission> Permissions { get; set; } = new List<AppPermission>();

        public static bool HasPermission(string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
                return false;

            return Permissions.Any(p =>
                string.Equals(p.PermissionKey, permissionKey, StringComparison.OrdinalIgnoreCase)
                && p.IsAllowed);
        }

        public static void Clear()
        {
            UserID = 0;
            UserName = string.Empty;
            FullName = string.Empty;
            RoleID = 0;
            RoleName = string.Empty;
            IsLoggedIn = false;
            Permissions = new List<AppPermission>();
        }
    }
}