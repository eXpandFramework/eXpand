using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Xpand.ExpressApp.XtraDashboard.Win.PropertyEditors;
using ItemCheckEventArgs = DevExpress.XtraEditors.Controls.ItemCheckEventArgs;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    public class DisableMultiDashboardTypeController : Dashboard.Controllers.DisableMultiDashboardTypeController {
        protected override void OnControlCreated(object sender, EventArgs e) {
            ((CheckedComboBoxEdit) ((DashboardTypesEditor) sender).Control).Popup += OnPopup;
        }

        private void OnPopup(object sender, EventArgs e) {
            ((CheckedComboBoxEdit) sender).InternalListBox.ItemCheck += OnItemCheck;
        }

        private void OnItemCheck(object sender, ItemCheckEventArgs e) {
            if (e.State == CheckState.Checked) {
                var list = (CheckedListBoxControl) sender;
                var items = new List<CheckedListBoxItem>();
                foreach (var index in list.CheckedIndices) {
                    if (index == e.Index) continue;
                    items.Add(list.Items[index]);
                }

                foreach (var item in items)
                    item.CheckState = CheckState.Unchecked;
            }
        }
    }
}