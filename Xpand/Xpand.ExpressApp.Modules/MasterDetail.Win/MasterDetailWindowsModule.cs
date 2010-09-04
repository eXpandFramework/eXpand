using Xpand.ExpressApp.Logic.Win;
using Xpand.ExpressApp.Win.SystemModule;
using Xpand.ExpressApp;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public sealed class MasterDetailWindowsModule : XpandModuleBase
    {
        public MasterDetailWindowsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(LogicWindowsModule));
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
        }

    }
}