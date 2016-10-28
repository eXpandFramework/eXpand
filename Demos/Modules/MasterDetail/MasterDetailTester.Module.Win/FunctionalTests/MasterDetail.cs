using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.MasterDetail;

namespace MasterDetailTester.Module.Win.FunctionalTests {
    public class MasterDetail : ObjectViewController<ListView, ISupportMasterDetail> {
        public MasterDetail(){
            var singleChoiceAction = new SingleChoiceAction(this,"MasterDetail",PredefinedCategory.View);
            singleChoiceAction.Execute+=SimpleActionOnExecute;
            singleChoiceAction.ItemType=SingleChoiceActionItemType.ItemIsOperation;
            singleChoiceAction.Items.Add(new ChoiceActionItem("ExpandMaster", null));
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            var populateControllers = Frame.GetControllers<PopulateController<IContextMasterDetailRule>>();
            foreach (var populateController in populateControllers) {
                populateController.DisablePropertyEditorReplacement = true;
            }
        }

        private void SimpleActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs){
            var gridControl = ((XpandGridListEditor) View.Editor).Grid;
            var gridView = (GridView) gridControl.MainView;
            var selectedRow = gridView.GetSelectedRows()[0];
            var masterRowExpanded = gridView.GetMasterRowExpanded(selectedRow);
            if (!masterRowExpanded){
                gridView.MasterRowExpanded+=GridViewOnMasterRowExpanded;
                
            }
            gridView.SetMasterRowExpanded(selectedRow, !masterRowExpanded);
        }

        private void GridViewOnMasterRowExpanded(object sender, CustomMasterRowEventArgs e){
            var gridView = ((GridView)sender);
            var detailView = (ColumnView)gridView.GetDetailView(e.RowHandle, e.RelationIndex);
            var project = ((Project) View.CurrentObject);
            project.Relations = gridView.GetRelationCount(e.RowHandle);
            project.DetailViewType = detailView.GetType().Name;
            detailView.GridControl.FocusedView = detailView;
            detailView.FocusedRowHandle = 0;
        }
    }

    
}
