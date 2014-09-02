using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.XtraDashboard.Win {
    [ToolboxBitmap(typeof(DashboardWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class DashboardWindowsFormsModule : XpandModuleBase {
        public DashboardWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(DashboardModule));
            RequiredModuleTypes.Add(typeof(Security.Win.XpandSecurityWinModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemRichEdit>();
        }
    }
}