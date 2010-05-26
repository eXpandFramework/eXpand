using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.Editors
{
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {
        public ActionButtonDetailItem(IModelDetailViewItem model, Type objectType) :
            base(model, objectType) { }

        protected override object CreateControlCore()
        {
            var button = new SimpleButton {Text = Caption};
            button.Click += (sender, args) => InvokeExecuted(args);
            return button;
        }
    }
}