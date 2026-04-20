using System;
using System.Drawing;
using System.Windows.Forms;

namespace water3.Forms
{
    public partial class InvoicePrintPreviewForm : Form
    {
        private CheckBox _chkSelectAllInvoices;
        private CheckBox _chkSelectAllSms;

        // Flags لمنع إعادة الدخول (Loop)
        private bool _syncingInvoicesHeader;
        private bool _syncingSmsHeader;

        /// <summary>
        /// يركّب CheckBox في هيدر عمود التحديد (Select) ويرجعه (بدون ref).
        /// </summary>
        private CheckBox InstallSelectAllHeader(
            DataGridView grid,
            string selectColName,
            CheckBox existingChk,
            Func<bool> getSyncingFlag,
            Action<bool> setSyncingFlag)
        {
            if (grid == null) return null;
            if (!grid.Columns.Contains(selectColName)) return null;

            // لو موجود ومركّب سابقًا، فقط أعد تموضعه وارجعه
            if (existingChk != null && !existingChk.IsDisposed)
            {
                PositionHeaderCheckBox(grid, selectColName, existingChk);
                return existingChk;
            }

            var chk = new CheckBox
            {
                Size = new Size(18, 18),
                BackColor = Color.Transparent,
                ThreeState = true // مهم لدعم الحالة المختلطة
            };

            // Handler باسم متغير حتى نقدر نفصله لو احتجنا
            EventHandler headerChanged = null;
            headerChanged = (s, e) =>
            {
                // إذا نحن نزامن من الصفوف للهيدر، لا تكتب على الصفوف
                if (getSyncingFlag()) return;

                grid.EndEdit();

                // لو مختلط (Indeterminate) وخبطه المستخدم: اعتبره Checked
                bool newValue = chk.CheckState == CheckState.Indeterminate ? true : chk.Checked;

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    row.Cells[selectColName].Value = newValue;
                }

                grid.RefreshEdit();
                // بعد التطبيق: اجعلها Checked/Unchecked وليس Indeterminate
                setSyncingFlag(true);
                try
                {
                    chk.CheckState = newValue ? CheckState.Checked : CheckState.Unchecked;
                }
                finally
                {
                    setSyncingFlag(false);
                }
            };

            chk.CheckedChanged += headerChanged;

            // أضف للهيدر
            grid.Controls.Add(chk);

            // تموضع
            void position() => PositionHeaderCheckBox(grid, selectColName, chk);

            grid.Scroll += (s, e) => position();
            grid.ColumnWidthChanged += (s, e) => position();
            grid.SizeChanged += (s, e) => position();
            grid.ColumnDisplayIndexChanged += (s, e) => position();

            position();

            return chk;
        }

        private void PositionHeaderCheckBox(DataGridView grid, string selectColName, CheckBox chk)
        {
            if (chk == null || chk.IsDisposed) return;
            if (!grid.Columns.Contains(selectColName)) return;

            var rect = grid.GetCellDisplayRectangle(grid.Columns[selectColName].Index, -1, true);

            chk.Location = new Point(
                rect.X + (rect.Width - chk.Width) / 2,
                rect.Y + (rect.Height - chk.Height) / 2
            );
        }

        /// <summary>
        /// مزامنة الهيدر حسب حالة الصفوف: Checked/Unchecked/Indeterminate
        /// </summary>
        private void SyncHeaderCheckBox(
            DataGridView grid,
            string selectColName,
            CheckBox chk,
            Func<bool> getSyncingFlag,
            Action<bool> setSyncingFlag)
        {
            if (grid == null) return;
            if (chk == null || chk.IsDisposed) return;
            if (!grid.Columns.Contains(selectColName)) return;

            bool any = false;
            bool all = true;
            bool none = true;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                any = true;

                bool v = row.Cells[selectColName].Value != null &&
                         Convert.ToBoolean(row.Cells[selectColName].Value);

                if (v) none = false;
                else all = false;
            }

            // لا صفوف = رجّع Unchecked
            CheckState state;
            if (!any) state = CheckState.Unchecked;
            else if (all) state = CheckState.Checked;
            else if (none) state = CheckState.Unchecked;
            else state = CheckState.Indeterminate;

            setSyncingFlag(true);
            try
            {
                chk.CheckState = state;
            }
            finally
            {
                setSyncingFlag(false);
            }
        }

        // ======== مثال استخدام جاهز ========
        // نادِ هذه الدالة بعد إنشاء الأعمدة أو بعد Bind/Fill
        private void SetupSelectAllForInvoicesGrid(DataGridView dgvInvoices, string selectColName)
        {
            _chkSelectAllInvoices = InstallSelectAllHeader(
                dgvInvoices,
                selectColName,
                _chkSelectAllInvoices,
                () => _syncingInvoicesHeader,
                v => _syncingInvoicesHeader = v
            );

            // مزامنة عند تغيير خلايا التحديد
            dgvInvoices.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgvInvoices.Columns[e.ColumnIndex].Name != selectColName) return;

                SyncHeaderCheckBox(
                    dgvInvoices, selectColName, _chkSelectAllInvoices,
                    () => _syncingInvoicesHeader, v => _syncingInvoicesHeader = v
                );
            };

            // لازم للـ CheckBox column عشان يطلق CellValueChanged مباشرة
            dgvInvoices.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvInvoices.IsCurrentCellDirty)
                    dgvInvoices.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            // أول مزامنة
            SyncHeaderCheckBox(
                dgvInvoices, selectColName, _chkSelectAllInvoices,
                () => _syncingInvoicesHeader, v => _syncingInvoicesHeader = v
            );
        }

        private void SetupSelectAllForSmsGrid(DataGridView dgvSms, string selectColName)
        {
            _chkSelectAllSms = InstallSelectAllHeader(
                dgvSms,
                selectColName,
                _chkSelectAllSms,
                () => _syncingSmsHeader,
                v => _syncingSmsHeader = v
            );

            dgvSms.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgvSms.Columns[e.ColumnIndex].Name != selectColName) return;

                SyncHeaderCheckBox(
                    dgvSms, selectColName, _chkSelectAllSms,
                    () => _syncingSmsHeader, v => _syncingSmsHeader = v
                );
            };

            dgvSms.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvSms.IsCurrentCellDirty)
                    dgvSms.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            SyncHeaderCheckBox(
                dgvSms, selectColName, _chkSelectAllSms,
                () => _syncingSmsHeader, v => _syncingSmsHeader = v
            );
        }
    }
}