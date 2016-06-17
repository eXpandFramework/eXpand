using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.FilterDataStore.Model {
    [KeyProperty("Name")]
    [DisplayProperty("Name")]
    public interface IModelDisabledDataStoreFilter : IModelNode {
        [Required]
        string Name { get; set; }
    }
}