using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.ListEditors {
    public class ColumnOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelColumn, IModelColumnOptionsBase> {
        public ColumnOptionsModelSynchronizer(object control, IModelColumn model)
            : base(control, model) {
        }
        protected override object GetControl() {
            return ((XafGridView)Control).Columns.OfType<XafGridColumn>().Single(column => column.Model.Id == Model.Id);
        }
    }
}