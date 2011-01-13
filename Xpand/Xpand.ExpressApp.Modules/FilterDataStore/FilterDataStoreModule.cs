using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.FilterDataStore.Core;
using Xpand.ExpressApp.FilterDataStore.Model;

namespace Xpand.ExpressApp.FilterDataStore {
    public sealed partial class FilterDataStoreModule : XpandModuleBase {
        public FilterDataStoreModule() {
            InitializeComponent();
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelClass, IModelClassDisabledDataStoreFilters>();
            extenders.Add<IModelApplication, IModelApplicationFilterDataStore>();
        }
    }
}