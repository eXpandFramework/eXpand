using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class ReloadApplicationModelController : ViewController<DetailView> {
        public ReloadApplicationModelController() {
            TargetObjectType = typeof(ModelDifferenceObject);
            var reloadModelAction = new SimpleAction(this, "Reload model", PredefinedCategory.ObjectsCreation);
            reloadModelAction.Execute += ObjectCreationOnExecute;
        }

        void ObjectCreationOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var currentObject = View.CurrentObject;
            var userModelDifferenceObject = currentObject as UserModelDifferenceObject;
            ReplaceLayer((ModelApplicationBase)Application.Model, ((ModelDifferenceObject)currentObject).Model,userModelDifferenceObject!=null&&userModelDifferenceObject.IsCurrentUserModel,simpleActionExecuteEventArgs.ShowViewParameters);
        }

        protected virtual void ReplaceLayer(ModelApplicationBase model, ModelApplicationBase layer, bool isCurrentUserModel, ShowViewParameters showViewParameters) {
            if (isCurrentUserModel)
                ModelApplicationHelper.RemoveLayer(model);
            else
                model.ReplaceLayer(layer);
        }
    }
}
