using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ListEditors {
    public class GridViewOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelListView, IModelListViewMainViewOptionsBase>
    {
        public GridViewOptionsModelSynchronizer(object control, IModelListView model) : base(control, model) {
        }

        protected override object GetControl() {
            return Control;
        }
    }
}