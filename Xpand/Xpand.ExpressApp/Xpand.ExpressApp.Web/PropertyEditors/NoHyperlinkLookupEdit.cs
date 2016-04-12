using System;
using System.Web;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(object), false)]
    public class NoHyperlinkLookupEdit : ASPxLookupPropertyEditor {
        /// <summary>
        /// Initializes a new instance of the NoHyperlinkLookupEdit class.
        /// </summary>
        public NoHyperlinkLookupEdit(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }

        protected override WebControl CreateViewModeControlCore() {
            var result = new Label {ID = Guid.NewGuid().ToString()};
            return result;
        }

        protected override void ReadViewModeValueCore() {
            if (InplaceViewModeEditor != null) {
                ((Label) InplaceViewModeEditor).Text = HttpUtility.HtmlEncode(GetPropertyDisplayValue());
            }
        }
    }
}