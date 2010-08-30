using eXpand.ExpressApp.Logic.Win;
using eXpand.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public sealed class MasterDetailWindowsModule : ModuleBase
    {
        public MasterDetailWindowsModule() {
            RequiredModuleTypes.Add(typeof(eXpandSystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(LogicWindowsModule));
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
        }

    }
}