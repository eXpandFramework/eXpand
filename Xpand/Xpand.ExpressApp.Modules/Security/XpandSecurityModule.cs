using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(XpandSecurityModule))]
    [ToolboxItem(true)]
    public sealed class XpandSecurityModule : XpandModuleBase {
        public XpandSecurityModule() {
            RequiredModuleTypes.Add(typeof(SecurityModule));
        }
        #region Overrides of XpandModuleBase
        protected override Type ApplicationType() {
            return typeof(ISettingsStorage);
        }
        #endregion
    }
}