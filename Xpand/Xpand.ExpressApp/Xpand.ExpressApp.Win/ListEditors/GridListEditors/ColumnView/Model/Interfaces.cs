using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model {


    public interface IModelLayoutDesignStore : IModelNodeEnabled {
        [Browsable(false)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        string LayoutStore { get; set; }
    }

}
