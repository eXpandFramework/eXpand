using System;
using System.Web.UI;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Security;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Security {
    public class ControlTypeConverter:ReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(Control);
        }
    }
}