using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using Xpand.ExpressApp.TreeListEditors.Model;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListModelAdapterController : TreeListModelAdapterController<TreeListEditor> {

        protected override ModelSynchronizer ModelSynchronizer() {
            return new TreeListEditorDynamicModelSynchronizer(_treeListEditor);
        }

        protected override Type TreeListColumnType() {
            return typeof(TreeListColumn);
        }

        protected override Type TreeListType() {
            return typeof(TreeList);
        }

    }
}
