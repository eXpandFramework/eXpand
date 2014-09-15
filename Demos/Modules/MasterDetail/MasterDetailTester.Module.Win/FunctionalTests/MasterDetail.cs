using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;

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
            var gridView = (GridView) ((XpandGridListEditor) View.Editor).Grid.MainView;
            var masterRowExpanded = gridView.GetMasterRowExpanded(0);
            if (!masterRowExpanded){
                gridView.MasterRowExpanded+=GridViewOnMasterRowExpanded;
                
            }
            gridView.SetMasterRowExpanded(0,!masterRowExpanded);    
        }

        private void GridViewOnMasterRowExpanded(object sender, CustomMasterRowEventArgs e){
            var detailView = (GridView) ((GridView) sender).GetDetailView(e.RowHandle, e.RelationIndex);
            detailView.GridControl.FocusedView = detailView;
            detailView.FocusedRowHandle = 0;
        }
    }

    
}
