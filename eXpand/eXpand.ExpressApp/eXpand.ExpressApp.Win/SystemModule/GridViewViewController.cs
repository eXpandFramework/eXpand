using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;
using NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition;

namespace eXpand.ExpressApp.Win.SystemModule {
    public interface IModelMemberGridViewOptions : IModelNode {
        [Category("eXpand")]
        [DefaultValue(AutoFilterCondition.Contains)]
        [Description("Control gridview property")]
        AutoFilterCondition AutoFilterCondition { get; set; }

        [Category("eXpand")]
        [Description("Control gridview property")]
        bool ImmediateUpdateAutoFilter { get; set; }
    }

    
    public interface IModelColumnGridViewOptions : IModelNode {
        [Category("eXpand")]
        [DefaultValue(AutoFilterCondition.Contains)]
        [ModelValueCalculator("((IModelMemberGridViewOptions)ModelMember)", "AutoFilterCondition")]
        [Description("Control gridview property")]
        AutoFilterCondition AutoFilterCondition { get; set; }

        [Category("eXpand")]
        [ModelValueCalculator("((IModelMemberGridViewOptions)ModelMember)", "ImmediateUpdateAutoFilter")]
        [Description("Control gridview property")]
        bool ImmediateUpdateAutoFilter { get; set; }
    }

    [DomainLogic(typeof (IModelColumnGridViewOptions))]
    public class ModelColumnGridViewOptionsLogic {
        public static AutoFilterCondition Get_AutoFilterCondition(IModelColumnGridViewOptions node) {
            if (((IModelColumn) node).ModelMember != null && (node as IModelColumn).ModelMember.Type == typeof (String)) {
                return AutoFilterCondition.Contains;
            }
            return AutoFilterCondition.Default;
        }
    }
    public interface IModelClassGridViewOptions : IModelNode {
        [Category("eXpand")]
        [Description("Control gridview property")]
        EditorShowMode EditorShowMode { get; set; }

        [Category("eXpand")]
        [Description("If gridview in master detail auto expand new inserter row")]
        bool AutoExpandNewRow { get; set; }

        [Category("eXpand")]
        [Description("Only loads listview records when a filter is present")]
        bool DoNotLoadWhenNoFilterExists { get; set; }

        [DefaultValue(true)]
        [Category("eXpand")]
        [Description("Control gridview method")]
        bool GuessAutoFilterRowValuesFromFilter { get; set; }

        [Category("eXpand")]
        [Description("Expand all groups of a gridview up to this level")]
        int GroupLevelExpandIndex { get; set; }

        [Category("eXpand")]
        [DefaultValue(true)]
        [Description("Control gridview column visibility")]
        bool IsColumnHeadersVisible { get; set; }

