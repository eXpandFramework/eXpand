using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.PivotGrid;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.ExpressApp.ViewVariantsModule;
using Xpand.ExpressApp.Dashboard.Win;

namespace Xpand.ExpressApp.PivotGrid.Win {
    [ToolboxBitmap(typeof(XpandPivotGridWinModule))]
    [ToolboxItem(true)]
    public sealed class XpandPivotGridWinModule : XpandModuleBase {
        public XpandPivotGridWinModule() {
            RequiredModuleTypes.Add(typeof(PivotGridModule));
            RequiredModuleTypes.Add(typeof(PivotGridWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(DashboardWindowsFormsModule));
        }

    }
}