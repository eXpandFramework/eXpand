﻿using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelClassAutoExpandNewRow : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("If gridview in master detail auto expand new inserter row")]
        bool AutoExpandNewRow { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassAutoExpandNewRow), "ModelClass")]
    public interface IModelListViewAutoExpandNewRow : IModelClassAutoExpandNewRow {
    }
    public class AutoExpandNewRowController : ListViewController<ColumnsListEditor>, IModelExtender {
        GridView _xafGridView;
        bool _newRowAdded;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (((IModelListViewAutoExpandNewRow)View.Model).AutoExpandNewRow) {
                var gridControl = (GridControl)View.Editor.Control;
                gridControl.ProcessGridKey += GridControlOnProcessGridKey;
                _xafGridView = ((WinColumnsListEditor)View.Editor).GridView();
                _xafGridView.FocusedRowChanged += GridView_OnFocusedRowChanged;
                _xafGridView.InitNewRow += GridView_OnInitNewRow;
            }
        }

        void GridControlOnProcessGridKey(object sender, KeyEventArgs e) {
            var view = ((GridView)((GridControl)sender).FocusedView);
            if (view.IsNewItemRow(view.FocusedRowHandle) && (e.KeyData == Keys.Enter ||
                (e.KeyData == Keys.Tab && view.FocusedColumn == view.Columns[view.Columns.Count - 1]))) {
                view.CloseEditor();
                view.UpdateCurrentRow();
                e.Handled = true;
            }
        }

        void GridView_OnInitNewRow(object sender, InitNewRowEventArgs e) {
            _newRowAdded = true;
        }

        void GridView_OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
            if (_newRowAdded && _xafGridView.IsValidRowHandle(e.FocusedRowHandle)) {
                _newRowAdded = false;
                int visibleDetailRelationIndex = _xafGridView.GetVisibleDetailRelationIndex(e.FocusedRowHandle);
                var gridView = ((GridView)_xafGridView.GridControl.FocusedView);
                gridView.ExpandMasterRow(e.FocusedRowHandle, visibleDetailRelationIndex);
                visibleDetailRelationIndex = gridView.GetVisibleDetailRelationIndex(e.FocusedRowHandle);
                var detailView = ((GridView)gridView.GetDetailView(e.FocusedRowHandle, visibleDetailRelationIndex));
                if (detailView != null) {
                    _xafGridView.GridControl.FocusedView = detailView;
                    detailView.FocusedRowHandle = GridControl.NewItemRowHandle;
                }
            }
        }


        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassAutoExpandNewRow>();
            extenders.Add<IModelListView, IModelListViewAutoExpandNewRow>();
        }

        #endregion
    }
}