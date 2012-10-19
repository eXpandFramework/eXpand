using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Linq;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    [ModelAbstractClass]
    public interface IModelListViewRememberGridSelection : IModelListView {
        [Category("eXpand")]
        bool RememberSelection { get; set; }
        [Browsable(false)]
        string SelectedRows { get; set; }
    }
    public class RememberGridSelectionController : ViewController<ListView>, IModelExtender {
        bool _handleCreated;

        protected override void OnActivated() {
            base.OnActivated();
            if (Model.RememberSelection) {
                View.SelectionChanged += ViewOnSelectionChanged;
            }
        }

        IModelListViewRememberGridSelection Model {
            get { return ((IModelListViewRememberGridSelection)View.Model); }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var gridControl = View.Editor.Control as GridControl;
            if (Model.RememberSelection && gridControl != null) {
                gridControl.HandleCreated += GridControlOnHandleCreated;
            }
        }

        void GridControlOnHandleCreated(object sender, EventArgs eventArgs) {
            if (!string.IsNullOrEmpty(Model.SelectedRows)) {
                var columnView = (DevExpress.XtraGrid.Views.Grid.GridView)((GridControl)(View.Editor.Control)).FocusedView;
                columnView.ClearSelection();
                columnView.OptionsSelection.MultiSelect = true;
                columnView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
                foreach (var row in Model.SelectedRows.Split(',')) {
                    var rowHandle = Convert.ToInt32(row);
                    columnView.SelectRow(rowHandle);
                }
            }
            _handleCreated = true;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (((IModelListViewRememberGridSelection)View.Model).RememberSelection)
                View.SelectionChanged -= ViewOnSelectionChanged;
        }

        void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            var gridControl = View.Editor.Control as GridControl;
            if (gridControl != null && _handleCreated) {
                var columnView = ((DevExpress.XtraGrid.Views.Base.ColumnView)gridControl.MainView);
                string[] enumerable = columnView.GetSelectedRows().Select(i => i + "").ToArray();
                Model.SelectedRows = string.Join(",", enumerable);
            }
        }
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewRememberGridSelection>();
        }
        #endregion
    }
}
