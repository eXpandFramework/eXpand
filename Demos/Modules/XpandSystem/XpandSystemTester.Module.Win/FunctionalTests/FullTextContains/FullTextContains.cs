﻿using System;
using System.Data.SqlClient;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.Views.Base;
using Fasterflect;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.ExpressApp.Win.PropertyEditors;

namespace XpandSystemTester.Module.Win.FunctionalTests.FullTextContains {
    public class FullTextContains:ViewController{
        private readonly ParametrizedAction _parametrizedAction;
        public const string XpandGridListEditor = "XpandGridListEditor";
        public const string AdvBandedListEditor = "AdvBandedListEditor";
        private const string CollectionSourceCriteria = "CollectionSource.CriteriaApplying";
        public const string CriteriaPropertyEditorEx = "CriteriaPropertyEditorEx";
        public const string PopupCriteriaPropertyEditorEx = "PopupCriteriaPropertyEditorEx";
        public const string ColumnFilterChanged = "GridView.ColumnFilterChange";
        public const string XpandObjectSpaceProvider = "XpandObjectSpaceProvider";
        public FullTextContains(){
            var singleChoiceAction = new SingleChoiceAction(this,GetType().Name,PredefinedCategory.ObjectsCreation){
                ItemType = SingleChoiceActionItemType.ItemIsOperation,
            };
            TargetObjectType = typeof(FullTextContainsObject);
            singleChoiceAction.Items.Add(new ChoiceActionItem(XpandGridListEditor, null));
            singleChoiceAction.Items.Add(new ChoiceActionItem(AdvBandedListEditor, null));
            singleChoiceAction.Items.Add(new ChoiceActionItem(CollectionSourceCriteria, null));
            singleChoiceAction.Items.Add(new ChoiceActionItem(CriteriaPropertyEditorEx, null));
            singleChoiceAction.Items.Add(new ChoiceActionItem(ColumnFilterChanged, null));
            singleChoiceAction.Items.Add(new ChoiceActionItem(XpandObjectSpaceProvider, null));
            singleChoiceAction.Items.Add(new ChoiceActionItem(PopupCriteriaPropertyEditorEx, null));
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
            _parametrizedAction = new ParametrizedAction(this,"FullTextContainsParam",PredefinedCategory.View, typeof(string));
            _parametrizedAction.Execute+=ParametrizedActionOnExecute;
        }

        private void ParametrizedActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs){
            
        }

        public ParametrizedAction ParametrizedAction{
            get { return _parametrizedAction; }
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            switch (e.SelectedChoiceActionItem.Id){
                case PopupCriteriaPropertyEditorEx:{
                    var listView = CreateListView(PopupCriteriaPropertyEditorEx);
                    ConfigParameters(e.ShowViewParameters,listView);
                }
                    break;
                case XpandObjectSpaceProvider:{
                    Verify(() =>ObjectSpace.FindObject(TargetObjectType,
                                CriteriaOperator.Parse("FullText = 'Apostolis Bekiaris'")));
                }
                    break;
                case ColumnFilterChanged:{
                    var gridView = ((ColumnsListEditor) ((ListView) View).Editor).GridView();
                    gridView.ActiveFilterCriteria = CriteriaOperator.Parse("FullText = 'Apostolis Bekiaris'");
                    ParametrizedAction.Value = gridView.ActiveFilterCriteria;
                }
                    break;
                case CriteriaPropertyEditorEx:{
                    var detailView = CreateDetailView();
                    ConfigParameters(e.ShowViewParameters, detailView);
                    e.ShowViewParameters.Controllers.Add(Application.CreateController<ShowClauseMenu>());
                }
                    break;
                case CollectionSourceCriteria:
                    Verify(() => {
                        ((ListView)View).CollectionSource.Criteria[CollectionSourceCriteria] =
                            CriteriaOperator.Parse("FullText=?", "Apostolis Bekiaris");
                    });
                    break;
                case XpandGridListEditor:case AdvBandedListEditor:{
                    ListView listView = CreateListView("FullText_"+e.SelectedChoiceActionItem.Id);
                    ConfigParameters(e.ShowViewParameters, listView);
                    e.ShowViewParameters.Controllers.Add(Application.CreateController<ShowClauseMenu>());
                } 
                break;
            }
        }

