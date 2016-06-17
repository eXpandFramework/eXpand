using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.FilterDataStore.Model {
    [DisplayProperty("Name"), KeyProperty("Name")]
    public interface IModelFilterDataStoreSystemTable:IModelNode
    {
        string Name { get; set; }
    }
}