using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailDeleteViewController : ListViewController<ExpressApp.Win.ListEditors.GridListEditor>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
//            Frame.GetController<DeleteObjectsViewController>().Deleting += OnDeleting;
        }
        void OnDeleting(object sender, DeletingEventArgs deletingEventArgs)
        {
            GridControl gridControl = ((GridListEditor)View.Editor).Grid;
            if (gridControl.FocusedView != gridControl.MainView)
            {
                deletingEventArgs.Objects.Clear();
                IEnumerable selectedObjects = GetSelectedObjects((GridView)gridControl.FocusedView);
                if (selectedObjects != null)
                    foreach (var selectedObject in selectedObjects)
                    {
                        deletingEventArgs.Objects.Add(selectedObject);
                    }
            }
        }

        IEnumerable GetSelectedObjects(GridView focusedView)
        {
            int[] selectedRows = focusedView.GetSelectedRows();
            if ((selectedRows != null) && (selectedRows.Length > 0))
            {
                IEnumerable<object> objects = selectedRows.Where(rowHandle => rowHandle > -1).Select(rowHandle
                                                                                                     => focusedView.GetRow(rowHandle)).Where(obj => obj != null);
                return objects;
            }
            return null;
        }
    }
}