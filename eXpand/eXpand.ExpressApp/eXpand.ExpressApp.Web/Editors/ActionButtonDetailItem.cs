using System;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxEditors;

namespace eXpand.ExpressApp.Web.Editors
{
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {    
        public ActionButtonDetailItem(Type objectType, IModelDetailViewItem model) :
            base(objectType, model) { }
        protected override object CreateControlCore()
        {
            var button = new ASPxButton {Text = Caption};
            button.Click += (sender, args) => InvokeExecuted(args);
            
            return button;
        }
    }
}