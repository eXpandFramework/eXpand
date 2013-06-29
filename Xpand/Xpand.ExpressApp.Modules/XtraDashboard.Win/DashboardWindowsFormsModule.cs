using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.XtraDashboard.Win.PropertyEditors;

namespace Xpand.ExpressApp.XtraDashboard.Win {
    [ToolboxBitmap(typeof(DashboardWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class DashboardWindowsFormsModule : XpandModuleBase {
        public DashboardWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(DashboardModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorDashboardViewEditor>();
        }
    }
}