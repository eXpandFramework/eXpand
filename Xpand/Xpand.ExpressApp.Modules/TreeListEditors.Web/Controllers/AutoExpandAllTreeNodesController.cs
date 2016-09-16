using System;
using DevExpress.ExpressApp.TreeListEditors.Web;
using Xpand.ExpressApp.TreeListEditors.Controllers;

namespace Xpand.ExpressApp.TreeListEditors.Web.Controllers {
    public class AutoExpandAllTreeNodesController: TreeListEditors.Controllers.AutoExpandAllTreeNodesController {
        private ASPxTreeListEditor _treeListEditor;

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            _treeListEditor = View.Editor as ASPxTreeListEditor;
            if (_treeListEditor != null&& ((IModelListViewAutoExpandAllTreeNodes) View.Model).AutoExpandAllTreeNodes) {
                _treeListEditor.DataBinder.DataBound +=DataBinderOnDataBound;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_treeListEditor?.DataBinder != null) _treeListEditor.DataBinder.DataBound-=DataBinderOnDataBound;
        }

        private void DataBinderOnDataBound(object sender, EventArgs eventArgs){
            _treeListEditor.TreeList.ExpandAll();
        }
    }
}
