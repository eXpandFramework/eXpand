using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ConditionalControllerState.Security {
    public class ControllerTypeConverter : XpandReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(Controller);
        }
    }
}