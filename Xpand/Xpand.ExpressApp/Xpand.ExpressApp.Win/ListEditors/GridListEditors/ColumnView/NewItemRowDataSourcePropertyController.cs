using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public class NewItemRowDataSourcePropertyController : ViewController {
        IColumnViewEditor gridListEditor;
        RepositoryItemLookupEdit lookupEdit;
        RepositoryItemPopupCriteriaEdit popupCriteriaEdit;

        public NewItemRowDataSourcePropertyController() {
            TypeOfView = typeof(ListView);
            TargetViewNesting = Nesting.Any;
        }

        void ReleaseGridListEditor() {
            if (gridListEditor != null) {
                if (gridListEditor.ColumnView != null) {
                    gridListEditor.ColumnView.ShowingEditor -= GridView_ShowingEditor;
                    gridListEditor.ColumnView.HiddenEditor -= GridView_HiddenEditor;
                }
                gridListEditor = null;
            }
        }

        void View_ControlsCreated(object sender, EventArgs e) {
            var listView = (ListView)View;
            ReleaseGridListEditor();
            if (listView.Editor is GridView.GridListEditorBase) {
                gridListEditor = listView.Editor as IColumnViewEditor;
                if (gridListEditor != null) {
                    gridListEditor.ColumnView.ShowingEditor += GridView_ShowingEditor;
                    gridListEditor.ColumnView.HiddenEditor += GridView_HiddenEditor;
                }
            }
        }

        void GridView_HiddenEditor(object sender, EventArgs e) {
            if (lookupEdit != null) {
                lookupEdit.QueryPopUp -= lookupEdit_QueryPopUp;
                lookupEdit = null;
            }
            if (popupCriteriaEdit != null) {
                popupCriteriaEdit.ButtonClick -= popupCriteriaEdit_ButtonClick;
                popupCriteriaEdit = null;
            }
        }

        void GridView_ShowingEditor(object sender, CancelEventArgs e) {
            var gridView = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            lookupEdit = gridView.FocusedColumn.ColumnEdit as RepositoryItemLookupEdit;
            if (lookupEdit != null) {
                lookupEdit.QueryPopUp += lookupEdit_QueryPopUp;
            }
            popupCriteriaEdit = gridView.FocusedColumn.ColumnEdit as RepositoryItemPopupCriteriaEdit;
            if (popupCriteriaEdit != null) {
                popupCriteriaEdit.ButtonClick += popupCriteriaEdit_ButtonClick;
            }
        }

        void popupCriteriaEdit_ButtonClick(object sender, ButtonPressedEventArgs e) {
            EnsureCurrentObject();
        }

        void lookupEdit_QueryPopUp(object sender, CancelEventArgs e) {
            EnsureCurrentObject();
        }

        void EnsureCurrentObject() {
            if ((View).CurrentObject == null) {
                DevExpress.XtraGrid.Views.Base.ColumnView gridView =
                    ((IColumnViewEditor)((ListView)View).Editor).ColumnView;
                gridView.ActiveEditor.IsModified = true;
                ((IGridInplaceEdit)gridView.ActiveEditor).GridEditingObject = (View).CurrentObject;
                gridView.RefreshEditor(true);
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            View.ControlsCreated += View_ControlsCreated;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.ControlsCreated -= View_ControlsCreated;
            ReleaseGridListEditor();
        }
    }
}