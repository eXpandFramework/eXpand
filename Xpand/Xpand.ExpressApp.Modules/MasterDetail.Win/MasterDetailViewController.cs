using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using MS.Internal.Xml.XPath;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailViewController : MasterDetailViewControllerBase {
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing -= ShowNavigationItemActionOnExecuting;
            var editor = View.Editor as IMasterDetailGridListEditor;
            if (editor == null)
                return;

            if (editor.Grid != null) {
                editor.Grid.ViewRegistered -= Grid_ViewRegistered;
                editor.Grid.ViewRemoved -= Grid_ViewRemoved;
                if (editor.GridView == editor.Grid.MainView) {
                    var gridViews = editor.Grid.Views.OfType<IMasterDetailXafGridView>().ToList();
                    for (int i = gridViews.Count() - 1; i > -1; i--) {
                        IMasterDetailXafGridView xpandXafGridView = gridViews[i];
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

            //            _masterDetailRuleInfos = RequestRules.Invoke(Frame);
            if (RequestRules.Invoke(Frame).Any() & View.Editor is IMasterDetailGridListEditor) {
                Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executing +=
                    ShowNavigationItemActionOnExecuting;
                var view = ((IMasterDetailGridListEditor)View.Editor).GridView;
                var grid = ((IMasterDetailGridListEditor)View.Editor).Grid;
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
            InfoCollection availableViews = ((IMasterDetailGridListEditor)View.Editor).Grid.AvailableViews;
            foreach (var availableView in availableViews.OfType<IMasterDetailXafGridView>()) {
                CloseNestedWindow(availableView);
            }
        }

        void ViewOnMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e) {
            object row = ((IMasterDetailXafGridView)sender).GetRow(e.RowHandle);
            if (e.RelationIndex > -1)
                e.ChildList = (IList)RequestRules.Invoke(Frame)[e.RelationIndex].CollectionMember.MemberInfo.GetValue(row);
        }

        void ViewOnMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1)
                e.RelationName = RequestRules.Invoke(Frame)[e.RelationIndex].CollectionMember.Name;
        }

        void MasterRowGetRelationDisplayCaption(object sender, MasterRowGetRelationNameEventArgs e) {
            if (e.RelationIndex > -1) {
                var masterDetailRule = RequestRules.Invoke(Frame)[e.RelationIndex];
                e.RelationName = CaptionHelper.GetMemberCaption(masterDetailRule.TypeInfo, masterDetailRule.CollectionMember.Name);
            }
        }


        void ViewOnMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e) {
            e.RelationCount = RequestRules.Invoke(Frame).Count;
        }

        void ViewOnMasterRowGetLevelDefaultView(object sender, MasterRowGetLevelDefaultViewEventArgs e) {
            if (e.RelationIndex > -1) {
                var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
                var levelDefaultView = gridViewBuilder.GetLevelDefaultView((IMasterDetailXafGridView)sender, e.RowHandle, e.RelationIndex, View.Model, RequestRules.Invoke(Frame));
                e.DefaultView = (BaseView)levelDefaultView;
            }
        }

        void ViewOnMasterRowEmpty(object sender, MasterRowEmptyEventArgs e) {
            if (e.RelationIndex > -1) {
                var modelDetailRelationCalculator = new ModelDetailRelationCalculator(View.Model, (IMasterDetailXafGridView)sender, RequestRules.Invoke(Frame));
                e.IsEmpty = !modelDetailRelationCalculator.IsRelationSet(e.RowHandle, e.RelationIndex);
            }
        }

        void Grid_ViewRegistered(object sender, ViewOperationEventArgs e) {
            var gridViewBuilder = new GridViewBuilder(Application, ObjectSpace, Frame);
            var parentGridView = (XafGridView)e.View.ParentView;
            var detailXafGridView = (IMasterDetailXafGridView)parentGridView;
            var frame = detailXafGridView.Window ?? Frame;
            var masterDetailRuleInfos = RequestRules.Invoke(frame);

            var sourceRowHandle = e.View.SourceRowHandle;
            var relationIndex = parentGridView.GetRelationIndex(sourceRowHandle, e.View.LevelName);
            var masterModelListView = ((ListView)frame.View).Model;
            gridViewBuilder.ModifyInstanceGridView(detailXafGridView, sourceRowHandle, relationIndex, masterModelListView, masterDetailRuleInfos);
        }

        void Grid_ViewRemoved(object sender, ViewOperationEventArgs e) {
            CloseNestedWindow((IMasterDetailXafGridView)e.View);
        }

        void CloseNestedWindow(IMasterDetailXafGridView baseView) {
            var window = baseView.Window as WinWindow;
            if (window != null && window.Form != null)
                window.Form.Close();
        }
    }
    public class CustomGridCreateEventArgs : HandledEventArgs {
        public GridControl Grid { get; set; }
    }
    public class CustomGridViewCreateEventArgs : HandledEventArgs {
        public CustomGridViewCreateEventArgs(GridControl gridControl) {
            GridControl = gridControl;
        }

        public GridView GridView { get; set; }
        public GridControl GridControl { get; private set; }
    }
    public interface IMasterDetailXafGridView {
        Window Window { get; set; }
        Frame MasterFrame { get; set; }
        GridControl GridControl { get; set; }
        object DataSource { get; }
        int FocusedRowHandle { get; }
        BaseView GetDetailView(int rowHandle, int relationIndex);
        object GetRow(int rowHandle);
        string GetRelationName(int rowHandle, int relationIndex);
        GridHitInfo CalcHitInfo(Point location);
        event MasterRowGetRelationCountEventHandler MasterRowGetRelationCount;
        event MasterRowGetRelationNameEventHandler MasterRowGetRelationName;
        event MasterRowGetRelationNameEventHandler MasterRowGetRelationDisplayCaption;
        event MasterRowGetChildListEventHandler MasterRowGetChildList;
        event MasterRowEmptyEventHandler MasterRowEmpty;
        event MasterRowGetLevelDefaultViewEventHandler MasterRowGetLevelDefaultView;
        bool IsDataRow(int rowHandle);
        bool IsNewItemRow(int rowHandle);
        int[] GetSelectedRows();
    }

}