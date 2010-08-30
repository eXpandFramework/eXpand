using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.ListEditors;

namespace eXpand.ExpressApp.Win.SystemModule {
    public interface IModelClassAutoExpandNewRow : IModelNode
    {
        [Category("eXpand")]
        [Description("If gridview in master detail auto expand new inserter row")]
        bool AutoExpandNewRow { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassAutoExpandNewRow), "ModelClass")]
    public interface IModelListViewAutoExpandNewRow : IModelClassAutoExpandNewRow
    {
    }
    public class AutoExpandNewRowController:ListViewController<GridListEditor>,IModelExtender
    {
        XafGridView _xafGridView;
        bool _newRowAdded;

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (((IModelListViewAutoExpandNewRow)View.Model).AutoExpandNewRow) {
                _xafGridView = ((GridListEditor)View.Editor).GridView;
                _xafGridView.FocusedRowChanged += GridView_OnFocusedRowChanged;
                _xafGridView.InitNewRow += GridView_OnInitNewRow;
            }
        }
        void GridView_OnInitNewRow(object sender, InitNewRowEventArgs e)
        {
            _newRowAdded = true;
        }

        void GridView_OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (_newRowAdded && _xafGridView.IsValidRowHandle(e.FocusedRowHandle))
            {
                _newRowAdded = false;
                int visibleDetailRelationIndex = _xafGridView.GetVisibleDetailRelationIndex(e.FocusedRowHandle);
                var gridView = ((GridView) _xafGridView.GridControl.FocusedView);
                gridView.ExpandMasterRow(e.FocusedRowHandle, visibleDetailRelationIndex);
                visibleDetailRelationIndex = gridView.GetVisibleDetailRelationIndex(e.FocusedRowHandle);
                var detailView = ((GridView) gridView.GetDetailView(e.FocusedRowHandle, visibleDetailRelationIndex));
                if (detailView != null) {
                    _xafGridView.GridControl.FocusedView = detailView;
                    detailView.FocusedRowHandle = GridControl.NewItemRowHandle;
                }
            }
        }


        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassAutoExpandNewRow>();
            extenders.Add<IModelListView, IModelListViewAutoExpandNewRow>();
        }

        #endregion
    }
}