using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.Editors;

namespace Xpand.ExpressApp.Win.Editors{
    [ViewItem(typeof(IModelActionButton))]
    public class ActionButtonDetailItem : ExpressApp.Editors.ActionButtonDetailItem{
        public ActionButtonDetailItem(Type objectType, string id)
            : base(objectType, id){
        }

        public ActionButtonDetailItem(
            IModelViewItem model, Type objectType)
            : base(model, objectType){
        }

        protected override object CreateControlCore(){
            var button = new SimpleButton{
                Text = Caption,
                Image = ImageLoader.Instance.GetImageInfo(((IModelActionButton) Model).ActionId.ImageName).Image
            };
            button.Click += (sender, args) => InvokeExecuted(args);
            return button;
        }
    }
}