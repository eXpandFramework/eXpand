using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.FilterDataStore.Model {
    [DisplayProperty("Name"), KeyProperty("Name")]
    public interface IModelFilterDataStoreSystemTable
    {
        string Name { get; set; }
    }
}