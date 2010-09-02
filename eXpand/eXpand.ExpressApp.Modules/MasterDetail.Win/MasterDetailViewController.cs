using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.MasterDetail.Logic;
using eXpand.ExpressApp.SystemModule;
using GridListEditor = eXpand.ExpressApp.Win.ListEditors.GridListEditor;
using XafGridView = eXpand.ExpressApp.Win.ListEditors.XafGridView;


namespace eXpand.ExpressApp.MasterDetail.Win
{
    public class MasterDetailViewController : ListViewController<GridListEditor>, IModelExtender
    {
        Window _windowToDispose;


        void ViewOnMasterRowCollapsing(object sender, MasterRowCanExpandEventArgs masterRowCanExpandEventArgs)
        {
            _windowToDispose = ((XafGridView)((GridView)sender).GetDetailView(masterRowCanExpandEventArgs.RowHandle, masterRowCanExpandEventArgs.RelationIndex)).Window;
        }

        void ViewOnMasterRowCollapsed(object sender, CustomMasterRowEventArgs customMasterRowEventArgs)
        {
            ((WinWindow)_windowToDispose).Form.Close();
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            var editor = View.Editor as GridListEditor;
            if (editor != null && editor.Grid != null)
            {
                if (editor.GridView == editor.Grid.MainView)
                {
                    List<XafGridView> gridViews = editor.Grid.Views.OfType<XafGridView>().ToList();
                    for (int i = gridViews.Count() - 1; i > -1; i--)
                    {
                        XafGridView xafGridView = gridViews[i];
                        if (xafGridView.Window != null)
                            ((WinWindow)xafGridView.Window).Form.Close();
                    }
                }
            }
        }


        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var view = ((GridListEditor)View.Editor).GridView;
            view.MasterRowGetRelationCount += ViewOnMasterRowGetRelationCount;
            view.MasterRowGetRelationName += ViewOnMasterRowGetRelationName;
            view.MasterRowGetRelationDisplayCaption += MasterRowGetRelationDisplayCaption;
            view.MasterRowGetChildList += ViewOnMasterRowGetChildList;
            view.MasterRowCollapsed += ViewOnMasterRowCollapsed;
            view.MasterRowExpanded += view_MasterRowExpanded;
            view.MasterRowCollapsing += ViewOnMasterRowCollapsing;
            view.MasterRowEmpty += ViewOnMasterRowEmpty;
            view.MasterRowGetLevelDefaultView += ViewOnMasterRowGetLevelDefaultView;
        }


        void ViewOnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            object row = ((XafGridView)sender).GetRow(e.RowHandle);
            e.ChildList = (IList)Frame.GetController<MasterDetailRuleController>().MasterDetailRules[e.RelationIndex].CollectionMember.MemberInfo.GetValue(row);
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            var masterDetailRule = Frame.GetController<MasterDetailRuleController>().MasterDetailRules[e.RelationIndex];
            e.RelationName = masterDetailRule.CollectionMember.Name;
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e)
        {
            var masterDetailRule = Frame.GetController<MasterDetailRuleController>().MasterDetailRules[e.RelationIndex];
            e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.View.ModelClass.TypeInfo, masterDetailRule.CollectionMember.Name);
        }


        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = Frame.GetController<MasterDetailRuleController>().MasterDetailRules.Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e)
        {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            var masterGridView = (XafGridView)sender;
            var levelDefaultView = gridViewBuilder.GetLevelDefaultView(masterGridView, e.RowHandle, e.RelationIndex, View.Model, masterDetailRules);
            e.DefaultView = levelDefaultView;
        }



        void view_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            GridControl gridControl = ((GridListEditor)View.Editor).Grid;
            var masterGridView = (XafGridView)gridControl.MainView;
            List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            gridViewBuilder.ModifyInstanceGridView(masterGridView, e.RowHandle, e.RelationIndex, View.Model, masterDetailRules);
        }




        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs eventArgs)
        {
            List<IMasterDetailRule> masterDetailRules = Frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (XafGridView)sender, masterDetailRules);
            eventArgs.IsEmpty = !modelDetailRelationCalculator.IsRelationSet(eventArgs.RowHandle, eventArgs.RelationIndex);
        }
    }
}