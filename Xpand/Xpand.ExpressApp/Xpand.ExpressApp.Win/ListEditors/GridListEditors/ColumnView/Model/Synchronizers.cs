using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Fasterflect;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model {
    public abstract class ColumnViewEditorLayoutStoreSynchronizer : DevExpress.ExpressApp.Model.ModelSynchronizer<WinColumnsListEditor, IModelLayoutDesignStore> {
        protected ColumnViewEditorLayoutStoreSynchronizer(WinColumnsListEditor control, IModelLayoutDesignStore modelNode)
            : base(control, modelNode) {
        }
        protected override void ApplyModelCore() {
            if (Model.NodeEnabled || (Control is IColumnViewEditor && ((IColumnViewEditor)Control).OverrideViewDesignMode)) {
                ApplyModelFromLayoutStore(Control.Grid.MainView);
            }
        }

        public override void SynchronizeModel() {
            if (Control is IColumnViewEditor && (((IColumnViewEditor)Control).OverrideViewDesignMode||SynchronizeModelCore()))
                SaveToLayoutStore(Control.Grid.MainView);
        }

        protected virtual bool SynchronizeModelCore() {
            return false;
        }

        OptionsLayoutGrid GetOptionsLayoutGrid() {
            var optionsLayoutGrid = new OptionsLayoutGrid();

            optionsLayoutGrid.Columns.StoreLayout = true;
            optionsLayoutGrid.Columns.StoreAppearance = false;
            optionsLayoutGrid.Columns.RemoveOldColumns = false;
            optionsLayoutGrid.Columns.AddNewColumns = false;

            optionsLayoutGrid.StoreVisualOptions = false;
            optionsLayoutGrid.StoreDataSettings = false;
            optionsLayoutGrid.StoreAppearance = false;

            optionsLayoutGrid.StoreAllOptions = false;
            return optionsLayoutGrid;
        }
        void SaveToLayoutStore(BaseView baseView) {
            using (var saveStream = new MemoryStream()) {
                baseView.SaveLayoutToStream(saveStream, GetOptionsLayoutGrid());
                Model.LayoutStore = Encoding.UTF8.GetString(saveStream.ToArray());
            }
        }
        void ApplyModelFromLayoutStore(BaseView gridView) {
            var optionsLayoutGrid = GetOptionsLayoutGrid();
            if (!string.IsNullOrEmpty(Model.LayoutStore)) {
                using (var restoreStream = new MemoryStream(Encoding.UTF8.GetBytes(Model.LayoutStore))) {
                    gridView.RestoreLayoutFromStream(restoreStream, optionsLayoutGrid);
                }
            }
        }

    }


    public abstract class ColumnViewEditorColumnOptionsSynchronizer<TComponent, TModelListViewOptionsColumnView, TModelColumn> : Persistent.Base.ModelAdapter.ModelSynchronizer<TComponent, TModelListViewOptionsColumnView>
        where TModelListViewOptionsColumnView : IModelListViewOptionsColumnView
        where TModelColumn : IModelColumnOptionsColumnView {
        protected ColumnViewEditorColumnOptionsSynchronizer(TComponent control, TModelListViewOptionsColumnView modelNode)
            : base(control, modelNode) {
        }
        protected override void ApplyModelCore() {
            var gridColumnCollection = GetColumnView().Columns;
            foreach (var modelColumn in Model.Columns.OfType<TModelColumn>()) {
                var layoutViewColumn = gridColumnCollection[GetPropertyName(modelColumn)];
                var columnOptions = GetColumnOptions(modelColumn);
                if (columnOptions.NodeEnabled)
                    ApplyModel(columnOptions, layoutViewColumn, ApplyValues);
            }
        }

        string GetPropertyName(TModelColumn modelColumn) {
            var propertyName = modelColumn.PropertyName;
            if (modelColumn.ModelMember != null && modelColumn.ModelMember.MemberInfo.MemberTypeInfo.IsDomainComponent) {
                propertyName += "!";
            }
            return propertyName;
        }

        protected abstract DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView();

        protected abstract IModelColumnViewColumnOptions GetColumnOptions(TModelColumn modelColumnOptionsView);

        public override void SynchronizeModel() {
            var gridColumnCollection = GetColumnView().Columns;
            foreach (var column in gridColumnCollection.ToList()) {
                var propertyName = column.PropertyName();
                if (Model.Columns[propertyName] is TModelColumn) {
                    var modelColumn = (TModelColumn)Model.Columns[propertyName];
                    var columnOptions = GetColumnOptions(modelColumn);
                    if (columnOptions.NodeEnabled)
                        ApplyModel(columnOptions, column, SynchronizeValues);
                }
            }
        }


    }

    public abstract class GridViewModelSynchronizer : DevExpress.ExpressApp.Model.ModelSynchronizer<DevExpress.XtraGrid.Views.Grid.GridView, IModelListView> {
        private readonly ColumnsListEditor _columnsListEditor;


        protected GridViewModelSynchronizer(GridListEditor columnViewEditor)
            : base(columnViewEditor.GridView, columnViewEditor.Model) {
            _columnsListEditor = columnViewEditor;
            _columnsListEditor.ControlsCreated += ColumnsListEditorControlsCreated;
        }
        private void ColumnsListEditorControlsCreated(object sender, EventArgs e) {
            Control.OptionsView.ShowFooter = Model.IsFooterVisible;
            Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;
            Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
        }
        protected override void ApplyModelCore() {
            Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
            Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;

            var modelListViewShowAutoFilterRow = Model as IModelListViewShowAutoFilterRow;
            if (modelListViewShowAutoFilterRow != null) {
                Control.OptionsView.ShowAutoFilterRow = (modelListViewShowAutoFilterRow).ShowAutoFilterRow;
            }
            var modelListViewShowFindPanel = Model as IModelListViewShowFindPanel;
            if (modelListViewShowFindPanel != null) {
                if ((modelListViewShowFindPanel).ShowFindPanel) {
                    Control.ShowFindPanel();
                }
                else {
                    Control.HideFindPanel();
                }
            }
        }
        public override void SynchronizeModel() {
            Model.AutoExpandAllGroups = Control.OptionsBehavior.AutoExpandAllGroups;
            Model.IsGroupPanelVisible = Control.OptionsView.ShowGroupPanel;
            var modelListViewShowAutoFilterRow = Model as IModelListViewShowAutoFilterRow;
            if (modelListViewShowAutoFilterRow != null) {
                (modelListViewShowAutoFilterRow).ShowAutoFilterRow = Control.OptionsView.ShowAutoFilterRow;
            }
            var modelListViewShowFindPanel = Model as IModelListViewShowFindPanel;
            if (modelListViewShowFindPanel != null) {
                (modelListViewShowFindPanel).ShowFindPanel = Control.IsFindPanelVisible;
            }
        }
        public override void Dispose() {
            base.Dispose();
            if (_columnsListEditor != null) {
                _columnsListEditor.ControlsCreated -= ColumnsListEditorControlsCreated;
            }
        }
    }

    public class ListEditorModelSynchronizer : ModelListSynchronizer {
        public ListEditorModelSynchronizer(WinColumnsListEditor columnViewEditor, IEnumerable<IModelSynchronizable> modelSynchronizers)
            : this(columnViewEditor) {
            ModelSynchronizerList.AddRange(modelSynchronizers);
        }

        public ListEditorModelSynchronizer(WinColumnsListEditor columnViewEditor)
            : base(columnViewEditor, columnViewEditor.Model) {
            ((WinColumnsListEditor)Control).ColumnView.ColumnPositionChanged += Control_Changed;
        }

        public override void Dispose() {
            base.Dispose();
            var gridListEditor = Control as WinColumnsListEditor;
            if (gridListEditor != null && gridListEditor.ColumnView != null) {
                gridListEditor.ColumnView.ColumnPositionChanged -= Control_Changed;
            }
        }
    }

    public static class ColumnViewExtennsions {
        public static GridColumn Column(this ColumnWrapper columnWrapper) {
            return columnWrapper.GetPropertyValue("Column") as GridColumn;
        }

        public static Form CreateFilterBuilderDialogEx(this DevExpress.XtraGrid.Views.Base.ColumnView columnView, FilterColumnCollection filterColumns, FilterColumn defaultFilterColumn, IEnumerable<IModelMember> modelMembers) {
            return new XpandFilterBuilder(filterColumns, columnView.GridControl.MenuManager, columnView.GridControl.LookAndFeel, columnView, defaultFilterColumn, modelMembers);
        }

        public static string PropertyName(this GridColumn column) {
            IGridColumnModelSynchronizer columnInfo = GetGridColumnInfo(column);
            return columnInfo != null ? columnInfo.PropertyName : column.FieldName;
        }

        private static IGridColumnModelSynchronizer GetGridColumnInfo(GridColumn column) {
            return column != null && column.View is IModelSynchronizersHolder
                ? ((IModelSynchronizersHolder)column.View).GetSynchronizer(column) as IGridColumnModelSynchronizer
                : null;
        }

        public static IModelColumn GetModel(this GridColumn gridColumn) {
            var columnInfo = GetGridColumnInfo(gridColumn);
            return columnInfo != null ? columnInfo.Model : null;
        }

        public static IModelColumn Model(this GridColumn gridColumn) {
            return gridColumn.GetModel();
        }

    }

}
