using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.MasterDetail.Win {
    [ToolboxBitmap(typeof(MasterDetailWindowsModule))]
    [ToolboxItem(true)]
    public sealed class MasterDetailWindowsModule : ModuleBase {
        public MasterDetailWindowsModule() {
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
        }

    }
}