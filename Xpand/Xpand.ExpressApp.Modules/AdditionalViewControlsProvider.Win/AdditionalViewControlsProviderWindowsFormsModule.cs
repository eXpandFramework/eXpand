using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win {
    [ToolboxBitmap(typeof(AdditionalViewControlsProviderWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public class AdditionalViewControlsProviderWindowsFormsModule : XpandModuleBase {
        public AdditionalViewControlsProviderWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsModule));
        }
    }
}