using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors;


namespace Xpand.ExpressApp.MasterDetail.Win
{
    public class MasterDetailViewController : ListViewController<XpandGridListEditor>, IModelExtender
    {
        private List<IMasterDetailRule> MasterDetailRules
        {
            get { return Frame.GetController<MasterDetailRuleController>().MasterDetailRules; }
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            var editor = View.Editor as XpandGridListEditor;
            if (editor != null && editor.Grid != null)
            {
                if (editor.GridView == editor.Grid.MainView)
                {
                    List<XpandXafGridView> gridViews = editor.Grid.Views.OfType<XpandXafGridView>().ToList();
                    for (int i = gridViews.Count() - 1; i > -1; i--)
                    {
                        XpandXafGridView xpandXafGridView = gridViews[i];
                        if (xpandXafGridView.Window != null)
                            ((WinWindow)xpandXafGridView.Window).Form.Close();
                    }
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var view = ((XpandGridListEditor)View.Editor).GridView;
            var grid = ((XpandGridListEditor)View.Editor).Grid;
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
            object row = ((XpandXafGridView)sender).GetRow(e.RowHandle);
            e.ChildList = (IList)MasterDetailRules[e.RelationIndex].CollectionMember.MemberInfo.GetValue(row);
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = MasterDetailRules[e.RelationIndex].CollectionMember.Name;
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e)
        {
            var masterDetailRule = MasterDetailRules[e.RelationIndex];
            e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.TypeInfo, masterDetailRule.CollectionMember.Name);
        }


        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = MasterDetailRules.Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e)
        {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var levelDefaultView = gridViewBuilder.GetLevelDefaultView((XpandXafGridView)sender, e.RowHandle, e.RelationIndex, View.Model, MasterDetailRules);
            e.DefaultView = levelDefaultView;
        }

        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs eventArgs)
        {
            var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (XpandXafGridView)sender, MasterDetailRules);
            eventArgs.IsEmpty = !modelDetailRelationCalculator.IsRelationSet(eventArgs.RowHandle, eventArgs.RelationIndex);
        }

        void Grid_ViewRegistered(object sender, ViewOperationEventArgs e)
        {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var parentGridView = (XpandXafGridView)e.View.ParentView;
            var frame = parentGridView.Window ?? Frame;
            List<IMasterDetailRule> masterDetailRules = frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            gridViewBuilder.ModifyInstanceGridView(parentGridView, e.View.SourceRowHandle, parentGridView.GetRelationIndex(e.View.SourceRowHandle, e.View.LevelName), ((XpandListView)frame.View).Model, masterDetailRules);
        }

        void Grid_ViewRemoved(object sender, ViewOperationEventArgs e)
        {
            var window = (e.View as XpandXafGridView).Window as WinWindow;
            if (window != null && window.Form != null)
                window.Form.Close();
        }
    }
}