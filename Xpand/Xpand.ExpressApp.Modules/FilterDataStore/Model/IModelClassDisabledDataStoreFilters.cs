using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.FilterDataStore.Model {
    public interface IModelClassDisabledDataStoreFilters:IModelClass {
        IModelDisabledDataStoreFilters DisabledDataStoreFilters { get; }
    }
}