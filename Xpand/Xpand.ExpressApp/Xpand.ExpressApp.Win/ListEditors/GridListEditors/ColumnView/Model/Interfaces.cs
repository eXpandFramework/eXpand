using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model {

    public interface IModelListViewOptionsColumnView : IModelListView {
    }
    public interface IModelColumnViewColumnOptions : IModelNodeEnabled {

    }
    public interface IModelOptionsColumnView : IModelNodeEnabled {

    }
    [ModelAbstractClass]
    public interface IModelColumnOptionsColumnView : IModelColumn {

    }
    public interface IModelLayoutDesignStore : IModelNodeEnabled {
        [Browsable(false)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        string LayoutStore { get; set; }
    }

}
