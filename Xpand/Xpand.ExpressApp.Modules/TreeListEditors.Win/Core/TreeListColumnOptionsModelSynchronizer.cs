using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.TreeListEditors.Win.Controllers;

namespace Xpand.ExpressApp.TreeListEditors.Win.Core {
    public class TreeListColumnOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelColumn, IModelTreeViewColumnOptionsBase> {
        public TreeListColumnOptionsModelSynchronizer(object control, IModelColumn model)
            : base(control, model) {
        }

        protected override object GetControl() {
            return ((TreeList)Control).Columns.OfType<TreeListColumn>().Where(column => column.Name == Model.PropertyName).Single();
        }

        protected override void ApplyModelCore() {
            base.ApplyModelCore();

            var columnsModel = ((IModelTreeViewColumnMainOptions) Model).TreeListColumnOptions;
            if (columnsModel.Fixed.HasValue)
                ((TreeListColumn)GetControl()).Fixed = columnsModel.Fixed.Value;
        }
    }
}