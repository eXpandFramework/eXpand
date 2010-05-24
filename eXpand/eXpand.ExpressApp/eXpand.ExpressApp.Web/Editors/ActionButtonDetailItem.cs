using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxEditors;
using eXpand.ExpressApp.Editors;

namespace eXpand.ExpressApp.Web.Editors
{
    [DetailViewItem(typeof(IModelActionButton))]
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {
        public ActionButtonDetailItem(Type objectType, IModelDetailViewItem model) : base(model, objectType) { }
        protected override object CreateControlCore()
        {
            var button = new ASPxButton {Text = Caption};
            button.Click += (sender, args) => InvokeExecuted(args);
            
            return button;
        }
    }
}