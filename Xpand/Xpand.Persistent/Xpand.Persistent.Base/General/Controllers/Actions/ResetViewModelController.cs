using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.General.Controllers.Actions {
    public class ResetViewModelController : ModifyModelActionControllerBase {
        protected override void ModifyModelActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            var choiceActionItem = e.SelectedChoiceActionItem;
            if (choiceActionItem.Id == ModifyModelActionChoiceItemsUpdater.ResetViewModel){
                var modelApplicationBase = (ModelApplicationBase)Application.Model;
                var modelApplication = modelApplicationBase.CreatorInstance.CreateModelApplication();
                modelApplication.Id = modelApplicationBase.LastLayer.Id;
                new ModelXmlReader().ReadFromModel(modelApplication, modelApplicationBase.LastLayer);
                var modelViews = ((IModelApplication) modelApplication).Views;
                if (modelViews != null){
                    var modelView = modelViews[View.Id];
                    if (modelView != null){
                        if (!modelView.IsNewNode()){
                            modelView.Remove();       
                            ModelApplicationHelper.RemoveLayer(modelApplicationBase);
                            ModelApplicationHelper.AddLayer(modelApplicationBase, modelApplication);
                        }
                        else{
                            throw new UserFriendlyException("Cannot reset new views");
                        }
                    }
                }            
                Frame.GetController<ModelController>().SetView(e.ShowViewParameters);
            }
        }
    }
}
