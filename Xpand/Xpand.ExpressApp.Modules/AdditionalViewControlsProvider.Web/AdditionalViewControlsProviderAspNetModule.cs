using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web {
    [ToolboxBitmap(typeof(AdditionalViewControlsProviderAspNetModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public class AdditionalViewControlsProviderAspNetModule : XpandModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsModule));
        }
    }
}