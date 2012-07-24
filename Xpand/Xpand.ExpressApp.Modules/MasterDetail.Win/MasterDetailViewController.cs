using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using MS.Internal.Xml.XPath;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors;


namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailViewController : MasterDetailViewControllerBase {
        List<MasterDetailRuleInfo> _masterDetailRuleInfos;


        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing -= ShowNavigationItemActionOnExecuting;
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
            var needsRuleArgs = new NeedsRuleArgs(Frame);
            OnNeedsRule(needsRuleArgs);
            _masterDetailRuleInfos = needsRuleArgs.Rules;
            if (_masterDetailRuleInfos != null) {
                Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing +=
                    ShowNavigationItemActionOnExecuting;
                XpandXafGridView view = ((XpandGridListEditor)View.Editor).GridView;
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
        }

        void ShowNavigationItemActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            InfoCollection availableViews = ((XpandGridListEditor)View.Editor).Grid.AvailableViews;
            foreach (var availableView in availableViews.OfType<XpandXafGridView>()) {
                CloseNestedWindow(availableView);
            }
        }

        void ViewOnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e) {
            object row = ((XpandXafGridView)sender).GetRow(e.RowHandle);
            if (e.RelationIndex > -1)
                e.ChildList = (IList)_masterDetailRuleInfos[e.RelationIndex].CollectionMember.MemberInfo.GetValue(row);
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e) {
            if (View.Id != "MasterDetailAtAnyLevelCustomer_ListView" && View.Id != "MasterDetailAtAnyLevelOrder_ListView")
                Debug.Print("");

            if (e.RelationIndex > -1)
                e.RelationName = _masterDetailRuleInfos[e.RelationIndex].CollectionMember.Name;
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1) {
                var masterDetailRule = _masterDetailRuleInfos[e.RelationIndex];
                e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.TypeInfo, masterDetailRule.CollectionMember.Name);
            }
        }


        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e) {
            e.RelationCount = _masterDetailRuleInfos.Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            if (e.RelationIndex > -1) {
                var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
                var levelDefaultView = gridViewBuilder.GetLevelDefaultView((XpandXafGridView)sender, e.RowHandle, e.RelationIndex, View.Model, _masterDetailRuleInfos);
                e.DefaultView = levelDefaultView;
            }
        }

        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs e) {
            if (e.RelationIndex > -1) {
                var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (XpandXafGridView)sender, _masterDetailRuleInfos);
                e.IsEmpty = !modelDetailRelationCalculator.IsRelationSet(e.RowHandle, e.RelationIndex);
            }
        }

        void Grid_ViewRegistered(object sender, ViewOperationEventArgs e) {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var parentGridView = (XpandXafGridView)e.View.ParentView;
            var frame = parentGridView.Window ?? Frame;
            var needsRuleArgs = new NeedsRuleArgs(frame);
            OnNeedsRule(needsRuleArgs);
            gridViewBuilder.ModifyInstanceGridView(parentGridView, e.View.SourceRowHandle, parentGridView.GetRelationIndex(e.View.SourceRowHandle, e.View.LevelName), ((XpandListView)frame.View).Model, needsRuleArgs.Rules);
        }

        void Grid_ViewRemoved(object sender, ViewOperationEventArgs e) {
            CloseNestedWindow(e.View);
        }

        static void CloseNestedWindow(BaseView baseView) {
            var window = ((XpandXafGridView)baseView).Window as WinWindow;
            if (window != null && window.Form != null)
                window.Form.Close();
        }
    }

}