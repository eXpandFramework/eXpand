using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.WinSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed class XpandSystemWindowsFormsModule : XpandModuleBase {
        public XpandSystemWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
        }
        public override void ExtendModelInterfaces(DevExpress.ExpressApp.Model.ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelRootNavigationItems, IModelRootNavigationItemsAutoSelectedGroupItem>();
        }
    }
}