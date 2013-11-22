using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail {
    public abstract class MasterDetailViewController : ViewController<ListView> {

        public List<MasterDetailRuleInfo> FilterRules(object currentObject, Frame frame) {
            return CreateRules(frame).Where(info => info.Criteria.Fit(currentObject)).ToList();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing -= ShowNavigationItemActionOnExecuting;
            var editor = View.Editor as IColumnViewEditor;
            if (editor == null)
                return;

            var columnView = editor.ColumnView as IMasterDetailColumnView;
            if (editor.Grid != null) {
                editor.Grid.ViewRegistered -= Grid_ViewRegistered;
                editor.Grid.ViewRemoved -= Grid_ViewRemoved;
                if (columnView == editor.Grid.MainView) {
                    var gridViews = editor.Grid.Views.OfType<IMasterDetailColumnView>().ToList();
                    for (int i = gridViews.Count() - 1; i > -1; i--) {
                        var xpandXafGridView = gridViews[i];
                        CloseNestedWindow(xpandXafGridView);
                    }
                }
            }

            if (columnView != null) {
                columnView.MasterRowGetRelationCount -= ViewOnMasterRowGetRelationCount;
                columnView.MasterRowGetRelationName -= ViewOnMasterRowGetRelationName;
                columnView.MasterRowGetRelationDisplayCaption -= MasterRowGetRelationDisplayCaption;
                columnView.MasterRowGetChildList -= ViewOnMasterRowGetChildList;
                columnView.MasterRowEmpty -= ViewOnMasterRowEmpty;
                columnView.MasterRowGetLevelDefaultView -= ViewOnMasterRowGetLevelDefaultView;
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var columnViewEditor = View.Editor as IColumnViewEditor;
            if (columnViewEditor != null && IsMasterDetail()) {
                Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing +=
                    ShowNavigationItemActionOnExecuting;
                var view = (IMasterDetailColumnView)(columnViewEditor).ColumnView;
                var grid = (columnViewEditor).Grid;
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

        public abstract bool IsMasterDetail();

        void ShowNavigationItemActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            var availableViews = ((IColumnViewEditor)View.Editor).Grid.AvailableViews;
            foreach (var availableView in availableViews.OfType<IMasterDetailColumnView>()) {
                CloseNestedWindow(availableView);
            }
        }

        protected abstract List<MasterDetailRuleInfo> CreateRules(Frame frame1);

        void ViewOnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var masterDetailRuleInfo = GetRule(e.RelationIndex, currentObject, GetFrame(sender as IMasterDetailColumnView));
                if (masterDetailRuleInfo != null)
                    e.ChildList = (IList)masterDetailRuleInfo.CollectionMember.MemberInfo.GetValue(currentObject);
            }
        }

        MasterDetailRuleInfo GetRule(int index, object currentObject, Frame frame) {
            if (currentObject != null) {
                var masterDetailRuleInfos = FilterRules(currentObject, frame);
                return masterDetailRuleInfos.Count >= index + 1 ? masterDetailRuleInfos[index] : null;
            }
            return null;
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var masterDetailRuleInfo = GetRule(e.RelationIndex, currentObject, GetFrame(sender as IMasterDetailColumnView));
                if (masterDetailRuleInfo != null) e.RelationName = masterDetailRuleInfo.CollectionMember.Name;
            }
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var masterDetailRule = FilterRules(currentObject, GetFrame(sender as IMasterDetailColumnView))[e.RelationIndex];
                e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.TypeInfo, masterDetailRule.CollectionMember.Name);
            }
        }

        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e) {
            var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
            if (currentObject != null)
                e.RelationCount = FilterRules(currentObject, GetFrame(sender as IMasterDetailColumnView)).Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            if (e.RelationIndex > -1) {
                var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var masterDetailRuleInfos = FilterRules(currentObject, GetFrame(sender as IMasterDetailColumnView));
                var levelDefaultView = gridViewBuilder.GetLevelDefaultView((IMasterDetailColumnView)sender, e.RowHandle, e.RelationIndex, View.Model, masterDetailRuleInfos);
                e.DefaultView = levelDefaultView;
            }
        }

        Frame GetFrame(IMasterDetailColumnView columnView) {
            return columnView != null && columnView.Window != null
                       ? columnView.Window
                       : Frame;
        }

        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (IMasterDetailColumnView)sender, FilterRules(currentObject, GetFrame(sender as IMasterDetailColumnView)));
                e.IsEmpty = !modelDetailRelationCalculator.IsRelationSet(e.RowHandle, e.RelationIndex);
            }
        }

        void Grid_ViewRegistered(object sender, ViewOperationEventArgs e) {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var parentGridView = (IMasterDetailColumnView)e.View.ParentView;
            var detailXafGridView = parentGridView;
            var frame = detailXafGridView.Window ?? Frame;

            var masterDetailRuleInfos = FilterRules(e.View.SourceRow, frame);

            var sourceRowHandle = e.View.SourceRowHandle;
            var relationIndex = parentGridView.GetRelationIndex(sourceRowHandle, e.View.LevelName);
            var masterModelListView = ((ListView)frame.View).Model;
            gridViewBuilder.ModifyGridViewInstance(detailXafGridView, sourceRowHandle, relationIndex, masterModelListView, masterDetailRuleInfos);
        }

        void Grid_ViewRemoved(object sender, ViewOperationEventArgs e) {
            CloseNestedWindow((IMasterDetailColumnView)e.View);
        }

        void CloseNestedWindow(IMasterDetailColumnView baseView) {
            var window = baseView.Window as WinWindow;
            if (window != null && window.Form != null) {
                baseView.Window.View.SaveModel();
                window.Form.Close();
            }
        }

        public bool SynchronizeActions() {
            return FilterRules(View.CurrentObject, Frame).Any(info => info.SynchronizeActions);
        }
    }

}