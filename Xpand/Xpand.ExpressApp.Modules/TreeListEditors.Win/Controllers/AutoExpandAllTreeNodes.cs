﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraTreeList;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers{
    [ModelAbstractClass]
    public interface IModelListViewAutoExpandAllTreeNodes:IModelListView {
        [Category("eXpand.TreeList")]
        bool AutoExpandAllTreeNodes { get; set; }
    }
    public class AutoExpandAllTreeNodesController : ObjectViewController<ListView, ITreeNode>,IModelExtender{
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

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewAutoExpandAllTreeNodes>();
        }
    }
}