        private void Verify(Action action){
            try{
                action();
            }
            catch (Exception exception) {
                var executionErrorException = exception as SqlExecutionErrorException;
                if (executionErrorException!=null)
                    Verify(executionErrorException);
                else{
                    executionErrorException = exception.InnerException as SqlExecutionErrorException;
                    if (executionErrorException != null) {
                        Verify(executionErrorException);
                    }
                }
            }
        }

        private void Verify(SqlExecutionErrorException sqlExecutionErrorException) {
            var sqlException = sqlExecutionErrorException.InnerException as SqlException;
            if (sqlException != null && sqlException.Number == 7601)
                ParametrizedAction.Value = "ContainsException";
        }

        private DetailView CreateDetailView(){
            var objectSpace = Application.CreateObjectSpace();
            return Application.CreateDetailView(objectSpace, objectSpace.CreateObject(TargetObjectType));
        }

        private ListView CreateListView(string viewId=null){
            var listViewId = TargetObjectType.Name +"_LookupListView";
            if (viewId != null){
                listViewId = viewId+"_ListView";
            }
            var collectionSource = Application.CreateCollectionSource(Application.CreateObjectSpace(), TargetObjectType,listViewId);
            return Application.CreateListView(listViewId, collectionSource, true);
        }

        private void ConfigParameters(ShowViewParameters showViewParameters, ObjectView objectView){
            var parameters = showViewParameters;
            parameters.CreateAllControllers = false;
            parameters.TargetWindow = TargetWindow.NewWindow;
            parameters.Context = TemplateContext.PopupWindow;
            parameters.CreatedView=objectView;
        }
    }

    public class ShowClauseMenu : DialogController{
        public ShowClauseMenu(){
            var singleChoiceAction = new SingleChoiceAction(this, GetType().Name, PredefinedCategory.PopupActions){
                ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            singleChoiceAction.Items.Add(new ChoiceActionItem(FullTextContains.XpandGridListEditor, null));
            singleChoiceAction.Items.Add(new ChoiceActionItem(FullTextContains.CriteriaPropertyEditorEx, null));
            singleChoiceAction.Execute+=ActionOnExecute;
        }

        private void ActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if (e.SelectedChoiceActionItem.Id == FullTextContains.CriteriaPropertyEditorEx){
                var filterEditorControl = ((CriteriaPropertyEditorEx) ((DetailView) Frame.View).FindItem("Criteria")).Control;
                DisplayMenu(filterEditorControl.FilterControl);
            }
            else if (e.SelectedChoiceActionItem.Id == FullTextContains.XpandGridListEditor){
                var value = Application.MainWindow.GetController<FullTextContains>().ParametrizedAction.Value ?? "FullName";
                var gridView = ((ColumnsListEditor) ((ListView) Frame.View).Editor).GridView();
                var defaultColumn = gridView.Columns[value.ToString()];
                gridView.FilterEditorCreated += GridViewOnFilterEditorCreated;
                gridView.ShowFilterEditor(defaultColumn);
            }
        }

        private void GridViewOnFilterEditorCreated(object sender, FilterControlEventArgs e) {
            e.FilterBuilder.Shown += (o, args) => {
                var filterControl = (FilterControl) e.FilterBuilder.FilterControl;
                if (filterControl != null){
                    DisplayMenu(filterControl);
                }
            };
        }

        void DisplayMenu(FilterControl filterControl){
            var groupNode = filterControl.Model.RootNode;
            if (!groupNode.SubNodes.Any()){
                filterControl.SetDefaultColumn(filterControl.FilterColumns[Frame.View.ObjectTypeInfo.DefaultMember.Name]);
                groupNode.AddNode(filterControl.Model.CreateCriteriaByDefaultProperty());
            }
            filterControl.Model.FocusInfo = new FilterControlFocusInfo((Node) groupNode.SubNodes[0], 1);
            System.Windows.Forms.Application.DoEvents();
            filterControl.CallMethod("ShowClauseMenu");
        }
    }
}
