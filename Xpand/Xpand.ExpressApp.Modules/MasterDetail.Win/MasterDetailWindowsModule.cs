using System.ComponentModel;
using System.Drawing;
using Xpand.ExpressApp.Logic.Win;
using Xpand.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.MasterDetail.Win {
    [ToolboxBitmap(typeof(MasterDetailWindowsModule))]
    [ToolboxItem(true)]
    public sealed class MasterDetailWindowsModule : XpandModuleBase {
        public MasterDetailWindowsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(LogicWindowsModule));
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
        }

    }
}