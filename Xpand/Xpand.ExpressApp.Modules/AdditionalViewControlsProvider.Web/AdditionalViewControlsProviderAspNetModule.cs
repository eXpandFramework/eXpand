using System.ComponentModel;
using System.Drawing;
using Xpand.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web {
    [ToolboxBitmap(typeof(AdditionalViewControlsProviderAspNetModule))]
    [ToolboxItem(true)]
    public class AdditionalViewControlsProviderAspNetModule : XpandModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsModule));
            RequiredModuleTypes.Add(typeof(XpandSystemAspNetModule));
        }

    }
}