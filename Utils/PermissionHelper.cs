using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace water3.Utils
    {
        public static class PermissionHelper
        {
            public static void EnforceFormPermission(Form form, string permissionKey)
            {
                if (form == null)
                    return;

                if (!CurrentUser.HasPermission(permissionKey))
                {
                    MessageBox.Show(
                        "ليس لديك صلاحية للوصول إلى هذه الشاشة.",
                        "منع الوصول",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    form.Shown += (s, e) => form.Close();
                }
            }

            public static void ApplyControlPermission(Control control, string permissionKey, bool hideControl = true)
            {
                if (control == null)
                    return;

                bool allowed = CurrentUser.HasPermission(permissionKey);

                if (hideControl)
                    control.Visible = allowed;
                else
                    control.Enabled = allowed;
            }
        }
    }