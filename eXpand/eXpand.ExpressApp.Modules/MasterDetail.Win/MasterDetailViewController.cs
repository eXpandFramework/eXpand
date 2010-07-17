using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.MasterDetail.Logic;
using eXpand.ExpressApp.MasterDetail.Win.Logic;


namespace eXpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailViewController : MasterDetailBaseController, IModelExtender
    {
        
        Dictionary<LevelDefaultViewInfoCreation, bool> _levelDefaultViewCreation;
        Window _windowToDispose;


        void ViewOnMasterRowCollapsing(object sender, MasterRowCanExpandEventArgs masterRowCanExpandEventArgs) {
            _windowToDispose = ((ExpressApp.Win.ListEditors.XafGridView)((GridView) sender).GetDetailView(masterRowCanExpandEventArgs.RowHandle,masterRowCanExpandEventArgs.RelationIndex)).Window;
        }

        void ViewOnMasterRowCollapsed(object sender, CustomMasterRowEventArgs customMasterRowEventArgs) {
            ((WinWindow) _windowToDispose).Form.Close();
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            _levelDefaultViewCreation = null;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            _levelDefaultViewCreation = new Dictionary<LevelDefaultViewInfoCreation, bool>();
            XafGridView view = ((GridListEditor) View.Editor).GridView;
//            view.GridControl.FocusedViewChanged+=GridControlOnFocusedViewChanged;
            view.MasterRowCollapsed += ViewOnMasterRowCollapsed;
            view.MasterRowExpanded += view_MasterRowExpanded;
            view.MasterRowCollapsing += ViewOnMasterRowCollapsing;
            view.MasterRowEmpty += ViewOnMasterRowEmpty;
            view.MasterRowGetLevelDefaultView += ViewOnMasterRowGetLevelDefaultView;
        }

//        void GridControlOnFocusedViewChanged(object sender, ViewFocusEventArgs viewFocusEventArgs) {
//            Frame.GetController<DeleteObjectsViewController>().DeleteAction.Enabled.Changed+=ActiveOnChanged;
//            Window window = ((ExpressApp.Win.ListEditors.XafGridView) viewFocusEventArgs.View).Window;
//            ICollection<IActionContainer> actionContainers = Frame.Template.GetContainers();
//            foreach (IActionContainer actionContainer in actionContainers) {
//                if (actionContainer != null)
//                    foreach (ActionBase actionBase in actionContainer.Actions)
//                    {
//                        actionBase.SelectionContext =window!= null? window.View:View;
//                    }
//            }
//            
//        }

        void ActiveOnChanged(object sender, EventArgs eventArgs) {
            
        }


        void view_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            _levelDefaultViewCreation[new LevelDefaultViewInfoCreation(e.RowHandle, e.RelationIndex)] = false;
        }



        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs eventArgs) {
            List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (XafGridView)sender, masterDetailRules);
            eventArgs.IsEmpty=!modelDetailRelationCalculator.IsRelationSet(eventArgs.RowHandle, eventArgs.RelationIndex);
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            var levelDefaultViewInfoCreation = new LevelDefaultViewInfoCreation(e.RowHandle, e.RelationIndex);
            if (!_levelDefaultViewCreation.ContainsKey(levelDefaultViewInfoCreation) || !(_levelDefaultViewCreation[levelDefaultViewInfoCreation])) {
                if (!_levelDefaultViewCreation.ContainsKey(levelDefaultViewInfoCreation))
                    _levelDefaultViewCreation.Add(levelDefaultViewInfoCreation, true);
                var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace,(Window) Frame);
                List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
                var levelDefaultView =(ExpressApp.Win.ListEditors.XafGridView)
                    gridViewBuilder.GetLevelDefaultView((XafGridView) sender, e.RowHandle, e.RelationIndex, View.Model,masterDetailRules);
                e.DefaultView = levelDefaultView;
            }
        }



        
    }
}