using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Editors;

namespace Xpand.ExpressApp.Win.Editors
{
    [DetailViewItem(typeof(IModelActionButton))]
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem
    {
        public ActionButtonDetailItem(Type objectType, string id)
            : base( objectType,id)
        {
        }

        public ActionButtonDetailItem(
            IModelDetailViewItem model, Type objectType)
            : base(model, objectType)
        {
        }



        protected override object CreateControlCore()
        {
            var button = new SimpleButton {Text = Caption};
            button.Click += (sender, args) => InvokeExecuted(args);
            return button;
        }
    }
}