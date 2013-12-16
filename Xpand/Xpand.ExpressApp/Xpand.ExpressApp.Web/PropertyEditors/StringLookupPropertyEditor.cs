using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxEditors;
using Xpand.ExpressApp.PropertyEditors;
using System.Linq;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(string), Xpand.Persistent.Base.General.EditorAliases.StringLookupPropertyEditor, false)]
    public class StringLookupPropertyEditor : ASPxPropertyEditor, IComplexViewItem, IStringLookupPropertyEditor {
        public event EventHandler<HandledEventArgs> ItemsCalculating;

        protected virtual void OnItemsCalculating(HandledEventArgs e) {
            EventHandler<HandledEventArgs> handler = ItemsCalculating;
            if (handler != null) handler(this, e);
        }

        public IObjectSpace ObjectSpace {
            get { return helper.ObjectSpace; }
        }
        
        protected LookupEditorHelper helper;
        Label _viewModeLabelControl;

        public StringLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_viewModeLabelControl != null) {
                    _viewModeLabelControl.Dispose();
                    _viewModeLabelControl = null;
                }
            }
        }

        protected override WebControl CreateViewModeControlCore() {
            _viewModeLabelControl = new Label { CssClass = ViewModeControlCssClass, ID = Guid.NewGuid().ToString() };
            return _viewModeLabelControl;
        }
        protected override WebControl CreateEditModeControlCore() {
            var asPxComboBox = new ASPxComboBox { ReadOnly = !AllowEdit, AutoPostBack = Model.ImmediatePostData };
            asPxComboBox.ValueChanged += AsPxComboBoxOnValueChanged;
            RenderHelper.SetupASPxWebControl(asPxComboBox);
            return asPxComboBox;
        }

        void AsPxComboBoxOnValueChanged(object sender, EventArgs eventArgs) {
            EditValueChangedHandler(sender, EventArgs.Empty);
        }

        protected override void SetupControl(WebControl control) {
            base.SetupControl(control);
            if (ViewEditMode == ViewEditMode.Edit) {
                ListEditItemCollection items = ((ASPxComboBox)control).Items;
                items.Clear();
                ComboBoxItemsBuilder.Create()
                .WithPropertyEditor(this)
                .Build((enumerable, b) => items.AddRange(enumerable.Select(s => new ListEditItem(s)).ToList()), () => {
                    var handledEventArgs = new HandledEventArgs();
                    OnItemsCalculating(handledEventArgs);
                    return handledEventArgs.Handled;
                });
            }
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            if (helper == null)
                helper = new LookupEditorHelper(application, objectSpace, ObjectTypeInfo, Model);
            helper.SetObjectSpace(objectSpace);
        }
    }

}
