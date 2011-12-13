using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors;


namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailViewController : ListViewController<XpandGridListEditor> {
        private List<IMasterDetailRule> MasterDetailRules {
            get { return Frame.GetController<MasterDetailRuleController>().MasterDetailRules; }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            var editor = View.Editor as XpandGridListEditor;
            if (editor == null)
                return;

            if (editor.Grid != null) {
                editor.Grid.ViewRegistered -= Grid_ViewRegistered;
                editor.Grid.ViewRemoved -= Grid_ViewRemoved;

                if (editor.GridView == editor.Grid.MainView) {
                    List<XpandXafGridView> gridViews = editor.Grid.Views.OfType<XpandXafGridView>().ToList();
                    for (int i = gridViews.Count() - 1; i > -1; i--) {
                        XpandXafGridView xpandXafGridView = gridViews[i];
                        if (xpandXafGridView.Window != null)
                            ((WinWindow)xpandXafGridView.Window).Form.Close();
                    }
                }
            }

            if (editor.GridView != null) {
                editor.GridView.MasterRowGetRelationCount -= ViewOnMasterRowGetRelationCount;
                editor.GridView.MasterRowGetRelationName -= ViewOnMasterRowGetRelationName;
                editor.GridView.MasterRowGetRelationDisplayCaption -= MasterRowGetRelationDisplayCaption;
                editor.GridView.MasterRowGetChildList -= ViewOnMasterRowGetChildList;
                editor.GridView.MasterRowEmpty -= ViewOnMasterRowEmpty;
                editor.GridView.MasterRowGetLevelDefaultView -= ViewOnMasterRowGetLevelDefaultView;
            }
        }

        protected override void OnViewControlsCreated() {
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

        void ViewOnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e) {
            object row = ((XpandXafGridView)sender).GetRow(e.RowHandle);
            if (e.RelationIndex>-1)
                e.ChildList = (IList)MasterDetailRules[e.RelationIndex].CollectionMember.MemberInfo.GetValue(row);
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1)
                e.RelationName = MasterDetailRules[e.RelationIndex].CollectionMember.Name;
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex>-1){
                var masterDetailRule = MasterDetailRules[e.RelationIndex];
                e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.TypeInfo,
                                                                masterDetailRule.CollectionMember.Name);
            }
        }


        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e) {
            e.RelationCount = MasterDetailRules.Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            if (e.RelationIndex > -1) {
                var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
                var levelDefaultView = gridViewBuilder.GetLevelDefaultView((XpandXafGridView) sender, e.RowHandle,
                                                                           e.RelationIndex, View.Model,
                                                                           MasterDetailRules);
                e.DefaultView = levelDefaultView;
            }
        }

        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs e) {
            if (e.RelationIndex > -1) {
                var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model,
                                                                                      (XpandXafGridView) sender,
                                                                                      MasterDetailRules);
                e.IsEmpty =
                    !modelDetailRelationCalculator.IsRelationSet(e.RowHandle, e.RelationIndex);
            }
        }

        void Grid_ViewRegistered(object sender, ViewOperationEventArgs e) {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var parentGridView = (XpandXafGridView)e.View.ParentView;
            var frame = parentGridView.Window ?? Frame;
            List<IMasterDetailRule> masterDetailRules = frame.GetController<MasterDetailRuleController>().MasterDetailRules;
            gridViewBuilder.ModifyInstanceGridView(parentGridView, e.View.SourceRowHandle, parentGridView.GetRelationIndex(e.View.SourceRowHandle, e.View.LevelName), ((XpandListView)frame.View).Model, masterDetailRules);
        }

        void Grid_ViewRemoved(object sender, ViewOperationEventArgs e) {
            var window = ((XpandXafGridView)e.View).Window as WinWindow;
            if (window != null && window.Form != null)
                window.Form.Close();
        }

    }
}