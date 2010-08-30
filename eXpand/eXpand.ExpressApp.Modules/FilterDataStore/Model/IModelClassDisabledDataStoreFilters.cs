using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.FilterDataStore.Model {
    public interface IModelClassDisabledDataStoreFilters:IModelClass {
        IModelDisabledDataStoreFilters DisabledDataStoreFilters { get; }
    }
}