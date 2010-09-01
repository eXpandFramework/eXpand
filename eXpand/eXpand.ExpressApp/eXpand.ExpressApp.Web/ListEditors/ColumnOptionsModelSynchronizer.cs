using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;
using eXpand.ExpressApp.ListEditors;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Web.ListEditors {
    public class ColumnOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelColumn, IModelColumnOptionsBase>
    {
        public ColumnOptionsModelSynchronizer(object control, IModelColumn model)
            : base(control, model)
        {
        }

        protected override object GetControl()
        {
            return ((ASPxGridView)Control).Columns.OfType<GridViewDataColumnWithInfo>().Where(column => column.Model == Model).Single();
        }
    }
}