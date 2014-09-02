using System;
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
using Fasterflect;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.Persistent.Base.General;
using Frame = DevExpress.ExpressApp.Frame;

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
                if (!columnView.IsDetailView(editor)) {
                    var gridViews = editor.Grid.Views.OfType<IMasterDetailColumnView>().ToList();
                    for (int i = gridViews.Count() - 1; i > -1; i--) {
                        var xpandXafGridView = gridViews[i];
                        CloseNestedWindow(xpandXafGridView);
                    }
                }
            }

            if (columnView != null) {
                columnView.MasterRowCollapsing -= GridViewOnMasterRowCollapsing;
                columnView.MasterRowGetRelationCount -= ViewOnMasterRowGetRelationCount;
                columnView.MasterRowGetRelationName -= ViewOnMasterRowGetRelationName;
                columnView.MasterRowGetRelationDisplayCaption -= MasterRowGetRelationDisplayCaption;
                columnView.MasterRowGetChildList -= ViewOnMasterRowGetChildList;
                columnView.MasterRowEmpty -= ViewOnMasterRowEmpty;
                columnView.MasterRowGetLevelDefaultView -= ViewOnMasterRowGetLevelDefaultView;
                if (columnView.IsDetailView(editor)) {
                    View.CollectionSource.CriteriaApplied -= CollectionSourceOnCriteriaApplied;
                }
            }
        }

        void GridViewOnMasterRowCollapsing(object sender, MasterRowCanExpandEventArgs masterRowCanExpandEventArgs) {
            var detailView = (IMasterDetailColumnView)((IMasterDetailColumnView) sender).GetDetailView(masterRowCanExpandEventArgs.RowHandle, masterRowCanExpandEventArgs.RelationIndex);
            detailView.Window.View.SaveModel();
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var columnViewEditor = View.Editor as IColumnViewEditor;
            if (columnViewEditor != null ) {
                SyncronizeDataSourceWithCriteria(columnViewEditor);
                if (IsMasterDetail()) {
                    var masterDetailColumnView = (IMasterDetailColumnView) (columnViewEditor).ColumnView;
                    Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing +=ShowNavigationItemActionOnExecuting;
                    var grid = columnViewEditor.Grid;
                    grid.ViewRegistered += Grid_ViewRegistered;
                    grid.ViewRemoved += Grid_ViewRemoved;
                    masterDetailColumnView.MasterRowGetRelationCount += ViewOnMasterRowGetRelationCount;
                    masterDetailColumnView.MasterRowGetRelationName += ViewOnMasterRowGetRelationName;
                    masterDetailColumnView.MasterRowGetRelationDisplayCaption += MasterRowGetRelationDisplayCaption;
                    masterDetailColumnView.MasterRowGetChildList += ViewOnMasterRowGetChildList;
                    masterDetailColumnView.MasterRowEmpty += ViewOnMasterRowEmpty;
                    masterDetailColumnView.MasterRowGetLevelDefaultView += ViewOnMasterRowGetLevelDefaultView;
                    masterDetailColumnView.MasterRowCollapsing += GridViewOnMasterRowCollapsing;
                    grid.FocusedViewChanged+=GridOnFocusedViewChanged;
                }
            }
        }

        private void GridOnFocusedViewChanged(object sender, ViewFocusEventArgs viewFocusEventArgs){
            var columnView = ((IMasterDetailColumnView) viewFocusEventArgs.View);
            if (columnView != null && (columnView!=columnView.GridControl.MainView && columnView.Window != null)){
                ((ListView)columnView.Window.View).Editor.CallMethod("OnFocusedObjectChanged");
            }
        }

        void SyncronizeDataSourceWithCriteria(IColumnViewEditor columnViewEditor) {
            var detailColumnView = (IMasterDetailColumnView) (columnViewEditor).ColumnView.GridControl.FocusedView;
            if (detailColumnView.IsDetailView(columnViewEditor)) {
                EventHandler[] eventHandlers = {null};
                eventHandlers[0] = (sender, args) => {
                    var dataSource = ((IColumnViewEditor) View.Editor).ColumnView.DataSource;
                    ObjectSpace.ApplyCriteria(dataSource, View.CollectionSource.GetCriteria());
                    ((IColumnViewEditor)View.Editor).ColumnView.DataSourceChanged -= eventHandlers[0];
                };
                ((IColumnViewEditor)View.Editor).ColumnView.DataSourceChanged+= eventHandlers[0];
                View.CollectionSource.CriteriaApplied += CollectionSourceOnCriteriaApplied;
            }
        }

        void CollectionSourceOnCriteriaApplied(object sender, EventArgs eventArgs) {
            var dataSource = ((IColumnViewEditor) View.Editor).ColumnView.DataSource;
            ObjectSpace.ApplyCriteria(dataSource, View.CollectionSource.GetCriteria());
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

        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e){
            if (e.RowHandle==GridControl.InvalidRowHandle)
                e.RelationCount = 1;
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
                window.Form.Close();
            }
        }
    }

}