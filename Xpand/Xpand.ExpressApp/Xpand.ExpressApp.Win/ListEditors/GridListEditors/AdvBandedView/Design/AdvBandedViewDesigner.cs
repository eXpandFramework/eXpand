using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Design {
    internal class AdvBandedViewDesigner : ColumnViewDesigner {
        protected override void CreateGroups() {
            base.CreateGroups();
            var designerType = Type.GetType("DevExpress.XtraGrid.Frames.BandDesigner, DevExpress.XtraGrid" + XafApplication.CurrentVersion + ".Design");
            if (designerType != null)
                DefaultGroup.Add("Bands", "Adjust the Band collection of the current view and customize the layout of bands.", designerType, GetDefaultLargeImage(2), GetDefaultSmallImage(2), null);
        }
    }
}
