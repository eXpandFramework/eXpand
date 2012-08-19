using System;
using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win {
    [ToolboxBitmap(typeof(AdditionalViewControlsProviderWindowsFormsModule))]
    [ToolboxItem(true)]
    public class AdditionalViewControlsProviderWindowsFormsModule : XpandModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsModule));
        }
    }
}