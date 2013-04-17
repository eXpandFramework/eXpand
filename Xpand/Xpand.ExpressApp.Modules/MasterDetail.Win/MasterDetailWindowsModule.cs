using System;
using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.MasterDetail.Win {
    [ToolboxBitmap(typeof(MasterDetailWindowsModule))]
    [ToolboxItem(true)]
    public sealed class MasterDetailWindowsModule : XpandModuleBase {
        public MasterDetailWindowsModule() {
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
            RequiredModuleTypes.Add(typeof(ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule));
        }
    }
}