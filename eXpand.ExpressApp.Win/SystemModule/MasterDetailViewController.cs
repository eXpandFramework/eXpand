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
using DevExpress.Persistent.BaseImpl;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.SystemModule;


namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class MasterDetailViewController : BaseViewController
    {
        public const string DetailListRelationName = "DetailListRelationName";
        public const string DetailListView = "DetailListView";
        public const string ExpandAllRows = "ExpandAllRows";
        private readonly Dictionary<GridColumn, string> columnsProperties = new Dictionary<GridColumn, string>();
        private GridControl gridControl;

// private ListViewInfoNodeWrapper subModel;
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
                    return gridControl.FocusedView.GetRow(((GridView) gridControl.FocusedView).FocusedRowHandle);
                return null;
            }
        }

        private XafGridView gridView;
        private RepositoryEditorsFactory repositoryFactory;
        private ListViewInfoNodeWrapper subModel;

        public XafGridView GridView
        {
            get { return gridView; }
        }
        private void RefreshColumn(ColumnInfoNodeWrapper frameColumn, GridColumn column)
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
            if (column.VisibleIndex != frameColumn.VisibleIndex)
            {
                column.VisibleIndex = frameColumn.VisibleIndex;
            }
            column.SummaryItem.SummaryType = frameColumn.SummaryType;
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

        public GridColumn AddColumn(ColumnInfoNodeWrapper columnInfo,Type objectType,ListViewInfoNodeWrapper model)
        {
            if (columnsProperties.ContainsValue(columnInfo.PropertyName))
            {
                return columnsProperties.Where(pair => pair.Value == columnInfo.PropertyName).Single().Key;
//                throw new ArgumentException(string.Format(ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(ExceptionId.GridColumnExists), columnInfo.PropertyName), "columnInfo");
            }
            ColumnInfoNodeWrapper frameColumn = model.Columns.FindColumnInfo(columnInfo.PropertyName);
            if (frameColumn == null)
            {
                model.Columns.Node.AddChildNode(columnInfo.Node);
            }
            var column = new GridColumn();
            columnsProperties.Add(column, columnInfo.PropertyName);
            GridView.Columns.Add(column);
            var customArgs = new CustomCreateColumnEventArgs(column, columnInfo, repositoryFactory);
//            OnCustomCreateColumn(customArgs);
            if (!customArgs.Handled)
            {
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
                                                                       ((ListView) View).CollectionSource);
                        RepositoryItem repositoryItem = repositoryFactory.CreateRepositoryItem(!isGranted, new DetailViewItemInfoNodeWrapper(columnInfo.Node), objectType);
                        if (repositoryItem != null)
                        {
                            gridControl.RepositoryItems.Add(repositoryItem);
                            column.ColumnEdit = repositoryItem;
                            column.OptionsColumn.AllowEdit = IsDataShownOnDropDownWindow(repositoryItem) ? true : model.EditMode != EditMode.ReadOnly;
                            column.AppearanceCell.Options.UseTextOptions = true;
                            column.AppearanceCell.TextOptions.HAlignment = WinAlignmentProvider.GetAlignment(memberInfo.MemberType);
                            repositoryItem.ReadOnly |= model.EditMode != EditMode.Editable;
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
                
            }
//            OnColumnCreated(column, columnInfo);
            if (!gridControl.IsLoading && gridView.DataController.Columns.GetColumnIndex(column.FieldName) == -1)
            {
                gridView.DataController.RePopulateColumns();
            }
            return column;
        }

        public void RefreshColumns(ListViewInfoNodeWrapper model)
        {
            var objectType = View.ObjectTypeInfo.AssemblyInfo.Assembly.GetType(model.Node.GetAttributeValue(BaseViewInfoNodeWrapper.ClassNameAttribute));
            gridControl.BeginUpdate();
            try
            {
                var presentedColumns = new Dictionary<string, GridColumn>();
                var toDelete = new List<GridColumn>();
                foreach (GridColumn column in GridView.Columns)
                {
                    presentedColumns.Add(columnsProperties[column], column);
                    toDelete.Add(column);
                }
                foreach (ColumnInfoNodeWrapper column in from col in model.Columns.Items orderby col.SortIndex select col)
                {
                    GridColumn gridColumn;
                    if (presentedColumns.TryGetValue(column.PropertyName, out gridColumn))
                    {
                        RefreshColumn(column, gridColumn);
                    }
                    else
                    {
                        gridColumn = AddColumn(column,objectType, model);
                        presentedColumns.Add(column.PropertyName, gridColumn);
                    }
                    toDelete.Remove(gridColumn);
                }
                foreach (GridColumn gridColumn in toDelete)
                {
                    GridView.Columns.Remove(gridColumn);
                    columnsProperties.Remove(gridColumn);
                }
//                GridView.SortInfo.ClearSorting();
//                foreach (
//                    var gridColumn in
//                        from column in gridView.Columns.Cast<GridColumn>() orderby column.SortIndex select column)
//                    gridView.SortInfo.Add(gridColumn, gridColumn.SortOrder);
            }
            finally
            {
                gridControl.EndUpdate();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            string attributeValue = View.Info.GetAttributeValue(DetailListView);
            ExpandAllRowsSimpleAction.Active["key"] = false;
            CollapseAllRowsSimpleAction.Active["key"] = false;
            if (View is ListView && !string.IsNullOrEmpty(attributeValue) &&
                !string.IsNullOrEmpty(DetailListRelationName))
            {
                Frame.GetController<DeleteObjectsViewController>().DeleteAction.Executing += DeleteAction_OnExecuting;
                ExpandAllRowsSimpleAction.Active["key"] = true;
                CollapseAllRowsSimpleAction.Active["key"] = true;
                subModel =
                    new ListViewInfoNodeWrapper(
                        View.Info.GetRootNode().GetChildNode(ViewsNodeWrapper.NodeName).GetChildNode(
                            ListViewInfoNodeWrapper.NodeName, "ID",
                            attributeValue));
                repositoryFactory = new RepositoryEditorsFactory(Application, ObjectSpace);
                View.ControlsCreated += View_ControlsCreated;
            }
        }

        private void DeleteAction_OnExecuting(object sender, CancelEventArgs e)
        {

            if (!e.Cancel)
            {
                e.Cancel = true;

                var view = ((GridView) gridControl.FocusedView);
                var list = new List<object>();
                foreach (int selectedRow in view.GetSelectedRows())
                    list.Add(view.GetRow(selectedRow));
                ObjectSpace.Delete(list);
            }
        }


        public override Schema GetSchema()
        {
            string CommonTypeInfos = @"<Element Name=""Application"">

                    <Element Name=""Views"" >
                        <Element Name=""ListView"" >
                            <Attribute Name=""" + DetailListView + @""" RefNodeName=""{" + typeof(ViewIdRefNodeProvider).FullName + @"};ViewType=All|ListView"" />
                            <Attribute Name=""" + DetailListRelationName + @""" RefNodeName=""{" + typeof(ViewIdRefNodeProvider).FullName + @"};ClassName=@ClassName;Relations=All"" />
                            <Attribute Name=""" + ExpandAllRows +@""" Choice=""False,True""/>
                        </Element>
                    </Element>
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }

        private void View_ControlsCreated(object sender, EventArgs e)
        {
            gridControl = (GridControl)View.Control;
            gridControl.HandleCreated += GridControl_OnHandleCreated;


//            view.MasterRowExpanded += view_MasterRowExpanded;
//            view.MasterRowExpanding += View_OnMasterRowExpanding;
        }

        private void GridControl_OnHandleCreated(object sender, EventArgs e)
        {
            
            var view = (GridView) ((GridControl) sender).FocusedView;
            view.OptionsDetail.ShowDetailTabs = false;
            view.OptionsView.ShowDetailButtons = true;
            view.OptionsDetail.EnableMasterViewMode = true;
            gridControl.ShowOnlyPredefinedDetails = true;
            gridView = new XafGridView();
            GridViewViewController.SetOptions(gridView, subModel);
            gridView.GridControl = gridControl;
            RefreshColumns(subModel);
            gridControl.LevelTree.Nodes.Add(View.Info.GetAttributeValue(DetailListRelationName), gridView);

            if (View.Info.GetAttributeBoolValue(ExpandAllRows, false))
                ExpandAllRowsSimpleAction.DoExecute();
        }

//        private void View_OnMasterRowExpanding(object sender, MasterRowCanExpandEventArgs e)
//        {
//            e.Allow =
//                ((GridView) sender).GetRelationDisplayName(e.RowHandle, e.RelationIndex) ==
//                View.Info.GetAttributeValue(DetailListRelationName);
//        }

//        private void view_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
//        {
//            if (subModel != null)
//                if (gridControl.ViewCollection.Count > 1)
//                {
//                    var gridView = (GridView) gridControl.ViewCollection[gridControl.ViewCollection.Count - 1];
//                    if (gridControl.ViewCollection.Count > 2)
//                        gridView.Synchronize(gridControl.ViewCollection[1], SynchronizationMode.Visual);
//                    else
//                    {
//                        GridViewViewController.SetOptions(gridView, subModel);
//                        RefreshColumns(subModel, gridView);
//
////                        gridView.SortInfo.ClearSorting();
////                        foreach (ColumnInfoNodeWrapper wrapper in subModel.Columns.Items)
////                        {
////                            var descriptor = ReflectionHelper.FindMemberDescriptor(View.ObjectType.Assembly.GetType(subModel.Node.GetAttributeValue(BaseViewInfoNodeWrapper.ClassNameAttribute)), wrapper.PropertyName);
////                            RefreshColumn(wrapper, gridView.Columns.ColumnByFieldName(descriptor != null ? descriptor.BindingMemberName : wrapper.PropertyName));
////                        }
//                        gridView.OptionsDetail.ShowDetailTabs = false;
//                        gridView.OptionsView.ShowDetailButtons =
//                            !string.IsNullOrEmpty(subModel.Node.GetAttributeValue(DetailListView));
//                        gridView.OptionsDetail.EnableMasterViewMode = gridView.OptionsView.ShowDetailButtons;
//                    }
//                }
//        }

        private void ExpandAllRowsSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
//            int index =
//                ((GridView) gridControl.FocusedView).Columns.ColumnByFieldName("AnalysiUnitMethod.SortIndex").SortIndex;
            ObjectSpace.Session.PreFetch(((BaseObject) View.CurrentObject).ClassInfo,
                                         ((ListView) View).CollectionSource.Collection,
                                         View.Info.GetAttributeValue(DetailListRelationName));
            var view = (GridView) gridControl.MainView;
            for (int i = 0; i < view.RowCount; i++)
                view.ExpandMasterRow(i);
        }

        private void CollapseAllRowsSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var view = (GridView) gridControl.MainView;

            view.CollapseAllDetails();
        }
    }
}