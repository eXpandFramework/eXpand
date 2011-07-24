using System.ComponentModel;
using System.Drawing;
using Xpand.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win {
    [ToolboxBitmap(typeof(AdditionalViewControlsProviderWindowsFormsModule))]
    [ToolboxItem(true)]
    public class AdditionalViewControlsProviderWindowsFormsModule : XpandModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsModule));
            RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
        }
    }
}