        [Category("eXpand")]
        [DefaultValue(true)]
        [Description("Use TAB to navigate to the next column of a gridview else use ENTER")]
        bool UseTabKey { get; set; }
    }
    public interface IModelListViewGridViewOptions : IModelNode {
        [ModelValueCalculator("((IModelClassGridViewOptions)ModelClass)", "EditorShowMode")]
        [Category("eXpand")]
        [Description("Control gridview property")]
        EditorShowMode EditorShowMode { get; set; }

        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassGridViewOptions)ModelClass)", "AutoExpandNewRow")]
        [Description("If gridview in master detail auto expand new inserter row")]
        bool AutoExpandNewRow { get; set; }

        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassGridViewOptions)ModelClass)", "DoNotLoadWhenNoFilterExists")]
        [Description("Only loads listview records when a filter is present")]
        bool DoNotLoadWhenNoFilterExists { get; set; }

        [DefaultValue(true)]
        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassGridViewOptions)ModelClass)", "GuessAutoFilterRowValuesFromFilter")]
        [Description("Control gridview method")]
        bool GuessAutoFilterRowValuesFromFilter { get; set; }

        [Category("eXpand")]
        [ModelValueCalculator("((IModelClassGridViewOptions)ModelClass)", "GroupLevelExpandIndex")]
        [Description("Expand all groups of a gridview up to this level")]
        int GroupLevelExpandIndex { get; set; }

        [Category("eXpand")]
        [DefaultValue(true)]
        [ModelValueCalculator("((IModelClassGridViewOptions)ModelClass)", "IsColumnHeadersVisible")]
        [Description("Control gridview column visibility")]
        bool IsColumnHeadersVisible { get; set; }

        [Category("eXpand")]
        [DefaultValue(true)]
        [ModelValueCalculator("((IModelClassGridViewOptions)ModelClass)", "UseTabKey")]
        [Description("Use TAB to navigate to the next column of a gridview else use ENTER")]
        bool UseTabKey { get; set; }
    }

    public class GridViewViewController : ListViewController<GridListEditor>, IModelExtender {
        const string DoNotLoadWhenNoFilterExists = "DoNotLoadWhenNoFilterExists";
        FilterController filterController;
        GridControl gridControl;
        GridView mainView;
        bool newRowAdded;
        #region IModelExtender Members
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassGridViewOptions>();
            extenders.Add<IModelListView, IModelListViewGridViewOptions>();
            extenders.Add<IModelMember, IModelMemberGridViewOptions>();
            extenders.Add<IModelColumn, IModelColumnGridViewOptions>();
        }
        #endregion
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();

            View.ControlsCreated += View_OnControlsCreated;
        }


        void View_OnControlsCreated(object sender, EventArgs e) {
            gridControl = View.Control as GridControl;
            if (gridControl == null)
                return;

            gridControl.HandleCreated += GridControl_OnHandleCreated;

            mainView = gridControl.MainView as GridView;
            if (mainView != null) {
                mainView.FocusedRowChanged += GridView_OnFocusedRowChanged;
                mainView.InitNewRow += GridView_OnInitNewRow;
                mainView.ShownEditor += MainViewOnShownEditor;
                SetOptions(mainView, View.Model);
            }

            if (((IModelListViewGridViewOptions) View.Model).DoNotLoadWhenNoFilterExists &&
                ((GridView) gridControl.MainView).FilterPanelText == string.Empty) {
                if (mainView != null) mainView.ActiveFilter.Changed += ActiveFilter_OnChanged;

                filterController = Frame.GetController<FilterController>();
                filterController.FullTextFilterAction.Execute += FullTextFilterAction_Execute;
                SetDoNotLoadWhenFilterExistsCriteria();
            }
        }

        void FullTextFilterAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
            if (string.IsNullOrEmpty(e.ParameterCurrentValue as string))
                SetDoNotLoadWhenFilterExistsCriteria();
            else
                ClearDoNotLoadWhenFilterExistsCriteria();
        }

        void MainViewOnShownEditor(object sender, EventArgs args) {
            var view = (GridView) sender;
            if (view.IsFilterRow(view.FocusedRowHandle))
                view.ActiveEditor.Properties.EditValueChangedFiringMode = EditValueChangedFiringMode.Buffered;
        }


        void SetDoNotLoadWhenFilterExistsCriteria() {
            IMemberInfo memberInfo = View.ObjectTypeInfo.KeyMember;
            Type memberType = memberInfo.MemberType;
            object o = memberType.IsValueType ? Activator.CreateInstance(memberType) : null;
            (View).CollectionSource.Criteria[DoNotLoadWhenNoFilterExists] = new BinaryOperator(memberInfo.Name, o);
        }

        void ClearDoNotLoadWhenFilterExistsCriteria() {
            View.CollectionSource.Criteria[DoNotLoadWhenNoFilterExists] = null;
        }

        void ActiveFilter_OnChanged(object sender, EventArgs e) {
            if (((GridView) gridControl.MainView).FilterPanelText != string.Empty)
                ClearDoNotLoadWhenFilterExistsCriteria();
            else
                SetDoNotLoadWhenFilterExistsCriteria();
        }

        void GridView_OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
            if (newRowAdded && mainView.IsValidRowHandle(e.FocusedRowHandle)) {
                newRowAdded = false;
                if (((IModelListViewGridViewOptions) View.Model).AutoExpandNewRow)
                    mainView.ExpandMasterRow(e.FocusedRowHandle);
            }
        }

        void GridView_OnInitNewRow(object sender, InitNewRowEventArgs e) {
            newRowAdded = true;
        }

        public static void SetOptions(GridView gridView, IModelListView model) {
            gridView.OptionsView.NewItemRowPosition =
                (NewItemRowPosition)
                Enum.Parse(typeof (NewItemRowPosition), ((IModelListViewNewItemRow) model).NewItemRowPosition.ToString());
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
            gridView.OptionsView.ShowAutoFilterRow = ((IModelListViewShowAutoFilterRow) model).ShowAutoFilterRow;
            gridView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gridView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
            gridView.ActiveFilterEnabled = ((IModelListViewWin) model).IsActiveFilterEnabled;

            SetColumnOptions(gridView, model);

            if (((IModelListViewShowAutoFilterRow) model).ShowAutoFilterRow &&
                ((IModelListViewGridViewOptions) model).GuessAutoFilterRowValuesFromFilter) {
                gridView.GuessAutoFilterRowValuesFromFilter();
            }
        }

        public static void SetColumnOptions(GridView gridView, IModelListView listViewInfoNodeWrapper) {
            foreach (GridColumn column in gridView.Columns) {
                GridColumn column1 = column;
                IModelColumn columnInfo =
                    listViewInfoNodeWrapper.Columns.Single(
                        c => c.PropertyName == column1.FieldName.Replace("!", string.Empty));
                if (columnInfo != null) {
                    column.OptionsFilter.AutoFilterCondition =
                        ((IModelColumnGridViewOptions) columnInfo).AutoFilterCondition;
                    column.OptionsFilter.ImmediateUpdateAutoFilter =
                        ((IModelColumnGridViewOptions) columnInfo).ImmediateUpdateAutoFilter;
                }
            }
        }

        void GridControl_OnHandleCreated(object sender, EventArgs e) {
            mainView.GridControl.ForceInitialize();

            int groupLevel = ((IModelListViewGridViewOptions) View.Model).GroupLevelExpandIndex;
            if (groupLevel > 0) {
                if (mainView.GroupCount == groupLevel)
                    for (int i = -1;; i--) {
                        if (!mainView.IsValidRowHandle(i)) return;
                        if (mainView.GetRowLevel(i) < groupLevel - 1)
                            mainView.SetRowExpanded(i, true);
                    }
            }
        }
    }
}