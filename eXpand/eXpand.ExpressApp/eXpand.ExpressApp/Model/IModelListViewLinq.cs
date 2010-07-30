using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Model {
    public interface IModelListViewLinq : IModelNode
    {
        string XPQueryMethod { get; set; }
    }
}