using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxTreeList;
using Xpand.ExpressApp.TreeListEditors.Web.ListEditors;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.TreeNode;

namespace Xpand.ExpressApp.TreeListEditors.Web.Controllers {
    public class ColumnChooserTreeListEditorController:ObjectViewController<ListView,ColumnChooser>{
        private IModelListView _listViewModel;
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
            _treeListEditor.TreeList.FocusedNodeChanged+=TreeListOnFocusedNodeChanged;
            _parentFrame =  Frame.GetController<PopupParentFrameController>().ParentFrame;
            _listViewModel = ((ListView) _parentFrame.View).Model;
        }

        private void TreeListOnFocusedNodeChanged(object sender, EventArgs eventArgs){
            
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            foreach (var key in _unSelectedKeys){
                var modelColumn = _listViewModel.Columns.FirstOrDefault(column => column.Id==key);
                if (modelColumn!=null){
                    if (modelColumn.PropertyName.Contains(".") && modelColumn.PropertyName == modelColumn.Id)
                        modelColumn.Remove();
                    else
                        modelColumn.Index = -1;
                }
            }
            foreach (var keyValuePair in _selectedKeys){
                var modelColumn = _listViewModel.Columns.FirstOrDefault(column => column.Id == keyValuePair.Key);
                if (modelColumn == null) {
                    modelColumn = _listViewModel.Columns.AddNode<IModelColumn>(keyValuePair.Key);
                    modelColumn.PropertyName = keyValuePair.Value;
                }
                if (modelColumn.Index == null || modelColumn.Index < 0)
                    modelColumn.Index = 0;
            }
            if (_treeListEditor.TreeList != null){
                _treeListEditor.TreeList.Load -= TreeListOnLoad;
                _treeListEditor.TreeList.VirtualModeNodeCreated -= TreeListOnVirtualModeNodeCreated;
                _treeListEditor.TreeList.VirtualModeCreateChildren -= TreeListOnVirtualModeCreateChildren;
                _treeListEditor.TreeList.VirtualModeNodeCreating -= TreeListOnVirtualModeNodeCreating;
                _treeListEditor.TreeList.SelectionChanged -= TreeListOnSelectionChanged;
            }
        }
        private void TreeListOnLoad(object sender, EventArgs eventArgs){
            _treeListEditor.TreeList.ClientSideEvents.NodeClick = null;
        }

        readonly Dictionary<string,string> _selectedKeys=new Dictionary<string, string>();
        readonly IList<string> _unSelectedKeys=new List<string>();

        private void TreeListOnSelectionChanged(object sender, EventArgs eventArgs){
            var treeListFocusedNode = _treeListEditor.TreeList.FocusedNode;
            var key = treeListFocusedNode.Key;
            if (treeListFocusedNode.Selected){
                _selectedKeys.Add(key, ((ColumnChooser) _treeListEditor.TreeList.GetVirtualNodeObject(treeListFocusedNode)).PropertyName);
                _unSelectedKeys.Remove(key);
            }
            else{
                _selectedKeys.Remove(key);
                _unSelectedKeys.Add(key);
            }
        }

        private void TreeListOnVirtualModeNodeCreated(object sender, TreeListVirtualNodeEventArgs e){
            e.Node.Selected = (_listViewModel.Columns.Any(column => (column.Index == null || column.Index > -1) && column.Id == e.Node.Key) &&
                _unSelectedKeys.All(key => key != e.Node.Key))||_selectedKeys.Any(keyValuePair => keyValuePair.Key==e.Node.Key);
        }

        private void TreeListOnVirtualModeNodeCreating(object sender, TreeListVirtualModeNodeCreatingEventArgs e){
            var columnChooser = ((ColumnChooser)e.NodeObject);
            e.IsLeaf = !columnChooser.ModelColumn.ModelMember.MemberInfo.MemberTypeInfo.IsDomainComponent;
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
            if (parentChooser == null)
                return ColumnChooserList.Create(ObjectSpace, _listViewModel.Columns.ToArray()).Columns;
            var columnIds = _listViewModel.Columns.Select(column => column.Id).ToArray();
            var modelColumns = ((IColumnChooserListView)parentChooser.ModelColumn).ColumnChooserListView.Columns.ToArray();
            var columnChoosers = ColumnChooserList.Create(ObjectSpace, modelColumns, parentChooser).Columns.Where(chooser => !columnIds.Contains(chooser.PropertyName)).ToArray();
            return new BindingList<ColumnChooser>(columnChoosers);
        }
        

    }
}


