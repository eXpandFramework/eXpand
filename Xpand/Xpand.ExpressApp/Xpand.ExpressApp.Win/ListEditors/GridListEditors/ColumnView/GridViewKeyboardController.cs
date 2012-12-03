using System;
using System.Collections;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public class GridViewKeyboardController : ViewController, IModelExtender {
        GridColumn currentColumn;
        BarShortcut filterShortcut;
        BarShortcut groupShortcut;
        BarShortcut navigateBackShortcut;
        BarShortcut navigateForwardShortcut;
        BarShortcut sortShortcut;

        public GridViewKeyboardController() {
            TargetViewType = ViewType.ListView;
        }
        #region IModelExtender Members
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelListViewShortcuts>();
        }
        #endregion
        void GridViewKeyboardController_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e) {
            if (e.Column == currentColumn) e.Info.State = ObjectState.Hot;
        }

        void Group(DevExpress.XtraGrid.Views.Grid.GridView gridView) {
            if (gridView.OptionsView.ShowGroupPanel) {
                if (currentColumn != null && currentColumn.GroupIndex == -1) {
                    currentColumn.Group();
                } else {
                    if (currentColumn != null) {
                        currentColumn.UnGroup();
                    }
                }
            }
        }

        void ChangeSort(GridColumn column) {
            foreach (GridColumn gridColumn in column.View.Columns) {
                if (gridColumn != column) {
                    gridColumn.SortOrder = ColumnSortOrder.None;
                }
            }
            if (column.SortOrder == ColumnSortOrder.Ascending) {
                column.SortOrder = ColumnSortOrder.Descending;
            } else {
                if (column.SortOrder == ColumnSortOrder.Descending) {
                    column.SortOrder = column.GroupIndex == -1 ? ColumnSortOrder.None : ColumnSortOrder.Ascending;
                } else {
                    column.SortOrder = ColumnSortOrder.Ascending;
                }
            }
        }

        void Sort() {
            if (currentColumn != null) {
                ChangeSort(currentColumn);
            }
        }

        void Navigate(DevExpress.XtraGrid.Views.Grid.GridView gridView, bool back) {
            if (currentColumn == null) {
                currentColumn = gridView.FocusedColumn;
            }
            if (currentColumn != null) {
                var columns = new ArrayList();
                columns.AddRange(gridView.GroupedColumns);
                columns.AddRange(gridView.VisibleColumns);
                int index = currentColumn.GroupIndex > -1
                                ? currentColumn.GroupIndex
                                : gridView.GroupedColumns.Count + currentColumn.VisibleIndex;
                int nextIndex = index + (back ? -1 : 1);
                if (nextIndex < 0) {
                    nextIndex = columns.Count - 1;
                }
                if (nextIndex >= columns.Count) {
                    nextIndex = 0;
                }
                currentColumn = (GridColumn)columns[nextIndex];
                gridView.GridControl.Invalidate();
            }
        }

        void Init() {
            var listViewShortcuts = (IModelListViewShortcuts)Application.Model.Options;
            filterShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.FilterShortcut);
            groupShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.GroupShortcut);
            sortShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.SortShortcut);
            navigateBackShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.NavigateBackShortcut);
            navigateForwardShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.NavigateForwardShortcut);
        }

        GridListEditorBase GetGridListEditor() {
            var listView = (ListView)View;
            var gridListEditor = listView.Editor as GridListEditorBase;
            return gridListEditor;
        }

        void View_ControlsCreated(object sender, EventArgs e) {
            GridListEditorBase gridListEditor = GetGridListEditor();
            if (gridListEditor != null) {
                gridListEditor.Grid.KeyDown += GridViewKeyboardController_KeyDown;
                ((DevExpress.XtraGrid.Views.Grid.GridView)gridListEditor.GridView).CustomDrawColumnHeader += GridViewKeyboardController_CustomDrawColumnHeader;
            }
        }

        void GridViewKeyboardController_KeyDown(object sender, KeyEventArgs e) {
            var gridView = ((GridControl)sender).MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            #region Remove try-catch when  B32997 is fixed
            try {
                if (filterShortcut != null && e.KeyData == filterShortcut.Key && currentColumn != null) {
                    if (gridView != null) gridView.ShowFilterPopup(currentColumn);
                }
            } catch (Exception) {
            }
            try {
                if (groupShortcut != null && e.KeyData == groupShortcut.Key) {
                    Group(gridView);
                }
            } catch (Exception) {
            }
            try {
                if (sortShortcut != null && e.KeyData == sortShortcut.Key) {
                    Sort();
                }
            } catch (Exception) {
            }
            if (e.KeyCode == Keys.Tab) {
                currentColumn = null;
            }
            try {
                if (navigateForwardShortcut != null && e.KeyData == navigateForwardShortcut.Key) {
                    Navigate(gridView, false);
                    e.Handled = true;
                }
            } catch (Exception) {
            }
            try {
                if (navigateBackShortcut != null && e.KeyData == navigateBackShortcut.Key) {
                    Navigate(gridView, true);
                    e.Handled = true;
                }
            } catch (Exception) {
            }
            #endregion
        }

        protected override void OnActivated() {
            base.OnActivated();
            Init();
            View.ControlsCreated += View_ControlsCreated;
        }

        protected override void OnDeactivated() {
            currentColumn = null;
            View.ControlsCreated -= View_ControlsCreated;
            GridListEditorBase gridListEditor = GetGridListEditor();
            if (gridListEditor != null && gridListEditor.Grid != null) {
                gridListEditor.Grid.KeyDown -= GridViewKeyboardController_KeyDown;
                ((DevExpress.XtraGrid.Views.Grid.GridView)gridListEditor.GridView).CustomDrawColumnHeader -= GridViewKeyboardController_CustomDrawColumnHeader;
            }
            base.OnDeactivated();
        }
    }
}