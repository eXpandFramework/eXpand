using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView {
    public class ColumnViewEditorColumnChooserController : ColumnChooserControllerBase {
        DevExpress.XtraGrid.Views.Grid.GridView gridView;
        IXafGridColumn selectedColumn;

        public ColumnViewEditorColumnChooserController() {
            TypeOfView = typeof(ListView);
        }

        GridListEditorBase GridEditor {
            get { return ((ListView)View).Editor as GridListEditorBase; }
        }

        protected override ITypeInfo DisplayedTypeInfo {
            get { return View.ObjectTypeInfo; }
        }

        protected override Control ActiveListBox {
            get { return gridView.CustomizationForm.ActiveListBox; }
        }

        void CustomizationForm_FormClosing(object sender, FormClosingEventArgs e) {
            var form = sender as Form;
            if (form != null) {
                (form).Owner = null;
            }
        }

        void columnChooser_SelectedColumnChanged(object sender, EventArgs e) {
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = -1;
            }
            selectedColumn = (IXafGridColumn)gridView.CustomizationForm.ActiveListBox.SelectedItem;
            if (selectedColumn != null) {
                //                selectedColumn.ImageIndex = GridPainter.IndicatorFocused;
            }
            RemoveButton.Enabled = selectedColumn != null;
            gridView.CustomizationForm.Refresh();
        }

        void gridView_ShowCustomizationForm(object sender, EventArgs e) {
            InsertButtons();
            selectedColumn = null;
            gridView.CustomizationForm.FormClosing += CustomizationForm_FormClosing;
            gridView.CustomizationForm.ActiveListBox.SelectedItem = null;
            gridView.CustomizationForm.ActiveListBox.KeyDown += ActiveListBox_KeyDown;
            gridView.CustomizationForm.ActiveListBox.SelectedValueChanged += columnChooser_SelectedColumnChanged;
            //gridView.Images = GridPainter.Indicator;
        }

        void ActiveListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                RemoveSelectedColumn();
            }
        }

        void gridView_HideCustomizationForm(object sender, EventArgs e) {
            DeleteButtons();
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = -1;
            }
            gridView.Images = null;
        }

        void gridView_DragObjectDrop(object sender, DragObjectDropEventArgs e) {
            if ((gridView.CustomizationForm != null) && (selectedColumn != null)) {
                if (e.DragObject is GridColumn) {
                    selectedColumn.ImageIndex = -1;
                    if (gridView.CustomizationForm.ActiveListBox.Items.Count != 0) {
                        selectedColumn = (IXafGridColumn)gridView.CustomizationForm.ActiveListBox.Items[0];
                        selectedColumn.ImageIndex = GridPainter.IndicatorFocused;
                        gridView.CustomizationForm.ActiveListBox.InvalidateObject(selectedColumn);
                        gridView.CustomizationForm.ActiveListBox.Update();
                    } else {
                        selectedColumn = null;
                    }
                    gridView.CustomizationForm.ActiveListBox.SelectedItem = selectedColumn;
                }
            }
        }

        void GridEditorController_ViewControlsCreated(object sender, EventArgs e) {
            SubscribeToGridEditorEvents();
        }

        void SubscribeToGridEditorEvents() {
            if (GridEditor != null) {
                gridView = (DevExpress.XtraGrid.Views.Grid.GridView)GridEditor.GridView;
                gridView.ShowCustomizationForm += gridView_ShowCustomizationForm;
                gridView.HideCustomizationForm += gridView_HideCustomizationForm;
                gridView.DragObjectDrop += gridView_DragObjectDrop;
            }
        }

        protected override List<string> GetUsedProperties() {
            return GridEditor.Model.Columns.Select(columnInfoNodeWrapper => columnInfoNodeWrapper.PropertyName).ToList();
        }

        protected override void AddColumn(string propertyName) {
            IModelColumn columnInfo = FindColumnModelByPropertyName(propertyName);
            if (columnInfo == null) {
                columnInfo = ((ListView)View).Model.Columns.AddNode<IModelColumn>();
                columnInfo.Id = propertyName;
                columnInfo.PropertyName = propertyName;
                columnInfo.Index = -1;
                GridEditor.AddColumn(columnInfo);
            } else {
                throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(
                    ExceptionId.CannotAddDuplicateProperty, propertyName));
            }
            gridView.CustomizationForm.CheckAndUpdate();
        }

        protected override void RemoveSelectedColumn() {
            var xafGridColumn = gridView.CustomizationForm.ActiveListBox.SelectedItem as IXafGridColumn;
            if (xafGridColumn != null) {
                GridEditor.RemoveColumn(new XpandGridColumnWrapper(xafGridColumn));
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (!View.IsControlCreated) {
                View.ControlsCreated += GridEditorController_ViewControlsCreated;
            } else {
                SubscribeToGridEditorEvents();
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.ControlsCreated -= GridEditorController_ViewControlsCreated;
            if (gridView != null) {
                gridView.ShowCustomizationForm -= gridView_ShowCustomizationForm;
                gridView.HideCustomizationForm -= gridView_HideCustomizationForm;
                gridView.DragObjectDrop -= gridView_DragObjectDrop;
                gridView = null;
            }
            selectedColumn = null;
        }
    }
}