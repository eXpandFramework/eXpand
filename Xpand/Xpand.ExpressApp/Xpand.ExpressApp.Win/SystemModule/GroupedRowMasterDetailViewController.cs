using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelColumnMasterDetailView {
        [DataSourceProperty(ModelColumnDetailViewsDomainLogic.DetailViews)]
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelBrowsable(typeof(ModelColumnMasterDetailViewVisibilityCalculator))]
        IModelDetailView MasterDetailView { get; set; }
    }

    public class ModelColumnMasterDetailViewVisibilityCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            var modelColumn = ((IModelColumn)node);
            return ((IModelListView)modelColumn.ParentView).MasterDetailMode == MasterDetailMode.ListViewAndDetailView
                && (modelColumn.ModelMember!=null&&modelColumn.ModelMember.MemberInfo.MemberTypeInfo.IsDomainComponent);
        }
    }

    public class GroupedRowMasterDetailViewController : ViewController<ListView>, IModelExtender {

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (View.MasterDetailMode == MasterDetailMode.ListViewAndDetailView && GridView != null) {
                GridView.FocusedRowChanged += GridViewOnFocusedRowChanged;
            }
        }

        void GridViewOnFocusedRowChanged(object sender, FocusedRowChangedEventArgs focusedRowChangedEventArgs) {
            if (GridView.DataController.IsUpdateLocked) {
                return;
            }
            var focusedRowHandle = focusedRowChangedEventArgs.FocusedRowHandle;
            var masterDetailView = GetMasterDetailView(focusedRowHandle);
            if (masterDetailView != null) {
                var groupRowValue = GridView.GetGroupRowValue(focusedRowHandle);
                var detailView = CreateDetailView(groupRowValue, masterDetailView);
                View.EditFrame.SetView(detailView, null);
                if (View.IsControlCreated) {
                    View.EditFrame.View.CreateControls();
                    View.LayoutManager.ReplaceControl(ListView.DetailViewControlID, View.EditFrame.View.Control);
                }
            }
        }

        IModelDetailView GetMasterDetailView(int focusedRowHandle) {
            if (GridView.IsGroupRow(focusedRowHandle)) {
                var rowLevel = GridView.GetRowLevel(focusedRowHandle);
                var gridColumn = GridView.GroupedColumns[rowLevel];
                return ((IModelColumnMasterDetailView)gridColumn.Model()).MasterDetailView;
            }
            return null;
        }

        DetailView CreateDetailView(object groupRowValue, IModelDetailView modelDetailView) {
            var objectSpace = Application.CreateObjectSpace();
            groupRowValue = objectSpace.GetObject(groupRowValue);
            return Application.CreateDetailView(objectSpace, modelDetailView, false, groupRowValue);
        }

        GridView GridView {
            get {
                var columnsListEditor = View.Editor as ColumnsListEditor;
                return columnsListEditor != null ? columnsListEditor.GridView() : null;
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelColumn, IModelColumnMasterDetailView>();
        }
    }
}