using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.Web.ASPxTreeList;

namespace Xpand.ExpressApp.TreeListEditors.Web.Model {
    public class TreeListModelAdapterController : TreeListEditors.Model.TreeListModelAdapterController {

        protected override ModelSynchronizer ModelSynchronizer() {
            return new TreeListEditorDynamicModelSynchronizer(((ASPxTreeListEditor) ((ListView) View).Editor));
        }

        protected override bool GetValidEditor(){
            return ((ListView) View).Editor is ASPxTreeListEditor;
        }

        protected override Type TreeListColumnType() {
            return typeof(TreeListColumn);
        }

        protected override Type TreeListType() {
            return typeof(ASPxTreeList);
        }

    }

}
