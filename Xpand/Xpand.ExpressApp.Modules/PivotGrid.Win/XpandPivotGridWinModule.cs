using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.PivotGrid;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;


namespace Xpand.ExpressApp.PivotGrid.Win {
    [ToolboxBitmap(typeof(PivotGridWindowsFormsModule), "Resources.Toolbox_Module_PivotGridEditor_Win.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandPivotGridWinModule : XpandModuleBase, IDashboardUser {
        public XpandPivotGridWinModule() {
            RequiredModuleTypes.Add(typeof(PivotGridModule));
            RequiredModuleTypes.Add(typeof(PivotGridWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(DashboardModule));
        }

    }
}