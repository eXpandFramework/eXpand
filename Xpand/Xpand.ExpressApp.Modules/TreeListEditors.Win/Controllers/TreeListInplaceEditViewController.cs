using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers {
    public class TreeListInplaceEditViewController : ViewController<ListView>, ISupportAppearanceCustomization {
        private ObjectTreeList _treeList;
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var treeListEditor = View.Editor as TreeListEditor;
            if (treeListEditor != null) {
                _treeList = (ObjectTreeList)treeListEditor.TreeList;
                if (View.Model.AllowEdit) {
                    foreach (RepositoryItem ri in _treeList.RepositoryItems)
                        ri.ReadOnly = false;
                    foreach (var columnWrapper1 in treeListEditor.Columns) {
                        var columnWrapper = (TreeListColumnWrapper) columnWrapper1;
                        IModelColumn modelColumn = View.Model.Columns[columnWrapper.PropertyName];
                        if (modelColumn != null)
                            columnWrapper.Column.OptionsColumn.AllowEdit = modelColumn.AllowEdit;
                    }
                    _treeList.CellValueChanged += treeList_CellValueChanged;
                    _treeList.ShownEditor += treeList_ShownEditor;
                    _treeList.ShowingEditor +=TreeListOnShowingEditor;
                    _treeList.OptionsBehavior.ImmediateEditor = false;
                    _treeList.OptionsBehavior.Editable = true;
                }
            }
        }
        protected override void OnDeactivated() {
            if (_treeList != null) {
                _treeList.CellValueChanged -= treeList_CellValueChanged;
                _treeList.ShownEditor -= treeList_ShownEditor;
                _treeList.ShowingEditor-=TreeListOnShowingEditor;
            }
            base.OnDeactivated();
        }

        private void TreeListOnShowingEditor(object sender, CancelEventArgs cancelEventArgs){
            var fieldName = _treeList.FocusedColumn.FieldName;
            var eventArgs = new CustomizeAppearanceEventArgs(fieldName, "ViewItem", new TreeListCancelEventArgsAppearanceAdapter(_treeList, cancelEventArgs), _treeList.FocusedObject, ViewInfo.FromView(View));
            OnCustomizeAppearance(eventArgs);
        }

        private void treeList_ShownEditor(object sender, EventArgs e) {
            var activeEditor = _treeList.ActiveEditor as IGridInplaceEdit;
            if (activeEditor != null && _treeList.FocusedObject is IXPSimpleObject) {
                activeEditor.GridEditingObject = _treeList.FocusedObject;
            }
        }
        private void treeList_CellValueChanged(object sender, CellValueChangedEventArgs e) {
            object newValue = e.Value;
            if (e.Value is IXPSimpleObject)
                newValue = ObjectSpace.GetObject(e.Value);
            object focusedObject = _treeList.FocusedObject;
            if (focusedObject != null) {
                var focusedColumnMemberInfo =ObjectSpace.TypesInfo.FindTypeInfo(focusedObject.GetType()).FindMember(e.Column.FieldName);
                if (focusedColumnMemberInfo != null){
                    var type = Convert.ChangeType(newValue,focusedColumnMemberInfo.MemberType);
                    focusedColumnMemberInfo.SetValue(focusedObject, type);
                }
            }
        }

        public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;

        protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs e){
            var handler = CustomizeAppearance;
            if (handler != null) handler(this, e);
        }
    }

    public class TreeListCancelEventArgsAppearanceAdapter : IAppearanceEnabled {
        public TreeListCancelEventArgsAppearanceAdapter(TreeList treeList, CancelEventArgs cancelEdit) {
            Args = cancelEdit;
            TreeList = treeList;
        }
        public TreeList TreeList { get; private set; }
        public CancelEventArgs Args { get; private set; }
        #region IAppearanceEnabled Members
        public bool Enabled {
            get { return !Args.Cancel; }
            set { Args.Cancel = !value; }
        }
        public void ResetEnabled() { }
        #endregion
    }

}