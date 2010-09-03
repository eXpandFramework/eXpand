using System;
using System.Windows.Forms;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Security {
    public class ControlTypeConverter:XpandReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(Control);
        }
    }
}