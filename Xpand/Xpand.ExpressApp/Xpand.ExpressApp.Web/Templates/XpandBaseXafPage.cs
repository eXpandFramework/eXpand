using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;

namespace Xpand.ExpressApp.Web.Templates {
    public abstract class XpandBaseXafPage : BaseXafPage {
        protected XpandBaseXafPage() {
            PreInit+=OnPreInit;
        }

        void OnPreInit(object sender, EventArgs eventArgs) {
            RegisterActionContainers(new List<IActionContainer> {NavigationActionContainer});
        }

        public abstract NavigationActionContainer NavigationActionContainer { get; }
        
    }
}
