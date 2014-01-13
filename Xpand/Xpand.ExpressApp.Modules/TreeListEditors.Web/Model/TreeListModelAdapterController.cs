using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.Web.ASPxTreeList;
using Xpand.ExpressApp.TreeListEditors.Model;

namespace Xpand.ExpressApp.TreeListEditors.Web.Model {
    public class TreeListModelAdapterController : TreeListModelAdapterController<ASPxTreeListEditor> {

        protected override ModelSynchronizer ModelSynchronizer() {
            return new TreeListEditorDynamicModelSynchronizer(TreeListEditor);
        }

        protected override Type TreeListColumnType() {
            return typeof(TreeListColumn);
        }

        protected override Type TreeListType() {
            return typeof(ASPxTreeList);
        }

    }

}
