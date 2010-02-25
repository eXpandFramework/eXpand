using System;
using DevExpress.ExpressApp;
using DevExpress.Web.ASPxEditors;

namespace eXpand.ExpressApp.Web.Editors
{
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {    
        public ActionButtonDetailItem(Type objectType, DictionaryNode info) :
            base(objectType, info) { }
        protected override object CreateControlCore()
        {
            var button = new ASPxButton {Text = Caption};
            button.Click += (sender, args) => InvokeExecuted(args);
            
            return button;
        }
    }
}