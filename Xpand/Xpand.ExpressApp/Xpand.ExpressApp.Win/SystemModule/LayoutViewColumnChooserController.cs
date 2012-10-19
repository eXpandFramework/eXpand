using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.Customization;
using DevExpress.XtraLayout;
using Xpand.ExpressApp.Win.ListEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    [Obsolete("use Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.LayoutViewListEditor", true)]
    public class LayoutViewColumnChooserController : ColumnChooserControllerBase {
        private LayoutViewField selectedColumn;
        private LayoutView layoutView;
        private LayoutControl layoutControl;
        private LayoutViewCustomizationForm customizationFormCore;
        private LayoutViewListEditor ListEditor {
            get { return ((DevExpress.ExpressApp.ListView)View).Editor as LayoutViewListEditor; }
        }
        private void columnChooser_SelectedColumnChanged(object sender, EventArgs e) {
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = -1;
            }
            selectedColumn = ((ListBoxControl)ActiveListBox).SelectedItem as LayoutViewField;
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = GridPainter.IndicatorFocused;
            }
            RemoveButton.Enabled = selectedColumn != null;
        }
        private void layoutView_ShowCustomization(object sender, EventArgs e) {
            CustomizationForm.VisibleChanged += CustomizationForm_VisibleChanged;
        }
        private void CustomizationForm_VisibleChanged(object sender, EventArgs e) {
            ((Control)sender).VisibleChanged -= CustomizationForm_VisibleChanged;
            if (((Control)sender).Visible) {
                layoutControl = new List<LayoutControl>(FindNestedControls<LayoutControl>(CustomizationForm))[3];
                InsertButtons();
                AddButton.Text += " (TODO)";
                selectedColumn = null;
                ((ListBoxControl)ActiveListBox).SelectedItem = null;
                ActiveListBox.KeyDown += ActiveListBox_KeyDown;
                ((ListBoxControl)ActiveListBox).SelectedValueChanged += columnChooser_SelectedColumnChanged;
                layoutView.Images = GridPainter.Indicator;
            }
        }
        private void layoutView_HideCustomization(object sender, EventArgs e) {
            DeleteButtons();
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = -1;
            }
            layoutView.Images = null;
            ((ListBoxControl)ActiveListBox).SelectedValueChanged += columnChooser_SelectedColumnChanged;
            ActiveListBox.KeyDown += ActiveListBox_KeyDown;
            layoutControl = null;
            customizationFormCore = null;
            selectedColumn = null;
        }
        private void ActiveListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                RemoveSelectedColumn();
            }
        }
        protected LayoutViewCustomizationForm CustomizationForm {
            get {
                return customizationFormCore ??
                       (customizationFormCore =
                        typeof(LayoutView).GetProperty("CustomizationForm",
                                                        System.Reflection.BindingFlags.Instance |
                                                        System.Reflection.BindingFlags.NonPublic).GetValue(layoutView,
                                                                                                           null) as
                        LayoutViewCustomizationForm);
            }
        }
        protected override Control ActiveListBox {
            get {
                return layoutControl.Controls[4];
            }
        }
        private static IEnumerable<T> FindNestedControls<T>(Control container) where T : Control {
            //            if (container.Controls != null)
            foreach (Control item in container.Controls) {
                if (item is T)
                    yield return (T)item;
                foreach (T child in FindNestedControls<T>(item))
                    yield return child;
            }
        }
        protected override List<string> GetUsedProperties() {
            return ListEditor.Model.Columns.Select(columnInfoNodeWrapper => columnInfoNodeWrapper.PropertyName).ToList();
        }

        protected override ITypeInfo DisplayedTypeInfo {
            get { return View.ObjectTypeInfo; }
        }
        //TODO: Implement adding new properties into the customization form.
        protected override void AddColumn(string propertyName) {
            IModelColumn columnInfo = FindColumnModelByPropertyName(propertyName);
            if (columnInfo == null) {
                columnInfo = ListEditor.Model.Columns.AddNode<IModelColumn>();
                columnInfo.Id = propertyName;
                columnInfo.PropertyName = propertyName;
                columnInfo.Index = -1;
                var wrapper = ListEditor.AddColumn(columnInfo) as LayoutViewColumnWrapper;
                if (wrapper != null && wrapper.Column != null && wrapper.Column.LayoutViewField != null) {
                    ((ListBoxControl)ActiveListBox).Items.Add(wrapper.Column.LayoutViewField);
                }
            } else {
                throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotAddDuplicateProperty, propertyName));
            }
        }
        protected override void RemoveSelectedColumn() {
            var field = ((ListBoxControl)ActiveListBox).SelectedItem as LayoutViewField;
            if (field != null) {
                LayoutViewColumnWrapper columnInfo = (from LayoutViewColumn item in layoutView.Columns where item.FieldName == field.FieldName select ListEditor.FindColumn(((XafLayoutViewColumn)item).PropertyName) as LayoutViewColumnWrapper).FirstOrDefault();
                if (columnInfo != null)
                    ListEditor.RemoveColumn(columnInfo);
                ((ListBoxControl)ActiveListBox).Items.Remove(field);
            }
        }
        protected override void AddButtonsToCustomizationForm() {
            layoutControl.Controls.Add(RemoveButton);
            layoutControl.Controls.Add(AddButton);

            var hiddenItemsGroup = ((LayoutControlGroup)layoutControl.Items[0]);
            LayoutControlItem addButtonLayoutItem = hiddenItemsGroup.AddItem();
            addButtonLayoutItem.Control = AddButton;
            addButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
            addButtonLayoutItem.TextVisible = false;

            LayoutControlItem removeButtonLayoutItem = hiddenItemsGroup.AddItem();
            removeButtonLayoutItem.Control = RemoveButton;
            removeButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 5);
            removeButtonLayoutItem.TextVisible = false;
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            SubscribeLayoutViewEvents();
        }
        private void SubscribeLayoutViewEvents() {
            if (ListEditor != null) {
                layoutView = ListEditor.LayoutView;
                layoutView.ShowCustomization += layoutView_ShowCustomization;
                layoutView.HideCustomization += layoutView_HideCustomization;
            }
        }
        protected override void OnDeactivated() {
            UnsubscribeLayoutViewEvents();
            selectedColumn = null;
            base.OnDeactivated();
        }
        private void UnsubscribeLayoutViewEvents() {
            if (layoutView != null) {
                layoutView.ShowCustomization -= layoutView_ShowCustomization;
                layoutView.HideCustomization -= layoutView_HideCustomization;
                layoutView = null;
            }
        }
        public LayoutViewColumnChooserController() {
            TypeOfView = typeof(DevExpress.ExpressApp.ListView);
        }
    }
}
