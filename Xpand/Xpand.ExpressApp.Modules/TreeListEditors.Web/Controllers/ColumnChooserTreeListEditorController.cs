using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxTreeList;
using Xpand.ExpressApp.TreeListEditors.Web.ListEditors;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.TreeNode;

namespace Xpand.ExpressApp.TreeListEditors.Web.Controllers {
    public class ColumnChooserTreeListEditorController:ObjectViewController<ListView,ColumnChooser>{
        private IModelListView _parentModel;
        private XpandASPxTreeListEditor _treeListEditor;
        private Frame _parentFrame;

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            _treeListEditor = ((XpandASPxTreeListEditor)View.Editor);
            _treeListEditor.TreeList.Load+=TreeListOnLoad;
            _treeListEditor.TreeList.VirtualModeNodeCreated+=TreeListOnVirtualModeNodeCreated;
            _treeListEditor.TreeList.VirtualModeCreateChildren+=TreeListOnVirtualModeCreateChildren;
            _treeListEditor.TreeList.VirtualModeNodeCreating+=TreeListOnVirtualModeNodeCreating;
            _treeListEditor.TreeList.SelectionChanged+=TreeListOnSelectionChanged;
            _parentFrame =  Frame.GetController<PopupParentFrameController>().ParentFrame;
            _parentModel = ((ListView) _parentFrame.View).Model;
        }

        private void TreeListOnLoad(object sender, EventArgs eventArgs){
            _treeListEditor.TreeList.ClientSideEvents.NodeClick = null;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_treeListEditor.TreeList != null){
                _treeListEditor.TreeList.Load -= TreeListOnLoad;
                _treeListEditor.TreeList.VirtualModeNodeCreated -= TreeListOnVirtualModeNodeCreated;
                _treeListEditor.TreeList.VirtualModeCreateChildren -= TreeListOnVirtualModeCreateChildren;
                _treeListEditor.TreeList.VirtualModeNodeCreating -= TreeListOnVirtualModeNodeCreating;
                _treeListEditor.TreeList.SelectionChanged -= TreeListOnSelectionChanged;
            }
        }

        private void TreeListOnSelectionChanged(object sender, EventArgs eventArgs){
            var visibleColumns = _parentModel.Columns.Where(column => column.Index > -1);
            var selectedObjects = _treeListEditor.GetSelectedObjects();
            var selectedKeys = selectedObjects.Cast<ColumnChooser>().Select(chooser => chooser.Key).ToArray();
            var sources = visibleColumns.Select(column => column.Id).Except(selectedKeys);
            foreach (var source in sources) {
                _parentModel.Columns[source].Index = -1;
            }

            foreach (var selectedKey in selectedKeys) {
                var modelColumn = _parentModel.Columns.FirstOrDefault(column => column.Id == selectedKey);
                if (modelColumn == null) {
                    modelColumn = _parentModel.Columns.AddNode<IModelColumn>(selectedKey);
                    modelColumn.PropertyName = selectedKey;
                    modelColumn.Index = 0;
                }
                else if (modelColumn.Index == -1)
                    modelColumn.Index = 0;
            }
        }

        private void TreeListOnVirtualModeNodeCreated(object sender, TreeListVirtualNodeEventArgs e){
            e.Node.Selected = _parentModel.Columns.Any(column => column.Index > -1 && column.Id == e.Node.Key);
        }

        private void TreeListOnVirtualModeNodeCreating(object sender, TreeListVirtualModeNodeCreatingEventArgs e){
            var columnChooser = ((ColumnChooser)e.NodeObject);
            e.IsLeaf = !columnChooser.TypeInfo.IsDomainComponent;
            e.NodeKeyValue = columnChooser.Key;
        }

        private void TreeListOnVirtualModeCreateChildren(object sender, TreeListVirtualModeCreateChildrenEventArgs e){
            var parentChooser = ((ColumnChooser) e.NodeObject);
            var columnChoosers = GetColumnChoosers(parentChooser);
            foreach (var chooser in columnChoosers){
                e.Children.Add(chooser);
            }
        }

        private BindingList<ColumnChooser> GetColumnChoosers(ColumnChooser parentChooser){
            return parentChooser!=null ? ColumnChooserList.CreateNested(ObjectSpace, parentChooser, _parentModel.Columns).Columns : ColumnChooserList.CreateRoot(ObjectSpace, _parentModel.Columns).Columns;
        }
        

    }
}


