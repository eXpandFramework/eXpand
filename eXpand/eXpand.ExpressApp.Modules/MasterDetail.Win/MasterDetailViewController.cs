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
        private List<IMasterDetailRule> MasterDetailRules
        {
            get { return Frame.GetController<MasterDetailRuleController>().MasterDetailRules; }
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
            var grid = ((GridListEditor)View.Editor).Grid;
            grid.ViewRegistered += Grid_ViewRegistered;
            grid.ViewRemoved += Grid_ViewRemoved;
            view.MasterRowGetRelationCount += ViewOnMasterRowGetRelationCount;
            view.MasterRowGetRelationName += ViewOnMasterRowGetRelationName;
            view.MasterRowGetRelationDisplayCaption += MasterRowGetRelationDisplayCaption;
            view.MasterRowGetChildList += ViewOnMasterRowGetChildList;
            view.MasterRowEmpty += ViewOnMasterRowEmpty;
            view.MasterRowGetLevelDefaultView += ViewOnMasterRowGetLevelDefaultView;
        }

        void ViewOnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            object row = ((XafGridView)sender).GetRow(e.RowHandle);
            e.ChildList = (IList)MasterDetailRules[e.RelationIndex].CollectionMember.MemberInfo.GetValue(row);
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = MasterDetailRules[e.RelationIndex].CollectionMember.Name;
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e)
        {
            var masterDetailRule = MasterDetailRules[e.RelationIndex];
            e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.View.ModelClass.TypeInfo, masterDetailRule.CollectionMember.Name);
        }


        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = MasterDetailRules.Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e)
        {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var levelDefaultView = gridViewBuilder.GetLevelDefaultView((XafGridView)sender, e.RowHandle, e.RelationIndex, View.Model, MasterDetailRules);
            e.DefaultView = levelDefaultView;
        }

        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs eventArgs)
        {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (XafGridView)sender, MasterDetailRules);
            eventArgs.IsEmpty = !modelDetailRelationCalculator.IsRelationSet(eventArgs.RowHandle, eventArgs.RelationIndex);
        }

        void Grid_ViewRegistered(object sender, ViewOperationEventArgs e)
        {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var parentGridView = (XafGridView)e.View.ParentView;
            var frame = parentGridView.Window ?? Frame;
            List<IMasterDetailRule> masterDetailRules = frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            gridViewBuilder.ModifyInstanceGridView(parentGridView, e.View.SourceRowHandle, parentGridView.GetRelationIndex(e.View.SourceRowHandle, e.View.LevelName), ((ListView)frame.View).Model, masterDetailRules);
        }

        void Grid_ViewRemoved(object sender, ViewOperationEventArgs e)
        {
            var window = (e.View as XafGridView).Window as WinWindow;
            if (window != null && window.Form != null)
                window.Form.Close();
        }
    }
}