using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail {
    public abstract class MasterDetailViewController : ViewController<ListView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewMasterDetail>();
        }

        public List<MasterDetailRuleInfo> FilterRules(object currentObject) {
            return CreateRules().Where(info => info.Criteria.Fit(currentObject)).ToList();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing -= ShowNavigationItemActionOnExecuting;
            var editor = View.Editor as IColumnViewEditor;
            if (editor == null)
                return;

            var columnView = editor.GridView as IMasterDetailColumnView;
            if (editor.Grid != null) {
                editor.Grid.ViewRegistered -= Grid_ViewRegistered;
                editor.Grid.ViewRemoved -= Grid_ViewRemoved;
                if (columnView == editor.Grid.MainView) {
                    var gridViews = editor.Grid.Views.OfType<IMasterDetailColumnView>().ToList();
                    for (int i = gridViews.Count() - 1; i > -1; i--) {
                        var xpandXafGridView = gridViews[i];
                        if (xpandXafGridView.Window != null)
                            ((WinWindow)xpandXafGridView.Window).Form.Close();
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
                var view = (IMasterDetailColumnView)(columnViewEditor).GridView;
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

        public bool IsMasterDetail() {
            var optionsColumnView = ModelOptionsColumnView();
            if (optionsColumnView == null) return false;
            var masterDetail = optionsColumnView.GetNode("OptionsDetail").GetValue<bool?>("EnableMasterViewMode");
            return masterDetail.HasValue && masterDetail.Value && ((IModelListViewMasterDetail)View.Model).MasterDetails.Any();
        }

        IModelOptionsColumnView ModelOptionsColumnView() {
            var advBandedListEditor = View.Editor as AdvBandedListEditor;
            if (advBandedListEditor != null) {
                return ((IModelListViewOptionsAdvBandedView)View.Model).OptionsAdvBandedView;
            }
            var isGridListEditor = View.Editor is XpandGridListEditor || View.Editor is GridListEditor;
            return isGridListEditor ? ((IModelListViewOptionsGridView)View.Model).OptionsGridView : null;
        }

        void ShowNavigationItemActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            InfoCollection availableViews = ((IColumnViewEditor)View.Editor).Grid.AvailableViews;
            foreach (var availableView in availableViews.OfType<IMasterDetailColumnView>()) {
                CloseNestedWindow(availableView);
            }
        }

        IEnumerable<MasterDetailRuleInfo> MasterDetailRuleInfos(IModelListViewMasterDetail modelListView) {
            return modelListView != null ? MasterDetailRuleInfosCore(modelListView.MasterDetails) : Enumerable.Empty<MasterDetailRuleInfo>();
        }

        IEnumerable<MasterDetailRuleInfo> MasterDetailRuleInfosCore(IEnumerable<IModelMasterDetail> masterMasterDetailListViews) {
            return from masterDetailListView in masterMasterDetailListViews let collectionMember = masterDetailListView.CollectionMember select MasterDetailRuleInfo(collectionMember, masterDetailListView);
        }

        MasterDetailRuleInfo MasterDetailRuleInfo(IModelMember collectionMember, IModelMasterDetail masterDetail) {
            var criteriaWithReadOnlyParameters = CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(masterDetail.Criteria, View.ObjectTypeInfo.Type);
            var listElementTypeInfo = collectionMember.MemberInfo.ListElementTypeInfo;
            return new MasterDetailRuleInfo(masterDetail.ListView, collectionMember, listElementTypeInfo, criteriaWithReadOnlyParameters);
        }

        protected virtual List<MasterDetailRuleInfo> CreateRules() {
            return new List<MasterDetailRuleInfo>(MasterDetailRuleInfos(Frame.View.Model as IModelListViewMasterDetail));
        }

        void ViewOnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var masterDetailRuleInfo = GetRule(e.RelationIndex, currentObject);
                if (masterDetailRuleInfo != null)
                    e.ChildList = (IList)masterDetailRuleInfo.CollectionMember.MemberInfo.GetValue(currentObject);
            }
        }

        MasterDetailRuleInfo GetRule(int index, object currentObject) {
            if (currentObject != null) {
                var masterDetailRuleInfos = FilterRules(currentObject);
                return masterDetailRuleInfos.Count >= index + 1 ? masterDetailRuleInfos[index] : null;
            }
            return null;
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var masterDetailRuleInfo = GetRule(e.RelationIndex, currentObject);
                if (masterDetailRuleInfo != null) e.RelationName = masterDetailRuleInfo.CollectionMember.Name;
            }
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var masterDetailRule = FilterRules(currentObject)[e.RelationIndex];
                e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.TypeInfo, masterDetailRule.CollectionMember.Name);
            }
        }

        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e) {
            var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
            if (currentObject != null)
                e.RelationCount = FilterRules(currentObject).Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            if (e.RelationIndex > -1) {
                var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var levelDefaultView = gridViewBuilder.GetLevelDefaultView((IMasterDetailColumnView)sender, e.RowHandle, e.RelationIndex, View.Model, FilterRules(currentObject));
                e.DefaultView = levelDefaultView;
            }
        }

        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs e) {
            if (e.RelationIndex > -1) {
                var currentObject = ((IMasterDetailColumnView)sender).GetRow(e.RowHandle);
                var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (IMasterDetailColumnView)sender, FilterRules(currentObject));
                e.IsEmpty = !modelDetailRelationCalculator.IsRelationSet(e.RowHandle, e.RelationIndex);
            }
        }

        void Grid_ViewRegistered(object sender, ViewOperationEventArgs e) {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var parentGridView = (IMasterDetailColumnView)e.View.ParentView;
            var detailXafGridView = parentGridView;
            var frame = detailXafGridView.Window ?? Frame;

            var masterDetailRuleInfos = FilterRules(e.View.SourceRow);

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
            if (window != null && window.Form != null)
                window.Form.Close();
        }
    }

}