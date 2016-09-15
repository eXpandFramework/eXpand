using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.SystemModule;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class SupressConfirmationController: ExpressApp.SystemModule.SupressConfirmationController {
        private Controller[] _controllers;

        protected override void OnActivated(){
            base.OnActivated();
            if (((IModelObjectViewSupressConfirmation)View.Model).SupressConfirmation) {
                _controllers = Frame.Controllers.Cast<Controller>().Where(IsConfirmationController).ToArray();
                foreach (var controller in _controllers){
                    controller.Active[GetType().Name] = false;
                }
            }
        }

        private bool IsConfirmationController(Controller controller) => controller is WebConfirmUnsavedChangesController<ListView> || controller is WebConfirmUnsavedChangesController<DetailView>;

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_controllers != null)
                foreach (var controller in _controllers) {
                    controller.Active.RemoveItem(GetType().Name);
                }
        }
    }
}
