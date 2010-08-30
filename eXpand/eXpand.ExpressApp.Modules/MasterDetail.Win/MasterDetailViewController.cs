using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.MasterDetail.Logic;
using eXpand.ExpressApp.SystemModule;
using GridListEditor = eXpand.ExpressApp.Win.ListEditors.GridListEditor;
using XafGridView = eXpand.ExpressApp.Win.ListEditors.XafGridView;


namespace eXpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailViewController : ListViewController<GridListEditor>, IModelExtender
    {
        
        Dictionary<LevelDefaultViewInfoCreation, bool> _levelDefaultViewCreation;
        Window _windowToDispose;


        void ViewOnMasterRowCollapsing(object sender, MasterRowCanExpandEventArgs masterRowCanExpandEventArgs) {
            _windowToDispose = ((XafGridView)((GridView) sender).GetDetailView(masterRowCanExpandEventArgs.RowHandle,masterRowCanExpandEventArgs.RelationIndex)).Window;
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
            var view = ((GridListEditor) View.Editor).GridView;
            view.MasterRowCollapsed += ViewOnMasterRowCollapsed;
            view.MasterRowExpanded += view_MasterRowExpanded;
            view.MasterRowCollapsing += ViewOnMasterRowCollapsing;
            view.MasterRowEmpty += ViewOnMasterRowEmpty;
            view.MasterRowGetLevelDefaultView += ViewOnMasterRowGetLevelDefaultView;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            var levelDefaultViewInfoCreation = new LevelDefaultViewInfoCreation(e.RowHandle, e.RelationIndex);
            if (!_levelDefaultViewCreation.ContainsKey(levelDefaultViewInfoCreation) || !(_levelDefaultViewCreation[levelDefaultViewInfoCreation])) {
                if (!_levelDefaultViewCreation.ContainsKey(levelDefaultViewInfoCreation))
                    _levelDefaultViewCreation.Add(levelDefaultViewInfoCreation, true);
                var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
                List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
                var levelDefaultView =gridViewBuilder.GetLevelDefaultView((XafGridView) sender, e.RowHandle, e.RelationIndex, View.Model,masterDetailRules);
                e.DefaultView = levelDefaultView;
            }
        }



        void view_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            _levelDefaultViewCreation[new LevelDefaultViewInfoCreation(e.RowHandle, e.RelationIndex)] = false;
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            GridControl gridControl = ((GridListEditor)View.Editor).Grid;
            var masterGridView = (XafGridView)gridControl.MainView;
            List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            gridViewBuilder.ModifyInstanceGridView(masterGridView, e.RowHandle, e.RelationIndex,View.Model,masterDetailRules);
        }

        


        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs eventArgs) {
            List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (XafGridView)sender, masterDetailRules);
            eventArgs.IsEmpty=!modelDetailRelationCalculator.IsRelationSet(eventArgs.RowHandle, eventArgs.RelationIndex);
        }
    }
}