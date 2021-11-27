using System;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General.Controllers{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0004:Implement XAF Controller constructors correctly", Justification = "<Pending>")]
    public class DisableDialogController : Controller {
        private readonly Type[] _types;

        public DisableDialogController(params Type[] types) {
            _types = types;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            foreach (var type in _types){
                Frame.GetController(type,controller => controller.Active[GetType().Name] = false);
            }
        }
    }
}
