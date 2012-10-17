using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Design {
    internal class GridViewDesigner : ColumnViewDesigner {
        protected override void CreateGroups() {
            base.CreateGroups();
            var designerType = Type.GetType("DevExpress.XtraGrid.Frames.Layouts, DevExpress.XtraGrid" + XafApplication.CurrentVersion + ".Design");
            if (designerType != null)
                DefaultGroup.Add("Bands", "Customize the current view's layout and preview its data.", designerType, GetDefaultLargeImage(4), GetDefaultSmallImage(4), null);
        }
    }
}
