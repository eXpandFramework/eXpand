using System;
using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.Validation.Web {
    [ToolboxBitmap(typeof(XpandValidationWebModule))]
    [ToolboxItem(true)]
    public sealed class XpandValidationWebModule : XpandModuleBase {
        public XpandValidationWebModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
        }
    }
}