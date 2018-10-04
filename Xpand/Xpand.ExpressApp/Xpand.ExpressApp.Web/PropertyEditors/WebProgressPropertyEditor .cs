using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(decimal), EditorAliases.ProgressBarEditor, false)]
    public class WebProgressPropertyEditor : ASPxPropertyEditor {
        public WebProgressPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        private void SetProgressValue() {
            var progressBar = InplaceViewModeEditor as ASPxProgressBar ?? Editor;
            if (progressBar != null) {
                progressBar.Value = (decimal) PropertyValue;
            }
        }

        public new ASPxProgressBar Editor => (ASPxProgressBar) base.Editor;

        protected override WebControl CreateEditModeControlCore() {
            return new ASPxProgressBar {Width = Unit.Percentage(100), ID = "TaskProgressBar"};
        }
        protected override WebControl CreateViewModeControlCore() {
            return CreateEditModeControlCore();
        }
        protected override void ReadViewModeValueCore() {
            base.ReadViewModeValueCore();
            SetProgressValue();
        }
        protected override void ReadEditModeValueCore() {
            base.ReadEditModeValueCore();
            SetProgressValue();
        }
        
    }

}
