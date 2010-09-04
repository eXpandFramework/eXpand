using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.ConditionalControllerState.Security {
    public class ControllerTypeConverter : XpandReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(Controller);
        }
    }
}