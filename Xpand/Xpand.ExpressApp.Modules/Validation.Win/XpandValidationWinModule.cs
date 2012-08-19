using System;
using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.Validation.Win {
    [ToolboxBitmap(typeof(XpandValidationWinModule))]
    [ToolboxItem(true)]
    public sealed class XpandValidationWinModule : XpandModuleBase {
        public XpandValidationWinModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
        }
    }
}