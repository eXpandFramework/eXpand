using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;

namespace Xpand.ExpressApp.MasterDetail.Win {
    [ToolboxBitmap(typeof(MasterDetailWindowsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabModules)]
    public sealed class MasterDetailWindowsModule : XpandModuleBase {
        public MasterDetailWindowsModule() {
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
            RequiredModuleTypes.Add(typeof(ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule));
        }
    }
}