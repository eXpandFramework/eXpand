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
            var objectCreation = new SimpleAction(this, "Reload model", PredefinedCategory.ObjectsCreation);
            objectCreation.Execute += ObjectCreationOnExecute;
        }

        void ObjectCreationOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            ReplaceLayer();
        }

        protected virtual void ReplaceLayer() {
            var modelApplicationBase = ((ModelApplicationBase)Application.Model);
            modelApplicationBase.ReplaceLayer(((ModelDifferenceObject)View.CurrentObject).Model);
        }
    }
}
