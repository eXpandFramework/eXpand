using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.ListEditors {
    public class ColumnOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelColumn, IModelColumnOptionsBase> {
        public ColumnOptionsModelSynchronizer(object control, IModelColumn model)
            : base(control, model) {
        }
        protected override void ApplyModelCore() {
            base.ApplyModelCore();
            var modelColumnUnbound = Model as IModelColumnUnbound;
            if (modelColumnUnbound != null) {
                var columnWithInfo = ((GridViewDataColumnWithInfo)GetControl());
                columnWithInfo.UnboundType = UnboundColumnType.Integer;
                columnWithInfo.UnboundExpression = "1+1";
                columnWithInfo.FieldName = modelColumnUnbound.Id;
            }
        }
        public override void SynchronizeModel() {
            base.SynchronizeModel();
            var modelColumnUnbound = Model as IModelColumnUnbound;
            if (modelColumnUnbound != null) {
                var columnWithInfo = ((GridViewDataColumnWithInfo)GetControl());
                modelColumnUnbound.UnboundExpression = columnWithInfo.UnboundExpression;
            }
        }
        protected override object GetControl() {
            return ((ASPxGridView)Control).Columns.OfType<GridViewDataColumnWithInfo>().Where(column => column.Model == Model).Single();
        }
    }
}