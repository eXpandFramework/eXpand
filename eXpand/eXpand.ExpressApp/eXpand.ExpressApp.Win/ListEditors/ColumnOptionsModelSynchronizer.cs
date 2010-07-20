using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.ListEditors;
using eXpand.ExpressApp.SystemModule;
using System.Linq;

namespace eXpand.ExpressApp.Win.ListEditors {
    public class ColumnOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelColumn, IModelColumnOptionsBase>
    {
        public ColumnOptionsModelSynchronizer(object control, IModelColumn model)
            : base(control, model)
        {
        }

        protected override object GetControl() {
            return ((XafGridView)Control).Columns.OfType<XafGridColumn>().Where(column => column.PropertyName == Model.PropertyName).Single();
        }
    }
}