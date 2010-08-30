using System;
using System.Windows.Forms;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Security;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Security {
    public class ControlTypeConverter:ReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(Control);
        }
    }
}