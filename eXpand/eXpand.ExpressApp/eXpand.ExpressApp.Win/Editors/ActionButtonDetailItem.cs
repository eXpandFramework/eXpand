using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;

namespace eXpand.ExpressApp.Win.Editors
{
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {    
        public ActionButtonDetailItem(Type objectType, DictionaryNode info) :
            base(objectType, info) { }
        protected override object CreateControlCore()
        {
            var button = new ButtonEdit {Text = Caption};
            button.Click += (sender, args) => InvokeExecuted(args);
            return button;
        }
    }
}