using System;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model {
    public abstract class ColumnViewEditorLayoutStoreSynchronizer : DevExpress.ExpressApp.Model.ModelSynchronizer<IColumnViewEditor, IModelLayoutDesignStore> {
        protected ColumnViewEditorLayoutStoreSynchronizer(IColumnViewEditor control, IModelLayoutDesignStore modelNode)
            : base(control, modelNode) {
        }
        protected override void ApplyModelCore() {
            if (Model.NodeEnabled || Control.OverrideViewDesignMode)
                ApplyModelFromLayoutStore(Control.Grid.MainView);
        }

        public override void SynchronizeModel() {
            if (Control.OverrideViewDesignMode||SynchronizeModelCore())
                SaveToLayoutStore(Control.Grid.MainView);
        }

        protected virtual bool SynchronizeModelCore(){
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


    public abstract class ColumnViewEditorColumnOptionsSynchronizer<TGridDesignerEditor, TModelListViewOptionsColumnView, TModelColumn> : Persistent.Base.ModelAdapter.ModelSynchronizer<TGridDesignerEditor, TModelListViewOptionsColumnView>
        where TGridDesignerEditor : ColumnsListEditor
        where TModelListViewOptionsColumnView : IModelListViewOptionsColumnView
        where TModelColumn : IModelColumnOptionsColumnView {
        protected ColumnViewEditorColumnOptionsSynchronizer(TGridDesignerEditor control, TModelListViewOptionsColumnView modelNode)
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
            if (modelColumn.ModelMember.MemberInfo.MemberTypeInfo.IsDomainComponent) {
                propertyName += "!";
            }
            return propertyName;
        }

        protected abstract DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView();

        protected abstract IModelColumnViewColumnOptions GetColumnOptions(TModelColumn modelColumnOptionsView);

        public override void SynchronizeModel() {
            var gridColumnCollection = GetColumnView().Columns;
            foreach (var column in gridColumnCollection.OfType<GridColumn>().ToList()) {
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
        readonly IColumnViewEditor _columnViewEditor;

        protected GridViewModelSynchronizer(IColumnViewEditor columnViewEditor)
            : base((DevExpress.XtraGrid.Views.Grid.GridView)columnViewEditor.ColumnView, columnViewEditor.Model) {
            _columnsListEditor = (ColumnsListEditor)columnViewEditor;
            _columnsListEditor.ControlsCreated += ColumnsListEditorControlsCreated;
            _columnViewEditor = columnViewEditor;
        }
        private void SetupActiveFilterCriteriaToControl() {
            IObjectSpace objectSpace = _columnViewEditor.CollectionSource.ObjectSpace;
            ITypeInfo typeInfo = Model.ModelClass.TypeInfo;
            CriteriaOperator criteriaOperator = objectSpace.ParseCriteria(Model.Filter);
            if (_columnViewEditor.IsAsyncServerMode()) {
                new AsyncServerModeCriteriaProccessor(typeInfo).Process(criteriaOperator);
            }
            var criteriaProcessor = new FilterWithObjectsProcessor(objectSpace, typeInfo, _columnViewEditor.IsAsyncServerMode());
            criteriaProcessor.Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
            var enumParametersProcessor = new EnumPropertyValueCriteriaProcessor(_columnViewEditor.CollectionSource.ObjectTypeInfo);
            enumParametersProcessor.Process(criteriaOperator);
            Control.ActiveFilterCriteria = criteriaOperator;
        }
        private void ColumnsListEditorControlsCreated(object sender, EventArgs e) {
            Control.OptionsView.ShowFooter = Model.IsFooterVisible;
            Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;
            Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
            var modelListViewWin = Model;
            if (modelListViewWin != null) {
                if (_columnViewEditor.CollectionSource != null) {
                    SetupActiveFilterCriteriaToControl();
                }
                Control.ActiveFilterEnabled = (modelListViewWin).FilterEnabled;
            }
        }
        protected override void ApplyModelCore() {
            Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
            Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;
            var modelListViewWin = Model;
            if (modelListViewWin != null) {
                Control.ActiveFilterEnabled = (modelListViewWin).FilterEnabled;
                if (_columnViewEditor.CollectionSource != null) {
                    SetupActiveFilterCriteriaToControl();
                } else {
                    Control.ActiveFilterString = (modelListViewWin).Filter;
                }
            }
            var modelListViewShowAutoFilterRow = Model as IModelListViewShowAutoFilterRow;
            if (modelListViewShowAutoFilterRow != null) {
                Control.OptionsView.ShowAutoFilterRow = (modelListViewShowAutoFilterRow).ShowAutoFilterRow;
            }
            var modelListViewShowFindPanel = Model as IModelListViewShowFindPanel;
            if (modelListViewShowFindPanel != null) {
                if ((modelListViewShowFindPanel).ShowFindPanel) {
                    Control.ShowFindPanel();
                } else {
                    Control.HideFindPanel();
                }
            }
        }
        public override void SynchronizeModel() {
            Model.AutoExpandAllGroups = Control.OptionsBehavior.AutoExpandAllGroups;
            Model.IsGroupPanelVisible = Control.OptionsView.ShowGroupPanel;
            var modelListViewWin = Model;
            if (modelListViewWin != null) {
                (modelListViewWin).FilterEnabled = Control.ActiveFilterEnabled;
                if (!ReferenceEquals(Control.ActiveFilterCriteria, null) && _columnViewEditor.CollectionSource != null) {
                    (modelListViewWin).Filter = CriteriaOperator.ToString(Control.ActiveFilterCriteria);
                } else {
                    (modelListViewWin).Filter = null;
                }
            }
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
    public abstract class ListEditorModelSynchronizer : ModelListSynchronizer {
        protected ListEditorModelSynchronizer(IColumnViewEditor columnViewEditor)
            : base(columnViewEditor, columnViewEditor.Model) {
            var modelListView = (IModelListView)Model;
            ModelSynchronizerList.Add(new FooterVisibleModelSynchronizer(columnViewEditor, modelListView));
            ModelSynchronizerList.Add(new ColumnsListEditorModelSynchronizer((ColumnsListEditor)columnViewEditor, modelListView));
            ((IColumnViewEditor)Control).ColumnView.ColumnPositionChanged += Control_Changed;
        }
        public override void Dispose() {
            base.Dispose();
            var gridListEditor = Control as IColumnViewEditor;
            if (gridListEditor != null && gridListEditor.ColumnView != null) {
                gridListEditor.ColumnView.ColumnPositionChanged -= Control_Changed;
            }
        }
    }

    public static class ColumnViewExtennsions {
        public static string PropertyName(this GridColumn column) {
            var xafGridColumn = column as XafGridColumn;
            return xafGridColumn != null ? xafGridColumn.PropertyName : ((IXafGridColumn)column).PropertyName;
        }

        public static IModelColumnOptionsColumnView GetModel(this GridColumn gridColumn) {
            var xafGridColumn = gridColumn as XafGridColumn;
            if (xafGridColumn != null) {
                return (IModelColumnOptionsColumnView)xafGridColumn.Model;
            }
            var advBandedGridColumn = gridColumn as IXafGridColumn;
            return advBandedGridColumn != null ? (IModelColumnOptionsColumnView)advBandedGridColumn.Model : null;
        }
    }

}
