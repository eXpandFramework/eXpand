using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.Editors
{
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {    
        public ActionButtonDetailItem(Type objectType, IModelDetailViewItem model) :
            base(objectType, model) { }

        protected override object CreateControlCore()
        {
            var button = new ButtonEdit {Text = Caption};
            button.Click += (sender, args) => InvokeExecuted(args);
            return button;
        }
    }
}