using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;
using GridListEditor = DevExpress.ExpressApp.Win.Editors.GridListEditor;
using System.Linq;
using NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition;
using DevExpress.XtraGrid.Columns;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using System.ComponentModel;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelColumnGridViewOptions : IModelNode
    {
        AutoFilterCondition AutoFilterCondition { get; set; }
        bool ImmediateUpdateAutoFilter { get; set; }
    }

    public interface IModelListViewGridViewOptions : IModelNode
    {
        EditorShowMode EditorShowMode { get; set; }
        bool AutoExpandNewRow { get; set; }
        bool DoNotLoadWhenNoFilterExists { get; set; }
        [DefaultValue(AutoFilterCondition.Contains)]
        AutoFilterCondition AutoFilterCondition { get; set; }
        [DefaultValue(true)]
        bool GuessAutoFilterRowValuesFromFilter { get; set; }
        [DefaultValue(false)]
        bool ImmediateUpdateAutoFilter { get; set; }
        int GroupLevelExpandIndex { get; set; }
    }

    public partial class GridViewViewController : BaseViewController<ListView>
    {
        private const string DoNotLoadWhenNoFilterExists = "DoNotLoadWhenNoFilterExists";
        private GridControl gridControl;
        private GridView mainView;
        private bool newRowAdded;
        private DevExpress.ExpressApp.SystemModule.FilterController filterController;

        public GridViewViewController() { }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewGridViewOptions>();
            extenders.Add<IModelColumn, IModelColumnGridViewOptions>();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            gridControl = View.Control as GridControl;
            if (gridControl == null)
                return;

            gridControl.HandleCreated += GridControl_OnHandleCreated;

            mainView = gridControl.MainView as GridView;
            if (mainView != null)
            {
                mainView.FocusedRowChanged += GridView_OnFocusedRowChanged;
                mainView.InitNewRow += GridView_OnInitNewRow;
                mainView.ShownEditor += MainViewOnShownEditor;
                SetOptions(mainView, View.Model);
            }

            if (((IModelListViewGridViewOptions)View.Model).DoNotLoadWhenNoFilterExists &&
                ((GridView)gridControl.MainView).FilterPanelText == string.Empty)
            {
                if (mainView != null) mainView.ActiveFilter.Changed += ActiveFilter_OnChanged;

                filterController = Frame.GetController<DevExpress.ExpressApp.SystemModule.FilterController>();
                filterController.FullTextFilterAction.Execute += FullTextFilterAction_Execute;
                SetDoNotLoadWhenFilterExistsCriteria();
            }
        }

        private void FullTextFilterAction_Execute(object sender, DevExpress.ExpressApp.Actions.ParametrizedActionExecuteEventArgs e)
        {
            ClearDoNotLoadWhenFilterExistsCriteria();
        }

        private void MainViewOnShownEditor(object sender, EventArgs args)
        {
            var view = (GridView)sender;
            if (view.IsFilterRow(view.FocusedRowHandle))
                view.ActiveEditor.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
        }


        private void SetDoNotLoadWhenFilterExistsCriteria()
        {
            ((ListView)View).CollectionSource.Criteria[DoNotLoadWhenNoFilterExists] = new BinaryOperator("Oid", Guid.NewGuid());
        }

        private void ClearDoNotLoadWhenFilterExistsCriteria()
        {
            ((ListView)View).CollectionSource.Criteria[DoNotLoadWhenNoFilterExists] = null;
        }

        private void ActiveFilter_OnChanged(object sender, EventArgs e)
        {
            if (((GridView)gridControl.MainView).FilterPanelText != string.Empty)
                ClearDoNotLoadWhenFilterExistsCriteria();
            else
                SetDoNotLoadWhenFilterExistsCriteria();
        }

        private void GridView_OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (newRowAdded && mainView.IsValidRowHandle(e.FocusedRowHandle))
            {
                newRowAdded = false;
                if (((IModelListViewGridViewOptions)View.Model).AutoExpandNewRow)
                    mainView.ExpandMasterRow(e.FocusedRowHandle);
            }
        }

        private void GridView_OnInitNewRow(object sender, InitNewRowEventArgs e)
        {
            newRowAdded = true;
        }

        public static void SetOptions(GridView gridView, IModelListView model)
        {
            gridView.OptionsView.NewItemRowPosition = (NewItemRowPosition)Enum.Parse(typeof(NewItemRowPosition), ((IModelListViewNewItemRow)model).NewItemRowPosition.ToString());
            gridView.OptionsBehavior.EditorShowMode = EditorShowMode.Click;
            gridView.OptionsBehavior.Editable = true;
            gridView.OptionsBehavior.AllowIncrementalSearch = true;
            gridView.OptionsBehavior.AutoSelectAllInEditor = false;
            gridView.OptionsBehavior.AutoPopulateColumns = false;
            gridView.OptionsBehavior.FocusLeaveOnTab = true;
            gridView.OptionsBehavior.AutoExpandAllGroups = model.AutoExpandAllGroups;
            gridView.OptionsSelection.MultiSelect = true;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
            gridView.OptionsNavigation.AutoFocusNewRow = true;
            gridView.OptionsNavigation.AutoMoveRowFocus = true;
            gridView.OptionsView.ShowDetailButtons = false;
            gridView.OptionsDetail.EnableMasterViewMode = false;
            gridView.OptionsView.ShowIndicator = true;
            gridView.OptionsView.ShowGroupPanel = model.IsGroupPanelVisible;
            gridView.OptionsView.ShowFooter = model.IsFooterVisible;
            gridView.OptionsView.ShowAutoFilterRow = ((IModelListViewShowAutoFilterRow)model).ShowAutoFilterRow;
            gridView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gridView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
            gridView.ActiveFilterEnabled = ((IModelListViewWin)model).IsActiveFilterEnabled;

            SetColumnOptions(gridView, model);

            if (((IModelListViewShowAutoFilterRow)model).ShowAutoFilterRow && 
                ((IModelListViewGridViewOptions)model).GuessAutoFilterRowValuesFromFilter)
            {
                gridView.GuessAutoFilterRowValuesFromFilter();
            }
        }

        public static void SetColumnOptions(GridView gridView, IModelListView listViewInfoNodeWrapper)
        {
            foreach (GridColumn column in gridView.Columns)
            {
                var columnInfo = listViewInfoNodeWrapper.Columns.Single(c => c.ModelMember.Name == column.FieldName.Replace("!", string.Empty));
                if (columnInfo != null)
                {
                    column.OptionsFilter.AutoFilterCondition = ((IModelColumnGridViewOptions)columnInfo).AutoFilterCondition;
                    column.OptionsFilter.ImmediateUpdateAutoFilter = ((IModelColumnGridViewOptions)columnInfo).ImmediateUpdateAutoFilter;
                }
            }
        }

        private void GridControl_OnHandleCreated(object sender, EventArgs e)
        {
            mainView.GridControl.ForceInitialize();

            var groupLevel = ((IModelListViewGridViewOptions)View.Model).GroupLevelExpandIndex;
            if (groupLevel > 0)
            {
                if (mainView.GroupCount == groupLevel)
                    for (int i = -1; ; i--)
                    {
                        if (!mainView.IsValidRowHandle(i)) return;
                        if (mainView.GetRowLevel(i) < groupLevel - 1)
                            mainView.SetRowExpanded(i, true);
                    }
            }
        }
    }
}