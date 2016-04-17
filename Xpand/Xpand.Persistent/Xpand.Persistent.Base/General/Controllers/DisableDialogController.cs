using System;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General.Controllers{
    public class DisableDialogController : Controller {
        private readonly Type[] _types;

        public DisableDialogController(params Type[] types) {
            _types = types;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            foreach (var type in _types) {
                Frame.GetController(type).Active[GetType().Name] = false;
            }
        }
    }
}
