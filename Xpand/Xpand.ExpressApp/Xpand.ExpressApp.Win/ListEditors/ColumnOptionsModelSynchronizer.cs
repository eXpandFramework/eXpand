using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.Win.ListEditors {
    public class ColumnOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelColumn, IModelColumnOptionsBase> {
        public ColumnOptionsModelSynchronizer(object control, IModelColumn model)
            : base(control, model) {
        }
        //        protected override void ApplyModelCore() {
        //            base.ApplyModelCore();
        //            var column = (XafGridColumn)GetControl();
        //            column.ShowUnboundExpressionMenu = ((IModelColumnUnbound)Model).ShowUnboundExpressionMenu;
        //        }
        //        public override void SynchronizeModel() {
        //            base.SynchronizeModel();
        //            
        //        }
        protected override object GetControl() {
            return ((XpandXafGridView)Control).Columns.OfType<XafGridColumn>().Where(column => column.Model.Id == Model.Id).Single();
        }
    }
}