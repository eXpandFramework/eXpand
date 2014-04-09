using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
    public class ReloadApplicationModelController : ModelDifference.Controllers.ReloadApplicationModelController {
        protected override void ReplaceLayer(ModelApplicationBase model, ModelApplicationBase layer, bool isCurrentUserModel, ShowViewParameters showViewParameters){
            
            var showViewStrategyBase = (WinShowViewStrategyBase)Application.ShowViewStrategy;
            var modelApplicationBase = ((ModelApplicationBase)Application.Model);
            var lastLayer = modelApplicationBase.LastLayer;
            modelApplicationBase.RemoveLayer(lastLayer.Id);

            var dummyLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
            dummyLayer.Id = lastLayer.Id;
            ModelApplicationHelper.AddLayer(modelApplicationBase, dummyLayer);
            var keyValue = ObjectSpace.GetKeyValue(View.CurrentObject);
            var objectType = View.ObjectTypeInfo.Type;
            var modelDetailView = View.Model;
            showViewStrategyBase.CloseAllWindows();

            base.ReplaceLayer(model, layer, isCurrentUserModel,showViewParameters);
            showViewStrategyBase.ShowStartupWindow();

            var xafApplication = ApplicationHelper.Instance.Application;
            var objectSpace = xafApplication.CreateObjectSpace();
            var objectByKey = objectSpace.GetObjectByKey(objectType,keyValue);
            showViewStrategyBase = (WinShowViewStrategyBase)xafApplication.ShowViewStrategy;
            
            showViewParameters.CreatedView = xafApplication.CreateDetailView(objectSpace, modelDetailView, true, objectByKey);
            showViewStrategyBase.ShowView(showViewParameters, new ShowViewSource(null,null));

        }
    }
}
