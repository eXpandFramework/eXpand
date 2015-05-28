using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Model {
    [ModelAbstractClass]
    public interface IModelListViewLinq : IModelListView {
        [Category(AttributeCategoryNameProvider.Xpand)]
        string XPQueryMethod { get; set; }
    }
}
