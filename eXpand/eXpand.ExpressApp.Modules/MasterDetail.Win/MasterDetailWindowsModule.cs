using eXpand.ExpressApp.Logic.Win;
using eXpand.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public sealed class MasterDetailWindowsModule : XpandModuleBase
    {
        public MasterDetailWindowsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(LogicWindowsModule));
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
        }

    }
}