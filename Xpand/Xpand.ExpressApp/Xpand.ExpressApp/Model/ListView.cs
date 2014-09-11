using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Model {
    [ModelAbstractClass]
    public interface IModelListViewLinq : IModelListView {
        [Category(AttributeCategoryNameProvider.Xpand)]
        string XPQueryMethod { get; set; }
    }

    [ModelAbstractClass]
    public interface IModelStaticTextEx : IModelStaticText {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        new string Text { get; set; }
    }

}
