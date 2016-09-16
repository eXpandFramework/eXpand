using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using Xpand.ExpressApp.TreeListEditors.Controllers;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers{
    public class AutoExpandAllTreeNodesController : TreeListEditors.Controllers.AutoExpandAllTreeNodesController{
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var treeListEditor = View.Editor as TreeListEditor;
            if (treeListEditor != null&& ((IModelListViewAutoExpandAllTreeNodes) View.Model).AutoExpandAllTreeNodes){
                treeListEditor.TreeList.VisibleChanged += TreeList_VisibleChanged;
                treeListEditor.TreeList.HandleCreated += TreeList_HandleCreated;
            }
        }

        private void TreeList_VisibleChanged(object sender, EventArgs e){
            CustomizeTreeList((TreeList) sender);
        }

        private void TreeList_HandleCreated(object sender, EventArgs e){
            CustomizeTreeList((TreeList) sender);
        }

        private void CustomizeTreeList(TreeList treeList){
            if (treeList.IsHandleCreated & treeList.Visible){
                treeList.BeginInvoke(new MethodInvoker(treeList.ExpandAll));
                treeList.VisibleChanged -= TreeList_VisibleChanged;
                treeList.HandleCreated -= TreeList_HandleCreated;
            }
        }

    }
}