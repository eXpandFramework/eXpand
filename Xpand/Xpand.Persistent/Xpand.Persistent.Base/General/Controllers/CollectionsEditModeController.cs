using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelOptionsCollectionEditMode : IModelOptions {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DefaultValue(ViewEditMode.Edit)]
        ViewEditMode CollectionsEditMode { get; set; }
    }

    public class CollectionsEditModeController : WindowController,IModelExtender {
        private ShowViewStrategy _showViewStrategy;

        public CollectionsEditModeController(){
            TargetWindowType=WindowType.Main;
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.TemplateChanged+=FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs){
            if (Frame.Context == TemplateContext.ApplicationWindow){
                Frame.TemplateChanged -= FrameOnTemplateChanged;
                _showViewStrategy = ((ShowViewStrategy)Application.ShowViewStrategy);
                var collectionsEditMode = ((IModelOptionsCollectionEditMode)Application.Model.Options).CollectionsEditMode;
                _showViewStrategy.CollectionsEditMode = collectionsEditMode;
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions, IModelOptionsCollectionEditMode>();
        }
    }
}
