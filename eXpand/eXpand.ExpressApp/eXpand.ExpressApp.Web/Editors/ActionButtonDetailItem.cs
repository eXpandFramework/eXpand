using System;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxEditors;

namespace eXpand.ExpressApp.Web.Editors
{
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {
        public ActionButtonDetailItem(IModelDetailViewItem model, Type objectType) :
            base(model, objectType) { }
        protected override object CreateControlCore()
        {
            var button = new ASPxButton { Text = Caption };
            button.Click += (sender, args) => InvokeExecuted(args);

            return button;
        }
    }
}