using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers {
    public class TreeListInplaceEditViewController : ViewController<ListView> {
        private ObjectTreeList treeList;
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var treeListEditor = View.Editor as TreeListEditor;
            if (treeListEditor != null) {
                treeList = (ObjectTreeList)treeListEditor.TreeList;
                if (View.Model.AllowEdit) {
                    foreach (RepositoryItem ri in treeList.RepositoryItems)
                        ri.ReadOnly = false;
                    treeList.CellValueChanged += treeList_CellValueChanged;
                    treeList.ShownEditor += treeList_ShownEditor;
                    treeList.OptionsBehavior.Editable = true;
                    treeList.OptionsBehavior.ImmediateEditor = false;
                }
                treeList.OptionsView.EnableAppearanceEvenRow = true;
                treeList.OptionsView.EnableAppearanceOddRow = true;
            }
        }
        protected override void OnDeactivated() {
            if (treeList != null) {
                treeList.CellValueChanged -= treeList_CellValueChanged;
                treeList.ShownEditor -= treeList_ShownEditor;
            }
            base.OnDeactivated();
        }
        private void treeList_ShownEditor(object sender, EventArgs e) {
            var activeEditor = treeList.ActiveEditor as IGridInplaceEdit;
            if (activeEditor != null && treeList.FocusedObject is IXPSimpleObject) {
                activeEditor.GridEditingObject = treeList.FocusedObject;
            }
        }
        private void treeList_CellValueChanged(object sender, CellValueChangedEventArgs e) {
            object newValue = e.Value;
            if (e.Value is IXPSimpleObject)
                newValue = ObjectSpace.GetObject(e.Value);
            object focusedObject = treeList.FocusedObject;
            if (focusedObject != null) {
                IMemberInfo focusedColumnMemberInfo =
ObjectSpace.TypesInfo.FindTypeInfo(focusedObject.GetType()).FindMember(e.Column.FieldName);
                if (focusedColumnMemberInfo != null)
                    focusedColumnMemberInfo.SetValue(focusedObject, Convert.ChangeType(newValue,
focusedColumnMemberInfo.MemberType));
            }
        }
    }
}