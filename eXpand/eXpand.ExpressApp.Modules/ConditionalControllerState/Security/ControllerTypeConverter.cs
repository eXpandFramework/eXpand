using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ConditionalControllerState.Security {
    public class ControllerTypeConverter : ReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(Controller);
        }
    }
}