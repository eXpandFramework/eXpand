using System;
using System.Collections;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using eXpand.ExpressApp.Win.ListEditors;
using GridListEditor=eXpand.ExpressApp.Win.ListEditors.GridListEditor;
using ListView=DevExpress.ExpressApp.ListView;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class CustomSelectionListViewViewController : ExpressApp.SystemModule.CustomSelectionListViewViewController
    {
        private ArrayList checkedObjects=new ArrayList();

        public CustomSelectionListViewViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            checkedObjects=new ArrayList();
            if (HasCustomSelection)
            {
                
                View.ControlsCreated+=View_OnControlsCreated;
                Frame.GetController<WindowHintController>().WarningHintPanelReady+=WarningHintPanelReady;
                
            }
        }


        private void WarningHintPanelReady(object sender, HintPanelReadyEventArgs e)
        {
            if (View.Control is GridControl && CustomSelectionColumnIsVisible((GridControl) View.Control) &&
                ((GridView) ((GridControl) View.Control).MainView).Columns.ColumnByFieldName(CustomSelection) != null)
            {
                if (!(((ListView) View).Editor is GridListEditor))
                {
                    var bottomHintPanel = Frame.GetController<WindowHintController>().BottomHintPanel;
                    bottomHintPanel.Text = "Custom Selection Can only work with " + typeof (GridListEditor).FullName;
                    bottomHintPanel.Visible = true;
                }
            }
        }

        private void View_OnControlsCreated(object sender, EventArgs e)
        {
            var gridControl = View.Control as GridControl;
            if (gridControl != null)
            {
                if (CustomSelectionColumnIsVisible(gridControl))
                {
                    var editor = ((ListView) View).Editor as GridListEditor;
                    if (editor!= null)
                    {
                        editor.DataSourceChanged += (sender1, e1) => checkedObjects = new ArrayList();
                        editor.CustomGetSelectedObjects+=Editor_OnCustomGetSelectedObjects;
                        gridControl.KeyDown += GridControl_OnKeyDown;
                        gridControl.DoubleClick+=GridControl_OnDoubleClick;   
                        gridControl.FocusedView.MouseDown+=MouseDown;
                        ((GridView) gridControl.FocusedView).CustomRowCellEdit+=CustomRowCellEdit;
                    }
                }
            }
        }

        private void GridControl_OnDoubleClick(object sender, EventArgs e)
        {
            var control = ((GridControl) sender);
            var focusedView = (GridView) control.FocusedView;
            object row = focusedView.GetRow(focusedView.FocusedRowHandle);

            CheckSelectedObjects(new ArrayList(checkedObjects));

            checkedObjects.Add(row);
            SimpleAction currentObjectAction =
                Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction;
            currentObjectAction.Enabled[ActionBase.RequireSingleObjectContext] = true;
            currentObjectAction.DoExecute();
            View.Refresh();
                
        }

        private void CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName==CustomSelection)
                ((RepositoryItemBooleanEdit)e.RepositoryItem).NullStyle=StyleIndeterminate.Unchecked;
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            var view = ((GridView) sender);
            GridHitInfo info = view.CalcHitInfo(e.Location);
            if (info.HitTest==GridHitTest.RowCell)
//                if (info.Column != null && info.Column.FieldName == CustomSelection)
                CheckObject(view.GetRow(info.RowHandle));
        }

        private void Editor_OnCustomGetSelectedObjects(object sender, CustomGetSelectedObjectsArgs e)
        {
            e.Handled = true;
            e.List = checkedObjects;
//            Frame.GetController<FilterController>().FullTextFilterAction.Value = checkedObjects.Count;
                
        }

        private bool CustomSelectionColumnIsVisible(GridControl gridControl)
        {
            return ((GridView) gridControl.MainView).Columns.ColumnByFieldName(CustomSelection) != null;
        }

        private bool IsGroupRowHandle(int handle)
        {
            return handle < 0;
        }

        //custom listview
        private void GridControl_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                CheckObjects((GridControl) sender);
        }

        private void CheckObjects(GridControl sender)
        {
            ArrayList selectedObjects = GetSelectedObjects(sender);
            CheckSelectedObjects(selectedObjects);
        }

        private void CheckSelectedObjects(ArrayList selectedObjects)
        {
            foreach (var selectedObject in selectedObjects)
                CheckObject(selectedObject);
        }

        private void CheckObject(object selectedObject)
        {
            var xpBaseObject = ((XPBaseObject) selectedObject);
            if (xpBaseObject != null)
            {
                var b = (bool?) xpBaseObject.GetMemberValue(CustomSelection);
                if (!b.HasValue)
                    b = true;
                else
                    b = !b;
                xpBaseObject.SetMemberValue(CustomSelection,b);
                if (b.Value)
                    checkedObjects.Add(xpBaseObject);
                else
                    checkedObjects.Remove(xpBaseObject);
            }
        }

        private ArrayList GetSelectedObjects(object sender)
        {
            var selectedObjects = new ArrayList();

            var gridView = ((GridView) ((GridControl) sender).MainView);
            int[] selectedRows = gridView.GetSelectedRows();
            if ((selectedRows != null) && (selectedRows.Length > 0))
                foreach (int rowHandle in selectedRows)
                    if (!IsGroupRowHandle(rowHandle))
                    {
                        object obj = gridView.GetRow(rowHandle);
                        if (obj != null)
                            selectedObjects.Add(obj);
                    }
            return selectedObjects;
        }
    }
}