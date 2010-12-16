using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.TreeListEditors.Web.Controllers;

namespace Xpand.ExpressApp.TreeListEditors.Web.Core {
    public class TreeListOptionsModelSynchronizer<T> : OptionsModelSynchronizer<object, T, IModelTreeViewOptionsBase> where T:IModelNode{
        public TreeListOptionsModelSynchronizer(object control, T model)
            : base(control, model) {
        }

        protected override object GetControl() {
            return Control;
        }

    }
}
