using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.FilterDataStore.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.FilterDataStore {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class FilterDataStoreModule : XpandModuleBase {

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelClass, IModelClassDisabledDataStoreFilters>();
            extenders.Add<IModelApplication, IModelApplicationFilterDataStore>();
        }

    }
}