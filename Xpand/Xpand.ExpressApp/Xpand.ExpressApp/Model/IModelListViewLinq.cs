using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Model {
    public interface IModelListViewLinq : IModelNode
    {
        [Category("eXpand")]
        string XPQueryMethod { get; set; }
    }
}