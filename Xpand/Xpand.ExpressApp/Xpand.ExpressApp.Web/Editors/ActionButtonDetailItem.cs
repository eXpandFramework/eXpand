using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
using Xpand.ExpressApp.Editors;

namespace Xpand.ExpressApp.Web.Editors {
    [ViewItem(typeof(IModelActionButton))]
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem {
        public ActionButtonDetailItem(Type objectType, string id)
            : base(objectType, id) {
        }

        public ActionButtonDetailItem(
            IModelViewItem model, Type objectType)
            : base(model, objectType) {
        }
        protected override object CreateControlCore() {
            var button = new ASPxButton { Text = Caption };
            button.Click += (sender, args) => InvokeExecuted(args);

            return button;
        }
    }
}