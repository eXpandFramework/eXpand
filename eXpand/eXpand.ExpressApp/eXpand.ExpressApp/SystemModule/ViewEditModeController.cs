using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule {

    public interface IModelDetailViewEditMode : IModelNode
    {
        ViewEditMode? ViewEditMode { get; set; }
    }

    public class ViewEditModeController : ViewController<DetailView> {

        protected override void OnActivated() {
            base.OnActivated();
            var attributeValue = ((IModelDetailViewEditMode)View.Model).ViewEditMode;
            if (attributeValue != null)
                View.ViewEditMode = attributeValue.Value;
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelDetailView, IModelDetailViewEditMode>();
        }
    }
}