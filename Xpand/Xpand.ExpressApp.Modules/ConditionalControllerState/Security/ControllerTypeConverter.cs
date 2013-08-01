using System;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ConditionalControllerState.Security {
    public class ControllerTypeConverter : XpandReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(Controller);
        }
    }
}