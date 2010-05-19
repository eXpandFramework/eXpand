using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelListViewMasterDetailOptions : IModelNode
    {
        [DataSourceProperty("Application.Views")]
        [DataSourceCriteria("ModelClass Is Not Null And ModelClass.Name = '@This.Name'")]
        IModelListView DetailListView { get; set; }

        [DataSourceProperty("ModelClass.AllMembers")]
        [DataSourceCriteria("MemberInfo.IsList")]
        IModelMember DetailListRelationName { get; set; }
        bool ExpandAllRows { get; set; }
    }

    public partial class MasterDetailViewController : BaseViewController<ListView>, IModelExtender
    {
        private GridControl gridControl;
        private XafGridView gridView;
        private RepositoryEditorsFactory repositoryFactory;

        public MasterDetailViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public object CurrentObject
        {
            get
            {
                if (gridControl != null)
                    return gridControl.FocusedView.GetRow(((GridView)gridControl.FocusedView).FocusedRowHandle);
                return null;
            }
        }

        public XafGridView GridView
        {
            get { return gridView; }
        }

        private void RefreshColumn(IModelColumn frameColumn, GridColumn column)
        {
            column.Caption = frameColumn.Caption;
            if (string.IsNullOrEmpty(column.Caption))
            {
                column.Caption = column.FieldName;
            }
            if (!string.IsNullOrEmpty(frameColumn.DisplayFormat))
            {
                column.DisplayFormat.FormatString = frameColumn.DisplayFormat;
                column.DisplayFormat.FormatType = FormatType.Custom;
                column.GroupFormat.FormatString = frameColumn.DisplayFormat;
                column.GroupFormat.FormatType = FormatType.Custom;
            }
            column.GroupIndex = frameColumn.GroupIndex;
            column.SortIndex = frameColumn.SortIndex;
            column.SortOrder = frameColumn.SortOrder;
            column.Width = frameColumn.Width;
            column.VisibleIndex = frameColumn.Index;
            column.SummaryItem.SummaryType = (DevExpress.Data.SummaryItemType)Enum.Parse(typeof(DevExpress.Data.SummaryItemType), frameColumn.SummaryType.ToString());
        }
        protected internal bool IsDataShownOnDropDownWindow(RepositoryItem repositoryItem)
        {
            return DXPropertyEditor.RepositoryItemsTypesWithMandatoryButtons.Contains(repositoryItem.GetType());
        }
        protected string GetDisplayablePropertyName(string memberName, ITypeInfo typeInfo)
        {
            IMemberInfo displayableMemberDescriptor = ReflectionHelper.FindDisplayableMemberDescriptor(typeInfo, memberName);
            if (displayableMemberDescriptor != null)
            {
                return displayableMemberDescriptor.BindingName;
            }
            return memberName;
        }

        public GridColumn AddColumn(IModelColumn columnInfo, Type objectType, IModelListView model)
        {
            var column = new GridColumn();

            GridView.Columns.Add(column);
            IMemberInfo memberInfo = XafTypesInfo.Instance.FindTypeInfo(objectType).FindMember(columnInfo.PropertyName);
            if (memberInfo != null)
            {
                column.FieldName = memberInfo.BindingName;
                if (memberInfo.MemberType.IsEnum)
                {
                    column.SortMode = ColumnSortMode.Value;
                }
                else if (!SimpleTypes.IsSimpleType(memberInfo.MemberType))
                {
                    column.SortMode = ColumnSortMode.DisplayText;
                }
                column.FilterMode = SimpleTypes.IsClass(memberInfo.MemberType) ? ColumnFilterMode.DisplayText : ColumnFilterMode.Value;
            }
            else
            {
                column.FieldName = columnInfo.PropertyName;
            }

            RefreshColumn(columnInfo, column);
            if (memberInfo != null)
            {
                if (repositoryFactory != null)
                {
                    bool isGranted = DataManipulationRight.CanRead(objectType, columnInfo.PropertyName, null,
                                                                   ((ListView)View).CollectionSource);
                    RepositoryItem repositoryItem = repositoryFactory.CreateRepositoryItem(!isGranted, columnInfo, objectType);
                    if (repositoryItem != null)
                    {
                        gridControl.RepositoryItems.Add(repositoryItem);
                        column.ColumnEdit = repositoryItem;
                        column.OptionsColumn.AllowEdit = IsDataShownOnDropDownWindow(repositoryItem) ? true : model.AllowEdit;
                        column.AppearanceCell.Options.UseTextOptions = true;
                        column.AppearanceCell.TextOptions.HAlignment = WinAlignmentProvider.GetAlignment(memberInfo.MemberType);
                        repositoryItem.ReadOnly |= !model.AllowEdit;
                        if ((repositoryItem is ILookupEditRepositoryItem) && ((ILookupEditRepositoryItem)repositoryItem).IsFilterByValueSupported)
                        {
                            column.FilterMode = ColumnFilterMode.Value;
                        }
                        if ((repositoryItem is RepositoryItemPictureEdit) && (((RepositoryItemPictureEdit)repositoryItem).CustomHeight > 0))
                        {
                            GridView.OptionsView.RowAutoHeight = true;
                        }
                    }
                }
                if ((column.ColumnEdit == null) && !typeof(IList).IsAssignableFrom(memberInfo.MemberType))
                {
                    column.OptionsColumn.AllowEdit = false;
                    column.FieldName = GetDisplayablePropertyName(columnInfo.PropertyName, XafTypesInfo.Instance.FindTypeInfo(objectType));
                }
            }

            if (!gridControl.IsLoading && gridView.DataController.Columns.GetColumnIndex(column.FieldName) == -1)
            {
                gridView.DataController.RePopulateColumns();
            }
            return column;
        }

        public void RefreshColumns(IModelListView model)
        {
            gridControl.BeginUpdate();
            try
            {
                var presentedColumns = new Dictionary<string, GridColumn>();

                foreach (IModelColumn column in from col in model.Columns orderby col.SortIndex select col)
                {
                    GridColumn gridColumn;
                    if (presentedColumns.TryGetValue(column.PropertyName, out gridColumn))
                    {
                        RefreshColumn(column, gridColumn);
                    }
                    else
                    {
                        gridColumn = AddColumn(column, model.ModelClass.TypeInfo.Type, model);
                        presentedColumns.Add(column.PropertyName, gridColumn);
                    }
                }
            }
            finally
            {
                gridControl.EndUpdate();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            
            ExpandAllRowsSimpleAction.Active["key"] = false;
            CollapseAllRowsSimpleAction.Active["key"] = false;
            if (this.IsActive)
            {
                Frame.GetController<DeleteObjectsViewController>().DeleteAction.Executing += DeleteAction_OnExecuting;
                ExpandAllRowsSimpleAction.Active["key"] = true;
                CollapseAllRowsSimpleAction.Active["key"] = true;
                repositoryFactory = new RepositoryEditorsFactory(Application, ObjectSpace);
            }
        }

        private bool IsActive
        {
            get
            {
                return this.DetailListView != null && this.DetailListRelationName != null;
            }
        }

        private IModelListView DetailListView
        {
            get
            {
                return ((IModelListViewMasterDetailOptions)this.View.Model).DetailListView;
            }
        }

        private IModelMember DetailListRelationName
        {
            get
            {
                return ((IModelListViewMasterDetailOptions)this.View.Model).DetailListRelationName;
            }
        }

        private bool ExpandAllRows
        {
            get
            {
                return ((IModelListViewMasterDetailOptions)this.View.Model).ExpandAllRows;
            }
        }

        protected override void OnDeactivating()
        {
            if (this.IsActive)
            {
                Frame.GetController<DeleteObjectsViewController>().DeleteAction.Executing -= DeleteAction_OnExecuting;
            }

            base.OnDeactivating();
        }

        private void DeleteAction_OnExecuting(object sender, CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                e.Cancel = true;

                var view = ((GridView)gridControl.FocusedView);
                var list = new List<object>();
                if (view != null)
                    list.AddRange(view.GetSelectedRows().Select(selectedRow => view.GetRow(selectedRow)));
                ObjectSpace.Delete(list);
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewMasterDetailOptions>();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (this.IsActive)
            {
                gridControl = (GridControl)View.Control;
                gridControl.HandleCreated += GridControl_OnHandleCreated;
            }
        }

        private void GridControl_OnHandleCreated(object sender, EventArgs e)
        {
            var view = (GridView)((GridControl)sender).FocusedView;
            view.OptionsDetail.ShowDetailTabs = false;
            view.OptionsView.ShowDetailButtons = true;
            view.OptionsDetail.EnableMasterViewMode = true;
            gridControl.ShowOnlyPredefinedDetails = true;
            gridView = new XafGridView();
            GridViewViewController.SetOptions(gridView, this.DetailListView);
            gridView.GridControl = gridControl;
            RefreshColumns(this.DetailListView);
            gridControl.LevelTree.Nodes.Add(this.DetailListRelationName.Name, gridView);
            gridView.DoubleClick += gridView_DoubleClick;

            if (this.ExpandAllRows)
                ExpandAllRowsSimpleAction.DoExecute();
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            var parameter = new ShowViewParameters();
            ListViewProcessCurrentObjectController.ShowObject(((GridView)sender).GetFocusedRow(), parameter, Application, Frame, View);
            parameter.CreatedView.AllowNew["MasterDetail"] = false;
            Application.ShowViewStrategy.ShowView(parameter, new ShowViewSource(null, null));
        }

        private void ExpandAllRowsSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.CurrentObject != null)
            {
                ObjectSpace.Session.PreFetch(((PersistentBase)View.CurrentObject).ClassInfo,
                                             View.CollectionSource.List,
                                             this.DetailListRelationName.Name);
                var view = (GridView)gridControl.MainView;
                for (int i = 0; i < view.RowCount; i++)
                    view.ExpandMasterRow(i);
            }
        }

        private void CollapseAllRowsSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var view = (GridView)gridControl.MainView;

            view.CollapseAllDetails();
        }
    }
}